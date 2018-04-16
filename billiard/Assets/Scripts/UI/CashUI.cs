using UnityEngine;
using System.Collections;

public class CashUI : MonoBehaviour {
	public UILabel cash;
	public UILabel coin;
	public static CashUI instance;
	void Awake(){
		instance=this;
	}
	// Use this for initialization
	void Start () {
		cash.text=GlobalInfo.cash.ToString();
		coin.text=GlobalInfo.coin.ToString();
	}

	public void UpdateValue(){
		cash.text=GlobalInfo.cash.ToString();
		coin.text=GlobalInfo.coin.ToString();
	}

}
