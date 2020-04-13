using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour {

    [Range(1,300)] public float view_distance;
    public GameObject viewer;

    Vector2 viewer_pos;

    int visible_chunks;

    int chunkSize;

    Dictionary<Vector2, TerrainChunk> loaded_chunks = new Dictionary<Vector2, TerrainChunk>();

    void Start() {

        chunkSize = GetComponent<CubeMarch>().chunkSize; //TEMP SOLUTION
        visible_chunks = Mathf.RoundToInt(view_distance / chunkSize);
    }

    void Update() {
        viewer_pos = new Vector2(viewer.transform.position.x,viewer.transform.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks() {
        Vector2 viewerChunkCoord = new Vector2(Mathf.RoundToInt(viewer_pos.x/chunkSize), Mathf.RoundToInt(viewer_pos.y / chunkSize));
        for (int j = -visible_chunks; j <= visible_chunks; j++) {
            for (int i = -visible_chunks; i <= visible_chunks; i++) {
                Vector2 newChunkPos = new Vector2((int)(viewerChunkCoord.x+i),(int)(viewerChunkCoord.y+j));
                Debug.Log(newChunkPos);
                if (loaded_chunks.ContainsKey(newChunkPos)) { //if the chunk has already been generated

                    TerrainChunk chunk = loaded_chunks[newChunkPos];

                    if (chunk.getDistanceToEdge(viewer_pos) < view_distance) {
                        chunk.setVisible(true);
                    }

                } else {
                    loaded_chunks.Add(newChunkPos, new TerrainChunk(newChunkPos, chunkSize)); //Generate a new chunk
                }
            }
        }
    }

    public class TerrainChunk {

        //Vector2 position;
        GameObject meshObject;

        Bounds bounds; //used for finding distance

        public TerrainChunk(Vector2 grid_pos, int chunkSize) {
            Vector2 pos = grid_pos * chunkSize;
            //this.position = position;
            bounds = new Bounds(pos, Vector2.one * chunkSize);

            meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.position = new Vector3(pos.x,0,pos.y);
            meshObject.transform.localScale = Vector3.one * chunkSize / 10f;
            this.setVisible(false);
        }

        public void setVisible(bool visibility) {
            meshObject.SetActive(visibility);
        }

        public float getDistanceToEdge(Vector2 pos) { //get shortest distance from edge of chunk to a point
            return Mathf.Sqrt(bounds.SqrDistance(pos));
        }
    }
}


