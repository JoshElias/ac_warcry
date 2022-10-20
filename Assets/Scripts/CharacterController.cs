using UnityEngine;
	
public class CharacterController: MonoBehaviour {
		

	
	// Movement State Variables
	const float HumanSwarmRunningSpeed = 5.0f;
	const float DactlyRunningSpeed = 6.0f;
	const float ApeRunningSpeed = 8.0f;
	
	const float ClimbingSpeed = 1.0f;
	const float GlidingSpeed = 6.0f;
	const float SlidingSpeed = 5.0f;
	const float CrouchSpeed = 1.0f;
	
	const int HorizontalJumpAmount = 00;
	
	const float DefaultMass = 10.0f;
	const float SlidingMass = 0.2f;
	
	//please do not change these, they cannot be made const
	Vector3 HumanApeJumpImpulse = new Vector3(0,50,0);
	Vector3 DactylJumpImpulse = new Vector3(0,100,0);
	Vector3 SwarmJumpImpulse = new Vector3(0,0,0);

	
	Vector3 DefaultGravity = new Vector3(0,-100,0);
	Vector3 SlidingGravity = new Vector3(0,-50,0);
	Vector3 ClimbingGravity = new Vector3(0, 0,0);
	
	
	// Speed, Drag
	// Harpy Vertical Velocity
	
	
	
	
	public float MoveSpeed;
	public Vector3 Forward;
	public Vector3 JumpImpulse;
	public Vector3 CharacterGravity;
	
	Vector3 PositionToClimbTo;
	Vector3 Velocity;
	Vector3 TargetVelocity;
	Vector3 VelocityChange;
	float MaxVelocity;
	
	
	bool JumpFlag = false;
	
	//fuck you Josh.
	//for optimization purposes
	bool NeedToResetMovementVariables = false;
	
	public float JumpSpeed = 6f;
	
	//used for changing animation and blending them
	float AbsoluteGroundAngle = 0.0f;
	//used for jumping off an inclined plane while sliding
	float TrueGroundAngle = 0.0f;
	
	float CurrentMass = DefaultMass;
		
		
	void Awake()
	{
		// Make sure the capsule collider stays....erect?
		rigidbody.freezeRotation = true;
		// Disable Gravity so we can isolate the player's physics
		rigidbody.useGravity = false;
		
		// Speed Variables
		MoveSpeed = HumanSwarmRunningSpeed;
		Velocity = Vector3.zero;
		TargetVelocity = Vector3.zero;
		VelocityChange = Vector3.zero;
		MaxVelocity = 20.0f;
		JumpImpulse = HumanApeJumpImpulse;
		CharacterGravity = DefaultGravity;
		
		// Get Initial Direction
		Forward = Vector3.right;		
	}

	void Update()
	{	
		//see whether we are running, jumping, idle, etc and change our physics depends on that.
		if(NeedToResetMovementVariables)
		{
			NeedToResetMovementVariables = false;
			ChangeCurrentStateValues();
		}
		ChangeCurrentFormValues();
		switch(PlayerManager.Instance().CurrentDirection)
		{
			case(PlayerManager.Direction.RIGHT):
			{
				JumpImpulse.x = HorizontalJumpAmount;
				break;	
			}
			case(PlayerManager.Direction.LEFT):
			{
				JumpImpulse.x = -HorizontalJumpAmount;
				break;	
			}
			default:
			{
				break;
			}
		}
		
		if(Input.GetKeyDown("c"))
		{
			if(PlayerManager.Instance().CurrentState == PlayerManager.ActionState.CROUCHING)
			{
				ChangeState(PlayerManager.ActionState.IDLE);				
			}
			else
			{
			ChangeState(PlayerManager.ActionState.CROUCHING);
			}
		}
		
		

		
		
		if((Velocity.x >= 0.2f || Velocity.x <= -0.2f) && PlayerManager.Instance().CurrentState != PlayerManager.ActionState.JUMPING 
			&& PlayerManager.Instance().CurrentState != PlayerManager.ActionState.SLIDING 
			&& PlayerManager.Instance().CurrentState != PlayerManager.ActionState.CLIMBING 
			&& PlayerManager.Instance().CurrentState != PlayerManager.ActionState.CROUCHING)
		{
			ChangeState(PlayerManager.ActionState.RUNNING);
		}
		else if(PlayerManager.Instance().CurrentState != PlayerManager.ActionState.JUMPING 
			&& PlayerManager.Instance().CurrentState != PlayerManager.ActionState.SLIDING
			&& PlayerManager.Instance().CurrentState != PlayerManager.ActionState.CLIMBING 
			&& PlayerManager.Instance().CurrentState != PlayerManager.ActionState.CROUCHING)
		{
			ChangeState(PlayerManager.ActionState.IDLE);	
		}
		//Debug.Log(gameObject.transform.forward);
		
	  
		//Debug.Log (Velocity.x);
		
	}
	
