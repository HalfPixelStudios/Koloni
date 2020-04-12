using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class CubeMarch : MonoBehaviour {

    public string activation;

    Vector3[] verticies = new Vector3[] {
        new Vector3(0,0,0),new Vector3(1,0,0),new Vector3(0,0,1),new Vector3(1,0,1),
        new Vector3(0,1,0),new Vector3(1,1,0),new Vector3(0,1,1),new Vector3(1,1,1),
    };

    /*
    public void GenerateMesh() {
        int lookup_index = System.Convert.ToInt32(activation,2);

        int[] triangles = new int[16];
        for (int i = 0; i < 16; i++) {
            triangles[i] = triTable[lookup_index,i];
        }
        //Vector3[] verticies = 


            
            
    }
    */

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(new Vector3(0.5f,0.5f,0.5f),new Vector3(1,1,1));
        for (int i = 0; i < 8; i++) {
            char c = activation[i];
            if (c == '1') {
                Gizmos.color = Color.white;
            } else {
                Gizmos.color = Color.black;
            }

            Gizmos.DrawSphere(verticies[i],0.1f);
        }
    }



}
