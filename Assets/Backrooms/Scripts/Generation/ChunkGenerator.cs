using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public RoomFactory roomFactory;
    public CorridorFactory corridorFactory;

    [Header("Corridor")]
    public int segmentCount = 10;
    public float segmentSpacing = 10f;

    [Header("Rooms connectées")]
    public float lateralZ = 8f;   // distance du centre vers chaque room

    [Header("Murs entre les rooms")]
    public GameObject sideWallPrefab;
    public float corridorHalfWidth = 3f;
    public float wallThickness = 0.3f;
    public float wallHeight = 2f;

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

        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 corridorPos = basePos + new Vector3(i * segmentSpacing, 0, 0);

            // 1) Sol du couloir
            corridorFactory.CreateCorridor(corridorPos, chunk.transform);

            // 2) Rooms (toujours)
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

            // 3) MURS ENTRE LES ROOMS (pas sur les rooms)
            // On place le mur ENTRE ce segment et le suivant
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
