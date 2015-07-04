using UnityEngine;
using System.Collections;

public class GUIButtons : MonoBehaviour {
    public GameObject cam;


	void OnGUI () {
		int ScrWidth = Screen.width;
		int ScrHeight = Screen.height;
        // Make a background box
        //GUI.Box(new Rect(0.6f*ScrWidth,0,0.4f*ScrWidth,0.4f*ScrHeight), "Zoom");
        if (cam != null)
        {
            // Make the first button. If it is pressed, Application.Loadlevel (1) will be executed
            if (GUI.Button(new Rect(0.67f * ScrWidth + 10, 10, 0.15f * ScrWidth, 0.15f * ScrWidth), "+"))
            {
                //	Application.LoadLevel(1);

               cam.GetComponent<EyeMovement>().ZoomIn();

            }

            // Make the second button.
            if (GUI.Button(new Rect(0.83f * ScrWidth + 10, 10, 0.15f * ScrWidth, 0.15f * ScrWidth), "-"))
            {
                //	Application.LoadLevel(2);
                cam.GetComponent<EyeMovement>().ZoomOut();
            }
            if (GUI.Button(new Rect(20, 20, 0.15f * ScrWidth, 0.05f * ScrWidth), "RESET CAMERA"))
            {
                //	Application.LoadLevel(2);
                cam.GetComponent<EyeMovement>().ResetZoom();
            }
        }
	}
	// Use this for initialization
	//void Start () {
	
	//}
	
	// Update is called once per frame
	//void Update () {
	
	//}
}
