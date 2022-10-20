using UnityEngine;

public class PlayerManager {
	
	public static PlayerManager m_Instance;
	public static PlayerManager Instance()
	{
		if(m_Instance == null)
			m_Instance = new PlayerManager();
		
		return m_Instance;
	}
	
	public enum ActionState
	{
		IDLE,
		JUMPING,
		RUNNING,
		SLIDING,
		CLIMBING,
		WADING,
		GLIDING,
		ATTACKING,
		STAGGARED,
		TAUNTING,
		GRABBINGWALL,
		CROUCHING
	};
	
	public enum Direction
	{
		LEFT,
		RIGHT
	};
	
	public enum Forms
	{
		HUMAN,
		DACTYL,
		APE,
		SWARM
	};
	// Use this for initialization
	
	public ActionState CurrentState;
	public Direction CurrentDirection;
	public Vector3 Forward;
	public Forms CurrentForm;
	
	
	void Awake () {
		
		m_Instance = this;
		
		CurrentState = ActionState.ATTACKING;
		CurrentDirection = Direction.RIGHT;
		CurrentForm = Forms.HUMAN;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
