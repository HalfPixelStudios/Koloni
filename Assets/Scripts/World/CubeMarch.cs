using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class CubeMarch : MonoBehaviour {

    [Range(0.0001f, 10f)] public float zoom;

    [Range(1, 100)] public int chunkSize;
    [Range(0f, 1f)] public float surface_level;
    float[,,] density_map;

    //public GameObject meshObject;
    //public string activation;

    Vector3[] cubepoints = new Vector3[8] {
        new Vector3(0,0,0),new Vector3(0,0,1),new Vector3(1,0,1),new Vector3(1,0,0),
        new Vector3(0,1,0),new Vector3(0,1,1),new Vector3(1,1,1),new Vector3(1,1,0)
    };

    public void Generate() {

        //Generate density map from noise 
        density_map = new float[chunkSize+1, chunkSize+1, chunkSize+1];
        for (int k = 0; k < chunkSize+1; k++) {
            for (int j = 0; j < chunkSize+1; j++) {
                for (int i = 0; i < chunkSize+1; i++) {
                    density_map[k,j,i] = Perlin3D(i/zoom,j/zoom,k/zoom);
                }
            }
        }


        RenderMesh(GenerateMesh());
        
    }

    public Mesh GenerateMesh() {
        List<Vector3> raw_verticies = new List<Vector3>();

        //for each cube
        for (int k = 0; k < chunkSize; k++) {
            for (int j = 0; j < chunkSize; j++) {
                for (int i = 0; i < chunkSize; i++) {
                    List<Vector3> sub_verts = NextCube(i, j, k);

                    foreach (Vector3 v in sub_verts) {
                        raw_verticies.Add(v);
                    }
                }
            }
        }

        Vector3[] verticies = raw_verticies.ToArray();
        int[] triangles = new int[verticies.Length];
        for (int i = 0; i < verticies.Length; i++) {
            triangles[i] = i;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;

    }

    public List<Vector3> NextCube(int x, int y, int z) {

        List<Vector3> verticies = new List<Vector3>();

        int lookup_index = TriangulationLookup(x,y,z);

        //Grab edge positions for triangle verticies
        int[] triangulation = new int[16];
        for (int i = 0; i < 16; i++) {
            triangulation[i] = Triangulation.triangulation[lookup_index, i];
        }
        
        foreach (int edgeInd in triangulation) {
            if (edgeInd == -1) { break; }

            //get the two verticies that make up that edge
            int vertA = Triangulation.cornerIndexAFromEdge[edgeInd];
            int vertB = Triangulation.cornerIndexBFromEdge[edgeInd];

            //find position of point
            Vector3 pointA = new Vector3(x + cubepoints[vertA].x, y + cubepoints[vertA].y, z + cubepoints[vertA].z);
            Vector3 pointB = new Vector3(x + cubepoints[vertB].x, y + cubepoints[vertB].y, z + cubepoints[vertB].z);
            Vector3 point = (pointA + pointB)/2;
            verticies.Add(point);
        }
        return verticies;

    }

    public void RenderMesh(Mesh mesh) {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        meshFilter.sharedMesh = mesh;

    }

    public int TriangulationLookup(int x, int y, int z) { //return the 'case' number
        string bin = "";

        for (int i = 0; i < 8; i++) {
            Vector3 vertex_pos = new Vector3(x+cubepoints[i].x,y+cubepoints[i].y,z+cubepoints[i].z);

            if (density_map[(int)vertex_pos.z,(int)vertex_pos.y,(int)vertex_pos.x] >= surface_level) {
                bin = "1"+bin;
            } else {
                bin = "0"+bin;
            }
        }

        return System.Convert.ToInt32(bin, 2);
    }

    public float Perlin3D(float x, float y, float z) {
        //from https://www.youtube.com/watch?v=Aga0TBJkchM

        float xy = Mathf.PerlinNoise(x, y);
        float yz = Mathf.PerlinNoise(y, z);
        float xz = Mathf.PerlinNoise(x, z);
        float yx = Mathf.PerlinNoise(y, x);
        float zy = Mathf.PerlinNoise(z, y);
        float zx = Mathf.PerlinNoise(z, x);

        return (xy + yz + xz + yx + zy + zx) / 6f;
    }

    /*
    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector3(0.5f,0.5f,0.5f),new Vector3(1,1,1));
        if (activation.Length != 8) { return; }
        for (int i = 0; i < 8; i++) {
            char c = activation[i];
            if (c == '1') {
                Gizmos.color = Color.white;
            } else {
                Gizmos.color = Color.black;
            }

            Gizmos.DrawSphere(cubepoints[i],0.05f);
        }
    }
    */
    private void OnDrawGizmos() {

        if (density_map == null) { return;  }
        for (int k = 0; k < chunkSize + 1; k++) {
            for (int j = 0; j < chunkSize + 1; j++) {
                for (int i = 0; i < chunkSize + 1; i++) {
                    //Gizmos.color = new Color((float)i/(chunkSize+1),(float)j/(chunkSize+1),(float)k/(chunkSize+1));
                    Gizmos.color = Color.Lerp(Color.black,Color.white,density_map[k,j,i]);
                    Gizmos.DrawSphere(new Vector3(i,j,k),0.05f);
                }
            }
        }
    }



}
