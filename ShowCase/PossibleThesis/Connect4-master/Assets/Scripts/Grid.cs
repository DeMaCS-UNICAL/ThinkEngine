using ConnectFour;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Cell
{
    int x;
    int y;
    int content;

    public int Content
    {
        get { return this.content; }
        set { this.content = value; }
    }

    public int X
    {
        get { return this.x; }
        set { this.x = value; }
    }

    public int Y
    {
        get { return this.y; }
        set { this.y = value; }
    }

    //public int x { get; set; }
    //public int y { get; set; }
    //public int content { get; set; }
    public Cell()
    {
        content = 0;
        x = 0;
        y = 0;
    }

    public Cell(int v)
    {
        content = v;
        x = 0;
        y = 0;
    }
}

[ExecuteInEditMode]
public class Grid : MonoBehaviour
{
    private Cell[,] ground;
    private int numRows;
    private int numColumns;
    private GameController gc;
    public Field gcField;

    public Cell[,] GetGround()
    {
        return ground;
    }

    public int GetNumRows()
    {
        return numRows;
    }
    public void SetNumRows(int rows)
    {
        numRows = rows;
    }

    public int GetNumColumns()
    {
        return numColumns;
    }
    public void SetNumColumns(int columns)
    {
        numColumns = columns;
    }

    public void GetField()
    {
        gcField = gc.GetField();
        if (gcField != null)
        {
            Debug.Log("GCfield is NOT NULL");
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numColumns; j++)
                {
                    ground[j, i] = new Cell(gcField.field[j, i]);
                }
            }
        }
        else
            Debug.Log("gcField is NULL");
    }

    void Awake()
    {
        Debug.Log("Inizializzo la griglia");
        gc = GetComponent<GameController>();
        numColumns = gc.numColumns;
        numRows = gc.numRows;
        Debug.Log("NUMRows = " + numRows);
        Debug.Log("NUMCols = " + numColumns);
        ground = new Cell[numColumns, numRows];
        for (int i = 0; i < numRows; i++)
        {
            for (int j = 0; j < numColumns; j++)
            {
                ground[j, i] = new Cell(0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetField();
        //for (int i = 0; i < numColumns; i++)
        //    Debug.Log("Update grid cella [ " + i + "," + 5 + " ] = " + ground[i, 5].Content);
    }
}