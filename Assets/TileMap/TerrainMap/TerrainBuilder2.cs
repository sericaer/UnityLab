using Common.Math.TileMap;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

internal class TerrainBuilder2
{
    public TerrainBuilder2()
    {
    }

    internal Dictionary<(int x, int y), TerrainType> Build(Block[] blocks, int mapSize)
    {
        var rslt = new Dictionary<(int x, int y), TerrainType>();

        var terrainBlocks = GroupByTerrainType(blocks, mapSize);

        foreach(var dict in terrainBlocks.Select(x => GenrateTerrain(x.Key, x.Value)))
        {
            foreach(var pair in dict)
            {
                rslt.Add(pair.Key, pair.Value);
            }
        }

        return rslt;
    }

    private Dictionary<(int x, int y), TerrainType> GenrateTerrain(TerrainType terrainType, IEnumerable<Block> blocks)
    {
        switch(terrainType)
        {
            case TerrainType.Mount:
                return GenrateMount(blocks);
            case TerrainType.Hill:
                return GenrateHill(blocks);
            case TerrainType.Plain:
                return GenratePlain(blocks);
            case TerrainType.Water:
                return GenrateWater(blocks);
            default:
                throw new System.Exception();
        }
    }

    private Dictionary<(int x, int y), TerrainType> GenrateWater(IEnumerable<Block> blocks)
    {
        return new Dictionary<(int x, int y), TerrainType>();
    }

    private Dictionary<(int x, int y), TerrainType> GenratePlain(IEnumerable<Block> blocks)
    {
        return new Dictionary<(int x, int y), TerrainType>();
    }

    private Dictionary<(int x, int y), TerrainType> GenrateHill(IEnumerable<Block> blocks)
    {
        return new Dictionary<(int x, int y), TerrainType>();
    }

    private Dictionary<(int x, int y), TerrainType> GenrateMount(IEnumerable<Block> blocks)
    {
        var elements = blocks.SelectMany(x => x.elements).OrderBy(_ => System.Guid.NewGuid()).ToList();
        var edges = blocks.SelectMany(x => x.edges).ToHashSet();

        var rslt = new Dictionary<(int x, int y), TerrainType>();
        while(elements.Count > rslt.Count * 4)
        {
            var seeds = elements.Take(20).ToArray();
            foreach(var seed in seeds)
            {
                rslt.Add(seed, TerrainType.Hill);
                elements.Remove(seed);
            }

            foreach(var selected in rslt.Keys.ToArray())
            {
                var neighbors = Hexagon.GetNeighbors(selected).Where(x=> !edges.Contains(x));
                var vailds = neighbors.Intersect(elements).ToArray();
                if(vailds.Length == 0)
                {
                    continue;
                }

                var index = Random.Range(0, vailds.Length);
                rslt.Add(vailds[index], TerrainType.Hill);
                elements.Remove(vailds[index]);
            }
        }

        foreach (var elem in elements)
        {
            rslt.Add(elem, TerrainType.Mount);
        }

        return rslt;
    }

    internal Dictionary<TerrainType, IEnumerable<Block>> GroupByTerrainType(Block[] blocks, int mapSize)
    {
        var rslt = new Dictionary<TerrainType, IEnumerable<Block>>();

        rslt.Add(TerrainType.Water, blocks.Where(x => x.edges.Any(r => r.y == mapSize - 1 || r.x == 0)));

        blocks = blocks.Except(rslt.Values.SelectMany(x => x)).ToArray();

        rslt.Add(TerrainType.Mount, blocks.Where(x => x.edges.Any(r => r.y == 0)));

        blocks = blocks.Except(rslt.Values.SelectMany(x => x)).ToArray();

        var hills = new List<Block>();
        var plains = new List<Block>();
        foreach (var block in blocks)
        {
            if (rslt[TerrainType.Mount].Any(m => block.isNeighbor(m)))
            {
                hills.Add(block);
            }
            else if (rslt[TerrainType.Water].Any(m => block.isNeighbor(m)))
            {
                plains.Add(block);
            }
            else
            {
                var hillPercent = 50;
                var plainPercent = 50;

                if (hills.Any(m => block.isNeighbor(m)))
                {
                    hillPercent += 50;
                }
                if (hills.Any(m => block.isNeighbor(m)))
                {
                    plainPercent += 100;
                }

                var random = Random.Range(0, hillPercent + plainPercent);
                if (random < hillPercent)
                {
                    hills.Add(block);
                }
                else
                {
                    plains.Add(block);
                }
            }
        }

        rslt.Add(TerrainType.Hill, hills);
        rslt.Add(TerrainType.Plain, plains);

        return rslt;
    }
}