using System;
using System.Collections.Generic;

using UnityEngine;


namespace Gamination
{
	public interface ITileMap
	{
		int Rows { get; }
		int Cols { get; }
		Tile this[int x, int z] { get; }
		bool IsPassable(int x, int z);
	}


	public struct Vector2I : IEquatable<Vector2I>
	{
		public int X;
		public int Z;

		public Vector2I(int x, int z)
		{
			X = x;
			Z = z;
		}

		public bool Equals(Vector2I other)
		{
			return X == other.X && Z == other.Z;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is Vector2I && Equals((Vector2I) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (X * 397) ^ Z;
			}
		}

		public static bool operator ==(Vector2I left, Vector2I right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Vector2I left, Vector2I right)
		{
			return !left.Equals(right);
		}

		public static float Distance(Vector2I a, Vector2I b)
		{
			var dx = a.X - b.X;
			var dy = a.Z - b.Z;
			return Mathf.Sqrt(dx * dx + dy * dy);
		}
	}


	public class TileDijkstra
	{
		private readonly ITileMap _map;


		public TileDijkstra(ITileMap map)
		{
			_map = map;
		}


		public List<Vector2I> FindPath(Vector2I start, Vector2I end)
		{
			return FindPath(start, (rc, t) => rc == end);
		}


		public List<Vector2I> FindPath(Vector2I start, Func<Tile, bool> endCondition)
		{
			return FindPath(start, (rc, t) => endCondition(t));
		}


		public List<Vector2I> FindPath(Vector2I start, Func<Vector2I, bool> endCondition)
		{
			return FindPath(start, (rc, t) => endCondition(rc));
		}


		public List<Vector2I> FindPath(Vector2I start, Func<Vector2I, Tile, bool> endCondition)
		{
			var neighbors = new List<Vector2I>();

			var distances = new float[_map.Rows,_map.Cols];
			var visited = new bool[_map.Rows,_map.Cols];
			var origins = new Vector2I[_map.Rows,_map.Cols];

			for (int row = 0; row < _map.Rows; row++)
			{
				for (int column = 0; column < _map.Cols; column++)
				{
					distances[row, column] = float.PositiveInfinity;
				}
			}
			distances[start.X, start.Z] = 0.0f;

			Vector2I current = start;

			while (!endCondition(current, _map[current.X, current.Z]))
			{
				float distance = distances[current.X, current.Z];

				FillWalkableNeighbors(current.X, current.Z, neighbors);

				for (int i = 0; i < neighbors.Count; i++)
				{
					Vector2I neighbor = neighbors[i];
					if (!visited[neighbor.X, neighbor.Z])
					{
						float d = distance + Vector2I.Distance(current, neighbor);
						if (d < distances[neighbor.X, neighbor.Z])
						{
							distances[neighbor.X, neighbor.Z] = d;
							origins[neighbor.X, neighbor.Z] = current;
						}
					}
				}

				visited[current.X, current.Z] = true;

				Vector2I? next = FindNearestUnvisited(distances, visited);

				if (!next.HasValue)
				{
					return null;
				}

				current = next.Value;
			}

			return BackTrack(origins, start, current);
		}


		private void FillWalkableNeighbors(int row, int col, List<Vector2I> neighbors)
		{
			neighbors.Clear();

			bool n = _map.IsPassable(row + 1, col);
			if (n)
			{
				neighbors.Add(new Vector2I(row + 1, col));
			}

			bool e = _map.IsPassable(row, col + 1);
			if (e)
			{
				neighbors.Add(new Vector2I(row, col + 1));
			}

			bool s = _map.IsPassable(row - 1, col);
			if (s)
			{
				neighbors.Add(new Vector2I(row - 1, col));
			}

			bool w = _map.IsPassable(row, col - 1);
			if (w)
			{
				neighbors.Add(new Vector2I(row, col - 1));
			}
		}


		private List<Vector2I> BackTrack(Vector2I[,] origins, Vector2I start, Vector2I end)
		{
			var path = new List<Vector2I>();

			while (start != end)
			{
				path.Add(end);
				end = origins[end.X, end.Z];
			}
			path.Add(end);

			path.Reverse();

			return path;
		}


		private Vector2I? FindNearestUnvisited(float[,] distances, bool[,] visited)
		{
			var result = new Vector2I(-1, -1);

			float minDistance = float.PositiveInfinity;

			var rows = _map.Rows;
			var cols = _map.Cols;

			for (int row = 0; row < rows; row++)
			{
				for (int column = 0; column < cols; column++)
				{
					if (!visited[row, column])
					{
						float d = distances[row, column];
						if (d < minDistance)
						{
							result = new Vector2I(row, column);
							minDistance = d;
						}
					}
				}
			}

			if (float.IsPositiveInfinity(minDistance))
			{
				return null;
			}

			return result;
		}
	}
}