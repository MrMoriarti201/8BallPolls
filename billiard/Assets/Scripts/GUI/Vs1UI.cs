using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Vs1UI : MonoBehaviour {
	public UILabel ruleLbl;
	public static Vs1UI instance;
	public bool change_animation;
	public float animation_step=0;
	Transform CenterImage;
	string texture_location;
	Sprite image;
	Image cueImage;
	Image coinImage;

	void Awake(){
		instance=this;
	}
	// Use this for initialization
	void Start () {
        if (GameManager.IsScene("TournamentLobby"))
        {
			CenterImage=GameObject.Find("Cup").transform.Find("Cup_Background");
			cueImage=CenterImage.Find ("TournamentCue").GetComponent<Image>();
		}
		else
			CenterImage=GameObject.Find("City").transform.Find("City_Background");
		coinImage=CenterImage.Find ("PrizeCoins").GetComponent<Image>();
	}

	public void OnBetButton(){
		animation_step=180;
		if(GameManager.IsScene("TournamentLobby"))
		{
			GlobalInfo.curBet=(Bet)GlobalInfo.tBet_List[BetButton.bet_index-1];
			cueImage.sprite=Resources.Load<Sprite>("Materials/Tournament cues/TCue"+BetButton.bet_index);
			texture_location="Materials/Tournament cups/TCupImage"+BetButton.bet_index;
		}		
		else{
			GlobalInfo.curBet=(Bet)GlobalInfo.pBet_List[BetButton.bet_index-1];
			texture_location="Materials/Cities/City"+BetButton.bet_index;
		}

		GlobalInfo.bet_index=BetButton.bet_index;
		change_animation=true;
	}

	void FixedUpdate () {
		
		if(change_animation)
		{
			float step_unit=Time.deltaTime*500.0f;
			
			CenterImage.Rotate(new Vector3(0,step_unit,0));
			
			if(animation_step>90 && animation_step-step_unit<=90)
			{
				CenterImage.localEulerAngles=new Vector3(0,-90,0);
				//GameObject.Find("City").transform.FindChild("City_Background").GetComponent<Image>().sprite=Resources.Load<Sprite>("Materials/Play1vs1Lobby/Cities/City"+bet_amount);

				coinImage.sprite=Resources.Load<Sprite>("Materials/Prizes/PrizeCoin"+GlobalInfo.curBet.prize_amount);			
				ruleLbl.text=GlobalInfo.curBet.rules;
				CenterImage.GetComponent<Image>().sprite=Resources.Load<Sprite>(texture_location);;

			}
			if(animation_step<0)
			{
				CenterImage.localEulerAngles=new Vector3(0,0,0);
				change_animation=false;
				SetAllButtonEnabled();
			}
			
			animation_step-=step_unit;			
		}
	}

	private void SetAllButtonEnabled(){
		for(int i=1;i<9;i++)
			GameObject.Find ("Bet"+i+"_Button").transform.Find("BetBG_Button").GetComponent<CanvasGroup>().blocksRaycasts=true;
	}
}
