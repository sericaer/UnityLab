using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Math.TileMap
{
    class Hexagon
    {

        const int def = 1; //ODD_Q -1, EVEN_Q 1

        static (int q, int r)[] directs = new (int q, int r)[]
        {
        (1, 0),
        (1, -1),
        (0, -1),
        (-1, 0),
        (-1, 1),
        (0, 1)
        };

        public static IEnumerable<(int x, int y)> GetNeighbors((int x, int y) pos)
        {
            var axial = ToAxial(pos);

            return directs.Select(d => ToOffset((axial.q + d.q, axial.r + d.r)));
        }

        public static int GetDirectIndex((int x, int y) target, (int x, int y) origin)
        {
            var axialTarget = ToAxial(target);
            var axialOrigin = ToAxial(origin);

            return Array.IndexOf(directs, (axialTarget.q - axialOrigin.q, axialTarget.r - axialOrigin.r));
        }

        internal static (int q, int r) ScaleOffset((int x, int y) offset, int v)
        {
            var axial = ToAxial(offset);

            return ToOffset((axial.q * v, axial.r * v));
        }

        internal static (int x, int y) ToOffset((int q, int r) axial)
        {
            int col = axial.q;
            int row = axial.r + (int)((axial.q + def * (axial.q & 1)) / 2);

            return (row * -1, col);
        }

        internal static (int q, int r) ToAxial((int row, int col) offset)
        {
            offset = (offset.row * -1, offset.col);

            int q = offset.col;
            int r = offset.row - (int)((offset.col + def * (offset.col & 1)) / 2);

            return (q, r);
        }

        internal static (int q, int r) ScaleAxial((int q, int r) axial, int v)
        {
            return (axial.q * v, axial.r * v);
        }
    }

    public class Rectangle
    {

        public readonly static (int x, int y)[] NeighbourIndexsRect = new (int x, int y)[]
        {
        (0, 1), (1, 1), (1, 0), (1, -1), (0, -1), (-1, -1), (-1, 0), (-1, 1)
        };

        public static IEnumerable<(int x, int y)> Generate(int size)
        {
            return Enumerable.Range(size / 2 * -1, size).SelectMany(x => Enumerable.Range(size / 2 * -1, size).Select(y => (x, y)));
        }

        public static IEnumerable<(int x, int y)> GetNeighbours((int x, int y) coord)
        {
            return NeighbourIndexsRect.Select(index => (coord.x + index.x, coord.y + index.y));
        }

        internal static int GetDirect((int x, int y) target, (int x, int y) origin)
        {
            var direct = (target.x - origin.x, target.y - origin.y);
            return Array.IndexOf(NeighbourIndexsRect, direct);
        }

        internal static IEnumerable<(int x, int y)> GetRings((int x, int y) pos, int dist)
        {
            var range = Enumerable.Range(dist * -1, dist * 2 + 1);

            return range.Select(x => (x, dist))
                .Concat(range.Select(x => (x, dist * -1)))
                .Concat(range.Select(y => (dist, y)))
                .Concat(range.Select(y => (dist * -1, y))).Distinct()
                .Select(index => (pos.x + index.Item1, pos.y + index.Item2));
        }
    }

    public class Block
    {
        public HashSet<(int x, int y)> elements;
        public HashSet<(int x, int y)> edges;

        public Block(HashSet<(int x, int y)> elements)
        {
            this.elements = elements;
            this.edges = elements.Where(x => Hexagon.GetNeighbors(x).Any(n => !elements.Contains(n))).ToHashSet();
        }

        public bool isNeighbor(Block peer)
        {
            return this.edges.Any(e => Hexagon.GetNeighbors(e).Intersect(peer.edges).Any());
        }

        public class BuilderGroup
        {

            const int step = 10;

            private List<Builder> builders;

            public BuilderGroup(int size)
            {
                Builder.isExist = (n) =>
                {
                    return builders.Any(x => x.centers.Contains(n));
                };


                builders = new List<Builder>();
                for (int i = (size * -1) + step; i < size; i += step)
                {
                    var directRandoms = new int[] { 1, 2, 3, 8, 9, 10 };
                    for (int j = (size * -1) + step; j < size; j += step)
                    {
                        var startPos = (i + UnityEngine.Random.Range(step / -2, step / 2), j + UnityEngine.Random.Range(step / -2, step / 2));
                        builders.Add(new Builder(size, startPos, directRandoms.OrderBy(x=>Guid.NewGuid()).ToArray()));
                    }
                }
            }

            public StepResult[] BuildInStep()
            {
                return builders.Select(x => x.BuildInStep()).ToArray();
            }

            public Block[] Build()
            {
                do
                {
                    foreach (var builder in builders.Where(x => !x.isFinish))
                    {
                        builder.BuildInStep();
                    }
                } while (builders.Any(x => !x.isFinish));

                return builders.Select(x => new Block(x.centers)).ToArray();
            }
        }

        public class StepResult
        {
            public (int x, int y)[] elements;
        }

        private class Builder
        {
            internal static System.Func<(int x, int y), bool> isExist { get; set; }

            public bool isFinish { get; private set; } = false;

            public HashSet<(int x, int y)> centers;
            public HashSet<(int x, int y)> edges;
            private int size;
            private bool isStart = true;

            private (int x, int y) originPoint;
            private int[] weightDirectValues;

            public Builder(int size, (int x, int y) originPoint, int[] weightDirectValues)
            {
                edges = new HashSet<(int x, int y)>();

                this.centers = new HashSet<(int x, int y)>();
                this.originPoint = originPoint;
                this.size = size;
                this.weightDirectValues = weightDirectValues;
            }

            public StepResult BuildInStep()
            {
                if (isStart)
                {
                    isStart = false;

                    edges.Add(originPoint);

                    return new StepResult() { elements = edges.ToArray() };
                }

                var validNeighbors = edges.SelectMany(x => Hexagon.GetNeighbors(x))
                    .Where(n => !isExist(n))
                    .Where(n => (int)System.Math.Abs(n.x) < size && (int)System.Math.Abs(n.y) < size)
                    .ToHashSet();

                isFinish = validNeighbors.Count() == 0;

                var newEdges = new HashSet<(int x, int y)>();
                var revEdges = new HashSet<(int x, int y)>();

                foreach (var edge in validNeighbors)
                {
                    var oldEdges = Hexagon.GetNeighbors(edge).Where(x => edges.Contains(x)).ToArray();

                    var value = oldEdges.Max(e => weightDirectValues[Hexagon.GetDirectIndex(e, edge)]);

                    var real = UnityEngine.Random.Range(1, 10);
                    if (real <= value)
                    {
                        newEdges.Add(edge);
                    }
                    else
                    {
                        revEdges.UnionWith(oldEdges);
                    }
                }

                newEdges.UnionWith(revEdges);
                centers.UnionWith(newEdges);

                edges = newEdges;

                return new StepResult() { elements = newEdges.ToArray() };
            }
        }
    }

   
}