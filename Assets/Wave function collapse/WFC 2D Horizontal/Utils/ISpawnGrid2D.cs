using UnityEngine;
using UnityEngine.Events;

//@Author Sebastian Molano
//This interface provides methods to create a grid based on the current objects position.
public abstract class ISpawnGrid2D : MonoBehaviour
{
    [System.Serializable]
    public class SpawnEvent : UnityEvent<int, int, GameObject, Quaternion> {}

    //Calls Spawn method
    public SpawnEvent spawnEvent;

    public bool drawGrid = false;

    [Range(1, 100)]
    public int width = 1;
    [Range(1, 100)]
    public int height = 1;
    
    //Size of each cell in the grid
    [Range(0.1f, 50)]
    public float cellSize = 1;


    void Start()
    {
        spawnEvent.AddListener(Spawn);
    }


    private void Update() {
        if(drawGrid) {
            DrawGrid();
        }
    }

    //Spawns a given prefab on the abstract coordinates x, z of the grid
    public abstract void Spawn(int x, int z, GameObject prefab, Quaternion rotation = default);

    //Deletes all the objects in the grid
    public abstract void Clear();

    //Draws the grid on 3D space
    public abstract void DrawGrid();

    //Transforms abstract coordinates of the grid into real 3D space coordinates
    public abstract Vector2 TransformCoords(int x, int z);

}
