using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class AItem : MonoBehaviour {
	public UISprite map;
	public GameObject lockIcon;
	public UILabel level;
	public UISprite levelColor;
	public UISprite[] star;
	public AScrollView parent;

	int id;

	public void InitialData(int id)
	{	
		this.id=id;
	}

	public void SelectMap()
	{
//		GameData.selectedLevel=id;
//		PlayerPrefs.SetInt("selectedLevel",GameData.selectedLevel);
		parent.glowSprite.transform.position=new Vector3(transform.position.x-0.005f,transform.position.y+0.01f,0);
		parent.ChangeLevel();
	}
}
