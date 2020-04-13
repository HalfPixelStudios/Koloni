using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class CubeMarch : MonoBehaviour {

    public int seed;
    public Vector3 offset;
    public float noiseScale;
    public float noiseWeight;
    public int octaves;
    public float persistence;
    public float lacunarity;

    public Gradient terrain_gradient;
    public bool smooth_terrain;
    public bool drawGizmo;

    [Range(1, 100)] public int chunkSize;
    [Range(1, 100)] public int chunkHeight;
    [Range(0f, 1f)] public float surface_level;
    float[,,] density_map;

    //public GameObject meshObject;
    //public string activation;

    Vector3[] cubepoints = new Vector3[8] {
        new Vector3(0,0,0),new Vector3(0,0,1),new Vector3(1,0,1),new Vector3(1,0,0),
        new Vector3(0,1,0),new Vector3(0,1,1),new Vector3(1,1,1),new Vector3(1,1,0)
    };

    void OnValidate() { //called when one variable is changed
        //clamp all values
        if (noiseScale <= 0) { noiseScale = 0.0001f; }
        if (lacunarity < 1) { lacunarity = 1; }
        if (octaves < 0) { octaves = 0; }
    }

    public void Generate() {

        //Generate density map from noise 
        density_map = Noise.GenerateNoiseMap(seed, chunkSize, chunkHeight, offset, noiseScale, noiseWeight, octaves, persistence, lacunarity);

        RenderMesh(GenerateMesh());
        
    }

    public Mesh GenerateMesh() {
        List<Vector3> raw_verticies = new List<Vector3>();

        //for each cube
        for (int k = 0; k < chunkSize; k++) {
            for (int j = 0; j < chunkHeight; j++) {
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
        Color[] colors = new Color[verticies.Length];

        float minHeight = float.MaxValue;
        float maxHeight = float.MinValue;
        for (int i = 0; i < verticies.Length; i++) {
            triangles[i] = i; //assign triangle data

            //find min and maxheights
            float height = verticies[i].y;

            if (height < minHeight) { minHeight = height; }
            if (height > maxHeight) { maxHeight = height; }
        }

        //Color each vertex based on its y position
        for (int i = 0; i < verticies.Length; i++) {
            float height = verticies[i].y;
            colors[i] = terrain_gradient.Evaluate(Mathf.InverseLerp(minHeight,maxHeight,height));
        }


        Mesh mesh = new Mesh();
        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();

        return mesh;

    }

    public List<Vector3> NextCube(int x, int y, int z) {

        List<Vector3> verticies = new List<Vector3>();

        //find desnity values at all 8 edges
        float[] densities = new float[8];
        string bin = "";
        for (int i = 0; i < 8; i++) {
            Vector3 vertex_pos = new Vector3(x + cubepoints[i].x, y + cubepoints[i].y, z + cubepoints[i].z);

            densities[i] = density_map[(int)vertex_pos.z, (int)vertex_pos.y, (int)vertex_pos.x];

            if (densities[i] >= surface_level) {
                bin = "1" + bin;
            } else {
                bin = "0" + bin;
            }
        }
        
        int lookup_index = System.Convert.ToInt32(bin, 2);

        //if cube is in empty/full space, dont bother
        if (lookup_index == 0 || lookup_index == 255) {
            return verticies; 
        }

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

            if (smooth_terrain) {
                float densityA = densities[vertA];
                float densityB = densities[vertB];

                float percentage = (surface_level - densityA) / (densityB - densityA);
                verticies.Add(pointA + (pointB - pointA) * percentage);
            } else {
                verticies.Add((pointA + pointB)/2f);
            }

        }
        return verticies;

    }

    public void RenderMesh(Mesh mesh) {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        meshFilter.sharedMesh = mesh;

    }
    
    
    private void OnDrawGizmos() {
        if (!drawGizmo) { return; }
        if (density_map == null) { return;  }
        for (int k = 0; k < chunkSize + 1; k++) {
            for (int j = 0; j < chunkSize + 1; j++) {
                for (int i = 0; i < chunkSize + 1; i++) {
                    //Gizmos.color = new Color((float)i/(chunkSize+1),(float)j/(chunkSize+1),(float)k/(chunkSize+1));
                    Gizmos.color = Color.Lerp(new Color(0,0,0,0.5f), new Color(1f,1f,1f,0.5f), density_map[k,j,i]);

                    if (density_map[k, j, i] < surface_level) {
                        Gizmos.DrawSphere(new Vector3(i, j, k), 0.03f);
                    }
                }
            }
        }
    }
    



}
