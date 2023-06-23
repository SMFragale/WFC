using UnityEngine;

//Monobehaviour to be used with ChunkSpawnTestEditor which provides a
//button to Spawn objects on a ChunkSpawnGrid
[RequireComponent (typeof(ISpawnGrid2D))]
public class GridSpawnTest2D : MonoBehaviour
{
    [SerializeField]
    public int x;
    [SerializeField]
    public int z;
    [SerializeField]
    public GameObject prefab;

    [HideInInspector]
    public ISpawnGrid2D asg;

    private void Start() {
        asg = GetComponent<ISpawnGrid2D>();
    }

    public void ExecuteTest() {
        asg.spawnEvent.Invoke(x, z, prefab, Quaternion.identity);
    }


}

