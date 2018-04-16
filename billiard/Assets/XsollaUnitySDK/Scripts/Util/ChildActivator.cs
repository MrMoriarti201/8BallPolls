using UnityEngine;
using System.Collections;

public class ChildActivator : MonoBehaviour {

	public ActiveChildConroller parent;

	void OnEnable()
	{
		parent.ActivateOne ();
	}

	void OnDisable()
	{
		parent.DeactivateOne ();
	}

}
