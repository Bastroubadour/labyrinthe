using UnityEngine;

public class BatteryPickUp : MonoBehaviour
{
    public float batteryAmount = 25f; // Combien la pile recharge
    public AudioSource audioSource;   // Son de recharge

    private bool pickedUp = false;

    private void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return;

        FlashlightController flashlight = other.GetComponent<FlashlightController>();

        if (flashlight != null)
        {
            pickedUp = true;

            // Recharge la batterie
            flashlight.battery = Mathf.Clamp(flashlight.battery + batteryAmount, 0f, 100f);

            // Joue le son
            if (audioSource != null)
                audioSource.Play();

            // Désactive le mesh pour faire disparaître la pile immédiatement
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
                r.enabled = false;

            // Détruit l’objet après le son
            Destroy(gameObject, audioSource.clip.length);
        }
    }
}