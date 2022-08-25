using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EdgeMap : MonoBehaviour
{
    public Tilemap tileMap;

    public Sprite[] egdes;

    private Tile[] _tiles;

    public void SetEdges(Dictionary<(int x, int y), int> egdeIndexs)
    {
        foreach (var pair in egdeIndexs)
        {
            var pos = new Vector3Int(pair.Key.x, pair.Key.y, 0);

            tileMap.SetTileColor(pos, GetTile(pair.Value), Color.white) ;
        }
    }

    private Tile GetTile(int index)
    {
        if(_tiles == null)
        {
            _tiles = egdes.Select(x =>
            {
                var _tile = ScriptableObject.CreateInstance<Tile>();
                _tile.sprite = x;
                return _tile;
            }).ToArray();
        }

        return _tiles[index];
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
