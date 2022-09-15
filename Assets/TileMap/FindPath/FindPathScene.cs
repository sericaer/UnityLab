using Common.Math.TileMap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FindPathScene : MonoBehaviour
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
        var baseMap = Enumerable.Range(0, 10).SelectMany(x => Enumerable.Range(0, 10).Select(y => (x: x, y: y)));
        foreach(var pos in baseMap)
        {
            tilemap.SetTileColor(new Vector3Int(pos.x, pos.y), tile, Color.grey);
        }


        var blocks = new (int x, int y)[]
        {
            (0,1),
            (0,2),
            (3,6),
            (4,6),
            (5,6),
            (6,6),
            (7,6)
        };

        foreach (var pos in blocks)
        {
            tilemap.SetTileColor(new Vector3Int(pos.x, pos.y), tile, Color.black);
        }

        var costMap = baseMap.ToDictionary(x => x, _ => 1);
        foreach (var block in blocks)
        {
            costMap[block] = 2;
        }

        var startPos = (x: 0, y: 0);
        var endPos = (x: 5, y: 7);

        var path = FindPath(startPos, endPos, baseMap, costMap);

        foreach (var elem in path)
        {
            tilemap.SetTileColor(new Vector3Int(elem.x, elem.y), tile, Color.green);
        }


        tilemap.SetTileColor(new Vector3Int(startPos.x, startPos.y), tile, Color.red);
        tilemap.SetTileColor(new Vector3Int(endPos.x, endPos.y), tile, Color.blue);
    }

    private IEnumerable<(int x, int y)> FindPath((int x, int y) startPos, (int x, int y) endPos, IEnumerable<(int x, int y)>baseMap, Dictionary<(int x, int y), int> costMap = null)
    {
        var visitMap = costMap == null ? GenrateVisitMap(startPos, endPos, baseMap) : GenrateVisitMapWithCost(startPos, endPos, baseMap, costMap);

        var path = new List<(int x, int y)>();
        var currPos = endPos;
        path.Add(endPos);

        while (currPos != startPos)
        {
            var nextPos = visitMap[currPos];
            path.Add(nextPos);

            currPos = nextPos;
        }

        path.Reverse();
        return path;
    }

    private Dictionary<(int x, int y), (int x, int y)> GenrateVisitMapWithCost((int x, int y) startPos, (int x, int y) endPos, IEnumerable<(int x, int y)> baseMap, Dictionary<(int x, int y), int> costMap)
    {
        var queue = new PriorityQueue<(int x, int y)>();
        queue.Enqueue(startPos, 0);

        var costRecord = new Dictionary<(int x, int y), int>();
        costRecord[startPos] = 0;

        var dict = new Dictionary<(int x, int y), (int x, int y)>();
        dict.Add(startPos, (-1, -1));

        while (queue.Count != 0)
        {
            var currPos = queue.Dequeue();

            if (currPos == endPos)
            {
                break;
            }

            foreach (var neighor in Hexagon.GetNeighbors(currPos).Where(n=>baseMap.Contains(n)))
            {
                var newCost = costRecord[currPos] + costMap[neighor];

                if (dict.ContainsKey(neighor) && costRecord[neighor] <= newCost)
                {
                    continue;
                }

                costRecord[neighor] = newCost;
                queue.Enqueue(neighor, newCost);

                dict.Add(neighor, currPos);
            }
        }

        return dict;
    }

    private static Dictionary<(int x, int y), (int x, int y)> GenrateVisitMap((int x, int y) startPos, (int x, int y) endPos, IEnumerable<(int x, int y)> baseMap)
    {
        var queue = new Queue<(int x, int y)>();
        queue.Enqueue(startPos);

        var dict = new Dictionary<(int x, int y), (int x, int y)>();
        dict.Add(startPos, (-1, -1));

        while (queue.Count != 0)
        {
            var currPos = queue.Dequeue();

            if (currPos == endPos)
            {
                break;
            }

            foreach (var neighor in Hexagon.GetNeighbors(currPos).Where(n => baseMap.Contains(n)))
            {
                if (dict.ContainsKey(neighor))
                {
                    continue;
                }

                queue.Enqueue(neighor);
                dict.Add(neighor, currPos);


            }
        }

        return dict;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}