using UnityEngine;
using System.Collections.Generic;

public class ChunkManager : MonoBehaviour
{
    public Transform player;
    public ChunkGenerator chunkGenerator;

    public int renderDistance = 2;      // nombre de chunks devant et derrière
    public float destroyBehindAngle = 150f;

    private Dictionary<int, GameObject> visibleChunks = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> frozenChunks = new Dictionary<int, GameObject>();

    void Update()
    {
        int playerChunk = GetPlayerChunk();

        // 1) Liste des chunks qui doivent être visibles
        HashSet<int> mustBeVisible = new HashSet<int>();

        for (int offset = -renderDistance; offset <= renderDistance; offset++)
        {
            int coord = playerChunk + offset;

            // Chunk du joueur → toujours visible
            if (coord == playerChunk)
            {
                mustBeVisible.Add(coord);
                continue;
            }

            // Chunk devant le joueur → visible
            if (!IsChunkBehindPlayer(coord))
                mustBeVisible.Add(coord);
        }

        // 2) Activation ou création des chunks nécessaires
        foreach (int coord in mustBeVisible)
        {
            if (visibleChunks.ContainsKey(coord))
                continue;

            if (frozenChunks.ContainsKey(coord))
            {
                GameObject chunk = frozenChunks[coord];
                chunk.SetActive(true);
                visibleChunks.Add(coord, chunk);
                frozenChunks.Remove(coord);
                continue;
            }

            int seed = Random.Range(int.MinValue, int.MaxValue);
            GameObject newChunk = chunkGenerator.GenerateChunk(new Vector2Int(coord, 0), seed);
            visibleChunks.Add(coord, newChunk);
        }

        // 3) Geler les chunks non visibles
        List<int> toFreeze = new List<int>();

        foreach (var kvp in visibleChunks)
        {
            int coord = kvp.Key;

            if (!mustBeVisible.Contains(coord))
                toFreeze.Add(coord);
        }

        foreach (int coord in toFreeze)
        {
            GameObject oldChunk = visibleChunks[coord];
            visibleChunks.Remove(coord);
            Destroy(oldChunk);

            int seed = Random.Range(int.MinValue, int.MaxValue);
            GameObject frozen = chunkGenerator.GenerateChunk(new Vector2Int(coord, 0), seed);
            frozen.SetActive(false);

            frozenChunks[coord] = frozen;
        }
    }

    bool IsChunkBehindPlayer(int chunkCoord)
    {
        float chunkWorldX = chunkCoord * chunkGenerator.segmentCount * chunkGenerator.roomSpacing;

        Vector3 chunkWorldPos = new Vector3(chunkWorldX, 0, 0);
        Vector3 toChunk = chunkWorldPos - player.position;

        float angle = Vector3.Angle(player.forward, toChunk);

        return angle > destroyBehindAngle;
    }

    int GetPlayerChunk()
    {
        float chunkLength = chunkGenerator.segmentCount * chunkGenerator.roomSpacing;
        return Mathf.FloorToInt(player.position.x / chunkLength);
    }
}
