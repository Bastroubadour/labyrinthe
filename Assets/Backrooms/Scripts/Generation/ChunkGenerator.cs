using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public RoomFactory roomFactory;
    public CorridorFactory corridorFactory;

    [Header("Taille du chunk")]
    public int segmentCount = 10;     // nombre de segments de corridor dans ce chunk
    public float segmentSpacing = 10f; // distance entre chaque segment de corridor

    [Header("Rooms latérales")]
    [Range(0f, 1f)] public float sideRoomChance = 0.5f; // probabilité de room de chaque côté

    public GameObject GenerateChunk(Vector2Int chunkPos, int seed)
    {
        Random.InitState(seed);

        GameObject chunk = new GameObject($"Chunk_{chunkPos.x}_{chunkPos.y}");
        chunk.transform.position = new Vector3(
            chunkPos.x * segmentCount * segmentSpacing,
            0,
            chunkPos.y * 0f // on reste sur un seul axe pour le corridor principal
        );

        // Position de départ du corridor principal (axe X)
        Vector3 basePos = chunk.transform.position;

        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 corridorPos = basePos + new Vector3(i * segmentSpacing, 0, 0);

            // 1) Segment de corridor principal
            corridorFactory.CreateCorridor(corridorPos, chunk.transform);

            // 2) Rooms latérales (haut/bas = Nord/Sud)
            // Room au "Nord" (au-dessus du corridor, +Z)
            if (Random.value < sideRoomChance)
            {
                Vector3 roomPosNorth = corridorPos + new Vector3(0, 0, segmentSpacing);
                roomFactory.SpawnRoom(
                    roomPosNorth,
                    chunk.transform,
                    openNorth: false,
                    openSouth: true,  // ouverture vers le corridor
                    openEast: false,
                    openWest: false
                );
            }

            // Room au "Sud" (en-dessous du corridor, -Z)
            if (Random.value < sideRoomChance)
            {
                Vector3 roomPosSouth = corridorPos + new Vector3(0, 0, -segmentSpacing);
                roomFactory.SpawnRoom(
                    roomPosSouth,
                    chunk.transform,
                    openNorth: true,  // ouverture vers le corridor
                    openSouth: false,
                    openEast: false,
                    openWest: false
                );
            }
        }

        return chunk;
    }
}
