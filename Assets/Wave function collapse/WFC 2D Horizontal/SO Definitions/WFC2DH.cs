using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

// @Author Sebastian Molano (github.com/SMFragale)
// This class represents a 2D WFC implementation that can be used to generate a grid of procedurally generated tiles. 
// Use this class along with an ISpawnGrid2D implementation to generate a grid of tiles. The ISpawnGrid2D component is required for the algorithm to display anything.
// This is a greedy implementation with some minor optimizations. The algorithm is not optimized for performance, but for readability and understanding.
// In the future it would be great to add things like backtracking and other optimizations to make the algorithm faster, but for now this is a good starting point.
public class WFC2DH : WFC2D

//TODO Check the directions UP DOWN LEFT RIGHT
{
    //Grid that stores the prefab and other important information for each cell
    protected SpawnInfo2D[,] matrix;

    //List of cells that will be used to calculate the next iteration. This is saved as state so that the SpawnIteration method will work without parameters 
    private List<Vector2Int> currentIterationCells;

    //In debug mode, the spawn iteration method can be called from the inspector to review each step of the algorithm
    public bool debugMode = true;

    //Time in seconds between each iteration for the animation
    [Range(0.5f, 1000)]
    public float iterationSpeed = 1.0f;

    protected override void Start() {
        base.Start();
        matrix = new SpawnInfo2D[spawnGrid.width, spawnGrid.height];
        currentIterationCells = new List<Vector2Int>();

        //Setup each tile in the grid
        for(int i = 0; i < spawnGrid.width; i++) {
            for(int j = 0; j < spawnGrid.height; j++) {
                SpawnInfo2D initialInfo = new SpawnInfo2D(Quaternion.identity, null, possibleTiles.Count);
                initialInfo.options = possibleTiles;
                matrix[i, j] = initialInfo;
                currentIterationCells.Add(new Vector2Int(i, j));
            }
        }
    }

    public override void Generate()
    {
        StartCoroutine(SpawnIterationCoroutine());
    }

    private IEnumerator SpawnIterationCoroutine() {
        while(SpawnIteration() == false) {
            yield return new WaitForSeconds(1 / iterationSpeed);
        }

        bool allCellsCollapsed = true;

        for(int i = 0; i < matrix.GetLength(0) && allCellsCollapsed; i++) {
            for(int j = 0; j < matrix.GetLength(1) && allCellsCollapsed; j++) {
                if (!matrix[i,j].collapsed) {
                    allCellsCollapsed = false;
                    Debug.LogError("The algorithm did not collapse all the cells");
                } 
            }
        }

        if(!allCellsCollapsed) {
            Debug.LogError("Trying again.");
            this.Start();
            this.spawnGrid.Clear();
            this.Generate();
        }
    }

    public override bool SpawnIteration() {
        if(currentIterationCells.Count == 0) {
            Debug.Log("No more cells to iterate");
            return true;
        }

        //Pick a cell with the lowest entropy
        Vector2Int lowestEntropyCell = FindLowestEntropyCell(currentIterationCells);

        Debug.Log("Got lowest entropy cell: " + lowestEntropyCell);

        //Pick a random tile from the cell's options
        WFCTile2D tile = GetRandomTileFromCell(lowestEntropyCell);

        if(tile == null) {
            return true;
        }
        
        //Collapse the cell
        CollapseCell(lowestEntropyCell, tile);

        //Update the entropy and options for each calculated cell
        RecalculateEntropy(lowestEntropyCell);

        //Update the cells that will be used on the next iteration
        FindCurrentIterationCells();
        
        return false;
    }

    // Returns the cell with the lowest entropy within the adyacents of a recently collapsed cell, if there are more than one, returns a random one between those
    // This does not calculate the entropy, it only finds the lowest entropy cell
    private Vector2Int FindLowestEntropyCell(List<Vector2Int> adyacentCells) {
        List<Vector2Int> lowestEntropyCells = new List<Vector2Int>();
        int lowestEntropy = int.MaxValue;

        foreach(var cell in adyacentCells) {
            if (matrix[cell.x, cell.y].entropy < lowestEntropy) {
                    lowestEntropy = matrix[cell.x, cell.y].entropy;
                    lowestEntropyCells.Clear();
                    lowestEntropyCells.Add(new Vector2Int(cell.x, cell.y));
                }
                else if (matrix[cell.x, cell.y].entropy == lowestEntropy) {
                    lowestEntropyCells.Add(new Vector2Int(cell.x, cell.y));
                }
        }

        int randomIndex = Random.Range(0, lowestEntropyCells.Count);
        Vector2Int lowestEntropyCell = lowestEntropyCells[randomIndex];
        return lowestEntropyCell;
    }

    private WFCTile2D GetRandomTileFromCell(Vector2Int cell) {
        int randomIndex = Random.Range(0, matrix[cell.x, cell.y].options.Count);
        Debug.Log("Options cound: " + matrix[cell.x, cell.y].options.Count + " Random index: " + randomIndex + "");

        WFCTile2D tile = null;

        if(matrix[cell.x, cell.y].options.Count == 0) {
            Debug.LogError("No options found for cell: " + cell);
        }
        else {
            tile = matrix[cell.x, cell.y].options[randomIndex];
        }
       
        return tile;
    }

    private void CollapseCell(Vector2Int cell, WFCTile2D tile) {
        //Mark the cell as collapsed
        matrix[cell.x, cell.y].collapsed = true;
        matrix[cell.x, cell.y].tile = tile;

        if(debugMode == true) {
            if(tile.prefab != null)
                spawnGrid.Spawn(cell.x, cell.y, tile.prefab, Quaternion.identity);
        }

        //0 means that the cell is collapsed
        matrix[cell.x, cell.y].entropy = 0;
    }

    private List<Vector2Int> GetUnCollapsedAdyacentCells(Vector2Int cell) {
        List<Vector2Int> adyacentCells = new List<Vector2Int>();

        Vector2Int UP = new Vector2Int(cell.x, cell.y - 1);
        Vector2Int DOWN = new Vector2Int(cell.x, cell.y + 1);
        Vector2Int LEFT = new Vector2Int(cell.x - 1, cell.y);
        Vector2Int RIGHT = new Vector2Int(cell.x + 1, cell.y);

        //Check if the cell is inside the grid and if it's not collapsed
        if (UP.y >= 0 && UP.y < spawnGrid.height && !matrix[UP.x, UP.y].collapsed) 
            adyacentCells.Add(UP);
        if (DOWN.y >= 0 && DOWN.y < spawnGrid.height && !matrix[DOWN.x, DOWN.y].collapsed)
            adyacentCells.Add(DOWN);
        if (LEFT.x >= 0 && LEFT.x < spawnGrid.width && !matrix[LEFT.x, LEFT.y].collapsed) 
            adyacentCells.Add(LEFT);
        if (RIGHT.x >= 0 && RIGHT.x < spawnGrid.width && !matrix[RIGHT.x, RIGHT.y].collapsed)
            adyacentCells.Add(RIGHT);

        return adyacentCells;
    }

    private List<Vector2Int> GetCollapsedAdyacentCells(Vector2Int cell) {
        List<Vector2Int> adyacentCells = new List<Vector2Int>();

        Vector2Int UP = new Vector2Int(cell.x, cell.y + 1);
        Vector2Int DOWN = new Vector2Int(cell.x, cell.y - 1);
        Vector2Int LEFT = new Vector2Int(cell.x - 1, cell.y);
        Vector2Int RIGHT = new Vector2Int(cell.x + 1, cell.y);

        //Check if the cell is inside the grid and if it's collapsed
        if (UP.y >= 0 && UP.y < spawnGrid.height && matrix[UP.x, UP.y].collapsed)
            adyacentCells.Add(UP);
        if (DOWN.y >= 0 && DOWN.y < spawnGrid.height && matrix[DOWN.x, DOWN.y].collapsed)
            adyacentCells.Add(DOWN);
        if (LEFT.x >= 0 && LEFT.x < spawnGrid.width && matrix[LEFT.x, LEFT.y].collapsed)
            adyacentCells.Add(LEFT);
        if (RIGHT.x >= 0 && RIGHT.x < spawnGrid.width && matrix[RIGHT.x, RIGHT.y].collapsed)
            adyacentCells.Add(RIGHT);

        return adyacentCells;
    }

    private List<CellFacingFrom> GetCollapsedAdyacentCellsAndFace(Vector2Int cell) {
        List<CellFacingFrom> adyacentCells = new List<CellFacingFrom>();

        Vector2Int UP = new Vector2Int(cell.x, cell.y + 1);
        Vector2Int DOWN = new Vector2Int(cell.x, cell.y - 1);
        Vector2Int LEFT = new Vector2Int(cell.x - 1, cell.y);
        Vector2Int RIGHT = new Vector2Int(cell.x + 1, cell.y);

        //Check if the cell is inside the grid and if it's collapsed
        if (UP.y >= 0 && UP.y < spawnGrid.height && matrix[UP.x, UP.y].collapsed)
            adyacentCells.Add(new CellFacingFrom(UP, CellFacingFrom.FaceDirection.DOWN));
        if (DOWN.y >= 0 && DOWN.y < spawnGrid.height && matrix[DOWN.x, DOWN.y].collapsed)
            adyacentCells.Add(new CellFacingFrom(DOWN, CellFacingFrom.FaceDirection.UP));
        if (LEFT.x >= 0 && LEFT.x < spawnGrid.width && matrix[LEFT.x, LEFT.y].collapsed)
            adyacentCells.Add(new CellFacingFrom(LEFT, CellFacingFrom.FaceDirection.RIGHT));
        if (RIGHT.x >= 0 && RIGHT.x < spawnGrid.width && matrix[RIGHT.x, RIGHT.y].collapsed)
            adyacentCells.Add(new CellFacingFrom(RIGHT, CellFacingFrom.FaceDirection.LEFT));

        return adyacentCells;
    }

    

    // Recalculates the entropy of a cell and all it's adyacents. The cell must have already been collapsed in order to work
    private void RecalculateEntropy(Vector2Int cell) {

        List<Vector2Int> adyacentCells = GetUnCollapsedAdyacentCells(cell);

        foreach(var adyacentCell in adyacentCells) {

            //Debug.Log("\nFor adyacent cell: (" + adyacentCell.x + "," + adyacentCell.y + ") ");

            //Get the adyacent cells that have been collapsed and have their face direction
            List<CellFacingFrom> collapsedAdyacents = GetCollapsedAdyacentCellsAndFace(adyacentCell);

            //Debug.Log("There are " + collapsedAdyacents.Count + " collapsed adyacents");

            List<WFCTile2D> options = null;

            foreach(var cellPos in collapsedAdyacents) {
                //Find the tiles that match the collapsed adyacents based on their face direction for each collapsed adyacent

                //The directions for the Down and UP tiles are inverted because the grid is mirrored on the z axis to match the form of a C# matrix
                List<WFCTile2D> matchingTiles = new List<WFCTile2D>();
                if(cellPos.facingFrom == CellFacingFrom.FaceDirection.UP) {
                    matchingTiles.AddRange(matrix[cellPos.cell.x, cellPos.cell.y].tile.DOWN);
                }
                else if (cellPos.facingFrom == CellFacingFrom.FaceDirection.DOWN) {
                    matchingTiles.AddRange(matrix[cellPos.cell.x, cellPos.cell.y].tile.UP);
                }
                else if (cellPos.facingFrom == CellFacingFrom.FaceDirection.RIGHT) {
                    matchingTiles.AddRange(matrix[cellPos.cell.x, cellPos.cell.y].tile.RIGHT);
                }
                else if (cellPos.facingFrom == CellFacingFrom.FaceDirection.LEFT) {
                    matchingTiles.AddRange(matrix[cellPos.cell.x, cellPos.cell.y].tile.LEFT);
                }

                if(options == null) {
                    options = matchingTiles;
                }
                else {
                    //Intersect the options to find the tiles that can only be applied to the cell
                    options = options.Intersect(matchingTiles).ToList();
                }

                //Debug.Log("There are " + matchingTiles.Count + " matching tiles for the adyacent cell (" + cellPos.cell.x + "," + cellPos.cell.y + ")\n");

            }

            //TODO check if the algorithm cannot continue because there are no options left

            //Update the entropy of the cell and the matrix
            matrix[adyacentCell.x, adyacentCell.y].entropy = options.Count;
            matrix[adyacentCell.x, adyacentCell.y].options = options;            
        }
    }

    private List<Vector2Int> GetAllCollapsedCells() {
        List<Vector2Int> collapsedCells = new List<Vector2Int>();

        for(int i = 0; i < matrix.GetLength(0); i++) {
            for(int j = 0; j < matrix.GetLength(1); j++) {
                if(matrix[i, j].collapsed) {
                    collapsedCells.Add(new Vector2Int(i, j));
                }
            }
        }

        return collapsedCells;
    }


    private void FindCurrentIterationCells() {
        //Will find all the cells that are not collapsed adyacent to each collapsed cell
        List<Vector2Int> collapsedCells = GetAllCollapsedCells();

        List<Vector2Int> currentIterationCells = new List<Vector2Int>();

        foreach(var cell in collapsedCells) {
            List<Vector2Int> adyacentCells = GetUnCollapsedAdyacentCells(cell);

            foreach(var adyacentCell in adyacentCells) {
                if(!currentIterationCells.Contains(adyacentCell)) {
                    currentIterationCells.Add(adyacentCell);
                }
            }
        }

        this.currentIterationCells = currentIterationCells;
    }


    // For debugging purposes ----------------------------------------------

    private void PrintMatrix() {
        //TODO check if this is the best way to print the matrix
        string matrixString = "";
        for(int i = 0; i < matrix.GetLength(0); i++) {
            for(int j = 0; j < matrix.GetLength(1); j++) {
                if(matrix[i, j] == null) {
                    matrixString += "null ";
                }
                else {
                    matrixString += matrix[i, j].tile.name + " ";
                }
            }
            matrixString += "\n";
        }
        Debug.Log(matrixString);
    }

    private void PrintCurrentCollapsedCells() {
        string matrixString = "CurrentCollapsedCells: \n";
        for (int i = 0; i < matrix.GetLength(0); i++) {
            for (int j = 0; j < matrix.GetLength(1); j++) {
                string value = matrix[j, i].collapsed ? "1" : "0";
                matrixString += value + "  ";
            }
            matrixString += "\n";
        }
        matrixString += "\n\n";
        Debug.Log(matrixString);
    }

    private void PrintEntropy() {
        //TODO check if this is the best way to print the matrix
        string matrixString = "Current Entropy: \n";
        for (int i = 0; i < matrix.GetLength(0); i++) {
            for (int j = 0; j < matrix.GetLength(1); j++) {
                matrixString += matrix[j, i].entropy + "  ";
            }
            matrixString += "\n";
        }
        matrixString += "\n\n";
        Debug.Log(matrixString);
    }

    private void PrintCurrentIterationCells() {
        string matrixString = "CurrentIterationCells: \n";
        foreach(var cell in currentIterationCells) {
            matrixString += cell.x + ", " + cell.y + "\n";
        }
        Debug.Log(matrixString);    
    }
}

//Contains the position of a cell with the side it is facing in respect to another cell
public class CellFacingFrom {
    public Vector2Int cell;
    public FaceDirection facingFrom;

    public CellFacingFrom(Vector2Int cell, FaceDirection facingFrom) {
        this.cell = cell;
        this.facingFrom = facingFrom;
    }

    public enum FaceDirection {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }
}

//Contains the information of a cell in the matrix
public class SpawnInfo2D
{
    public bool collapsed = false;

    public int entropy;

    public Quaternion rotation;
    
    public WFCTile2D tile;

    public List<WFCTile2D> options;

    public SpawnInfo2D(Quaternion rotation, WFCTile2D tile, int entropy) {
        this.rotation = rotation;
        this.entropy = entropy;
        this.tile = tile;
        options = new List<WFCTile2D>();
    }

}