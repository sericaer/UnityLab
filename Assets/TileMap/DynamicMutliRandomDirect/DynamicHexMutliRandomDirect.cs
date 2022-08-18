using Common.Math.TileMap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DynamicHexMutliRandomDirect : MonoBehaviour
{
    public Tilemap tilemap;
    public Sprite sprite;

    public Block.BuilderGroup builderGroup;

    public HashSet<Color> colors;

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
        //colors = new HashSet<Color>();
        //while (colors.Count < 100)
        //{
        //    colors.Add(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        //}

        builderGroup = new Block.BuilderGroup(50);

        //StartCoroutine(OnTimer());

        var blocks = builderGroup.Build();

        colors = new HashSet<Color>();
        while (colors.Count < blocks.Length)
        {
            colors.Add(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        }

        for (int i = 0; i < blocks.Length; i++)
        {
            foreach (var elem in blocks[i].edges)
            {
                var pos = new Vector3Int(elem.x, elem.y, 0);
                SetTileColor(pos, colors.ElementAt(i));
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator OnTimer()
    {
        yield return new WaitForSeconds(1);

        var stepResults = builderGroup.BuildInStep();
        for(int i=0; i< stepResults.Length; i++)
        {
            foreach(var elem in stepResults[i].elements)
            {
                var pos = new Vector3Int(elem.x, elem.y, 0);
                SetTileColor(pos, colors.ElementAt(i));
            }
        }

        StartCoroutine(OnTimer());
    }

    private void SetTileColor(Vector3Int pos, Color color)
    {
        tilemap.SetTile(pos, tile);
        tilemap.SetTileFlags(pos, TileFlags.None);
        tilemap.SetColor(pos, color);
    }
}