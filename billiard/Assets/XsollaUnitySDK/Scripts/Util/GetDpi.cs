using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.UI;

public class GetDpi : MonoBehaviour {

	public Text text;
	// Use this for initialization
	void Start () {
		var stringBuilder = new StringBuilder();
		stringBuilder.Append("dpi=").Append (Screen.dpi).Append("\n");
		stringBuilder.Append("currentResolution height=")
			.Append (Screen.currentResolution.height).Append(" width=")
				.Append (Screen.currentResolution.width);
		text.text = stringBuilder.ToString();

	}

}
