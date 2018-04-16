using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LobyUI : MonoBehaviour {	
	public static LobyUI instance;

	void Awake(){
		instance=this;
        GameManager.status = 1;
	}

	void Start () {		
        PhotonManager.instance.LeftRoom();
	}
	
	public void OnPlay1vs1ButtonPressed(){
        GlobalInfo.game_type = GameType.Player1Vs1;
        GameManager.instance.LoadScene("Play1vs1");
	}
	
	public void OnTournamentButtonPressed(){
        Tournament.LoadTournament();        
        GameManager.instance.LoadScene("TournamentLobby");
	}
	
	public void OnWithFriendsButtonPressed(){		
		if (GlobalInfo.cash < GlobalInfo.cashNeededWithFriendPlay) {
			GameManager.instance.PopupMessage("You have not enough cash. Please charge cash.");
			return;
		}
        GameManager.instance.LoadScene("WithFriend");
	}
	
	public void OnShopButtonPressed(){
		GlobalInfo.ShopTabName="Coins";
		GoShop.lastScene=Application.loadedLevelName;
        GameManager.instance.LoadScene("Shop");
	}

    public void AddCoin()
    {
        Net.instance.SendMsg(new UserManager.AddCoin(50));
    }
}
