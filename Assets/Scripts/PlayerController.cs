using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Mouvement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;

    [Header("Stamina")]
    public float maxStamina = 5f;
    public float staminaDrain = 1f;
    public float staminaRegen = 0.5f;
    private float stamina;

    [Header("Souris")]
    public float mouseSensitivity = 150f;

    [Header("FOV Dynamique")]
    public Camera cam;
    public float normalFOV = 60f;
    public float sprintFOV = 75f;
    public float fovSpeed = 8f;

    [Header("Audio Pas")]
    public AudioSource footstepSource;
    public AudioClip walkStep;
    public AudioClip runStep;

    private bool leftFoot = true;

    [Header("Headbob")]
    public float walkBobSpeed = 8f;
    public float walkBobAmount = 0.05f;
    public float runBobSpeed = 13f;
    public float runBobAmount = 0.1f;

    private float bobTimer = 0f;
    private Vector3 camInitialPos;

    [Header("Crouch")]
    public float crouchHeight = 1.0f;
    public float standHeight = 2.0f;
    public float crouchSpeed = 6f;
    public AudioClip crouchSound;
    private bool isCrouching = false;

    private float currentHeight;
    private float heightVelocity;

    private float currentSpeed;
    private float speedVelocity;

    [Header("Respiration")]
    public AudioSource breathingSource;
    public AudioClip heavyBreathing;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.15f;
    public LayerMask groundMask;

    private CharacterController controller;
    private float xRotation = 0f;
    private float stepTimer = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        stamina = maxStamina;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        camInitialPos = cam.transform.localPosition;
        cam.fieldOfView = normalFOV;

        currentHeight = standHeight;
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        HandleCrouch();
        MovePlayer();
        LookAround();
        HandleFOV();
        HandleBreathing();
        HandleFootsteps();
        HandleHeadbob();

        // ⚠️ La lampe n'est plus gérée ici
    }

    void MovePlayer()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && stamina > 0 && !isCrouching;

        float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;

        if (isCrouching)
            targetSpeed = walkSpeed * 0.5f;

        currentSpeed = Mathf.SmoothDamp(
            currentSpeed,
            targetSpeed,
            ref speedVelocity,
            0.12f
        );

        if (isSprinting)
            stamina -= staminaDrain * Time.deltaTime;
        else
            stamina += staminaRegen * Time.deltaTime;

        stamina = Mathf.Clamp(stamina, 0, maxStamina);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleFOV()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && stamina > 0 && !isCrouching;

        float targetFOV = isSprinting ? sprintFOV : normalFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovSpeed);
    }

    void HandleBreathing()
    {
        bool isSprintingHard = Input.GetKey(KeyCode.LeftShift) && stamina < maxStamina * 0.5f;

        if (isSprintingHard && !breathingSource.isPlaying)
        {
            breathingSource.clip = heavyBreathing;
            breathingSource.Play();
        }

        if (!isSprintingHard && breathingSource.isPlaying)
        {
            breathingSource.Stop();
        }
    }

    void HandleFootsteps()
    {
        bool grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (!grounded)
            return;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool isMoving = Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f;
        if (!isMoving)
            return;

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && !isCrouching;

        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0f)
        {
            AudioClip clip;

            if (isRunning)
            {
                footstepSource.pitch = 1f;
                clip = runStep;
                stepTimer = 0.28f;
            }
            else
            {
                footstepSource.pitch = leftFoot
                    ? Random.Range(0.9f, 1.05f)
                    : Random.Range(1.05f, 1.2f);

                clip = walkStep;
                stepTimer = isCrouching ? 0.6f : 0.5f;
            }

            footstepSource.PlayOneShot(clip);
            leftFoot = !leftFoot;
        }
    }

    void HandleCrouch()
    {
        bool crouchKey = Input.GetKey(KeyCode.LeftControl);

        if (crouchKey && !isCrouching)
        {
            isCrouching = true;
            footstepSource.PlayOneShot(crouchSound);
        }
        else if (!crouchKey && isCrouching)
        {
            isCrouching = false;
            footstepSource.PlayOneShot(crouchSound);
        }

        float targetHeight = isCrouching ? crouchHeight : standHeight;

        currentHeight = Mathf.SmoothDamp(
            currentHeight,
            targetHeight,
            ref heightVelocity,
            0.15f
        );

        controller.height = currentHeight;
    }

    void HandleHeadbob()
    {
        bool grounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool isMoving = Mathf.Abs(x) > 0.1f || Mathf.Abs(z) > 0.1f;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && !isCrouching;

        Vector3 basePos = isCrouching
            ? camInitialPos + new Vector3(0, -0.5f, 0)
            : camInitialPos;

        if (!isMoving || !grounded)
        {
            cam.transform.localPosition = Vector3.Lerp(
                cam.transform.localPosition,
                basePos,
                Time.deltaTime * 6f
            );
            return;
        }

        float speed = isRunning ? runBobSpeed : walkBobSpeed;
        float amount = isRunning ? runBobAmount : walkBobAmount;

        if (isCrouching)
        {
            speed *= 0.6f;
            amount *= 0.5f;
        }

        bobTimer += Time.deltaTime * speed;

        float bobX = Mathf.Cos(bobTimer) * amount * 0.5f;
        float bobY = Mathf.Abs(Mathf.Sin(bobTimer)) * amount;

        cam.transform.localPosition = basePos + new Vector3(bobX, bobY, 0f);
    }
}