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
        visible_chunks = Mathf.RoundToInt(view_distance / visible_chunks);
        chunkSize = GetComponent<CubeMarch>().chunkSize; //TEMP SOLUTION
    }

    private void Update() {
        Vector2 viewer_pos = new Vector2(viewer.transform.position.x,viewer.transform.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks() {
        Vector2 viewerChunkCoord = new Vector2(Mathf.RoundToInt(viewer_pos.x/chunkSize), Mathf.RoundToInt(viewer_pos.y / chunkSize));
        Debug.Log(viewer_pos);
        for (int z = -visible_chunks; z <= visible_chunks; z++) {
            for (int x = -visible_chunks; x <= visible_chunks; x++) {
                Vector3 newChunkPos = new Vector2((int)(viewerChunkCoord.x+x),(int)(viewerChunkCoord.y+z));

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

        Vector3 position;
        GameObject meshObject;

        Bounds bounds; //used for finding distance

        public TerrainChunk(Vector3 position, int chunkSize) {
            this.position = position;
            bounds = new Bounds(this.position, Vector2.one * chunkSize);


            meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.position = this.position;
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


