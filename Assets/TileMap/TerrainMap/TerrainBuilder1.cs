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

        foreach(var pair in BuildHeightMap(blocks, mapSize))
        {
            if(pair.Value < 0)
            {
                rslt.Add(pair.Key, TerrainType.Water);
            }
            else if(pair.Value < 0.3)
            {
                rslt.Add(pair.Key, TerrainType.Plain);
            }
            else if (pair.Value < 0.6)
            {
                rslt.Add(pair.Key, TerrainType.Hill);
            }
            else
            {
                rslt.Add(pair.Key, TerrainType.Mount);
            }
        }

        return rslt;
    }


    internal Dictionary<(int x, int y), float> BuildHeightMap(Block[] blocks, int mapSize)
    {
        var rslt = new Dictionary<(int x, int y), float>();

        var waterBlock = blocks.Where(x => x.edges.Any(r => r.y == mapSize - 1 || r.x == 0));
        foreach (var element in waterBlock.SelectMany(x => x.elements))
        {
            rslt.Add(element, -1);
        }

        blocks = blocks.Except(waterBlock).ToArray();

        var heightMapCores = BuildHeightMapCore(blocks, mapSize);
        foreach (var pair in heightMapCores)
        {
            rslt.Add(pair.Key, pair.Value);
        }

        var emptyIndexs = blocks.SelectMany(x => x.elements).Except(heightMapCores.Keys);
        foreach (var elem in emptyIndexs)
        {
            //var distFactor = CalcHeighFactorByDistance(elem.y, mapSize);
            var neighorFactor = CalcHeighFactorByNeighor(elem, heightMapCores);

            rslt.Add(elem, neighorFactor);
        }

        return rslt;
    }

    private float CalcHeighFactorByNeighor((int x, int y) curr, Dictionary<(int x, int y), float> heightMapCores)
    {
        Dictionary<(int x, int y), int> neighors = new Dictionary<(int x, int y), int>();

        int distance = 0;
        while(neighors.Count < 6 || distance < 10)
        {
            foreach(var elem in Hexagon.GetRing(curr, distance).Where(x => heightMapCores.ContainsKey(x)))
            {
                neighors.Add(elem, distance);
            }

            distance++;
        }

        var sum = neighors.Sum(pair =>
        {
            var height = heightMapCores[pair.Key];
            return (10 - pair.Value) / 10.0f * height * Random.Range(1f, 2f);
        });

        return sum / neighors.Count();
    }

    private Dictionary<(int x, int y), float> BuildHeightMapCore(IEnumerable<Block> blocks, int mapSize)
    {
        var rslt = new Dictionary<(int x, int y), float>();

        var mapCoreIndexs = blocks.Select(b =>
        {
            return b.edges.Where(e => b.edges.Contains(Hexagon.GetNeighbor(e, 0)) || b.edges.Contains(Hexagon.GetNeighbor(e, 1)));
        }).SelectMany(x => x);

        

        foreach (var core in mapCoreIndexs)
        {
            var hegih = CalcHeighFactorByDistance(core.y, mapSize);

            rslt.Add(core, hegih);
        }

        return rslt;
    }

    private float CalcHeighFactorByDistance(int curr, int max)
    {
        var dist = max - curr;
        var distPercent = dist * 1.0f / max;

        var random = Random.Range(distPercent * distPercent, 1.5f);

        return random * distPercent;
    }
}