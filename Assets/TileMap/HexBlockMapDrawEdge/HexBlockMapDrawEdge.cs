using Common.Math.TileMap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexBlockMapDrawEdge : MonoBehaviour
{
    public Tilemap blockMap;
    public Tilemap edgeMap;
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
            foreach (var elem in blocks[i].elements)
            {
                var pos = new Vector3Int(elem.x, elem.y, 0);
                blockMap.SetTileColor(pos, tile, colors.ElementAt(i));
            }
        }

        var dict = new Dictionary<(int x, int y), Block>();
        foreach(var block in blocks)
        {
            foreach(var edge in block.edges.Select(e=>Hexagon.ScaleOffset(e, 2)))
            {
                dict.Add(edge, block);
            }
        }

        var edgeCenters = blocks.SelectMany(x => x.edges)
            .Select(x => Hexagon.ScaleOffset(x, 2))
            .ToHashSet();

        foreach (var elem in edgeCenters)
        {
            var edges = Hexagon.GetNeighbors(elem);
            edges = edges.Where(e =>
            {
                var neighbors = Hexagon.GetNeighbors(e)
                    .Where(n => edgeCenters.Contains(n));

                var nCount = neighbors.Select(n => dict[n]).Distinct().Count();
                return nCount > 1;
            });

            foreach(var edge in edges)
            {
                var pos = new Vector3Int(edge.x, edge.y, 0);
                edgeMap.SetTileColor(pos, tile, Color.white);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    //IEnumerator OnTimer()
    //{
    //    yield return new WaitForSeconds(1);

    //    var stepResults = builderGroup.BuildInStep();
    //    for(int i=0; i< stepResults.Length; i++)
    //    {
    //        foreach(var elem in stepResults[i].elements)
    //        {
    //            var pos = new Vector3Int(elem.x, elem.y, 0);
    //            SetTileColor(pos, colors.ElementAt(i));
    //        }
    //    }

    //    StartCoroutine(OnTimer());
    //}

    //private void SetTileColor(Vector3Int pos, Color color)
    //{
    //    blockMap.SetTile(pos, tile);
    //    blockMap.SetTileFlags(pos, TileFlags.None);
    //    blockMap.SetColor(pos, color);
    //}
}