	void FixedUpdate()
	{			
		// Add dummy gravity
		rigidbody.AddForce(CharacterGravity);	
		
		//set our mass for our rigidbody
		rigidbody.mass = CurrentMass;		
		
		//if you aren't sliding, full control over movement
		if(PlayerManager.Instance().CurrentState != PlayerManager.ActionState.SLIDING)
		{
			TargetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
			
		}
		//if you are sliding, you have no control over your movement.
		else
		{
			TargetVelocity = Vector3.zero;	
		}
		
		CheckDirection();
		
		//TODO: Fix the left/right movement so that you have some control in midair,
		//      but are not constrained to how far you press down the left/right sticks.
		if(!JumpFlag)
		{
			TargetVelocity *= MoveSpeed;
			Velocity = gameObject.rigidbody.velocity;
		
			// Try to always approach our targetVelocity
			VelocityChange = (TargetVelocity - Velocity);
		
			// Clamp the velocity on the X Axis
			VelocityChange.x = Mathf.Clamp(VelocityChange.x, -MaxVelocity, MaxVelocity);
			VelocityChange.y = 0;
			VelocityChange.z = 0;
		
			// Apply
			gameObject.rigidbody.AddForce(VelocityChange, ForceMode.VelocityChange);
		}
		JumpingAndClimbing();
		
		if(PlayerManager.Instance().CurrentState == PlayerManager.ActionState.CLIMBING)
		{
			rigidbody.velocity = new Vector3(0, 0, 0);
		}
	}
	
	void OnCollisionEnter(Collision collision)
	{
		// The our ground angle by get the surface normal of the platform we are standing on
		AbsoluteGroundAngle = Mathf.Abs( Mathf.Rad2Deg * Mathf.Atan(collision.contacts[0].normal.x / collision.contacts[0].normal.y) );
		TrueGroundAngle = Mathf.Rad2Deg * Mathf.Atan(collision.contacts[0].normal.x / collision.contacts[0].normal.y);

		//if a character hits ground under the tag of "Plats", we know they are standing on a walkable surface
		if(collision.gameObject.tag == "Plats" && collision.gameObject.name != "WallGrabStartPrefab" && PlayerManager.Instance().CurrentState != PlayerManager.ActionState.CLIMBING)
		{
			if(PlayerManager.Instance().CurrentState != PlayerManager.ActionState.SLIDING && PlayerManager.Instance().CurrentState != PlayerManager.ActionState.CROUCHING)
			{
				ChangeState(PlayerManager.ActionState.IDLE);
			}
			JumpFlag = false;
		}
		
	}
	
	void OnCollisionExit(Collision collision)
	{
		
	}
	
	void OnCollisionStay(Collision collision)
	{
		AbsoluteGroundAngle = Mathf.Abs( Mathf.Rad2Deg * Mathf.Atan(collision.contacts[0].normal.x / collision.contacts[0].normal.y) );
		TrueGroundAngle = Mathf.Rad2Deg * Mathf.Atan(collision.contacts[0].normal.x / collision.contacts[0].normal.y);
		
		//if you are on a big enough incline, start sliding
		if(AbsoluteGroundAngle >45 && AbsoluteGroundAngle < 90 && PlayerManager.Instance().CurrentState != PlayerManager.ActionState.JUMPING)
		{
			ChangeState(PlayerManager.ActionState.SLIDING);
		}
		//otherwise, if you are on level-ish ground, start running
		if(AbsoluteGroundAngle < 45 
			&& PlayerManager.Instance().CurrentState != PlayerManager.ActionState.JUMPING 
			&&  PlayerManager.Instance().CurrentState != PlayerManager.ActionState.CROUCHING)
		{
			ChangeState(PlayerManager.ActionState.RUNNING);	
		}
	
	}
	
	
	
	void ChangeState(PlayerManager.ActionState newState)
	{
		if(newState != PlayerManager.Instance().CurrentState) 
		{
			PlayerManager.Instance().CurrentState = newState;
			NeedToResetMovementVariables = true;
		}
	}
	
