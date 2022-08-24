using Common.Math.TileMap;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainMap : MonoBehaviour
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

        var terrainBuilder = new TerrainBuilder();
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

    }
}

internal class TerrainBuilder
{
    public TerrainBuilder()
    {
    }

    internal Dictionary<(int x, int y), TerrainType> Build(Block[] blocks, int mapSize)
    {
        var terrainGroup = GroupByTerrainType(blocks, mapSize);

        var dictSeeds = GenerateTerrainSeeds(terrainGroup);

        var rslt = new Dictionary<(int x, int y), TerrainType>();

        foreach(var pair in terrainGroup)
        {
            if(pair.Key == TerrainType.Water)
            {
                foreach(var elem in pair.Value.SelectMany(x=>x.elements))
                {
                    rslt.Add(elem, pair.Key);
                }
                continue;
            }

            foreach (var elem in pair.Value.SelectMany(x => x.elements))
            {
                if(dictSeeds.ContainsKey(elem))
                {
                    rslt.Add(elem, dictSeeds[elem]);
                    continue;
                }

                var neighbours = dictSeeds.Keys.Select(x=>(peer:x, distance:Hexagon.GetDistance(elem, x)))
                    .OrderBy(x => x.distance)
                    .Take(10)
                    .Select(x => x.peer);

                var terrain = CalcTerrainByNeighbors(neighbours.Select(x=>dictSeeds[x]).ToArray());
                rslt.Add(elem, terrain);
            }
        }

        return rslt;
    }

    private TerrainType CalcTerrainByNeighbors(TerrainType[] neighbours)
    {
        var dictCount = new Dictionary<TerrainType, int>();
        foreach (TerrainType terrain in System.Enum.GetValues(typeof(TerrainType)))
        {
            var count = neighbours.Count(x => x == terrain);
            dictCount.Add(terrain, count);
        }

        dictCount = AdjustTerrainGenerateDict(dictCount);

        var countArray = dictCount.ToArray();

        var index = CalcRandomIndex(countArray.Select(x=>x.Value));
        return countArray.ElementAt(index).Key;
    }

    private Dictionary<TerrainType, int> AdjustTerrainGenerateDict(Dictionary<TerrainType, int> dictCount)
    {
        if (dictCount[TerrainType.Plain]<2)
        {
            dictCount[TerrainType.Plain] = 2;
        }
        
        if (dictCount[TerrainType.Mount]>5)
        {
            dictCount[TerrainType.Hill] = 2;
        }

        if(dictCount[TerrainType.Hill] > 5)
        {
            dictCount[TerrainType.Plain] = 3;
        }

        if (dictCount[TerrainType.Hill] > 6)
        {
            dictCount[TerrainType.Mount] = 1;
        }

        return dictCount;
    }

    private int CalcRandomIndex(IEnumerable<int> counts)
    {
        var total = counts.Sum();

        var num = Random.Range(0, total);

        var offset = 0;
        for(int i=0; i<counts.Count(); i++)
        {
            var elem = counts.ElementAt(i);
            if (num < elem + offset)
            {
                return i;
            }

            offset += elem;
        }

        throw new System.Exception();
    }

    private IEnumerable<DistanceItem> GenerateDistanceMap(IEnumerable<(int x, int y)> pos)
    {
        var rslt = new List<DistanceItem>();
        foreach(var p1 in pos)
        {
            foreach(var p2 in pos)
            {
                var distance = System.Math.Abs(Hexagon.GetDistance(p1, p2));
                rslt.Add(new DistanceItem(p1, p2, distance));
            }
        }
        return rslt;
    }

    class DistanceItem
    {
        public (int x, int y) p1;
        public (int x, int y) p2;
        public int distance;

        public DistanceItem((int x, int y) p1, (int x, int y) p2, int distance)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.distance = distance;
        }
    }

    private static Dictionary<(int x, int y), TerrainType> GenerateTerrainSeeds(Dictionary<TerrainType, IEnumerable<Block>> terrainGroup)
    {
        var dictSeeds = new Dictionary<(int x, int y), TerrainType>();

        foreach (TerrainType terrain in System.Enum.GetValues(typeof(TerrainType)))
        {
            var elements = terrainGroup[terrain].SelectMany(x => x.elements);
            if (terrain == TerrainType.Water)
            {
                continue;
            }

            var indexs = new HashSet<int>();
            while (indexs.Count < elements.Count() / 10)
            {
                indexs.Add(Random.Range(0, elements.Count()));
            }

            foreach (var index in indexs)
            {
                dictSeeds.Add(elements.ElementAt(index), terrain);
            }
        }

        return dictSeeds;
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
public enum TerrainType
{
    Plain,
    Hill,
    Mount,
    Water
}