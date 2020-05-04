using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Image life1, life2, life3, life4, timeband;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePlayerLivesHUD(int playerLives)
    {
        switch (playerLives)
        {
            case 4:
                life1.enabled = true;
                life2.enabled = true;
                life3.enabled = true;
                life4.enabled = true;
                break;
            case 3:
                life1.enabled = true;
                life2.enabled = true;
                life3.enabled = true;
                life4.enabled = false;
                break;
            case 2:
                life1.enabled = true;
                life2.enabled = true;
                life3.enabled = false;
                life4.enabled = false;
                break;
            case 1:
                life1.enabled = true;
                life2.enabled = false;
                life3.enabled = false;
                life4.enabled = false;
                break;
            case 0:
                life1.enabled = false;
                life2.enabled = false;
                life3.enabled = false;
                life4.enabled = false;
                break;
        }
    }
}
