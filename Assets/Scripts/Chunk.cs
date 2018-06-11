using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour {

    public BlockPool blockPool;

    private Block[] blocks;
    // Chunk coords
    public Vector2 pos;

    // World generator
    public WorldGenerator wG;

    // Flag
    public bool generate = false;

    public System.DateTime date;

    // Use this for initialization
    public void init ()
    {
        if (blocks.Length == 0 )
        {
            Debug.Log("algo no va bien");
            return;
        }

        GameObject obj = GameObject.Find("WorldGenerator");
        if (obj)
            wG = obj.GetComponent<WorldGenerator>();

        BoxCollider bC = GetComponent<BoxCollider>();

        float oldValue = Time.timeScale;
        Time.timeScale = 0;
        StartCoroutine(combineRenderMeshesByChunkType(Block.BlockType.BLOCK_TYPE_GRASS));
        StartCoroutine(combineRenderMeshesByChunkType(Block.BlockType.BLOCK_TYPE_SOIL, 10));
        StartCoroutine(combineRenderMeshesByChunkType(Block.BlockType.BLOCK_TYPE_STONE, 20));
        StartCoroutine(combineRenderMeshesByChunkType(Block.BlockType.BLOCK_TYPE_WATER, 30));
        Time.timeScale = oldValue;
        bC.enabled = true;

        for (int j = 0; j < blocks.Length; j++)
        {
            /*Object.Destroy(blocks[j].gameObject);*/
            blocks[j].gameObject.SetActive(false);
            blockPool.addBlockToQueue(blocks[j].gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Alguien entró");
        CameraMove player = collision.gameObject.GetComponent<CameraMove>();

        if(player)
        {
            wG.generateChunkByNeighbor(this);
        }
    }

    public void setBlocks(Block[] blocks)
    {
        this.blocks = blocks;
    }

    private IEnumerator combineRenderMeshesByChunkType(Block.BlockType blockType, int timeToWait = 0)
    {
        List<MeshFilter> meshFiltersList =  new List<MeshFilter>();

        foreach (Block block in blocks)
            if(block.GetBlockType() == blockType)
                meshFiltersList.Add(block.GetComponent<MeshFilter>());

        if (meshFiltersList.Count == 0)
            yield return null;

        
        GameObject subChunk = (GameObject)Instantiate(Resources.Load("SubChunk"), transform);
        MeshFilter newMF = subChunk.GetComponent<MeshFilter>();
        MeshRenderer mr = subChunk.GetComponent<MeshRenderer>();
        MeshCollider mc = subChunk.GetComponent<MeshCollider>();
        
        mr.material.color = Block.getColorByBlockType(blockType);

        MeshFilter[] meshFilters = meshFiltersList.ToArray();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }
       
        newMF.mesh = new Mesh();
        //Debug.Log("La cantidad es " + combine.Length);
        yield return new WaitForSecondsRealtime(0.1f * timeToWait);

        newMF.mesh.CombineMeshes(combine);

        mc.sharedMesh = newMF.sharedMesh;
        if (blockType == Block.BlockType.BLOCK_TYPE_WATER)
        {
            float time = ((float)(System.DateTime.Now - date).TotalMilliseconds);
            Debug.Log("Tiempo de chunk: " + pos + " " + time);
            PerfomanceReport.addTimeToList(time);
        }
    }
}
