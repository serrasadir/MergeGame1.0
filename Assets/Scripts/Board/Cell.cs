using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int x, y;
    
    public bool IsOccupied { get; private set; }


    public void SetOccupied(bool occupied)
    {
        IsOccupied = occupied;
    }

    public void Setup(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

}
