using UnityEngine;
using System.Collections;

public class MainUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//if (Net.url!="http://212.42.207.188/")
        //AdManager.instance.LoadScene("Login");
	}
	
	public void OnLogInButtonPressed(){
        GameManager.instance.LoadScene("Login");
	}
	
	public void OnPlay_Guest(){
		GlobalInfo.myProfile=new Profile();
		GlobalInfo.myProfile.user_id=39;
		GlobalInfo.myProfile.LoadMainProfileInfoFromID();
        
		GlobalInfo.opponentProfile.user_id=Random.Range (1,41);
		GlobalInfo.opponentProfile.LoadMainProfileInfoFromID();
		GlobalInfo.game_type=GameType.PlayAsGuest;
		GlobalInfo.bet_index=1;
		GlobalInfo.curBet=(Bet)GlobalInfo.pBet_List[0];
        GameManager.instance.LoadScene("Game");
	}	
}