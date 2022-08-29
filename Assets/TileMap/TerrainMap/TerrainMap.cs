using Common.Math.TileMap;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainMap : MonoBehaviour
{
    public Grid mapGrid;
    public Tilemap tileMap;
    public Sprite sprite;

    //public BlockMap blockMap;

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


    private Dictionary<TerrainType, Color> colors = new Dictionary<TerrainType, Color>()
    {
        { TerrainType.Plain, Color.green},
        { TerrainType.Hill, Color.yellow},
        { TerrainType.Mount, new Color(128 / 255f, 0, 128 / 255f)},
        { TerrainType.Water, Color.blue},
    };

    // Start is called before the first frame update
    void Start()
    {
        int mapSize = 100;
        var builderGroup = new Block.BuilderGroup(mapSize);
        var blocks = builderGroup.Build().ToArray();
        //blockMap.SetBlocks(blocks);

        var terrainBuilder = new TerrainBuilder2();
        var terrainDict = terrainBuilder.Build(blocks, mapSize);

        foreach (var pair in terrainDict)
        {
            var pos = new Vector3Int(pair.Key.x, pair.Key.y, 0);
            tileMap.SetTileColor(pos, tile, colors[pair.Value]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Debug.Log(mapGrid.WorldToCell(mousePos));
        }
    }
}
public enum TerrainType
{
    Plain,
    Hill,
    Mount,
    Water
}