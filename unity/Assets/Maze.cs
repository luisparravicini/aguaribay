using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
// maze generator based on the description at http://pcg.wikidot.com/pcg-algorithm:maze and http://weblog.jamisbuck.org/2011/1/27/maze-generation-growing-tree-algorithm
//
public class Maze : MonoBehaviour
{
    public enum NextCandidate
    {
        Newest,
        Oldest,
        Random,
    };

    public Vector2Int size;
    public float stepTime;
    public Vector2Int startPosition;
    YieldInstruction stepWait;
    MeshRenderer[,] blocks;
    GameObject[,] wallsVert;
    GameObject[,] wallsHoriz;
    public NextCandidate nextCandidate;
    public GameObject vertWallPrefab;
    public GameObject horizWallPrefab;
    public GameObject floorPrefab;
    public Material candidateMaterial;
    public Material corridorMaterial;
    bool[,] visited;

    private void Start()
    {
        Generate();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(size.x, 1, size.y));
    }

    public void Generate()
    {
        blocks = new MeshRenderer[size.x, size.y];
        visited = new bool[size.x, size.y];
        wallsVert = new GameObject[size.x + 1, size.y + 1];
        wallsHoriz = new GameObject[size.x + 1, size.y + 1];
        CreateUI();

        stepWait = new WaitForSeconds(stepTime);
        StartCoroutine(GenerateSteps());
    }

    void CreateUI()
    {
        var delta = new Vector3(size.x / 2 - 0.5f, 0, size.y / 2 - 0.5f);
        for (var y = 0; y <= size.y; y++)
            for (var x = 0; x <= size.x; x++)
            {
                var pos = new Vector3(x, 0, y) - delta;

                if (y < size.y && x < size.x)
                {
                    var block = Instantiate(floorPrefab, pos, floorPrefab.transform.rotation);
                    block.transform.SetParent(transform);
                    blocks[x, y] = block.GetComponent<MeshRenderer>();
                }

                if (y < size.y)
                {
                    var wall = Instantiate(vertWallPrefab, pos, vertWallPrefab.transform.rotation);
                    wall.transform.SetParent(transform);
                    wallsVert[x, y] = wall;
                }

                if (x < size.x)
                {
                    var wall = Instantiate(horizWallPrefab, pos, horizWallPrefab.transform.rotation);
                    wall.transform.SetParent(transform);
                    wallsHoriz[x, y] = wall;
                }
            }
    }

    IEnumerator GenerateSteps()
    {
        var candidates = new List<Vector2Int>();
        candidates.Add(startPosition);

        var deltas = new Vector2Int[]
        {
            new Vector2Int(-1,0),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0,-1),
        };
        var neighbours = new List<Vector2Int>();
        while (candidates.Count > 0)
        {
            var candidateIndex = GetNextIndex(candidates);
            var candidate = candidates[candidateIndex];
            visited[candidate.x, candidate.y] = true;
            blocks[candidate.x, candidate.y].sharedMaterial = candidateMaterial;

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
                blocks[candidate.x, candidate.y].sharedMaterial = corridorMaterial;
                candidates.RemoveAt(candidateIndex);
            }
            else
            {
                var neighbour = neighbours[Random.Range(0, neighbours.Count)];
                RemoveWall(candidate, neighbour);
                candidates.Add(neighbour);
            }

            neighbours.Clear();

            yield return stepWait;
        }
    }

    private void RemoveWall(Vector2Int src, Vector2Int dst)
    {
        var delta = dst - src;

        if (delta.x > 0)
            wallsVert[src.x + 1, src.y].SetActive(false);
        if (delta.x < 0)
            wallsVert[src.x, src.y].SetActive(false);
        if (delta.y > 0)
            wallsHoriz[src.x, src.y+1].SetActive(false);
        if (delta.y < 0)
            wallsHoriz[src.x, src.y].SetActive(false);
    }

    int GetNextIndex(List<Vector2Int> candidates)
    {
        int index = int.MaxValue;
        switch (nextCandidate)
        {
            case NextCandidate.Oldest:
                index = 0;
                break;
            case NextCandidate.Newest:
                index = candidates.Count - 1;
                break;
            case NextCandidate.Random:
                index = Random.Range(0, candidates.Count);
                break;
        }

        return index;
    }
}
