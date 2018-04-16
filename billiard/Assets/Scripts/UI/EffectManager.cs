using UnityEngine;
using System.Collections;

public class EffectManager : MonoBehaviour {
	public static EffectManager instance;

	public GameObject coinEffect;

	void Awake(){
		instance=this;
	}
	// Use this for initialization
	void Start () {
	
	}

	public void Init(){
	}

}
