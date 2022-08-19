using Common.Math.TileMap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EdgeMap : MonoBehaviour
{
    public Tilemap blockMap;
    public Tilemap edgeMap;

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

        foreach(var elem in Enumerable.Range(0, 10).Select(x => (x:Random.Range(-20, 20), y:Random.Range(-20, 20))))
        {
            var center = new Vector3Int(elem.x, elem.y);
            blockMap.SetTileColor(center, tile, Color.red);

            var axial = Hexagon.ToAxial(elem);
            var newAxial = (axial.q * 2, axial.r * 2);
            var offset = Hexagon.ToOffset(newAxial);
            edgeMap.SetTileColor(new Vector3Int(offset.x, offset.y), tile, Color.green);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
