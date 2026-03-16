using System;
using System.Collections.Generic;

namespace Rpg_Dungeon
{
    internal static class Pathfinding
    {
        internal readonly struct GridPoint : IEquatable<GridPoint>
        {
            public int X { get; }
            public int Y { get; }
            public GridPoint(int x, int y) { X = x; Y = y; }
            public bool Equals(GridPoint other) => X == other.X && Y == other.Y;
            public override bool Equals(object? obj) => obj is GridPoint gp && Equals(gp);
            public override int GetHashCode() => HashCode.Combine(X, Y);
            public override string ToString() => $"({X},{Y})";
        }

        // Simple A* implementation on a rectangular boolean grid (walkable = true)
        // walkable.GetLength(0) == rows (Y), GetLength(1) == cols (X) - callers should document layout
        public static List<GridPoint> GetPath(GridPoint start, GridPoint goal, bool[,] walkable)
        {
            var path = new List<GridPoint>();
            if (walkable == null) return path;

            int rows = walkable.GetLength(0);
            int cols = walkable.GetLength(1);

            bool InBounds(GridPoint p) => p.X >= 0 && p.X < cols && p.Y >= 0 && p.Y < rows;
            if (!InBounds(start) || !InBounds(goal)) return path;
            if (!walkable[start.Y, start.X] || !walkable[goal.Y, goal.X]) return path;

            var open = new PriorityQueue<GridPoint, int>();
            var gScore = new Dictionary<GridPoint, int>();
            var fScore = new Dictionary<GridPoint, int>();
            var cameFrom = new Dictionary<GridPoint, GridPoint>();

            int Heuristic(GridPoint a, GridPoint b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y); // Manhattan

            gScore[start] = 0;
            fScore[start] = Heuristic(start, goal);
            open.Enqueue(start, fScore[start]);

            var directions = new[] { new GridPoint(0, -1), new GridPoint(0, 1), new GridPoint(-1, 0), new GridPoint(1, 0) };

            while (open.Count > 0)
            {
                var current = open.Dequeue();
                if (current.Equals(goal))
                {
                    // reconstruct
                    var cur = current;
                    while (!cur.Equals(start))
                    {
                        path.Add(cur);
                        cur = cameFrom[cur];
                    }
                    path.Add(start);
                    path.Reverse();
                    return path;
                }

                foreach (var dir in directions)
                {
                    var neighbor = new GridPoint(current.X + dir.X, current.Y + dir.Y);
                    if (!InBounds(neighbor)) continue;
                    if (!walkable[neighbor.Y, neighbor.X]) continue;

                    int tentativeG = gScore.ContainsKey(current) ? gScore[current] + 1 : int.MaxValue;
                    if (!gScore.TryGetValue(neighbor, out var neighborG) || tentativeG < neighborG)
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeG;
                        int f = tentativeG + Heuristic(neighbor, goal);
                        fScore[neighbor] = f;
                        // enqueue with priority f
                        open.Enqueue(neighbor, f);
                    }
                }
            }

            // no path found
            return path;
        }
    }
}
