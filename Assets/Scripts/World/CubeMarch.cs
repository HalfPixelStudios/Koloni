using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class CubeMarch : MonoBehaviour {

    //public GameObject meshObject;
    public string activation;

    Vector3[] cubepoints = new Vector3[8] {
        new Vector3(0,0,0),new Vector3(0,0,1),new Vector3(1,0,1),new Vector3(1,0,0),
        new Vector3(0,1,0),new Vector3(0,1,1),new Vector3(1,1,1),new Vector3(1,1,0)
    };

    public void Generate() {
        RenderMesh(GenerateMesh());
        
    }

    public Mesh GenerateMesh() {

        List<Vector3> verticies_raw = new List<Vector3>();

        int lookup_index = System.Convert.ToInt32(Utils.ReverseString(activation), 2); //remember that binary is read backwards

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
            Debug.Log($"{cubepoints[vertA]} and {cubepoints[vertB]}");
            Vector3 point = (cubepoints[vertA] + cubepoints[vertB])/2;
            verticies_raw.Add(point);
            Debug.Log(point);
        }

        Vector3[] verticies = verticies_raw.ToArray();
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

    public void RenderMesh(Mesh mesh) {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        meshFilter.sharedMesh = mesh;

    }

    
    
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



}
