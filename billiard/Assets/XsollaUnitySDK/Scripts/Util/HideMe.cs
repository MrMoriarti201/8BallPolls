using UnityEngine;
using System.Collections;

public class HideMe : MonoBehaviour {

	// Use this for initialization
	void Update () {
		if(isActiveAndEnabled == true)
			StartCoroutine (HideMePlease());
	}

	private IEnumerator HideMePlease(){
		yield return new WaitForSeconds(2);
		gameObject.SetActive (false);
	}

}
