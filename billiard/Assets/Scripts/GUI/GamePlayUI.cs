using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GamePlayUI : Photon.MonoBehaviour  {
    public Transform pos0;
	public GameObject chartPart;
	public GameObject chartWindow;
	public GameObject cityObj;
	public GameObject vsLbl;
	public GameObject topCoins;
	public GameObject smallLogo;	
	public GameObject quitGuestBtn;
	public UILabel popupMessage;
	public GameObject[] blinkEffect;
	//Debug Part
	public GameObject debugLbls;
	public UILabel playerStatus; 
	public UILabel status;
	public GameObject fireEffect1;
	public GameObject tournamentEffect;
	public GameObject winnerEffect1;
	public GameObject winnerEffect2;
	public UISprite[] clock;
	public LeveUpUI rankUp;
	public TrophyUI trophy;
	Game.GameEnd curPhp;

	static public int pocketId=-1;
	static public GamePlayUI instance;

	void Awake(){
		instance=this;	
	}

	void Start () {

		for (int i=0;i<6;i++)
			blinkEffect[i].SetActive(false);
		
		if (GlobalInfo.IsPlayGuest() || GameManager.isMobileScene){
			chartPart.SetActive(false);
			chartWindow.SetActive(false);
			cityObj.SetActive(false);
		} 
		else{
			chartPart.SetActive(true);
			chartWindow.SetActive(true);
			cityObj.SetActive(true);
		}
        
		vsLbl.SetActive(GlobalInfo.IsPlayGuest()||GlobalInfo.IsFriendMatch());
        smallLogo.SetActive(GlobalInfo.IsPlayGuest() || GlobalInfo.IsFriendMatch());
	}

	public void Init(){
		fireEffect1.SetActive(false);
		tournamentEffect.SetActive(false);
		winnerEffect1.SetActive(false);
		winnerEffect2.SetActive(false);
	}

    public void SetFullBall(){
        SetFullBall0(CueController.isFullBall);
        if (Player.IsNet())
        {
            GetComponent<PhotonView>().RPC("SetFullBall0", PhotonTargets.OthersBuffered, !CueController.isFullBall);
        }
	}

    [PunRPC]
    void SetFullBall0(bool isFullBall0)
    {
        CueController.isFullBall = isFullBall0;
        if (isFullBall0)
            ShowPopupMessage0("You are on solids!");
        else
            ShowPopupMessage0("You are on stripes!");
        CueController.instance.SetTopBalls();
    }

    public void SetBallOut(int ballId)
    {
        SetBallOut0(ballId);
        if (Player.IsNet())
        {
            GetComponent<PhotonView>().RPC("SetBallOut0", PhotonTargets.OthersBuffered, ballId);
        }
    }

    [PunRPC]
    void SetBallOut0(int ballId)
    {
        CueController.instance.TopBarBalls[ballId].transform.position = new Vector3(1000, 0, 0);        
    }
	
	public void ShowPopupMessage(string str){
        if (Player.isTimeOut)
        {
            Player.isTimeOut = false;
            str = "Time run out!";
        }
		ShowPopupMessage0(str);

		if (Player.IsNet())
        {
            str = str.Replace("You", GlobalInfo.myProfile.Nickname);
            GetComponent<PhotonView>().RPC("ShowPopupMessage0", PhotonTargets.OthersBuffered, str);
        }
	}

	[PunRPC]
	void ShowPopupMessage0(string str){
		popupMessage.gameObject.SetActive(true);
		popupMessage.text=str;
		popupMessage.GetComponent<TweenAlpha>().ResetToBeginning();
		popupMessage.GetComponent<TweenAlpha>().PlayForward();
		popupMessage.GetComponent<TweenPosition>().ResetToBeginning();
		popupMessage.GetComponent<TweenPosition>().PlayForward();
		AudioController.Play("Alert");

		if (str == "White Ball Pocketed!" && !Player.IsPlayer())
			CueController.instance.white_moving = true;
	}

	public void OnDebug(){
//		debugLbls.SetActive(!debugLbls.activeSelf);
	}

	public void OnClickBlink(int id){
		print ("Onclick "+id);
	}

	public void ShowBlink(bool flag,int id=-1){
		if (Player.player_type==PLAYER_TYPE.MASTER || Player.player_type==PLAYER_TYPE.SLAVE)
			GetComponent<PhotonView>().RPC ("ShowBlink0",PhotonTargets.OthersBuffered,flag,id);
		ShowBlink0(flag,id);
	}

	[PunRPC]
	void ShowBlink0(bool flag,int id=-1){
		for (int i=0;i<6;i++){
			if (i==id) 
				blinkEffect[i].SetActive(true);
			else
				blinkEffect[i].SetActive(flag);
		}
		CueController.instance.canUpdate=!flag;
	}

	public void SetAIPocket(int minhole){
		if (CueController.instance.canUpdate) //Call Pocket Mode?
			return;
		StartCoroutine(ShowAIBlink(minhole));
		pocketId=minhole;
	}

	IEnumerator ShowAIBlink(int minhole){
		yield return new WaitForSeconds(1f);
		ShowBlink(false,minhole);
	}

	void Update () {
        if (GameManager.isMobileScene)
        {
            clock[0].fillAmount=CueController.instance.clock_left/Constant.play_time;		
		    clock[1].fillAmount=CueController.instance.clock_right/Constant.play_time;
        }
        else
        {
            clock[0].fillAmount = 1 - CueController.instance.clock_left / Constant.play_time;
            clock[1].fillAmount = 1 - CueController.instance.clock_right / Constant.play_time;
            clock[0].transform.localRotation = Quaternion.Euler(0f, 0f, -clock[0].fillAmount * 360);
            clock[1].transform.localRotation = Quaternion.Euler(0f, 0f, -clock[1].fillAmount * 360);
        }
		
        //		for (int i=0;i<30;i++){
		//			Border bo=Borders.GetBorder (Borders.BORDERS_8GAME,i);
		//			if (bo.pnr==3){
		//				Debug.DrawLine(bo.r1, bo.r2, Color.red);
		//
		//				Debug.DrawLine(bo.r2, bo.r3, Color.green);
		//				Debug.DrawLine(bo.r3, bo.r1, Color.yellow);
		////				print (i);
		//			}
		//		}
	}


	public void ShowFireEffect(){ 
		if (GlobalInfo.IsTournament()){
			tournamentEffect.transform.localPosition=new Vector3(-Player.win*45f,43f,365f);
			tournamentEffect.SetActive(true);
//			StartCoroutine(HideEffect());
		}
		else {
			fireEffect1.transform.localPosition=new Vector3(-Player.win*45f,43f,365f);
			fireEffect1.SetActive(true);
		}
		winnerEffect1.transform.localPosition=new Vector3(-Player.win*5.21f,9.94f,5.69f);
		winnerEffect1.SetActive(true);
		winnerEffect2.transform.localPosition=new Vector3(-Player.win*0.33f,0.13f,-1.184f);
		winnerEffect2.SetActive(true);
	}

	IEnumerator HideEffect(){
		yield return new WaitForSeconds(3.5f);
		tournamentEffect.SetActive(false);
	}

	public void GameEnd(){  //First Game End Func Before Web.
        ShowBlink(false);
        popupMessage.gameObject.SetActive(false);
        CueController.instance.GameEnd();
        if (GlobalInfo.IsPlayGuest())
            OnGameEnd();
        else
        {
            GameEnd1(Player.win);
            if (Player.player_type == PLAYER_TYPE.MASTER || Player.player_type == PLAYER_TYPE.SLAVE)
                GetComponent<PhotonView>().RPC("GameEnd1", PhotonTargets.OthersBuffered, -Player.win);
        }		
	}

	[PunRPC]
	void GameEnd1(int win0){  // second Game End, before web.
		Player.win=win0;
		int isWin=((Player.win==1)?1:0);
		GlobalInfo.playCnt++;
		GlobalInfo.winCnt+=isWin;
        AimLine.HideAimLines();
		//net
		Net.instance.SendMsg(new Game.GameEnd(this));
	}
		
	public void RecvGameEnd( Game.GameEnd php ) {  //3th Game End // after Web
		curPhp=php;
		if (php.coin_bonus>0){  //show trophy
			trophy.Show(php.mis_id,php.coin_bonus);
			GlobalInfo.trophyList[php.mis_id-1]=true;
		}
		else {
			ShowLevelUp();
		}	
	}

    public void OnGameEnd()
    {  //(4th Game Endafter to received server Game End request, 
        //from tween  or after click levelup, or click Rank up
        SearchOponentAnimation.instance.StartReverseAnimation();
        Tournament.SetWinner();
        ShowLevelUpEffect(false);
    }
    public void ShowRankup()
    {
        ShowLevelUpEffect(false);
        if (curPhp.rankBonus > 0)
        {
            rankUp.ShowLevelUP(false, curPhp);
            ShowLevelUpEffect(true);
        }
        else
            OnGameEnd();
    }

    void ShowLevelUpEffect(bool flag)
    {
        winnerEffect2.transform.localPosition = Vector3.zero;
        winnerEffect2.SetActive(flag);
        EffectManager.instance.coinEffect.SetActive(flag);
    }

    public void ShowLevelUp()
    {
        trophy.gameObject.SetActive(false);
        if (curPhp.levelBonus > 0)
        {
            rankUp.ShowLevelUP(true, curPhp);
            ShowLevelUpEffect(true);
        }
        else
            ShowRankup();
    }

	public void YouWin(){
		Player.win=1;
		GameEnd();
	}

	public void YouLose(){
		Player.win=-1;
		GameEnd();
	}

	public void OnMasseBtn(){
        CueController.instance.clock_left += 10;
        Player.masse = true;
		if (Player.player_type==PLAYER_TYPE.MASTER || Player.player_type==PLAYER_TYPE.SLAVE)
			GetComponent<PhotonView>().RPC ("OnMasseBtn0",PhotonTargets.OthersBuffered);
	}

	[PunRPC]
	void OnMasseBtn0(){
		Player.masse=true;
        CueController.instance.clock_right+= 10;
	}

    public void SetOpponentProfile()
    {
        //if (Player.player_type == PLAYER_TYPE.MASTER || Player.player_type == PLAYER_TYPE.SLAVE)
        GetComponent<PhotonView>().RPC("OnOpponentProfile", PhotonTargets.OthersBuffered,
            GlobalInfo.myProfile.Nickname, GlobalInfo.myProfile.user_id, GlobalInfo.myProfile.level, GlobalInfo.myProfile.point, GlobalInfo.myProfile.maxPoint, GlobalInfo.myProfile.avatar,GlobalInfo.myProfile.rank);
    }

    [PunRPC]
    void OnOpponentProfile(string name,int id,int level0,int point0,int maxPoint0,string avatar0,int rank0)
    {
        GlobalInfo.opponentProfile.Nickname = name;
        GlobalInfo.opponentProfile.level = level0;
        GlobalInfo.opponentProfile.user_id = id;
        GlobalInfo.opponentProfile.point = point0;
        GlobalInfo.opponentProfile.maxPoint = maxPoint0;
        GlobalInfo.opponentProfile.avatar = avatar0;
        GlobalInfo.opponentProfile.rank = rank0;
        Debug.Log(name+" received Opponent");
        if (PhotonNetwork.isMasterClient)
            Player.player_type = PLAYER_TYPE.MASTER;
        else
            Player.player_type = PLAYER_TYPE.SLAVE;
        SearchOponentAnimation.instance.SetFound();
    }
}
