using Common.Math.TileMap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RingMap : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {
        var center = (-37, -34);
        
        foreach(var pos in Hexagon.GetRing(center, 13).Prepend(center))
        {
            tileMap.SetTileColor(new Vector3Int(pos.Item1, pos.Item2), tile, Color.white);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
