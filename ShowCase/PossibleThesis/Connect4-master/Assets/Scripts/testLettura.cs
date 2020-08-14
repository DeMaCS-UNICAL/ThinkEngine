using ConnectFour;
using UnityEngine;

[ExecuteInEditMode]
public class testLettura : MonoBehaviour
{
    private Grid grid;
    [SerializeField] private GameController gc;

    public int provetta;
    // Start is called before the first frame update
    void Awake()
    {
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

            for (int i = 0; i < 3; i++)
            {
                // gc.SpawnPiece(new Vector3(i, 0f, 0f));
                Debug.Log("Rilascio la CELLA " + i);
            }

    }
}
