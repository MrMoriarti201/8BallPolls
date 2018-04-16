using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Xsolla;

public class SetText : MonoBehaviour {

	public InputField input;
	public string token = "";

	public void SetTextToInput(){
		input.GetComponent<Xsolla.XsollaSDK> ().SetToken(token);
		input.text = token;	
	}

}
