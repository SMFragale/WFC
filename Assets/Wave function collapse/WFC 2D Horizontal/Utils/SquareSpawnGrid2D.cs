using UnityEngine;

//@Author Sebastian Molano
//The ChunkSpawnGrid defines a spawning grid over the gameobject the script is attached to.
//
//In order to define this Grid, a number of rows and columns must be specified.
//
//Also a point in the bottom left corner of the area where objects can spawn must be defined in order to calculate the size of the Grid
//This point will be replicated on the other side (top-right) in order to complete the square that defines the grid. 
//
//The script will then calculate the interval at which each object should be able to spawn. 

public class SquareSpawnGrid2D : ISpawnGrid2D
{
    [SerializeField]
    private int yOffset = 0;

    [SerializeField]
    private int side = 5;

    private float ZInterval = 1;
    private float XInterval = 1;

    public override void Clear()
    {
        throw new System.NotImplementedException();
    }

    public override void DrawGrid() {
        for (int i = 0; i < side; i++) {
            for (int j = 0; j < side; j++) {
                Vector2 realCoords = TransformCoords(i, j);

                Vector3 botLeft = new Vector3(realCoords[0] - XInterval, yOffset, realCoords[1] - ZInterval) + transform.position;
                Vector3 topLeft = new Vector3(realCoords[0] - XInterval, yOffset, realCoords[1] + ZInterval) + transform.position;
                Vector3 topRight = new Vector3(realCoords[0] + XInterval, yOffset, realCoords[1] + ZInterval) + transform.position;
                Vector3 botRight = new Vector3(realCoords[0] + XInterval, yOffset, realCoords[1] - ZInterval) + transform.position;

                Debug.DrawLine(botLeft, topLeft);
                Debug.DrawLine(topLeft, topRight);
                Debug.DrawLine(topRight, botRight);
                Debug.DrawLine(botRight, botLeft);

            }
        }
    }


    //Will spawn an object at a given (x, z) position of the grid.
    //Will fail if the given x, z position is outside the bounds of the Grid as defined with Rows, Cols
    public override void Spawn(int x, int z, GameObject prefab, Quaternion rotation = default) {
        if(x < 0 || x >= side || z >= side) {
            Debug.LogError("Error trying to spawn a new object in the chunk matrix\nThe coordinate is greater than the maximum allowed: " + (side-1) + " or less than zero.");
            return;
        }
        Vector2 realCoords = TransformCoords(x, z);

        Debug.Log("Transformed given coords to: " + realCoords);

        Vector3 spawnPoint = new Vector3(realCoords.x + (cellSize / 2), yOffset, realCoords.y + (cellSize / 2));
        GameObject spawned = Instantiate(prefab);
        spawned.transform.parent = this.gameObject.transform;
        spawned.transform.localPosition = spawnPoint;
        spawned.transform.rotation = prefab.transform.rotation;
        spawned.name = prefab.name + "_" + x + z;

        Debug.Log("Spawned object at: " + spawnPoint);

    }

    public override Vector2 TransformCoords(int x, int z) {
        float xCoord = (x - side/2) * XInterval;
        float zCoord = (z - side/2) * ZInterval;

        Vector2 newCoords = new Vector2(xCoord, zCoord);
        return newCoords;
    }
}
