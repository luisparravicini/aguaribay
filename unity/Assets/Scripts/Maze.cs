using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Maze : MonoBehaviour
{

    public Vector2Int size;
    public float stepTime;
    public Vector2Int startPosition;
    public bool randomPosition;
    public GrowingTree.NextCandidateStrategy nextCandidate;
    public GameObject vertWallPrefab;
    public GameObject horizWallPrefab;
    public GameObject floorPrefab;
    public Material candidateMaterial;
    public Material corridorMaterial;
    public Material startMaterial;
    public Button btnStart;
    public Button btnReset;
    public Button btnWalk;
    public Movement walker;
    public GameObject mainCam;
    public Text compass;
    Coroutine generator;
    YieldInstruction stepWait;
    MeshRenderer[,] blocks;
    GameObject[,] wallsVert;
    GameObject[,] wallsHoriz;
    bool walking;
    MazeSpec maze;
    Vector3 baseDelta;

    private void Start()
    {
        Reset();
    }

    public void OnBtnStart()
    {
        btnStart.interactable = false;
        Generate();
    }

    public void OnBtnReset()
    {
        btnStart.interactable = true;
        Reset();
    }

    public void OnBtnWalk()
    {
        walking = !walking;
        compass.gameObject.SetActive(walking);

        if (walking)
        {
            var delta = transform.TransformPoint(baseDelta);
            walker.Setup(delta, startPosition, maze, Direction.N, compass);
        }
        btnStart.interactable = btnReset.interactable = !walking;
        btnWalk.GetComponentInChildren<Text>().text = (walking ? "Exit" : "Walk");

        walker.gameObject.SetActive(walking);
        mainCam.SetActive(!walking);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(size.x, 1, size.y));
    }

    private void Reset()
    {
        compass.gameObject.SetActive(false);
        btnWalk.interactable = false;

        if (generator != null)
        {
            StopCoroutine(generator);
            generator = null;
        }

        foreach (Transform child in transform)
            Destroy(child.gameObject);

        blocks = new MeshRenderer[size.x, size.y];
        wallsVert = new GameObject[size.x + 1, size.y + 1];
        wallsHoriz = new GameObject[size.x + 1, size.y + 1];
        baseDelta = new Vector3(-size.x / 2 + 0.5f, 0, -size.y / 2 + 0.5f);
        CreateUI();
    }

    public void Generate()
    {
        Reset();
        stepWait = new WaitForSeconds(stepTime);
        generator = StartCoroutine(GenerateSteps());
    }

    void CreateUI()
    {
        for (var y = 0; y <= size.y; y++)
            for (var x = 0; x <= size.x; x++)
            {
                var pos = baseDelta + new Vector3(x, 0, y);

                if (y < size.y && x < size.x)
                {
                    var block = Instantiate(floorPrefab, pos, floorPrefab.transform.rotation);
                    block.transform.SetParent(transform, false);
                    blocks[x, y] = block.GetComponent<MeshRenderer>();
                }

                if (y < size.y)
                {
                    var wall = Instantiate(vertWallPrefab, pos, vertWallPrefab.transform.rotation);
                    wall.transform.SetParent(transform, false);
                    wallsVert[x, y] = wall;
                }

                if (x < size.x)
                {
                    var wall = Instantiate(horizWallPrefab, pos, horizWallPrefab.transform.rotation);
                    wall.transform.SetParent(transform, false);
                    wallsHoriz[x, y] = wall;
                }
            }
    }

    IEnumerator GenerateSteps()
    {
        if (randomPosition)
            startPosition = new Vector2Int(Random.Range(0, size.x), Random.Range(0, size.y));

        var growingTree = new GrowingTree(size, startPosition, nextCandidate);
        growingTree.OnVisit += (pos) =>
        {
            blocks[pos.x, pos.y].sharedMaterial = (startPosition == pos ? startMaterial : candidateMaterial);
        };
        growingTree.OnDeadEnd += (pos) => blocks[pos.x, pos.y].sharedMaterial = corridorMaterial;
        growingTree.OnCarvePassage += RemoveWall;

        growingTree.Start();
        while (!growingTree.finished)
        {
            growingTree.Step();
            yield return stepWait;
        }

        maze = growingTree.maze;
        generator = null;
        btnWalk.interactable = true;
        btnStart.interactable = true;
    }

    private void RemoveWall(Vector2Int src, Vector2Int dst)
    {
        var delta = dst - src;

        if (delta.x != 0)
            wallsVert[src.x + (delta.x > 0 ? 1 : 0), src.y].SetActive(false);
        if (delta.y != 0)
            wallsHoriz[src.x, src.y + (delta.y > 0 ? 1 : 0)].SetActive(false);
    }
}
