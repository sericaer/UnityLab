using Common.Math.TileMap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HexBlockMapDrawEdge : MonoBehaviour
{
    public BlockMap blockMap;
    public EdgeMap edgeMap;
    public BorderLines borderLines;

    public Grid mapGrid;

    // Start is called before the first frame update
    void Start()
    {

        var builderGroup = new Block.BuilderGroup(100);
        var blocks = builderGroup.Build();

        blockMap.SetBlocks(blocks);
        
        var edges = GenerateEdges(blocks);
        edgeMap.SetEdges(edges);

        var lines = GenerateBorderLines(edges);
        borderLines.SetLines(lines);
    }

    private IEnumerable<(Vector3 p1, Vector3 p2)> GenerateBorderLines(IEnumerable<(int x, int y)> edges)
    {
        var cellLines = new List<((int x, int y) p1, (int x, int y) p2)>();

        var edgeList = new LinkedList<(int x, int y)>(edges);
        while(edgeList.Count() != 0)
        {
            var curr = edgeList.First;
            var lines = Hexagon.GetNeighbors(curr.Value).Where(n => edges.Contains(n)).Select(n => (p1: curr.Value, p2: n));

            edgeList.RemoveFirst();
            foreach(var line in lines)
            {
                edgeList.Remove(line.p2);
            }
            cellLines.AddRange(lines);
        }

        return cellLines.Select(line => (
            mapGrid.CellToWorld(new Vector3Int(line.p1.x, line.p1.y)) / 2,
            mapGrid.CellToWorld(new Vector3Int(line.p2.x, line.p2.y)) / 2));
    }

    private IEnumerable<(int x, int y)> GenerateEdges(Block[] blocks)
    {
        var dict = new Dictionary<(int x, int y), Block>();
        foreach (var block in blocks)
        {
            foreach (var edge in block.edges.Select(e => Hexagon.ScaleOffset(e, 2)))
            {
                if (dict.ContainsKey(edge))
                {
                    continue;
                }
                dict.Add(edge, block);
            }
        }

        var rlst = new List<(int x, int y)>();
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
            rlst.AddRange(edges);
        }

        return rlst.Distinct();
    }
}
