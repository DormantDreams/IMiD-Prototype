using UnityEngine;
using System.Collections;

public class CharacterControllerLogic : MonoBehaviour {


	private Vector3 forward;
	private Vector3 right;

	[SerializeField]
	private Animator animator;
	[SerializeField]
	private float speed;
	[SerializeField]
	private float minSpeed;
	[SerializeField]
	private float maxSpeed;



	private float characterSpeed;

	public Animator Animator
	{
		get	{return this.animator;}
	}
	
	public float Speed
	{
		get	{return this.speed;}
	}


	float syncSpeed = 0f;
	Vector3 syncPositionOld = Vector3.zero;
	Vector3 syncPositionNew = Vector3.zero;
	Vector3 movementDir = Vector3.zero;
	//Vector3 lagPosition = Vector3.zero;
	Vector3 movement = Vector3.zero;
	float syncTime = 0f;
	float lastSyncTime = 0f;

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{

		if (stream.isWriting) 
		{
			syncPositionNew = transform.position;
			movementDir = transform.forward;
			stream.Serialize(ref movementDir);
			stream.Serialize(ref syncPositionNew);
			stream.Serialize(ref speed);
		} 
		else 
		{
			syncPositionOld = syncPositionNew;
			stream.Serialize(ref movementDir);
			stream.Serialize(ref syncPositionNew);
			stream.Serialize(ref speed);
			movement = syncPositionNew - syncPositionOld;

			syncTime = 0f;
			var T = Time.time - lastSyncTime;
			lastSyncTime = Time.time;
			//stream.Serialize(ref syncSpeed);
			//speed = syncSpeed;
		}
	}


	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		forward = GameObject.Find("Eye").transform.forward;
		forward. y = 0;
		forward = Vector3.Normalize(forward);
		right = Quaternion.Euler(new Vector3(0,90,0)) * forward;
		characterSpeed = maxSpeed - minSpeed;
		if (!GetComponent<NetworkView>().isMine)
			animator.applyRootMotion = false;
	}
	// Update is called once per frame
	void Update () {
		if (GetComponent<NetworkView>().isMine) {
#if UNITY_ANDROID
			var leftX = VirtualJoystickRegion.VJRnormals.x;
			var leftY = VirtualJoystickRegion.VJRnormals.y;
#else
			var leftX = Input.GetAxis ("JoyHorizontal");
			var leftY = Input.GetAxis ("JoyVertical");	
#endif
			var direction = new Vector3 (leftX, 0, leftY);
			speed = direction.magnitude;
			//syncSpeed = speed;
				if (speed > 0.1f) {
					Vector3 rightMovement = right * Time.deltaTime * leftX;
					Vector3 upMovement = forward * Time.deltaTime * leftY;
					Vector3 heading = Vector3.Normalize (rightMovement + upMovement);
					transform.forward = heading;

					animator.SetFloat("Speed",speed * characterSpeed + minSpeed);
					//animator.SetFloat ("Speed", speed);
				} else 
					animator.SetFloat ("Speed", 0);
			} 
		else {
			transform.forward = movementDir;
			transform.position = syncPositionOld;
			//speed = movement.magnitude;
			if(speed > 0.1f)
				animator.SetFloat ("Speed", speed);
			else 
				animator.SetFloat ("Speed", 0);
		}
	}
}
