using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public string chunkName;
    public int chunkSize;
    public ChunkData chunkData;
    public string[] childNames = new string[childCount];
    public string parentName;
    public Vector3 absolutePosition;
    public bool divided;

    private MeshRenderer mr;
    private Transform myTransform;

    private static int childCount = 4;
    private static Vector3[] childPositions =
    {
        new Vector3(0, 0, 0), new Vector3(0, 0, 1),
        new Vector3(1, 0, 1), new Vector3(1, 0, 0)
    };

    private void Awake()
    {
        myTransform = transform;
        mr = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (divided)
        {
            DivideChunk();
            divided = false;
        }
    }

    private void DivideChunk()
    {
        int halfSize = chunkSize / 2;

        for (int i = 0; i < childCount; i++)
        {
            Vector3 position = myTransform.position + childPositions[i] * halfSize;
            Chunk chunk = ChunkManager.chunkManager.CreateChunkGameobject(chunkName + i, myTransform, position, Quaternion.identity);
            chunk.parentName = chunkName;
            childNames[i] = chunk.chunkName;
            chunk.absolutePosition = position;
        }
    }
}
