using UnityEngine;

[ExecuteInEditMode]
public class player : MonoBehaviour
{
    Brain brain;
    Grid grid;
    int rows;
    int cols;
    public bool mioTurno;
    int posX; //colonne
    int posY; //righe
    
    void Awake()
    {
        brain = GameObject.FindObjectOfType<Brain>();
        grid = GameObject.FindObjectOfType<Grid>();
        rows = grid.GetNumRows();
        cols = grid.GetNumColumns();
        mioTurno = true;
        //posX = 0; //colonne
        //posY = 0; //righe
        Debug.Log("player, metodo awake");
        InvokeRepeating("playing", 0, 0.2f);
    }

    public void playing()
    {
        if (mioTurno)
        {
            Debug.Log("player, mio turno");
            // grid.GetGround()[posX, posY].Content = 1;
            Debug.Log("Cella Modificata " + posX + " " + posY);
            mioTurno = false;
        }
    }

    //void Update()
    //{
    //    Awake();
    //    playing();
    //}
}


