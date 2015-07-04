using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
	[SerializeField]
	private const string typeName = "IMiDGame";
	[SerializeField]
	private string gameName = "Room's Name";
	
	private bool isRefreshingHostList = false;
	private HostData[] hostList;
	
	private int menu = 0;
	private bool menuOn = true;
	private GameObject serverPlayer;
	public GameObject playerPrefab;
	[SerializeField]
	public Camera MenuCamera;
	[SerializeField]
	public Camera CharacterSelectCamera;
	[SerializeField]
	public Camera PlayerCamera;
	
	private float marginTop = (float)(Screen.height * 0.05);
	private float marginLeft = (float)(Screen.width/2);
	
	private float buttonWidth = (float)(Screen.width * 0.35);
	private float buttonHeight = (float)(Screen.height * 0.15);
	
	private int charSelected;
	
	void OnGUI ()
	{
		if (menuOn) {
			if (menu == 0) {
				if (GUI.Button (new Rect (marginLeft - buttonWidth/2, marginTop, buttonWidth, buttonHeight), "Start Playing"))
					menu = 1;
				if (GUI.Button (new Rect (marginLeft - buttonWidth/2, (marginTop + buttonHeight) * 1.15f, buttonWidth, buttonHeight), "Character Select"))
				{
					MenuCamera.enabled = false;
					CharacterSelectCamera.enabled = true;
					menu = 4;
				}
			}
			if (menu == 1) 
			{
				if (GUI.Button (new Rect (marginLeft - buttonWidth, marginTop, buttonWidth, buttonHeight), "Start Server")) 
				{
					menu = 5;
				}
				if (GUI.Button (new Rect (marginLeft - buttonWidth, (marginTop*2 + buttonHeight), buttonWidth, buttonHeight), "Refresh Hosts"))
					RefreshHostList ();
				if (GUI.Button (new Rect (marginLeft - buttonWidth, (marginTop*3 + buttonHeight*2), buttonWidth, buttonHeight), "Return"))
					menu = 0;
				if (hostList != null) {
					for (int i = 0; i < hostList.Length; i++) {
						if (GUI.Button (new Rect ((marginLeft + buttonWidth  * 0.2f),(marginTop * 1.1f * (i+1) + buttonHeight *i), buttonWidth, buttonHeight), hostList [i].gameName)) {
							PlayerCamera.enabled = true;
							CharacterSelectCamera.enabled = false;
							menuOn = false;
							JoinServer (hostList [i]);
						}
					}
				}
				
			}
			if(menu == 3)
			{
				if (GUI.Button (new Rect (marginLeft - buttonWidth/2, marginTop, buttonWidth, buttonHeight), "Leave"))
				{
					Destroy(serverPlayer);
					Network.Disconnect();
					menu = 1;
				}
				if (GUI.Button (new Rect (marginLeft - buttonWidth/2, marginTop * 2f + buttonHeight, buttonWidth, buttonHeight), "Return to game"))
				{
					menuOn = false;
					
					PlayerCamera.enabled = true;
					MenuCamera.enabled = false;

					GameObject.Find("Eye").GetComponentInChildren<GUIButtons>().enabled=true;
				}
			}
			if(menu == 4)
			{
				if (GUI.Button (new Rect (Screen.width / 2f, Screen.height - marginTop - buttonHeight, buttonWidth/2, buttonHeight), "Blacksmith"))
				{
					menu = 1;
					charSelected = 1;
				}
			}
			if(menu == 5)
			{
				gameName = GUI.TextField (new Rect (Screen.width/2 - buttonWidth/2, marginTop, buttonWidth, buttonHeight/3), gameName);
				if (GUI.Button (new Rect (Screen.width/2 - buttonWidth/2, marginTop * 2f + buttonHeight/3, buttonWidth, buttonHeight), "Launch")) 
				{
					PlayerCamera.enabled = true;
					CharacterSelectCamera.enabled = false;
					menuOn = false;
					StartServer ();
				}
				if (GUI.Button (new Rect (Screen.width/2 - buttonWidth/2, (marginTop * 3f + 4*buttonHeight/3) , buttonWidth, buttonHeight), "Cancel")) 
				{
					menu = 0;
				}
			}
		}
	}
	
	public void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		if(Network.isServer)
		foreach (var player in Network.connections) {
			Network.DestroyPlayerObjects(player);
			Network.RemoveRPCs(player);
		}
	}
	
	private void StartServer ()
	{
		Network.InitializeServer (5, 25000, !Network.HavePublicAddress ());
		MasterServer.RegisterHost (typeName, gameName);
	}
	
	void OnServerInitialized ()
	{
		SpawnPlayer ();
	}
	
	
	void Update ()
	{
		if (Input.GetKeyDown ("escape")) 
		{
			PlayerCamera.enabled = false;
			MenuCamera.enabled = true;
			menu = 3;
			menuOn = true;
			GameObject.Find("Eye").GetComponentInChildren<GUIButtons>().enabled=false;
		}
		if (isRefreshingHostList && MasterServer.PollHostList ().Length > 0) {
			isRefreshingHostList = false;
			hostList = MasterServer.PollHostList ();
		}
	}
	
	private void RefreshHostList ()
	{
		if (!isRefreshingHostList) {
			isRefreshingHostList = true;
			MasterServer.RequestHostList (typeName);
		}
	}
	
	
	private void JoinServer (HostData hostData)
	{
		#if UNITY_ANDROID
		//AndroidJNI.CallIntMethod ();
		#endif
		Network.Connect (hostData);
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Debug.Log("Clean up after player " + player);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
	
	void OnConnectedToServer ()
	{
		SpawnPlayer ();
	}
	
	
	private void SpawnPlayer ()
	{
		GameObject player;
		if(Network.isServer)
		{
			player = (GameObject) Network.Instantiate (playerPrefab, new Vector3(7,1.1f,8), Quaternion.identity, 0);
			serverPlayer = player;
			PlayerCamera.GetComponent<EyeMovement>().Player = player;
			PlayerCamera.GetComponent<EyeMovement>().IsActive = true;
		}
		else
		{
			player = (GameObject)Network.Instantiate (playerPrefab, new Vector3(7,1.1f,8), Quaternion.identity, 0);
			PlayerCamera.GetComponent<EyeMovement>().Player = player;
			PlayerCamera.GetComponent<EyeMovement>().IsActive = true;
		}
		if(GetComponent<NetworkView>().isMine)
			player.tag = "Player";
	}
}
