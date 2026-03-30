using UnityEngine;

public class CorridorFactory : MonoBehaviour
{
    public GameObject corridorPrefab;

    public void CreateCorridor(Vector3 position, Transform parent, 
                               GameObject roomA, GameObject roomB, 
                               string directionFromA)
    {
        // Instanciation du corridor
        Instantiate(corridorPrefab, position, Quaternion.identity, parent);

        // Ouvrir les murs des rooms adjacentes
        OpenWall(roomA, directionFromA);
        OpenWall(roomB, Opposite(directionFromA));
    }

    void OpenWall(GameObject room, string direction)
    {
        Transform walls = room.transform.Find("Walls");

        switch (direction)
        {
            case "North":
                walls.Find("North wall").gameObject.SetActive(false);
                break;
            case "South":
                walls.Find("South wall").gameObject.SetActive(false);
                break;
            case "East":
                walls.Find("East wall").gameObject.SetActive(false);
                break;
            case "West":
                walls.Find("West wall").gameObject.SetActive(false);
                break;
        }
    }

    string Opposite(string dir)
    {
        switch (dir)
        {
            case "North": return "South";
            case "South": return "North";
            case "East": return "West";
            case "West": return "East";
        }
        return "";
    }
}