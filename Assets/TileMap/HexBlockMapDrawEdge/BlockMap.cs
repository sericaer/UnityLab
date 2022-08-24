using Common.Math.TileMap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockMap : MonoBehaviour
{
    public Tilemap tileMap;
    public Sprite sprite;

    public Tile tile
    {
        get
        {
            if (_tile == null)
            {
                _tile = ScriptableObject.CreateInstance<Tile>();
                _tile.sprite = sprite;
            }

            return _tile;
        }
    }

    private Tile _tile;

    public void SetBlocks(Block[] blocks)
    {
        var colors = new HashSet<Color>();
        while (colors.Count < blocks.Length)
        {
            colors.Add(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        }

        for (int i = 0; i < blocks.Length; i++)
        {
            foreach (var elem in blocks[i].elements)
            {
                var pos = new Vector3Int(elem.x, elem.y, 0);
                tileMap.SetTileColor(pos, tile, colors.ElementAt(i));
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
