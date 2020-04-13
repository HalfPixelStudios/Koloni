using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalContainer;

public class ChunkLoader : MonoBehaviour {

    [Range(1,300)] public float view_distance;
    public GameObject viewer;

    Dictionary<Vector2, TerrainChunk> loaded_chunks = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> loaded = new List<TerrainChunk>();
    Vector2 viewer_pos;
    int visible_chunks;

    int chunkSize;
    void Start() {
        chunkSize = Global.mapGenerator.chunkSize;
        visible_chunks = Mathf.RoundToInt(view_distance / chunkSize);
    }

    void Update() {
        viewer_pos = new Vector2(viewer.transform.position.x,viewer.transform.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks() {

        //Reset all chunks
        foreach (TerrainChunk chunk in loaded) {
            chunk.setVisible(false);
        }
        loaded.Clear();

        Vector2 viewerChunkCoord = new Vector2(Mathf.RoundToInt(viewer_pos.x/chunkSize), Mathf.RoundToInt(viewer_pos.y / chunkSize));
        for (int j = -visible_chunks; j <= visible_chunks; j++) {
            for (int i = -visible_chunks; i <= visible_chunks; i++) {
                Vector2 newChunkPos = new Vector2((int)(viewerChunkCoord.x+i),(int)(viewerChunkCoord.y+j));

                if (loaded_chunks.ContainsKey(newChunkPos)) { //if the chunk has already been generated

                    TerrainChunk chunk = loaded_chunks[newChunkPos];

                    if (chunk.getDistanceToEdge(viewer_pos) < view_distance) {
                        chunk.setVisible(true);
                        loaded.Add(chunk);
                    }

                } else {
                    loaded_chunks.Add(newChunkPos, new TerrainChunk(newChunkPos)); //Generate a new chunk
                }
            }
        }
    }

    class TerrainChunk {

        GameObject meshObject;

        Bounds bounds; //used for finding distance

        public TerrainChunk(Vector2 grid_pos) {
            int chunkSize = Global.mapGenerator.chunkSize;
            Vector2 pos = grid_pos * chunkSize;
            bounds = new Bounds(pos, Vector2.one * chunkSize);


            meshObject = Global.mapGenerator.GenerateChunk(grid_pos);

            setVisible(false);
        }

        public void setVisible(bool visibility) {
            meshObject.SetActive(visibility);
        }

        public float getDistanceToEdge(Vector2 pos) { //get shortest distance from edge of chunk to a point
            return Mathf.Sqrt(bounds.SqrDistance(pos));
        }
    }
}


