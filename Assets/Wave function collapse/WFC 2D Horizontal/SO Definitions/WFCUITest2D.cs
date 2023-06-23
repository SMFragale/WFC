using UnityEngine;

[RequireComponent(typeof(WFC2D))]
public class WFCUITest2D : MonoBehaviour
{
    [SerializeField]
    public WFC2D wfc;

    private void Start() {
        wfc = GetComponent<WFC2D>();
    }

    public void Iterate() {
        wfc.SpawnIteration();
    }

    public void Generate() {
        wfc.Generate();
    }
}
