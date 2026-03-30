using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FlashlightController : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public Light flashlight;
    public float battery = 100f;
    public float drainRate = 5f;
    public float onIntensity = 2f;
    public float offIntensity = 0f;
    public float smooth = 10f;

    private bool flashlightOn = false;

    [Header("Flashlight Sounds")]
    public AudioSource audioSource;
    public AudioClip soundOn;
    public AudioClip soundOff;
    public AudioClip soundEmpty;

    [Header("HUD Batterie")]
    public Image batteryBar;        // Option A
    public TMP_Text batteryText;    // Option B

    void Update()
    {
        // Toggle flashlight
        if (Input.GetMouseButtonDown(1))
        {
            if (battery <= 0f)
            {
                flashlightOn = false;
                audioSource.PlayOneShot(soundEmpty);
                return;
            }

            flashlightOn = !flashlightOn;

            if (flashlightOn)
                audioSource.PlayOneShot(soundOn);
            else
                audioSource.PlayOneShot(soundOff);
        }

        // Battery drain
        if (flashlightOn)
        {
            battery -= drainRate * Time.deltaTime;

            if (battery <= 0f)
            {
                battery = 0f;
                flashlightOn = false;
                audioSource.PlayOneShot(soundEmpty);
            }
        }

        // Smooth intensity
        float targetIntensity = flashlightOn ? onIntensity : offIntensity;
        flashlight.intensity = Mathf.Lerp(flashlight.intensity, targetIntensity, Time.deltaTime * smooth);

        // HUD update
        UpdateHUD();
    }

    void UpdateHUD()
    {
        float percent = battery / 100f;

        if (batteryBar != null)
            batteryBar.fillAmount = percent;

        if (batteryText != null)
            batteryText.text = Mathf.RoundToInt(battery) + "%";
    }
}