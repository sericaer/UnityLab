using Common.Math.TileMap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

using Block = System.Collections.Generic.List<(int x, int y)>;

public class CellGenerator : MonoBehaviour
{
    public Tilemap tilemap;

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
        var mapPositions = Enumerable.Range(0, 80)
            .SelectMany(x => Enumerable.Range(0, 120).Select(y => (x, y)));

        var blocks = Build(mapPositions);

        var colors = new HashSet<Color>();
        while (colors.Count < blocks.Count())
        {
            colors.Add(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
        }

        for (int i = 0; i < blocks.Count(); i++)
        {
            var block = blocks.ElementAt(i);

            foreach (var pos in block)
            {
                tilemap.SetTileColor(new Vector3Int(pos.x, pos.y), tile, colors.ElementAt(i));
            }
        }
    }

    private static IEnumerable<Block> Build(IEnumerable<(int x, int y)> mapPositions)
    {
        var whiteNoiseMap = mapPositions
            .ToDictionary(k => k, _ => Random.Range(0f, 1.0f));

        var cellularMap = whiteNoiseMap.ToDictionary(k => k.Key, v =>
        {
            var range = Hexagon.GetNeighbors(v.Key).Where(r => whiteNoiseMap.ContainsKey(r));
            var max = range.Max(b => whiteNoiseMap[b]);
            var min = range.Min(b => whiteNoiseMap[b]);
            return 1 - max < min ? max : min;
        });

        var originPositions = cellularMap.Keys.OrderBy(_ => Random.Range(0, int.MaxValue))
                .Take(cellularMap.Count() / 50).ToArray();

        var usedPositions = new HashSet<(int x, int y)>(originPositions);

        var block2Queue = originPositions
            .ToDictionary(k => new Block(),
                          v => new Dictionary<(int x, int y), (float curr, float need)>() { { v, (cellularMap[v], cellularMap[v]) } });

        while (block2Queue.Keys.Sum(x => x.Count) != cellularMap.Count)
        {
            foreach (var pair in block2Queue)
            {
                var block = pair.Key;
                var pos2Fill = pair.Value;

                if (pos2Fill.Count == 0)
                {
                    continue;
                }

                var pos2FillPair = pos2Fill.ElementAt(Random.Range(0, pos2Fill.Count()));

                var pos = pos2FillPair.Key;
                var fill = pos2FillPair.Value;

                var currValue = fill.curr + 0.1f;
                if (currValue < fill.need)
                {
                    pos2Fill[pos] = (currValue, fill.need);
                    continue;
                }

                pos2Fill.Remove(pos);
                block.Add(pos);

                foreach (var next in Hexagon.GetNeighbors(pos).Where(n => cellularMap.ContainsKey(n)))
                {
                    if (usedPositions.Contains(next))
                    {
                        continue;
                    }

                    pos2Fill.Add(next, (0f, cellularMap[next]));
                    usedPositions.Add(next);
                }
            }
        }

        var blocks = block2Queue.Keys;
        return blocks;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
