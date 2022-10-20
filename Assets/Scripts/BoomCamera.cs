using UnityEngine;
using System.Collections;

public class BoomCamera : MonoBehaviour
{
	//what we're going to look at ( set in editor )
	public GameObject Target;
	
	//default camera ofset
	private Vector3 _cameraOffset = new Vector3(0.0f, 5.0f, -6.0f);
	
	//set by adding the the target's position and the camera's offset
	private Vector3 _desiredPosition;
	
	//how much dampening the camera has
	public float _drag = 0.1f;

	void Start ()
	{
	
		//Set Initial Camera Offset from Player
		transform.position = Target.transform.position + _cameraOffset;
		
		// Set Initial Rotation
		//transform.rotation = Quaternion.Euler(StartRotation);
	}
	
	void FixedUpdate ()
	{
		//set the camera's position
		_desiredPosition = Target.transform.position + _cameraOffset;
    	transform.position = Vector3.Lerp(transform.position, _desiredPosition, _drag);
				
		// Look at Player
		transform.LookAt(Target.transform);
	}
	
	//
	public void SetCameraPosition(float x, float y, float z)
	{
		Debug.Log("SetCameraPosition");
		_cameraOffset = new Vector3(x, y, z);
		
		return;
	}
	
	//
	public void SetDrag(float drag)
	{
		Debug.Log ("SetDrag");
		_drag = drag;
		
		return;
	}
}
