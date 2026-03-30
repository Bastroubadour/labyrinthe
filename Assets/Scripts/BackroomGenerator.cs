using UnityEngine;

public class BackroomsGenerator : MonoBehaviour
{
    public GameObject roomPrefab;
    public GameObject corridorPrefab;

    public int width = 10;   // nombre de rooms en X
    public int height = 10;  // nombre de rooms en Z
    public float roomSize = 10f;

    void Start()
    {
        Generate();
    }

    void Generate()
    {
        bool[,] grid = new bool[width, height];

        // Génération des rooms
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(x * roomSize, 0, z * roomSize);
                Instantiate(roomPrefab, pos, Quaternion.identity, transform);

                grid[x, z] = true;
            }
        }

        // Génération des couloirs (sans impasses)
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // Couloir vers la droite
                if (x < width - 1 && Random.value > 0.2f)
                {
                    Vector3 pos = new Vector3(x * roomSize + roomSize / 2, 0, z * roomSize);
                    Instantiate(corridorPrefab, pos, Quaternion.identity, transform);
                }

                // Couloir vers le haut
                if (z < height - 1 && Random.value > 0.2f)
                {
                    Vector3 pos = new Vector3(x * roomSize, 0, z * roomSize + roomSize / 2);
                    Instantiate(corridorPrefab, pos, Quaternion.Euler(0, 90, 0), transform);
                }
            }
        }
    }
}