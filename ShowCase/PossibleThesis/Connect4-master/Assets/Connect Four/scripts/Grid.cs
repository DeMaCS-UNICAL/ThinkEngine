using ConnectFour;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Cell
{
    int content;
    
    public Cell()
    {
        content = 0;
    }

    public Cell(int v)
    {
        content = v;
    }

    public int Content
    {
        get => content;
        set => content = value;
    }
}

[ExecuteInEditMode]
public class Grid : MonoBehaviour
{
    private Cell[,] ground;
    private int numRows;
    private int numColumns;
    //private Dictionary<int, int> freeCells;
    //private List<Cell> blueCells;
    //private List<Cell> redCells;
    //private List<int> possibleCols;
    private GameController gc;
    private Field gcField;

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
        if (gcField != null)
        {
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numColumns; j++)
                {
                    ground[j, i] = new Cell(gcField.field[j, i]);
                    //ground[j, i] = new Cell();
                    //ground[i, j].Content = gcField.field[i, j];
                }
            }
            for (int i = 0; i < numColumns; i++)
                Debug.Log(ground[i, 5].Content);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Inizializzo la griglia");
        gc = GetComponent<GameController>();
        gcField = gc.GetField();
        if (gcField != null)
        {
            numColumns = gcField.NumColumns;
            numRows = gcField.NumRows;
            ground = new Cell[numColumns, numRows];
        }
    }

    // Update is called once per frame
    void Update()
    {
        gcField = gc.GetField();
        GetField();
    }
}
