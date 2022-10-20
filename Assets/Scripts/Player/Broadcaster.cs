using UnityEngine;
using System.Collections;

public abstract class Broadcaster {

	ArrayList m_Listeners = new ArrayList();
	void AddListener() {}
	void RemoveListener() {}
	void Notify() {}
} 
