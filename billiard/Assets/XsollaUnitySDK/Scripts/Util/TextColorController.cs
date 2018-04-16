using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextColorController : MonoBehaviour {

	const float onePercent = 2.55f;
	private Color thisBaseTextColor;
	private Color thisTextColor;
	public int Red = 255;
	public int Green = 255;
	public int Blue = 255;

	void Start() 
	{
		thisTextColor = GetComponent<Text> ().color;
		thisBaseTextColor = new Color (thisTextColor.r, thisTextColor.g, thisTextColor.b);
	}

	public void SelectBaseColor(bool b)
	{
		if (b)
			GetComponent<Text> ().color = thisBaseTextColor;
		else
			GetComponent<Text> ().color = new Color (Red/onePercent, Green/onePercent, Blue/onePercent);
	}
}
