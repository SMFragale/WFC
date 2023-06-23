using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Tile 2D Horizontal", menuName = "WFC/Tile 2D Horizontal")]
//Represents a tile in a 2D WFC model where items are placed horizontally
public class WFCTile2D : WFCTile
{
    [Space(10)]

    [Header ("Possible adyacent Tiles for each side")]
    //Each list contains all the possible tiles that each side can have
    [SerializeField]
    public List<WFCTile2D> UP;
    [SerializeField]
    public List<WFCTile2D> DOWN;
    [SerializeField]
    public List<WFCTile2D> LEFT;
    [SerializeField]
    public List<WFCTile2D> RIGHT;
}
