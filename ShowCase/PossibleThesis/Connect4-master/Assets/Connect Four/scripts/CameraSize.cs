using UnityEngine;

namespace ConnectFour
{
	public class CameraSize : MonoBehaviour 
	{
		Camera cam;
		
		void Awake () 
		{
			cam = GetComponent<Camera>();
			cam.orthographic = true;
		}
		
		void LateUpdate()
		{
			float maxY = (GameObject.Find ("GameController").GetComponent<GameController>().numRows) + 2;

			cam.orthographicSize = maxY / 2f;
		}
	}
}
