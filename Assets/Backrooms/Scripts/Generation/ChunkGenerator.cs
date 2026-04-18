using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{   
    public float roomSpacing => segmentSpacing;

    public RoomFactory roomFactory;
    public CorridorFactory corridorFactory;

    [Header("Corridor")]
    public int segmentCount = 10;
    public float segmentSpacing = 10f;
    public float corridorHalfWidth = 7.5f; // couloir de 15

    [Header("Rooms connectées")]
    public float roomHalfWidth = 5f; // si ta room fait 10
    public float lateralZ = 12.5f;   // corridorHalfWidth + roomHalfWidth

    [Header("Murs entre les rooms")]
    public GameObject sideWallPrefab;
    public float wallHeight = 2f;
    public float wallThickness = 0.3f;

    [Header("Poteaux symétriques")]
    public GameObject polePrefab;
    public GameObject poleAnomalyPrefab;
    public float poleChance = 1f;        // 1 = poteaux à chaque segment
    public float anomalyChance = 0.03f;  // 3% d'anomalie
    public float poleOffsetFromWall = 0.1f; // offset local
    public float poleSpacing = 1.2f;

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
        // 🔥 MUR AVANT LE PREMIER SEGMENT (spawn)
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
        // BOUCLE PRINCIPALE
        // ---------------------------------------------------------
        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 corridorPos = basePos + new Vector3(i * segmentSpacing, 0, 0);

            // Sol du couloir
            corridorFactory.CreateCorridor(corridorPos, chunk.transform);

            // ---------------------------------------------------------
            // ROOMS
            // ---------------------------------------------------------
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
            // MURS ENTRE LES ROOMS
            // ---------------------------------------------------------
            GameObject leftWall = null;
            GameObject rightWall = null;

            if (i < segmentCount - 1)
            {
                float wallZ = corridorHalfWidth;

                // Mur gauche
                Vector3 leftWallPos = corridorPos + new Vector3(segmentSpacing / 2f, wallHeight / 2f, wallZ);
                leftWall = Instantiate(sideWallPrefab, leftWallPos, Quaternion.identity, chunk.transform);

                // Mur droit
                Vector3 rightWallPos = corridorPos + new Vector3(segmentSpacing / 2f, wallHeight / 2f, -wallZ);
                rightWall = Instantiate(sideWallPrefab, rightWallPos, Quaternion.identity, chunk.transform);
            }

            // ---------------------------------------------------------
            // 🌟 P O T E A U X   S Y M É T R I Q U E S  (version propre)
            // ---------------------------------------------------------
            if (Random.value < poleChance && leftWall != null && rightWall != null)
            {
                void SpawnPoleLocal(Transform wall, float localX)
                {
                    bool isAnomaly = Random.value < anomalyChance;
                    GameObject prefab = isAnomaly ? poleAnomalyPrefab : polePrefab;

                    float zOffset = wall.localScale.z / 2f + poleOffsetFromWall;

                    // Mur gauche → poteaux vers l'intérieur = -zOffset
                    // Mur droit  → poteaux vers l'intérieur = +zOffset
                    float localZ = (wall.position.z > 0) ? -zOffset : zOffset;

                    Vector3 localPos = new Vector3(localX, 0, localZ);

                    // Instantiation
                    GameObject obj = Instantiate(prefab, wall.TransformPoint(localPos), Quaternion.identity, wall);

                    // 🔥 Correction : empêcher l’héritage du scale du mur
                    obj.transform.localScale = Vector3.one;
                }

                // Deux poteaux à gauche
                SpawnPoleLocal(leftWall.transform, 0f);
                SpawnPoleLocal(leftWall.transform, -poleSpacing);

                // Deux poteaux à droite
                SpawnPoleLocal(rightWall.transform, 0f);
                SpawnPoleLocal(rightWall.transform, +poleSpacing);
            }
        }

        return chunk;
    }
}

