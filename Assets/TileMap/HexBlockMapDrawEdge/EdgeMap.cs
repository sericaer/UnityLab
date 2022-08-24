using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EdgeMap : MonoBehaviour
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

    public void SetEdges(IEnumerable<(int x, int y)> cellIndexs)
    {
        foreach (var elem in cellIndexs)
        {
            var pos = new Vector3Int(elem.x, elem.y, 0);
            tileMap.SetTileColor(pos, tile, Color.white);
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
