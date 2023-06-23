using UnityEngine;
using System.Collections.Generic;

// This class representes a WFC model that can be used to generate a grid
// The point of this Model is that it will be able to connect to other models to create a larger grid much
// much like in sudoku, however new rules can be applied such as being able to connect equivalent models together seamlessly
// and also being able to connect models that share no tiles together. 

[CreateAssetMenu(fileName = "WFC2D Model", menuName = "WFC/WFC2D Model")]
public class WFC2DModel : ScriptableObject
{
    public List<WFCTile2D> possibleTiles;
}