using UnityEngine;

public class PlayerAnimation: MonoBehaviour {
		
	public GameObject CurrentModel;
	
	public GameObject HumanModel;
	public GameObject DactylModel;
	public GameObject ApeModel;
	
	float GroundAngle = 0.0f;
	float InclineBlend = 0.0f;
	
	
	
	
	void Awake() {
			
		// Set Animations
		
		// Human
		HumanModel.animation.wrapMode = WrapMode.Loop;
		HumanModel.animation["Idle"].layer = 1;
		HumanModel.animation["Walk"].layer = 1;
		HumanModel.animation["Run"].layer = 1;
		HumanModel.animation["Falling"].layer = 1;
		HumanModel.animation.SyncLayer(1);
		
		// Dactyl
		DactylModel.animation.wrapMode = WrapMode.Loop;
		DactylModel.animation["Idle"].layer = 1;
		DactylModel.animation["Walk"].layer = 1;
		DactylModel.animation["Run"].layer = 1;
		DactylModel.animation["Gliding"].layer = 1;
		DactylModel.animation["Falling"].layer = 1;
		DactylModel.animation.SyncLayer(1);
		
		// Ape
		ApeModel.animation.wrapMode = WrapMode.Loop;
		ApeModel.animation["Idle"].layer = 1;
		ApeModel.animation["Walk"].layer = 1;
		ApeModel.animation["Run"].layer = 1;
		ApeModel.animation["Falling"].layer = 1;
		ApeModel.animation.SyncLayer(1);
		
		// Set Current Model
		PlayerManager.Instance().CurrentForm = PlayerManager.Forms.HUMAN;
		CurrentModel = HumanModel;
		CurrentModel.SetActiveRecursively(true);
		
		// Make sure no animations are playing before we start our script
		CurrentModel.animation.Stop();
		
		PlayerManager.Instance().Forward = gameObject.transform.forward;	
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if( !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift) ) {
			ChangeForm( PlayerManager.Forms.HUMAN );
		}
		else if( Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.RightShift) ) {
			ChangeForm( PlayerManager.Forms.SWARM );
		}
		else if( Input.GetKey(KeyCode.LeftShift) ) {
			ChangeForm( PlayerManager.Forms.DACTYL );
		}
		else if( Input.GetKey(KeyCode.RightShift) ) {
			ChangeForm( PlayerManager.Forms.APE );
		}
			
	}
	
	void FixedUpdate()
	{	
		// Get the current Direction
		if( PlayerManager.Instance().CurrentDirection == PlayerManager.Direction.RIGHT) {
			gameObject.transform.forward = PlayerManager.Instance().Forward;	
		} else {
			gameObject.transform.forward = -PlayerManager.Instance().Forward;
		}
		
		// Change Animation
		switch( PlayerManager.Instance().CurrentState ) {
			
			case (PlayerManager.ActionState.RUNNING):
				//if(rigidbody.velocity.x < 1.5f)
					CurrentModel.animation.CrossFade("Walk");
				if (Mathf.Abs(rigidbody.velocity.x) > 4.5f)
					CurrentModel.animation.Blend("Run" , (Mathf.Abs(rigidbody.velocity.x) / 4.5f) * 10.0f);
				break;
			
			case (PlayerManager.ActionState.JUMPING):
				CurrentModel.animation.CrossFade("Falling");
				break;
			
			default:
				CurrentModel.animation.CrossFade("Idle", 0.2f);
				break;
		}
		
		Debug.Log (rigidbody.velocity);
		
	}
	
	void OnCollisionEnter(Collision collision)
	{
		// Get an Incline blend thats between 0-1 to add weight to our animations
		//InclineBlend = GroundAngle / 45;

	}
	
	void ChangeForm( PlayerManager.Forms newForm ) {
		
		if(PlayerManager.Instance().CurrentForm != newForm) {
			PlayerManager.Instance().CurrentForm = newForm;
			CurrentModel.SetActiveRecursively(false);
			
			switch (newForm) {
				
			case PlayerManager.Forms.HUMAN:
				CurrentModel = HumanModel;
				
				break;
			case PlayerManager.Forms.DACTYL:
				CurrentModel = DactylModel;
				
				break;
			case PlayerManager.Forms.APE:
				CurrentModel = ApeModel;
				break;
				
			}
			
			CurrentModel.SetActiveRecursively(true);
		}
		
	}
	
}
