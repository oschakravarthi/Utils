using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class DirectGraph<TVertex, TEdge> where TEdge : class
{
    private readonly Dictionary<KeyValuePair<TVertex, TVertex>, TEdge> edges = [];
    private readonly List<TVertex> vertices = [];
    private double[,] distances;
    private int[,] nexts;

    public void AddEdge(TVertex vertexA, TVertex vertexB, TEdge edge)
    {
        if (!vertices.Contains(vertexA))
        {
            vertices.Add(vertexA);
        }

        if (!vertices.Contains(vertexB))
        {
            vertices.Add(vertexB);
        }

        edges[new KeyValuePair<TVertex, TVertex>(vertexA, vertexB)] = edge;

        Uninitialize();
    }

    public void AddVertex(TVertex vertex)
    {
        if (vertices.Contains(vertex))
        {
            return;
        }

        Uninitialize();

        vertices.Add(vertex);
    }

    public bool Adjacent(TVertex vertexA, TVertex vertexB)
    {
        return edges.ContainsKey(new KeyValuePair<TVertex, TVertex>(vertexA, vertexB));
    }

    public TEdge GetEdge(TVertex vertexA, TVertex vertexB) //returns a distance between 2 nodes
    {
        if (edges.TryGetValue(new KeyValuePair<TVertex, TVertex>(vertexA, vertexB), out var edgeDistance))
        {
            return edgeDistance;
        }

        return default;
    }

    public TVertex GetNearestAmong(TVertex from, IEnumerable<TVertex> otherVertices, out List<TEdge> path)
    {
        path = [];
        TVertex nearest = default;
        var distance = int.MaxValue;
        foreach (var otherVertex in otherVertices)
        {
            var newPath = GetShortestPath(from, otherVertex);
            if (newPath.Count < distance)
            {
                nearest = otherVertex;
                distance = newPath.Count;
                path = newPath;
            }
        }

        return nearest;
    }

    public List<TEdge> GetShortestPath(TVertex from, TVertex to)
    {
        if (distances == null || nexts == null)
        {
            Initialize();
            FloydWarshall();
        }

        var path = new List<TEdge>();
        var u = vertices.IndexOf(from);
        var v = vertices.IndexOf(to);

        if (u < 0 || v < 0 || nexts[u, v] == -1)
        {
            return path;
        }

        while (u != v)
        {
            v = nexts[u, v];

            var edge = edges[new KeyValuePair<TVertex, TVertex>(vertices[u], vertices[v])];
            path.Add(edge);
            u = v;
        }

        return path;
    }

    public void RemoveEdge(TVertex vertexA, TVertex vertexB)
    {
        if (edges.Remove(new KeyValuePair<TVertex, TVertex>(vertexA, vertexB)))
        {
            Uninitialize();
        }
    }

    private void FloydWarshall()
    {
        var verticesCount = vertices.Count;
        for (var k = 0; k < verticesCount; k++)
            for (var i = 0; i < verticesCount; i++)
                for (var j = 0; j < verticesCount; j++)
                {
                    // We cannot travel through
                    // edge that doesn't exist
                    if (double.IsPositiveInfinity(distances[i, k]) || double.IsPositiveInfinity(distances[k, j]))
                    {
                        continue;
                    }

                    if (distances[i, j] > distances[i, k] + distances[k, j])
                    {
                        distances[i, j] = distances[i, k] + distances[k, j];
                        nexts[i, j] = nexts[i, k];
                    }
                }
    }

    private void Initialize()
    {
        var verticesCount = vertices.Count;
        distances = new double[verticesCount, verticesCount];
        nexts = new int[verticesCount, verticesCount];
        for (var i = 0; i < verticesCount; i++)
        {
            var vertexI = vertices[i];
            for (var j = 0; j < verticesCount; j++)
            {
                var len = double.PositiveInfinity;
                var vertexJ = vertices[j];
                var edge = GetEdge(vertexI, vertexJ);
                if (edge != default(TEdge))
                {
                    len = 1;
                }

                distances[i, j] = len;
                if (double.IsPositiveInfinity(len))
                {
                    nexts[i, j] = -1;
                }
                else
                {
                    nexts[i, j] = j;
                }
            }
        }
    }

    private void Uninitialize()
    {
        distances = null;
        nexts = null;
    }
}