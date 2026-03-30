using UnityEngine;

[System.Serializable]
public class RoomEntry
{
    public GameObject prefab;
    public float weight = 1f; // taux d'apparition
}

public class RoomFactory : MonoBehaviour
{
    public RoomEntry[] rooms; // liste pondérée

    GameObject GetRandomRoom()
    {
        float total = 0f;

        foreach (var r in rooms)
            total += r.weight;

        float pick = Random.value * total;

        foreach (var r in rooms)
        {
            if (pick < r.weight)
                return r.prefab;

            pick -= r.weight;
        }

        return rooms[0].prefab;
    }

    public GameObject SpawnRoom(Vector3 position, Transform parent,
        bool openNorth, bool openSouth, bool openEast, bool openWest)
    {
        GameObject prefab = GetRandomRoom();
        GameObject room = Instantiate(prefab, position, Quaternion.identity, parent);

        Transform walls = room.transform.Find("Walls");

        walls.Find("North wall").gameObject.SetActive(!openNorth);
        walls.Find("South wall").gameObject.SetActive(!openSouth);
        walls.Find("East wall").gameObject.SetActive(!openEast);
        walls.Find("West wall").gameObject.SetActive(!openWest);

        return room;
    }

    public void OpenNorth(GameObject room)
    {
        room.transform.Find("Walls/North wall").gameObject.SetActive(false);
    }

    public void OpenSouth(GameObject room)
    {
        room.transform.Find("Walls/South wall").gameObject.SetActive(false);
    }

    public void OpenEast(GameObject room)
    {
        room.transform.Find("Walls/East wall").gameObject.SetActive(false);
    }

    public void OpenWest(GameObject room)
    {
        room.transform.Find("Walls/West wall").gameObject.SetActive(false);
    }
}
