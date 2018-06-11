using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour {

    // BlockPrefab
    private GameObject blockPrefab;

    // Height limit values
    private int treeMaxHeight = 9;
    private int treeMinHeight = 5;

    // Branch limit lenght
    private int minBranchLength = 1;
    private int maxBranchLength = 4;

    // Private values
    private int treeHeight = 0;
    private int branches = 0;

    // Tree Blocks
    private Dictionary<Vector3, GameObject> treeBlocks = new Dictionary<Vector3, GameObject>();

    private List<GameObject> blocks;

    // Use this for initialization
    void Start ()
    {
        blockPrefab = (GameObject)Resources.Load("Cube");
        treeHeight = Random.Range(treeMinHeight, treeMaxHeight);
        branches = Random.Range(2, 4);

        float randomValue = Random.Range(0f, 1f);
        placeTrunk();

        if (randomValue > 0.5f)
            placeLeaves();
        else
            placeLeavesOak();

        blocks = new List<GameObject>(treeBlocks.Values);
        StartCoroutine(combineRenderMeshesByChunkType(Block.BlockType.BLOCK_TYPE_WOOD));
        StartCoroutine(combineRenderMeshesByChunkType(Block.BlockType.BLOCK_TYPE_LEAF, 10));

        foreach(GameObject block in blocks)
        {
            block.SetActive(false);
        }
    }

    public void placeBranch(int y)
    {
        int direction = Random.Range(0, 5);

        switch (direction)
        {
            case 1:
                for (int i = 1; i < minBranchLength + 1; i++)
                {
                    placeWoodBlock(new Vector3(i, y, 0));
                }
                break;
            case 2:
                for (int i = 1; i < minBranchLength + 1; i++)
                {
                    placeWoodBlock(new Vector3(0, y, i));
                }
                break;
            case 3:
                for (int i = 1; i < minBranchLength + 1; i++)
                {
                    placeWoodBlock(new Vector3(-i, y, 0));
                }
                break;
            default:
                for (int i = 1; i < minBranchLength + 1; i++)
                {
                    placeWoodBlock(new Vector3(0, y, -i));
                }
                break;
        }

        branches--;
    }

    public void placeTrunk()
    {
        for (int i = 0; i < treeHeight; i++)
        {
            placeWoodBlock(new Vector3(0, i, 0));
            if (i < treeHeight / 2 && branches > 0)
            {
                float chance = 0.25f * i;
                float randomValue = Random.Range(0f, 1f);

                if (chance > randomValue)
                    placeBranch(i);
            }
        }
    }

    public void placeLeaves()
    {
        List<Vector3> keys = new List<Vector3>(treeBlocks.Keys);

        foreach (Vector3 key in keys)
        {
            GameObject woodBlock = treeBlocks[key];

            Vector3 pos = woodBlock.transform.position;

            if (pos.y < (float)treeHeight / 1.5f)
                continue;

            List<Vector3> vectorList = new List<Vector3>();

            for (int i = -2; i < 3; i++)
                for (int k = -1; k < 2; k++)
                    for (int j = -2; j < 3; j++)
                    {
                        vectorList.Add(new Vector3(pos.x + i, pos.y + k, pos.z + j));
                    }

            foreach (Vector3 vector in vectorList)
                if (!treeBlocks.ContainsKey(vector))
                {
                    GameObject trunk = GameObject.Instantiate(blockPrefab, vector, Quaternion.identity, transform);
                    trunk.GetComponent<Block>().setBlockType(Block.BlockType.BLOCK_TYPE_LEAF);
                    treeBlocks.Add(vector, trunk);
                }
        }
    }

    public void placeLeavesOak()
    {
        for (int l = 0; l < 3; l++)
        {
            GameObject woodBlock = treeBlocks[new Vector3(0, treeHeight - l - 1, 0)];
            Vector3 pos = woodBlock.transform.position;
            int currentHeight = treeHeight - l;

            if (pos.y < (float)treeHeight / 2f)
                continue;

            List<Vector3> vectorList = new List<Vector3>();

            for (int i = -1 - l; i < 1 + l; i++)
                for (int j = -1 - l; j < 1 + l; j++)
                {
                    vectorList.Add(new Vector3(i, currentHeight, j));
                }

            foreach (Vector3 vector in vectorList)
                if (!treeBlocks.ContainsKey(vector))
                {
                    GameObject trunk = GameObject.Instantiate(blockPrefab, vector, Quaternion.identity, transform);
                    trunk.GetComponent<Block>().setBlockType(Block.BlockType.BLOCK_TYPE_LEAF);
                    treeBlocks.Add(vector, trunk);
                }
        }
    }

    public void placeWoodBlock(Vector3 pos)
    {
        GameObject trunk = GameObject.Instantiate(blockPrefab, pos, Quaternion.identity, transform);
        trunk.GetComponent<Block>().setBlockType(Block.BlockType.BLOCK_TYPE_WOOD);
        treeBlocks.Add(pos, trunk);
    }

    private IEnumerator combineRenderMeshesByChunkType(Block.BlockType blockType, int timeToWait = 0)
    {
        List<MeshFilter> meshFiltersList = new List<MeshFilter>();

        foreach (GameObject block in blocks)
            if (block.GetComponent<Block>().GetBlockType() == blockType)
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
    }
}
