﻿using System.Collections;
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
    public Button btnStart;
    public Button btnWalk;
    Coroutine generator;
    YieldInstruction stepWait;
    MeshRenderer[,] blocks;
    GameObject[,] wallsVert;
    GameObject[,] wallsHoriz;


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

    public void OnBtnWalk() { }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(size.x, 1, size.y));
    }

    private void Reset()
    {
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
        var firstPos = startPosition;
        if (randomPosition)
            firstPos = new Vector2Int(Random.Range(0, size.x), Random.Range(0, size.y));
        var growingTree = new GrowingTree(size, firstPos, nextCandidate);
        growingTree.OnVisit += (pos) => blocks[pos.x, pos.y].sharedMaterial = candidateMaterial;
        growingTree.OnDeadEnd += (pos) => blocks[pos.x, pos.y].sharedMaterial = corridorMaterial;
        growingTree.OnCarvePassage += (src, dst) => RemoveWall(src, dst);

        growingTree.Start();
        while (!growingTree.finished)
        {
            growingTree.Step();
            yield return stepWait;
        }

        generator = null;
        btnWalk.interactable = true;
        btnStart.interactable = true;
    }

    private void RemoveWall(Vector2Int src, Vector2Int dst)
    {
        var delta = dst - src;

        if (delta.x > 0)
            wallsVert[src.x + 1, src.y].SetActive(false);
        if (delta.x < 0)
            wallsVert[src.x, src.y].SetActive(false);
        if (delta.y > 0)
            wallsHoriz[src.x, src.y + 1].SetActive(false);
        if (delta.y < 0)
            wallsHoriz[src.x, src.y].SetActive(false);
    }
}
