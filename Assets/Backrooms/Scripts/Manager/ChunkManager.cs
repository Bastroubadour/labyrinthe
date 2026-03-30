using UnityEngine;
using System.Collections.Generic;

public class ChunkManager : MonoBehaviour
{
    public Transform player;
    public ChunkGenerator chunkGenerator;

    public int renderDistance = 2;
    public float destroyBehindAngle = 150f;

    private Dictionary<Vector2Int, GameObject> visibleChunks = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int, GameObject> frozenChunks = new Dictionary<Vector2Int, GameObject>();

    void Update()
    {
        Vector2Int playerChunk = GetPlayerChunk();

        // 1) On calcule la liste des chunks qui DOIVENT être visibles
        HashSet<Vector2Int> mustBeVisible = new HashSet<Vector2Int>();

        for (int x = -renderDistance; x <= renderDistance; x++)
        {
            for (int y = -renderDistance; y <= renderDistance; y++)
            {
                Vector2Int coord = new Vector2Int(playerChunk.x + x, playerChunk.y + y);

                // Le chunk du joueur doit toujours être visible
                if (coord == playerChunk)
                {
                    mustBeVisible.Add(coord);
                    continue;
                }

                // Chunk devant toi → doit être visible
                if (!IsChunkBehindPlayer(coord))
                    mustBeVisible.Add(coord);
            }
        }

        // 2) RÈGLE B : Tant qu’un chunk est visible → on NE LE CHANGE PAS
        foreach (var coord in mustBeVisible)
        {
            // Déjà visible → on ne touche pas
            if (visibleChunks.ContainsKey(coord))
                continue;

            // Était gelé → on l'active
            if (frozenChunks.ContainsKey(coord))
            {
                GameObject chunk = frozenChunks[coord];
                chunk.SetActive(true);
                visibleChunks.Add(coord, chunk);
                frozenChunks.Remove(coord);
                continue;
            }

            // Nouveau chunk → seed aléatoire
            int seed = Random.Range(int.MinValue, int.MaxValue);
            GameObject newChunk = chunkGenerator.GenerateChunk(coord, seed);
            visibleChunks.Add(coord, newChunk);
        }

        // 3) Tous les chunks NON visibles → détruits → recréés → gelés
        List<Vector2Int> toFreeze = new List<Vector2Int>();

        foreach (var kvp in visibleChunks)
        {
            Vector2Int coord = kvp.Key;

            if (!mustBeVisible.Contains(coord))
                toFreeze.Add(coord);
        }

        foreach (var coord in toFreeze)
        {
            GameObject oldChunk = visibleChunks[coord];
            visibleChunks.Remove(coord);
            Destroy(oldChunk);

            int seed = Random.Range(int.MinValue, int.MaxValue);
            GameObject frozen = chunkGenerator.GenerateChunk(coord, seed);
            frozen.SetActive(false);

            frozenChunks[coord] = frozen;
        }
    }

    bool IsChunkBehindPlayer(Vector2Int chunkCoord)
    {
        Vector3 chunkWorldPos = new Vector3(
            chunkCoord.x * chunkGenerator.chunkSize * chunkGenerator.roomSpacing,
            0,
            chunkCoord.y * chunkGenerator.chunkSize * chunkGenerator.roomSpacing
        );

        Vector3 toChunk = chunkWorldPos - player.position;
        float angle = Vector3.Angle(player.forward, toChunk);

        return angle > destroyBehindAngle;
    }

    Vector2Int GetPlayerChunk()
    {
        int cx = Mathf.FloorToInt(player.position.x / (chunkGenerator.chunkSize * chunkGenerator.roomSpacing));
        int cy = Mathf.FloorToInt(player.position.z / (chunkGenerator.chunkSize * chunkGenerator.roomSpacing));
        return new Vector2Int(cx, cy);
    }
}
