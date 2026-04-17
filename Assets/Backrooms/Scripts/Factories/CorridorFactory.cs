using UnityEngine;

public class CorridorFactory : MonoBehaviour
{
    public GameObject corridorPrefab;

    // Version simple : un segment de corridor à une position donnée
    public GameObject CreateCorridor(Vector3 position, Transform parent)
    {
        if (corridorPrefab == null) return null;
        return Instantiate(corridorPrefab, position, Quaternion.identity, parent);
    }

    // L’ancienne version avec roomA/roomB reste là si tu en as besoin plus tard,
    // mais elle n’est plus utilisée dans le système "corridor principal + rooms latérales".
}
