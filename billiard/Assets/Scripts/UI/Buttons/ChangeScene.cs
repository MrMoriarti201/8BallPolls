using UnityEngine;
using System.Collections;

public class ChangeScene : MonoBehaviour {

	public void goScene(string SceneName){
//		Debug.Log ("Change Scene:"+SceneName);
		if (AudioController.Instance!=null)
			AudioController.Play("Button");
		if (SceneName=="CloseShop")
            GameManager.instance.LoadScene(GoShop.lastScene);
		else
            GameManager.instance.LoadScene(SceneName);
//		Debug.Log ("Change Scene2:"+SceneName);
	}
}
