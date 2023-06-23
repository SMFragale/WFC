using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WFCUITest2D))]
public class WFC2DEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WFCUITest2D test = (WFCUITest2D)target;
        test.wfc = (WFC2D)EditorGUILayout.ObjectField("WFC implementation", test.wfc, typeof(WFC2D), true);

        if(GUILayout.Button("Iterate")) {
            test.Iterate();
        }

        if(GUILayout.Button("Generate")) {
            test.Generate();
        }
    }
}
