using ConnectFour;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggiornaK : MonoBehaviour
{
    public int k;
    [SerializeField] private GameController gameController;
    // Start is called before the first frame update
    void Start()
    {
        k = 0;
    }

    // Update is called once per frame
    void Update()
    {
        gameController.K = k;
        Debug.Log("K: " + k);
    }
}
