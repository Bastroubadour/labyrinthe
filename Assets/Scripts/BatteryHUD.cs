using UnityEngine;
using UnityEngine.UI;

public class BatteryHUD : MonoBehaviour
{
    [Header("Références")]
    public FlashlightController flashlight; // Ton script lampe
    public Image batteryFill;               // L'intérieur de la barre (Fill)

    void Update()
    {
        if (flashlight == null || batteryFill == null)
            return;

        // Convertit la batterie (0–100) en pourcentage (0–1)
        float percent = flashlight.battery / 100f;

        // Met à jour UNIQUEMENT l'intérieur
        batteryFill.fillAmount = percent;
    }
}