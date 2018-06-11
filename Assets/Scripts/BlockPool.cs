using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPool : MonoBehaviour {

    public GameObject blockPreFab;
    private Queue<GameObject> blockPool = new Queue<GameObject>();

    // Parametros - Sustituír luego por worldfenerator y pillar cosas de ahi
    public int chunkNumber = 0;
    public int chunksBlockWidth = 0;
    public int chunksBlockHeight = 0;
    public int chunksBlockDepth = 0;

    private int blockTotal;

	// Use this for initialization
	void Start ()
    {
        blockTotal = chunkNumber * chunksBlockWidth * chunksBlockHeight * chunksBlockDepth;
        for (int i = 0; i < chunkNumber; i++)
            for (int x = 0; x < chunksBlockWidth; x++)
                for (int y = 0; y < chunksBlockHeight; y++)
                    for(int z = 0; z < chunksBlockDepth; z++)
                    {
                        GameObject block = Instantiate(blockPreFab, transform.position, Quaternion.identity, transform);
                        block.SetActive(false);
                        blockPool.Enqueue(block);
                    }
	}
	
    public GameObject getBlockFromQueue()
    {
        
        if (blockPool.Count == 0)
        {
            Debug.LogError("Se acabó el pool");
            return Instantiate(blockPreFab, transform.position, Quaternion.identity);
        }
        GameObject block = blockPool.Dequeue();

        return block;
    }

    public void addBlockToQueue(GameObject block)
    {
        block.SetActive(false);
        block.transform.parent = transform;
        blockPool.Enqueue(block);
    }

    public void refillQueue()
    {
        for (int i = blockPool.Count; i < blockTotal; i++)
            StartCoroutine(createBlock(i));
    }

    public IEnumerator createBlock(int timeToWait = 0)
    {
        yield return new WaitForSecondsRealtime(timeToWait * 1f);

        GameObject block = Instantiate(blockPreFab, transform.position, Quaternion.identity);
        block.SetActive(false);
        blockPool.Enqueue(block);
    }
}
