using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public RoomFactory roomFactory;
    public CorridorFactory corridorFactory;

    public int chunkSize = 1;          // nombre de rooms par chunk
    public float roomSpacing = 10f;    // distance entre rooms

    public GameObject GenerateChunk(Vector2Int chunkPos, int seed)
    {
        Random.InitState(seed);

        GameObject chunk = new GameObject($"Chunk_{chunkPos.x}_{chunkPos.y}");
        chunk.transform.position = new Vector3(
            chunkPos.x * chunkSize * roomSpacing,
            0,
            chunkPos.y * chunkSize * roomSpacing
        );

        // Stockage des rooms pour connecter les corridors
        GameObject[,] rooms = new GameObject[chunkSize, chunkSize];

        // 1) Génération des rooms avec murs fermés
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                Vector3 pos = chunk.transform.position +
                              new Vector3(x * roomSpacing, 0, z * roomSpacing);

                rooms[x, z] = roomFactory.SpawnRoom(
                    pos,
                    chunk.transform,
                    false, false, false, false
                );
            }
        }

        // 2) Connexions entre rooms (Système B)
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                GameObject roomA = rooms[x, z];

                // Connexion Est
                if (x < chunkSize - 1 && Random.value > 0.05f)
                {
                    GameObject roomB = rooms[x + 1, z];

                    Vector3 corridorPos = chunk.transform.position +
                        new Vector3(x * roomSpacing + roomSpacing / 2f, 0, z * roomSpacing);

                    corridorFactory.CreateCorridor(
                        corridorPos,
                        chunk.transform,
                        roomA,
                        roomB,
                        "East"
                    );

                    // OUVERTURE DES MURS
                    roomFactory.OpenEast(roomA);
                    roomFactory.OpenWest(roomB);
                }

                // Connexion Nord
                if (z < chunkSize - 1 && Random.value > 0.05f)
                {
                    GameObject roomB = rooms[x, z + 1];

                    Vector3 corridorPos = chunk.transform.position +
                        new Vector3(x * roomSpacing, 0, z * roomSpacing + roomSpacing / 2f);

                    corridorFactory.CreateCorridor(
                        corridorPos,
                        chunk.transform,
                        roomA,
                        roomB,
                        "North"
                    );

                    // OUVERTURE DES MURS
                    roomFactory.OpenNorth(roomA);
                    roomFactory.OpenSouth(roomB);
                }
            }
        }

        return chunk;
    }
}
