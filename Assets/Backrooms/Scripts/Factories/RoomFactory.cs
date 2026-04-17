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

        if (walls != null)
        {
            Transform n = walls.Find("North wall");
            Transform s = walls.Find("South wall");
            Transform e = walls.Find("East wall");
            Transform w = walls.Find("West wall");

            if (n != null) n.gameObject.SetActive(!openNorth);
            if (s != null) s.gameObject.SetActive(!openSouth);
            if (e != null) e.gameObject.SetActive(!openEast);
            if (w != null) w.gameObject.SetActive(!openWest);
        }

        return room;
    }

    public void OpenNorth(GameObject room)
    {
        room.transform.Find("Walls/North wall")?.gameObject.SetActive(false);
    }

    public void OpenSouth(GameObject room)
    {
        room.transform.Find("Walls/South wall")?.gameObject.SetActive(false);
    }

    public void OpenEast(GameObject room)
    {
        room.transform.Find("Walls/East wall")?.gameObject.SetActive(false);
    }

    public void OpenWest(GameObject room)
    {
        room.transform.Find("Walls/West wall")?.gameObject.SetActive(false);
    }
}
