using UnityEngine;
using System.Collections;

public class EyeMovement : MonoBehaviour {
	
	private float camDist;
	private Vector3 camVec;
	private float cameraHeight;
	private bool isActive;

	[SerializeField]
	private GameObject player;
	[SerializeField]
	private int defaultZoom = 6;
	[SerializeField]
	private int minZoom = 10;
	[SerializeField]
	private int maxZoom =2;
	[SerializeField]
	private bool experimentalPositioning = false;
	[SerializeField]
	private float distance = 80;
	[SerializeField]
	private float verticalAngle = 45;
	[SerializeField]
	private float horizontalAngle = -45;
	[SerializeField]
	private int zoomStep = 4;

	public GameObject Player 
	{
		get{return player;}
		set{player = value;}
	}
	public bool IsActive 
	{
		get{return isActive;} 
		set{
			if(value == true)
				FindPlace();
			GetComponentInChildren<GUIButtons>().enabled=value;
			isActive = value;
		}

	}
	
	// Use this for initialization
	void Start () {
		if (player!=null)
		{ 
			isActive = true;
			FindPlace();
		}
		else
			isActive =false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(isActive){
			var varY = transform.position.y;
			transform.position = player.transform.position + camVec.normalized * camDist;
			transform.position = new Vector3(transform.position.x,varY,transform.position.z);

			//zooming in
			if (Input.GetAxis("Mouse ScrollWheel") > 0)
			{
				ZoomIn();
			}
			//zooming out			
			if (Input.GetAxis("Mouse ScrollWheel") < 0)
			{
				ZoomOut();
			}
			//Rotating the camera
			if (Input.GetAxis("CamHorizontal") != 0)
			{
				horizontalAngle += Input.GetAxis("CamHorizontal");
				//Debug.Log((Input.GetAxis("CamVertical") < 0) ? "Left!" : "Right!");
			}
			//default zoom
			if (Input.GetKeyDown(KeyCode.Mouse2))
			{
				ResetZoom();
			}
		}
        if(experimentalPositioning)
		    FindPlace();
	}
	
	void LateUpdate(){
		//transform.position = new Vector3(transform.position.x,cameraHeight,transform.position.z);  
	}
	
	public void ZoomIn()
	{
        if (GetComponent<Camera>().orthographicSize > minZoom)
		{
            print("Zoom IN!");
			GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize - zoomStep;
			//transform.LookAt(player.transform);
		}
	}
	public void ZoomOut()
	{
        if (GetComponent<Camera>().orthographicSize < maxZoom)
		{
            print("Zoom OUT!");
			GetComponent<Camera>().orthographicSize = GetComponent<Camera>().orthographicSize + zoomStep;
			//transform.LookAt(player.transform);
		}
	}
	public void ResetZoom()
	{
        print("Reset Zoom");
        GetComponent<Camera>().orthographicSize = defaultZoom;
		//transform.LookAt(player.transform);
	}
	void FindPlace()
	{
		//player =p;
		//		if (player==null)
		//			player = GameObject.FindWithTag("Player");
		
		float a = horizontalAngle * Mathf.PI / 180;
		float b = verticalAngle * Mathf.PI / 180;
		float c = distance;
		
		float x = c/Mathf.Sqrt(1+(Mathf.Tan(a)*Mathf.Tan(a))+(Mathf.Tan(b)*Mathf.Tan(b))+(Mathf.Tan(a)*Mathf.Tan(a))*(Mathf.Tan(b)*Mathf.Tan(b)));
		float y = Mathf.Tan(b)*Mathf.Sqrt(c*c/(1+(Mathf.Tan(b))*Mathf.Tan(b)));
		float z = (c*(Mathf.Tan(a))) / Mathf.Sqrt((1+(Mathf.Tan(a)*Mathf.Tan(a)))*(1+(Mathf.Tan(b)*Mathf.Tan(b))));
		
		transform.position = new Vector3(x,y,z);
        if (player != null)
        {
            camDist = Vector3.Distance(player.transform.position, transform.position);
            camVec = transform.position - player.transform.position;
            cameraHeight = transform.position.y;

            //print(camDist);
            //print(distance);

            GetComponent<Camera>().orthographicSize = defaultZoom;
            transform.LookAt(player.transform);
        }
	}
}