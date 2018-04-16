using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActiveChildConroller : MonoBehaviour {

	int count = 0;
	bool isActive = true;

	public void ActivateOne()
	{
		count++;
		if(!isActive){
			GetComponent<LayoutElement>().gameObject.SetActive(true);
			isActive = true;
		}
	}

	public void DeactivateOne(){
		count--;
		if(count <= 0){
			if(isActive){
				GetComponent<LayoutElement>().gameObject.SetActive(false);
				isActive = false;
			}
		}
	}


}
