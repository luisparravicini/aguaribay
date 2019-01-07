using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    public Vector2Int size;
    public float stepTime;
    public Vector2Int startPosition;
    YieldInstruction stepWait;
    public event EventHandler OnFinish;
    GameObject[,] blocks;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(size.x, 1, size.y));
    }

    public void Generate()
    {
#if UNITY_EDITOR
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
#endif

        blocks = new GameObject[size.x, size.y];
        var delta = new Vector3(size.x / 2 - 0.5f, 0, size.y / 2 - 0.5f);
        for (var y = 0; y < size.y; y++)
            for (var x = 0; x < size.x; x++)
            {
                var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                block.transform.position = new Vector3(x, 0, y) - delta;
                block.transform.SetParent(transform);

                blocks[x, y] = block;
            }

        stepWait = new WaitForSeconds(stepTime);
        StartCoroutine(GenerateSteps());
    }

    IEnumerator GenerateSteps()
    {
        var candidates = new List<Vector2Int>();
        candidates.Add(startPosition);

        while (candidates.Count > 0)
        {
            var candidate = candidates[0];
            candidates.RemoveAt(0);

            DestroyImmediate(blocks[candidate.x, candidate.y]);
            blocks[candidate.x, candidate.y] = null;

            yield return stepWait;
        }


        OnFinish(this, null);
    }
}
