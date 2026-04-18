using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public RoomFactory roomFactory;
    public CorridorFactory corridorFactory;

    [Header("Corridor")]
    public int segmentCount = 10;
    public float segmentSpacing = 10f;
    public float corridorHalfWidth = 7.5f; // pour un couloir de 15

    [Header("Rooms connectées")]
    public float lateralZ = 12.5f;   // corridorHalfWidth + roomHalfWidth
    public float roomHalfWidth = 5f;

    [Header("Murs entre les rooms")]
    public GameObject sideWallPrefab;
    public float wallHeight = 2f;
    public float wallThickness = 0.3f;

    public float roomSpacing => segmentSpacing;

    public GameObject GenerateChunk(Vector2Int chunkPos, int seed)
    {
        Random.InitState(seed);

        GameObject chunk = new GameObject($"Chunk_{chunkPos.x}_{chunkPos.y}");
        chunk.transform.position = new Vector3(
            chunkPos.x * segmentCount * segmentSpacing,
            0,
            0
        );

        Vector3 basePos = chunk.transform.position;

        // ---------------------------------------------------------
        // 🔥 FIX : MUR AVANT LE PREMIER SEGMENT (spawn)
        // ---------------------------------------------------------
        {
            float wallZ = corridorHalfWidth;

            Vector3 firstCorridorPos = basePos;

            // Mur gauche
            Vector3 leftWallPos = firstCorridorPos + new Vector3(-segmentSpacing / 2f, wallHeight / 2f, wallZ);
            Instantiate(sideWallPrefab, leftWallPos, Quaternion.identity, chunk.transform);

            // Mur droit
            Vector3 rightWallPos = firstCorridorPos + new Vector3(-segmentSpacing / 2f, wallHeight / 2f, -wallZ);
            Instantiate(sideWallPrefab, rightWallPos, Quaternion.identity, chunk.transform);
        }

        // ---------------------------------------------------------
        // Boucle principale
        // ---------------------------------------------------------
        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 corridorPos = basePos + new Vector3(i * segmentSpacing, 0, 0);

            // Sol du couloir
            corridorFactory.CreateCorridor(corridorPos, chunk.transform);

            // Rooms
            Vector3 leftRoomPos = corridorPos + new Vector3(0, 0, lateralZ);
            roomFactory.SpawnRoom(
                leftRoomPos,
                chunk.transform,
                openNorth: false,
                openSouth: true,
                openEast: false,
                openWest: false
            );

            Vector3 rightRoomPos = corridorPos + new Vector3(0, 0, -lateralZ);
            roomFactory.SpawnRoom(
                rightRoomPos,
                chunk.transform,
                openNorth: true,
                openSouth: false,
                openEast: false,
                openWest: false
            );

            // ---------------------------------------------------------
            // MUR ENTRE LES ROOMS (pas sur les rooms)
            // ---------------------------------------------------------
            if (i < segmentCount - 1)
            {
                float wallZ = corridorHalfWidth;

                // Mur gauche
                Vector3 leftWallPos = corridorPos + new Vector3(segmentSpacing / 2f, wallHeight / 2f, wallZ);
                Instantiate(sideWallPrefab, leftWallPos, Quaternion.identity, chunk.transform);

                // Mur droit
                Vector3 rightWallPos = corridorPos + new Vector3(segmentSpacing / 2f, wallHeight / 2f, -wallZ);
                Instantiate(sideWallPrefab, rightWallPos, Quaternion.identity, chunk.transform);
            }
        }

        return chunk;
    }
}
