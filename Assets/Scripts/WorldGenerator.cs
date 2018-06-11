using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldGenerator : MonoBehaviour
{
    // Const values
    public const int blockDistance = 1;
    public int maxHeight = 16;
    public const int minHeight = -5;
    public float scaler = 0.015f;
    public float seed = 0f;
    public int chunkLoadDistance = 1;

    //  Chunk values
    public int chunkWidth = 4;
    public int chunkDepth = 4;

    // Biome dimension in blocks
    public int widthBlocksInChunk = 0;
    public int depthBlocksInChunk = 0;

    // HeightMap
    private int[,] heightMap;

    // ChunkMap - Diccionario coordenadas-> Chunk
    public Dictionary<Vector2,GameObject> chunkMap;

    // BlockPool
    public BlockPool blockPool;

    // Tree Generator
    public GameObject treePreFab;

    // Generated blocks
    //private List<Block> chunkBlocks;

    // Flag
    public bool createTrees = true;

    // BlockPrefab
    public GameObject blockPrefab;
    public GameObject chunkFab;
    public GameObject subChunkFab;

    private void Start()
    {
        PerfomanceReport.firstTime = System.DateTime.Now;
        seed = PlayerPrefs.GetInt("seed");
        scaler = PlayerPrefs.GetFloat("scaler");
        createTrees = PlayerPrefs.GetInt("trees") == 1;

        int integerSeed = Mathf.RoundToInt(seed * 1000000);
        Random.InitState(integerSeed);

        chunkMap = new Dictionary<Vector2, GameObject>();

        float seconds = Time.realtimeSinceStartup;
        for (int x = 0; x < chunkWidth; x++)
            for (int z = 0; z < chunkDepth; z++)
                StartCoroutine(generateChunk(x, z));

        seconds = Time.realtimeSinceStartup - seconds;
        Debug.Log("El tiempo fue: " + seconds);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void initHeightMap(int chunkX, int chunkZ)
    {
        heightMap = new int[widthBlocksInChunk, depthBlocksInChunk];

        for (int x = 0; x < widthBlocksInChunk; x++)
            for (int z = 0; z < depthBlocksInChunk; z++)
                heightMap[x, z] = Mathf.RoundToInt(((Mathf.PerlinNoise((x + chunkX) * scaler + seed, (z + chunkZ) * scaler + seed) - 0.5f) * 2f) * maxHeight);
                //heightMap[x, z] = Mathf.RoundToInt((Mathf.PerlinNoise(x * scaler + seed, z * scaler + seed) * maxHeight));
    }

    public void placeHeightBlocks(Transform parent, bool usePool = false, List<Block> blocks = null)
    {
        for (int x = 0; x < widthBlocksInChunk; x++)
            for (int z = 0; z < depthBlocksInChunk; z++)
            {
                Block block = placeBlock(new Vector3(x, heightMap[x, z], z), parent, usePool, blocks, true);

                // Trees
                float randomValue = Random.Range(0f, 1f);
                float chance = 0.005f;
                if (block.GetBlockType() == Block.BlockType.BLOCK_TYPE_SOIL || block.GetBlockType() == Block.BlockType.BLOCK_TYPE_GRASS)
                    if (randomValue < (block.GetBlockType() == Block.BlockType.BLOCK_TYPE_GRASS ? chance*2 : chance))
                    {
                        Vector3 relativePos = new Vector3(x, heightMap[x, z] + 1, z);
                        if (createTrees)
                        {
                            GameObject tree = GameObject.Instantiate(treePreFab, parent.position + relativePos, Quaternion.identity, parent);
                        }
                        //Debug.Log("La posición del arbol es " + tree.transform.position);
                    }
                    putBlocksToBottom(x, heightMap[x, z], z, parent, usePool, blocks);
            }
    }

    public void putBlocksToBottom(int x , int y, int z, Transform parent, bool usePool = false, List<Block> blocks = null)
    {
        for (int i = 0; i < y; i++)
        {
            placeBlock(new Vector3(x, i, z), parent, usePool, blocks);
        }
    }

    public void setBlockTypeByHeight(GameObject gobj, float height , bool firstBlock = false)
    {
        Block block = gobj.GetComponent<Block>();
        if (!block)
        {
            Debug.LogError("Es null el Block");
            return;
        }

        
        if(height / maxHeight > 0.25f)
                block.setBlockType(Block.BlockType.BLOCK_TYPE_GRASS);
        else if(height / maxHeight >= -0.25f)
                block.setBlockType(Block.BlockType.BLOCK_TYPE_SOIL);
        else if (height / maxHeight >= -0.5f)
            block.setBlockType(Block.BlockType.BLOCK_TYPE_STONE);
        else
            block.setBlockType(Block.BlockType.BLOCK_TYPE_WATER);
    }

    // Generate Chunks by neighbor
    public void generateChunkByNeighbor(Chunk nei)
    {
        
        List<Vector2> offset = new List<Vector2>();

        for (int i = -chunkLoadDistance; i < chunkLoadDistance + 1; i++)
            for (int j = -chunkLoadDistance; j < chunkLoadDistance + 1; j++)
                    offset.Add(new Vector2(i, j));

        // Generating networks
        int time = 0;
        float oldValue = Time.timeScale;
        Time.timeScale = 0;
        foreach (Vector2 pos in offset)
        {
            Vector2 newPos = nei.pos + pos;

            if(!chunkMap.ContainsKey(newPos))
            {
                int x = Mathf.RoundToInt(newPos.x);
                int z = Mathf.RoundToInt(newPos.y);
                StartCoroutine(generateChunk(x, z, time, true));
                time++;
            }
            else
            {
                MeshRenderer[] meshRenderers = chunkMap[newPos].GetComponentsInChildren<MeshRenderer>();
                foreach(MeshRenderer mR in meshRenderers)
                {
                    mR.enabled = true;
                }
            }
        }

        // Disable what you cannot view
        foreach (KeyValuePair<Vector2, GameObject> entry in chunkMap)
        {
            if (!offset.Contains(entry.Key - nei.pos) && nei.pos != entry.Key)
            {
                MeshRenderer[] meshRenderers = chunkMap[entry.Key].GetComponentsInChildren<MeshRenderer>();
                foreach(MeshRenderer mR in meshRenderers)
                {
                    mR.enabled = false;
                }
            }
        }

        Time.timeScale = oldValue;
    }

    // Generate a chunk given its coords
    public IEnumerator generateChunk(int x, int z, int timeToWait = 0, bool usePool = false)
    {
        System.DateTime date = System.DateTime.Now;
        GameObject newChunk;
        Vector2 chunkPos = new Vector2(x, z);
        List<Block> chunkBlocks = new List<Block>();
        //Debug.Log("Voy a esperar " + timeToWait * 0.1f);
        
        // Summon Chunk
        Vector3 pos = new Vector3(x * widthBlocksInChunk, 0, z * depthBlocksInChunk);
        newChunk = GameObject.Instantiate(chunkFab, pos, Quaternion.identity);
        newChunk.gameObject.name = "Chunk" + x + z;
        //Debug.LogWarning("Generando chunk: " + newChunk.name);
        
        // Add to dict
        chunkMap[chunkPos] = newChunk;

        yield return new WaitForSecondsRealtime(timeToWait * 0.15f);

        // Place blokcs of this Chunk
        Transform parent = newChunk.transform;
        initHeightMap(x * widthBlocksInChunk, z * depthBlocksInChunk);
        placeHeightBlocks(parent, usePool, chunkBlocks);
        
        // Set pos to chunk
        Chunk chunk = newChunk.GetComponent<Chunk>();
        chunk.pos = chunkPos;
        chunk.setBlocks(chunkBlocks.ToArray());
        chunk.blockPool = blockPool;
        chunk.wG = this;
        chunk.date = date;
        chunk.init();  
    }

    public Block placeBlock(Vector3 pos, Transform parent, bool usePool = false, List<Block> chunkBlocks = null, bool firstTime = false)
    {
        GameObject gobj;
        if(usePool)
        {
            gobj = blockPool.getBlockFromQueue();
            //gobj.transform.parent = parent;
            gobj.transform.position = new Vector3(pos.x, pos.y, pos.z);
            gobj.transform.rotation = Quaternion.identity;
        }
        else
            gobj = GameObject.Instantiate(blockPrefab, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity, parent);


        Block block = gobj.GetComponent<Block>();
        block.pos = pos;
        setBlockTypeByHeight(gobj, pos.y, firstTime);
        chunkBlocks.Add(block);

        return block;
    }

}
