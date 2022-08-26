using Common.Math.TileMap;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class TerrainBuilder1
{
    public TerrainBuilder1()
    {
    }

    internal Dictionary<(int x, int y), TerrainType> Build(Block[] blocks, int mapSize)
    {
        var rslt = new Dictionary<(int x, int y), TerrainType>();

        var waterBlock = blocks.Where(x => x.edges.Any(r => r.y == mapSize - 1 || r.x == 0));
        foreach (var element in waterBlock.SelectMany(x=>x.elements))
        {
            rslt.Add(element, TerrainType.Water);
        }

        blocks = blocks.Except(waterBlock).ToArray();

        foreach (var elem in blocks.SelectMany(x=>x.elements))
        {
            var dist = elem.y;
            var distPercent = dist * 100 / mapSize;

            var random = Random.Range(0, 100);
            if (random > distPercent)
            {
                rslt.Add(elem, TerrainType.Hill);
            }
            else
            {
                rslt.Add(elem, TerrainType.Plain);
            }
        }

        //blocks = blocks.Except(waterBlock).ToArray();
        //var pointGroups = FindThreeBlockVexters(blocks);

        //foreach(var point in pointGroups)
        //{
        //    var distance = point.y;
        //    var distancePercent =  distance * 100 / mapSize;
        //    if(distancePercent < 25)
        //    {
        //        rslt.Add(point, TerrainType.Mount);
        //    }
        //    else
        //    {
        //        var random = Random.Range(0, 100);
        //        if (random < distancePercent)
        //        {
        //            rslt.Add(point, TerrainType.Hill);
        //        }
        //        else
        //        {
        //            rslt.Add(point, TerrainType.Plain);
        //        }
        //    }
        //}

        //foreach(var elem in blocks.SelectMany(x=>x.edges))
        //{
        //    if(rslt.ContainsKey(elem))
        //    {
        //        continue;
        //    }

        //    var distance = elem.y;
        //    var distancePercent = distance * 100 / mapSize;


        //    var random = Random.Range(0, 100);
        //    if (random < distancePercent)
        //    {
        //        rslt.Add(elem, TerrainType.Hill);
        //    }
        //    else
        //    {
        //        rslt.Add(elem, TerrainType.Mount);
        //    }
        //}

        return rslt;
    }

    private IEnumerable<(int x, int y)> FindThreeBlockVexters(Block[] blocks)
    {
        var rslt = new List<(int x, int y)>();

        foreach (var edge in blocks.SelectMany(x=>x.edges))
        {
            var neighborCount = Hexagon.GetNeighbors(edge)
                .Select(n => blocks.SingleOrDefault(x => x.edges.Contains(n)))
                .Where(n=> n != null)
                .Distinct()
                .Count();
            if(neighborCount >= 3)
            {
                rslt.Add(edge);
            }
        }

        return rslt;
    }
}