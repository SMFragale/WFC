using UnityEngine;

public class RectangleSpawnGrid2D : ISpawnGrid2D
{
    public override void Clear()
    {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
    }

    public override void DrawGrid()
    {
        for(int x = 0 ; x < width; x++) {
            for (int z = 0; z < height; z++) {
                Vector2 coords = TransformCoords(x, z);
                Debug.DrawLine(new Vector3(coords.x, 0, coords.y), new Vector3(coords.x + cellSize, 0, coords.y), Color.red);
                Debug.DrawLine(new Vector3(coords.x, 0, coords.y), new Vector3(coords.x, 0, coords.y + cellSize), Color.red);
                Debug.DrawLine(new Vector3(coords.x + cellSize, 0, coords.y), new Vector3(coords.x + cellSize, 0, coords.y + cellSize), Color.red);
                Debug.DrawLine(new Vector3(coords.x, 0, coords.y + cellSize), new Vector3(coords.x + cellSize, 0, coords.y + cellSize), Color.red);
            }
        }
    }

    public override void Spawn(int x, int z, GameObject prefab, Quaternion rotation = default)
    {
        if(x < 0 || x >= width || z >= height || z < 0) {
            Debug.LogError("Error trying to spawn a new object in the matrix\nThe coordinate is greater than the maximum allowed or less than zero.");
            return;
        }
        if(prefab == null) {
            Debug.LogError("Error trying to spawn a new object in the matrix\nThe prefab is null.");
            return;
        }

        Vector2 realCoords = TransformCoords(x, z);
        Vector3 spawnPoint = new Vector3(realCoords.x + (cellSize / 2), transform.position.y, realCoords.y + (cellSize / 2));
        GameObject spawned = Instantiate(prefab);
        spawned.transform.parent = this.gameObject.transform;
        spawned.transform.localPosition = spawnPoint;
        spawned.transform.rotation = prefab.transform.rotation;
        spawned.name = prefab.name + "_" + x + z;

        //TODO update the rotation of the object
    }

    public override Vector2 TransformCoords(int x, int z)
    {
        Vector2 anchor = new Vector2(transform.position.x, transform.position.z);
        return anchor + new Vector2(x * cellSize, z * cellSize * -1 /* This -1 can cause problems, but this is the best way to match a C# matrix which starts from the top left */);
    }
}
