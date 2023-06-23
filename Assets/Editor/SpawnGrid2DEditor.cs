using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridSpawnTest2D))]
public class SpawnGrid2DEditor : Editor
{
   public override void OnInspectorGUI()
    {
        GridSpawnTest2D test = (GridSpawnTest2D)target;
        test.x = EditorGUILayout.IntField("X", test.x);
        test.z = EditorGUILayout.IntField("Z", test.z);
        test.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", test.prefab, typeof(GameObject), true);
        test.asg = (ISpawnGrid2D)EditorGUILayout.ObjectField("Spawn Grid", test.asg, typeof(ISpawnGrid2D), true);

        if(GUILayout.Button("Spawn")) {
            test.ExecuteTest();
        }
    }
}
