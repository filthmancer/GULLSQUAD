using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Gull;
public class Level : MonoBehaviour
{
    public List<Chip> chips;
    public List<SpawnPoint> spawnPoints;


    public void Rebuild()
    {
        chips = new List<Chip>(transform.GetComponentsInChildren<Chip>());
        spawnPoints = new List<SpawnPoint>(transform.GetComponentsInChildren<SpawnPoint>());
    }
}



#if UNITY_EDITOR
[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Build Level"))
        {
            (target as Level).Rebuild();
        }
    }
}
#endif