	void OnTriggerEnter(Collider other) 
	{
        if(other.gameObject.name == "WallGrabVolume")
		{
			ChangeState(PlayerManager.ActionState.GRABBINGWALL);
		}
		
		//you have grabbed a wall brah!
		if(other.gameObject.name == "WallGrabStartPrefab")
		{
			ChangeState(PlayerManager.ActionState.CLIMBING);
			//set you position to the prefabs position, so you are hanging from the wall.
			gameObject.transform.position = other.gameObject.transform.position;
			//Vector3.Lerp (gameObject.transform.position, other.gameObject.transform.position,);
			foreach (Transform child in other.transform)
			{
   				PositionToClimbTo = child.transform.position;
			}
			//Debug.Log ("lerping");
			JumpFlag = false;
			
		}
    }
	
	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.name == "WallGrabStartPrefab")
		{
			ChangeState(PlayerManager.ActionState.JUMPING);
			
		}	
	}
	
	void JumpingAndClimbing()
	{
		if(Input.GetKeyDown("space") && !JumpFlag)
		{

			// climbs up onto the ledges or jumps off while hanging
			if(PlayerManager.Instance().CurrentState == PlayerManager.ActionState.CLIMBING)
			{
				if(Input.GetAxis("Horizontal") < 0)
				{
						gameObject.rigidbody.AddForce(new Vector3(-20,JumpImpulse.y,0), ForceMode.Impulse);
				}
				else if(Input.GetAxis("Horizontal") > 0)
				{
						gameObject.rigidbody.AddForce(new Vector3(20,JumpImpulse.y,0), ForceMode.Impulse);
				}
				else
				{
					gameObject.transform.position = PositionToClimbTo;
				}
			}
			else if (PlayerManager.Instance().CurrentState == PlayerManager.ActionState.SLIDING)
			{
				if(TrueGroundAngle > 0 && TrueGroundAngle < 90)
				{
					gameObject.rigidbody.AddForce(new Vector3(2,4,0), ForceMode.Impulse);
					Debug.Log(TrueGroundAngle);
				}
				if(TrueGroundAngle > -90 && TrueGroundAngle < 0)
				{
					gameObject.rigidbody.AddForce(new Vector3(-2,4,0), ForceMode.Impulse);
					Debug.Log(TrueGroundAngle);
				}
				
			}
			else if (PlayerManager.Instance().CurrentState != PlayerManager.ActionState.SLIDING)
			{
				gameObject.rigidbody.AddForce(HumanApeJumpImpulse,ForceMode.Impulse);	
			}
			ChangeState(PlayerManager.ActionState.JUMPING);
			JumpFlag = true;
		}
	}
	
	void ChangeCurrentStateValues()
	{
		switch(PlayerManager.Instance().CurrentState)
			{
			case(PlayerManager.ActionState.RUNNING):
				{
					MoveSpeed = HumanSwarmRunningSpeed;
					CurrentMass = DefaultMass;
					CharacterGravity = DefaultGravity;
					break;
				}
			
			case(PlayerManager.ActionState.IDLE):
				{
					MoveSpeed = HumanSwarmRunningSpeed;
					CurrentMass = DefaultMass;
					CharacterGravity = DefaultGravity;
					break;
				}
			
			case(PlayerManager.ActionState.SLIDING):
				{
					MoveSpeed = SlidingSpeed;
					CurrentMass = SlidingMass;
					CharacterGravity = SlidingGravity;
					break;
				}
				
				
			case(PlayerManager.ActionState.CLIMBING):
				{
					CharacterGravity = ClimbingGravity;
					break;
				}
			
			case(PlayerManager.ActionState.JUMPING):
				{
					CurrentMass = DefaultMass;
					CharacterGravity = DefaultGravity;
					break;
				}
			
			case(PlayerManager.ActionState.CROUCHING):
				{
				MoveSpeed = CrouchSpeed;
				break;
				}
			
			default:
				{
				
					break;
				}
			}
	}
	
	void ChangeCurrentFormValues()
	{
		switch(PlayerManager.Instance().CurrentForm)
			{
				case(PlayerManager.Forms.HUMAN):
				{
					MoveSpeed = HumanSwarmRunningSpeed;
					JumpImpulse = HumanApeJumpImpulse;
					break;
				}
				case(PlayerManager.Forms.APE):
				{
					MoveSpeed = ApeRunningSpeed;
					JumpImpulse = HumanApeJumpImpulse;
					break;
				}
				case(PlayerManager.Forms.DACTYL):
				{
					MoveSpeed = DactlyRunningSpeed;
					JumpImpulse = DactylJumpImpulse;
					break;
				}	
				case(PlayerManager.Forms.SWARM):
				{
					MoveSpeed = HumanSwarmRunningSpeed;
					JumpImpulse = SwarmJumpImpulse;
					break;	
				}
					
				default:
				{
					break;
				}
			}
	}
	
	void CheckDirection()
	{
		//just checking which direction you were going.
		if (Input.GetAxis("Horizontal") < 0)
		{
			PlayerManager.Instance().CurrentDirection = PlayerManager.Direction.LEFT;	
				
		}
		else if(Input.GetAxis("Horizontal") > 0)
		{
			PlayerManager.Instance().CurrentDirection = PlayerManager.Direction.RIGHT;
				
		}
	}
	
			
}
