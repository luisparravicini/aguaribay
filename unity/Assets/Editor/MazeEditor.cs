using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;

[CustomEditor(typeof(Maze))]
public class MazeEditor : Editor
{
    bool generating;

    void OnEnable()
    {
        var maze = (Maze)target;
        maze.OnFinish += OnFinish;
    }

    private void OnDisable()
    {
        var maze = (Maze)target;
        maze.OnFinish -= OnFinish;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        string msg = (generating ? "Stop" : "Generate");
        if (GUILayout.Button(msg))
        {
            if (!generating)
            {
                ((Maze)target).Generate();
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            else
            {

            }

            generating = !generating;
        }
    }

    private void OnFinish(object sender, EventArgs e)
    {
        generating = false;
    }
}
