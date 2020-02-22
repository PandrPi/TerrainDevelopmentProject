using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkManager
{
    public static ChunkManager chunkManager;

    [SerializeField] private GameObject chunkPrefab;

    private Dictionary<string, Chunk> chunks;


    public void Initialize()
    {
        chunkManager = this;
        chunks = new Dictionary<string, Chunk>();
    }

    public void AddItem(string name, Chunk chunk)
    {
        chunks.Add(name, chunk);
    }

    public Chunk GetItem(string name)
    {
        if (chunks.ContainsKey(name))
            return chunks[name];
        else
            return null;
    }

    public Chunk CreateChunkGameobject(string name, Transform parent, Vector3 pos, Quaternion rot)
    {
        GameObject chunkGO = GameObject.Instantiate(chunkPrefab, pos, rot, parent);
        Chunk chunk = chunkGO.GetComponent<Chunk>();
        chunk.chunkData = new ChunkData(chunkGO.transform);
        chunk.chunkName = name;

        AddItem(chunk.chunkName, chunk);

        return chunk;
    }

    public void AssignMesh(string chunkName, ref Vector3[] vertices, ref int[] triangles,
        ref Vector2[] uvs, ref Vector4[] tangents, bool useCollider)
    {
        Chunk chunk = GetItem(chunkName);
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.tangents = tangents;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        chunk.chunkData.mf.sharedMesh = mesh;
        if (useCollider)
            chunk.chunkData.mc.sharedMesh = mesh;
        mesh.UploadMeshData(true);
    }
}
