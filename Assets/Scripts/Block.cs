using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    // Tipo de bloques
    public enum BlockType
    {
        BLOCK_TYPE_STONE,
        BLOCK_TYPE_SOIL,
        BLOCK_TYPE_GRASS,
        BLOCK_TYPE_WATER,
        BLOCK_TYPE_WOOD,
        BLOCK_TYPE_LEAF
    }

    // Tipo de este bloque
    private BlockType blockType = BlockType.BLOCK_TYPE_WATER;

    // Material
    private Material material;

    // Renderer
    private Renderer rend;

    // Chunk coords
    public Vector3 pos;
        
	void Awake ()
    {
        rend = GetComponent<Renderer>();
        material = rend.material;
        material.color = Color.green;
    }

    public void setBlockType(BlockType type)
    {
        blockType = type;
        switch(type)
        {
            case BlockType.BLOCK_TYPE_STONE:
                material.color = Color.gray;
                break;
            case BlockType.BLOCK_TYPE_SOIL:
                material.color = new Color32(205, 133, 63, 0);
                break;
            case BlockType.BLOCK_TYPE_GRASS:
                material.color = Color.green;
                break;
            case BlockType.BLOCK_TYPE_WATER:
                material.color = Color.blue;
                break;
            case BlockType.BLOCK_TYPE_WOOD:
                material.color = new Color32(139, 69, 19, 0);
                break;
            case BlockType.BLOCK_TYPE_LEAF:
                material.color = new Color32(0, 100, 0, 0);
                break;
        }
    }

    public BlockType GetBlockType()
    {
        return blockType;
    }

    public static Color getColorByBlockType(BlockType type)
    {
        switch (type)
        {
            case BlockType.BLOCK_TYPE_SOIL:
                return new Color32(205, 133, 63, 0);
            case BlockType.BLOCK_TYPE_GRASS:
                return Color.green;
            case BlockType.BLOCK_TYPE_WATER:
                return Color.blue;
            case BlockType.BLOCK_TYPE_WOOD:
                return new Color32(139, 69, 19, 0);
            case BlockType.BLOCK_TYPE_LEAF:
                return new Color32(0, 100, 0, 0);
            default:
                return Color.gray;
        }
    }
}
