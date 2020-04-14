using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GlobalContainer;

public class CubeMarch : MonoBehaviour {

    public int seed;
    public Vector3 noiseOffset;
    public float noiseScale;
    public float noiseWeight;
    public int octaves;
    public float persistence;
    public float lacunarity;

    public Material terrainMaterial;
    public Gradient terrain_gradient;
    public bool smooth_terrain;

    [Range(1, 100)] public int chunkSize;
    [Range(1, 100)] public int chunkHeight;
    [Range(0f, 1f)] public float surface_level;

    Vector3[] cubepoints = new Vector3[8] {
        new Vector3(0,0,0),new Vector3(0,0,1),new Vector3(1,0,1),new Vector3(1,0,0),
        new Vector3(0,1,0),new Vector3(0,1,1),new Vector3(1,1,1),new Vector3(1,1,0)
    };

    public GameObject chunks; //gameobject to act as parent of generated chunks

    public int editor_pregen_size;

    private void Start() {
        clearChunks();
    }

    void OnValidate() { //called when one variable is changed
        //clamp all values
        if (noiseScale <= 0) { noiseScale = 0.0001f; }
        if (lacunarity < 1) { lacunarity = 1; }
        if (octaves < 0) { octaves = 0; }
    }

    public void GenerateInEditor() {

        clearChunks();

        for (int j = -(editor_pregen_size-1); j <= (editor_pregen_size-1); j++) {
            for (int i = -(editor_pregen_size-1); i <= (editor_pregen_size-1); i++) {
                GameObject meshObject = GenerateChunk(new Vector2(i, j)*chunkSize);
                meshObject.transform.parent = chunks.transform;
            }
        }
    }

    public void clearChunks() {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in chunks.transform) { //clear all loaded chunks

            children.Add(child.gameObject);
        }

        while (children.Count > 0) {
            GameObject first = children[0];
            DestroyImmediate(first);
            children.RemoveAt(0);
        }
    }

    public GameObject GenerateChunk(Vector2 pos) {
        //Generate density map from noise 
        Vector3 center = new Vector3(pos.x, 0, pos.y);
        float[,,] density_map = NoiseGenerator.GenerateNoiseMap(seed, chunkSize, chunkHeight, noiseOffset + center, noiseScale, noiseWeight, octaves, persistence, lacunarity);

        //Create new Object
        GameObject meshObject = new GameObject();
        meshObject.AddComponent<MeshFilter>();
        meshObject.AddComponent<MeshRenderer>();

        MeshFilter meshFilter = meshObject.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = meshObject.GetComponent<MeshRenderer>();

        Mesh mesh = GenerateMesh(density_map);
        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = terrainMaterial;

        meshObject.transform.position = new Vector3(pos.x-chunkSize/2f,0,pos.y-chunkSize/2f);
        meshObject.transform.parent = chunks.transform;
        return meshObject;
        
    }

    public Mesh GenerateMesh(float[,,] density_map) {
        List<Vector3> raw_verticies = new List<Vector3>();

        //for each cube
        for (int k = 0; k < chunkSize; k++) {
            for (int j = 0; j < chunkHeight; j++) {
                for (int i = 0; i < chunkSize; i++) {
                    List<Vector3> sub_verts = NextCube(density_map, i, j, k);

                    foreach (Vector3 v in sub_verts) {
                        raw_verticies.Add(v);
                    }
                }
            }
        }

        Vector3[] verticies = raw_verticies.ToArray();
        int[] triangles = new int[verticies.Length];
        Color[] colors = new Color[verticies.Length];

        //float minHeight = float.MaxValue;
        //float maxHeight = float.MinValue;
        for (int i = 0; i < verticies.Length; i++) {
            triangles[i] = i; //assign triangle data

            //find min and maxheights
            float height = verticies[i].y;

            //if (height < minHeight) { minHeight = height; }
            //if (height > maxHeight) { maxHeight = height; }
        }

        //Color each vertex based on its y position
        for (int i = 0; i < verticies.Length; i++) {
            float height = verticies[i].y;
            //colors[i] = terrain_gradient.Evaluate(Mathf.InverseLerp(minHeight,maxHeight,height));
            float maxHeight = NoiseGenerator.maxPossibleHeight(chunkHeight,noiseWeight,octaves,persistence);
            colors[i] = terrain_gradient.Evaluate(Mathf.InverseLerp(0, maxHeight, height));
        }


        Mesh mesh = new Mesh();
        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.colors = colors;
        mesh.RecalculateNormals();

        return mesh;

    }

    public List<Vector3> NextCube(float[,,] density_map, int x, int y, int z) {

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


}
