using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnVisitHandler(Vector2Int pos);
public delegate void OnDeadEndHandler(Vector2Int pos);
public delegate void OnCarvePassageHandler(Vector2Int src, Vector2Int dst);

/*
 *
 * Maze generator based on the growing tree algorithm
 * Great explanation of it at http://weblog.jamisbuck.org/2011/1/27/maze-generation-growing-tree-algorithm
 * 
*/
public class GrowingTree
{
    public enum NextCandidateStrategy
    {
        Newest,
        Oldest,
        Random,
    };

    Vector2Int startPosition;
    NextCandidateStrategy nextCandidate;
    bool[,] visited;
    Vector2Int size;
    public event OnVisitHandler OnVisit;
    public event OnDeadEndHandler OnDeadEnd;
    public event OnCarvePassageHandler OnCarvePassage;
    Vector2Int[] deltas;
    List<Vector2Int> neighbours;
    List<Vector2Int> candidates;

    public GrowingTree(Vector2Int size, Vector2Int startPosition, NextCandidateStrategy nextCandidate)
    {
        this.size = size;
        this.startPosition = startPosition;
        this.nextCandidate = nextCandidate;
    }

    public bool finished { get; internal set; }

    int GetNextIndex(List<Vector2Int> candidates)
    {
        int index = int.MaxValue;
        switch (nextCandidate)
        {
            case NextCandidateStrategy.Oldest:
                index = 0;
                break;
            case NextCandidateStrategy.Newest:
                index = candidates.Count - 1;
                break;
            case NextCandidateStrategy.Random:
                index = Random.Range(0, candidates.Count);
                break;
        }

        return index;
    }

    public void Start()
    {
        visited = new bool[size.x, size.y];
        deltas = new Vector2Int[]
        {
            new Vector2Int(-1,0),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0,-1),
        };
        neighbours = new List<Vector2Int>();
        candidates = new List<Vector2Int>
        {
            startPosition
        };
    }

    public void Step()
    {
        if (candidates.Count == 0)
        {
            finished = true;
            return;
        }

        var candidateIndex = GetNextIndex(candidates);
        var candidate = candidates[candidateIndex];
        visited[candidate.x, candidate.y] = true;

        OnVisit(candidate);

        foreach (var delta in deltas)
        {
            var pos = candidate + delta;

            if (pos.x < 0 || pos.x >= size.x)
                continue;

            if (pos.y < 0 || pos.y >= size.y)
                continue;

            if (visited[pos.x, pos.y])
                continue;

            neighbours.Add(pos);
        }

        if (neighbours.Count == 0)
        {
            candidates.RemoveAt(candidateIndex);
            OnDeadEnd(candidate);
        }
        else
        {
            var neighbour = neighbours[Random.Range(0, neighbours.Count)];
            candidates.Add(neighbour);
            OnCarvePassage(candidate, neighbour);
        }

        neighbours.Clear();

    }
}
