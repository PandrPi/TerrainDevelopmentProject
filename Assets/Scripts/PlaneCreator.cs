using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaneCreator : MonoBehaviour
{
    public static Mesh Create (float size, int segmentCount) {
        Mesh mesh = new Mesh();

        float length = size;
        float width = size;
        int resX = segmentCount;
        int resZ = segmentCount;

		Vector3[] vertices = new Vector3[resX * resZ];
        for (int z = 0; z < resZ; z++)
        {
            // [ -length / 2, length / 2 ]
            float zPos = ((float)z / (resZ - 1)) * length;
            for (int x = 0; x < resX; x++)
			{
                float xPos = ((float)x / (resX - 1)) * width;
				vertices [x + z * resX] = new Vector3 (xPos, 0, zPos);
            }
        }
			
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int v = 0; v < resZ; v++)
        {
            for (int u = 0; u < resX; u++)
            {
                uvs[u + v * resX] = new Vector2((float)u / (resX - 1), (float)v / (resZ - 1));
            }
        }

		int nbFaces = (resX - 1) * (resZ - 1);
        int[] triangles = new int[nbFaces * 6];
        int t = 0;
		for (int face = 0; face < nbFaces; face++) {
			// Retrieve lower left corner from face ind
			int i = face % (resX - 1) + (face / (resZ - 1) * resX);

			triangles [t++] = i + resX;
			triangles [t++] = i + 1;
			triangles [t++] = i;

			triangles [t++] = i + resX;
			triangles [t++] = i + resX + 1;
			triangles [t++] = i + 1;
		}

		mesh.vertices = vertices;
		mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
		mesh.RecalculateNormals();
        mesh.RecalculateTangents();
       
        return mesh;
	}

    private static void AddTriangle(ref List<Vector3> verts, ref List<Vector2> uvs,
        ref List<int> tris, int v1, int v2, int v3, Vector3 v, Vector2 uv)
    {
        verts.Add(v);
        int v4 = verts.Count - 1;
        tris.Add(v1); tris.Add(v2); tris.Add(v4);
        tris.Add(v4); tris.Add(v2); tris.Add(v3);
        uvs.Add(uv);
    }
}

/*
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaneCreator : MonoBehaviour {
	
	public static Mesh Create (float size, int segmentCount) {
        Mesh mesh = new Mesh();

        float length = size;
        float width = size;
        int resX = segmentCount;
        int resZ = segmentCount;

		Vector3[] vertices = new Vector3[resX * resZ];
        for (int z = 0; z < resZ; z++)
        {
            // [ -length / 2, length / 2 ]
            float zPos = ((float)z / (resZ - 1) - .5f) * length;
            for (int x = 0; x < resX; x++)
			{
                float xPos = ((float)x / (resX - 1) - .5f) * width;
				vertices [x + z * resX] = new Vector3 (xPos, 0, zPos);
            }
        }
			
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int v = 0; v < resZ; v++)
        {
            for (int u = 0; u < resX; u++)
            {
                uvs[u + v * resX] = new Vector2((float)u / (resX - 1), (float)v / (resZ - 1));
            }
        }

		int nbFaces = (resX - 1) * (resZ - 1);
        int[] triangles = new int[nbFaces * 6];
        int t = 0;
		for (int face = 0; face < nbFaces; face++) {
			// Retrieve lower left corner from face ind
			int i = face % (resX - 1) + (face / (resZ - 1) * resX);

			triangles [t++] = i + resX;
			triangles [t++] = i + 1;
			triangles [t++] = i;

			triangles [t++] = i + resX;
			triangles [t++] = i + resX + 1;
			triangles [t++] = i + 1;
		}

		mesh.vertices = vertices;
		mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
		mesh.RecalculateNormals ();
       
        return mesh;
	}
}
*/
