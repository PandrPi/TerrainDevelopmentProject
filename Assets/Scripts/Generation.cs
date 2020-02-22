using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

public class Generation : MonoBehaviour
{
    [SerializeField] private TaskExecutionManager taskExecutionManager;
    [SerializeField] private ChunkManager chunkManager;
    [SerializeField] private int pointsNumber;
    [SerializeField] private int constructDivisionNumber;
    [SerializeField] private int worldSize;
    [SerializeField] private int chunkSize;
    [SerializeField] private GameObject chunkPrefab;
    [SerializeField] private FastNoiseSIMD fastNoise1;
    [SerializeField] private FastNoiseSIMD fastNoise2;

    private Queue<Action> actionsQueue = new Queue<Action>();
    private Queue<JobHandle> jobHandles = new Queue<JobHandle>();
    private Matrix4x4 worldMatrix;
    private Mesh gridMesh;
    private int[] triangles;
    private Vector2[] uvs;
    private Vector4[] tangents;

    private Transform myTransform;
    void Start()
    {
        myTransform = transform;
        taskExecutionManager.Initialize();
        chunkManager.Initialize();

        worldMatrix = Matrix4x4.TRS(myTransform.position, myTransform.rotation, Vector3.one);
        gridMesh = PlaneCreator.Create(chunkSize, pointsNumber);
        triangles = gridMesh.triangles;
        uvs = gridMesh.uv;
        tangents = gridMesh.tangents;

        var fnsuComponents = myTransform.GetComponents<FastNoiseSIMDUnity>();
        fastNoise1 = fnsuComponents[0].fastNoiseSIMD;
        fastNoise2 = fnsuComponents[1].fastNoiseSIMD;
        taskExecutionManager.noiseGroup[0] = new NoiseGroup(fastNoise1, fastNoise2);
        for (int i = 1; i < taskExecutionManager.ranTaskCount; i++)
        {
            var fn1 = new FastNoiseSIMD();
            var fn2 = new FastNoiseSIMD();
            fnsuComponents[0].InitNoise(fn1);
            fnsuComponents[1].InitNoise(fn2);
            taskExecutionManager.noiseGroup[i] = new NoiseGroup(fn1, fn2);
        }

        float f = Time.realtimeSinceStartup;

        int counter = 0;
        int halfWorldSize = worldSize / 2;
        for (int x = -halfWorldSize; x < halfWorldSize; x += chunkSize)
        {
            for (int z = -halfWorldSize; z < halfWorldSize; z += chunkSize)
            {
                Vector3 position = new Vector3(x, 0, z);
                Chunk chunk = chunkManager.CreateChunkGameobject($"Chunk {counter}", myTransform, position, Quaternion.identity);
                chunk.absolutePosition = position;
                var verts = gridMesh.vertices;
                Generate(chunk.chunkName, verts, chunk.chunkData.transformationMatrix, x, 0, z);

                counter++;
            }
        }

        Debug.Log($"Default : {(Time.realtimeSinceStartup - f) * 1000.0f} ms");
    }


    private void MyHandler(object sender, UnhandledExceptionEventArgs args)
    {
        Exception e = (Exception)args.ExceptionObject;
        Debug.Log("MyHandler caught : " + e.Message + "\nRuntime terminating: " + args.IsTerminating);
    }

    private void Generate(string chunkName, Vector3[] vertices, Matrix4x4 chunkMatrix, int xStart, int yStart, int zStart)
    {
        AppDomain app1 = AppDomain.CurrentDomain;
        app1.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);

        Matrix4x4 ChunkToPlanet = worldMatrix.inverse * chunkMatrix;
        Matrix4x4 PlanetToChunk = chunkMatrix.inverse * worldMatrix;

        float[] noise1 = fastNoise1.GetEmptyNoiseSet(pointsNumber, pointsNumber, 1);
        float[] noise2 = fastNoise2.GetEmptyNoiseSet(pointsNumber, pointsNumber, 1);
        fastNoise1.FillSampledNoiseSetVector(noise1, new FastNoiseSIMD.VectorSet(vertices), xStart, yStart, zStart);
        fastNoise2.FillSampledNoiseSetVector(noise2, new FastNoiseSIMD.VectorSet(vertices), xStart, yStart, zStart);

        var newDataGroup = new ModifyVerticesDataGroup(vertices, noise1, noise2, ChunkToPlanet, PlanetToChunk);
        var job = new ModifyVerticesJob
        {
            dataGroup = newDataGroup
        };

        var jobHandle = job.Schedule(vertices.Length, 250);

        actionsQueue.Enqueue(() =>
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            jobHandle.Complete();
            newDataGroup.Vertices.CopyTo(vertices);
            newDataGroup.Dispose();            
            chunkManager.AssignMesh(chunkName, ref vertices, ref triangles, ref uvs, ref tangents, false);

            stopwatch.Stop();
            Debug.Log($"Ellapsed time - {stopwatch.ElapsedMilliseconds} ms");
        });
    }

    void Update()
    {
        if(actionsQueue.Count > 0)
        {
            actionsQueue.Dequeue()();// dequeue the action and call it
        }

        taskExecutionManager.UpdateManager();
    }
}

public struct NoiseGroup
{
    public FastNoiseSIMD fastNoise1;
    public FastNoiseSIMD fastNoise2;

    public NoiseGroup(FastNoiseSIMD fastNoise1, FastNoiseSIMD fastNoise2)
    {
        this.fastNoise1 = fastNoise1;
        this.fastNoise2 = fastNoise2;
    }
}