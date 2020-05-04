using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PositionToInt:MonoBehaviour
    {
        public int x;
        public int y;

        private void Update()
        {
            x= (int)(transform.position.x+0.499f);
            y = (int)(transform.position.y + 0.499f);
        }
    }
