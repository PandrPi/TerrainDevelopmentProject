using UnityEngine;

public struct ChunkData
{
    public Transform trs;
    public Matrix4x4 transformationMatrix;
    public MeshFilter mf;
    public MeshCollider mc;

    public ChunkData(Transform transform)
    {
        this.trs = transform;
        this.transformationMatrix = Matrix4x4.TRS(trs.position, trs.rotation, Vector3.one);
        this.mf = transform.GetComponent<MeshFilter>();
        this.mc = transform.GetComponent<MeshCollider>();
    }
}
