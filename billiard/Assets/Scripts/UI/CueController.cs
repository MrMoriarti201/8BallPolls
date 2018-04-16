#if (UNITY_IPHONE || UNITY_ANDROID)
#define MOBILE
#endif

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CueController : Photon.MonoBehaviour 
{
	public LineRenderer line1;
	public LineRenderer line2;
	public LineRenderer line3;
	public LineRenderer circleLine;
	public LineRenderer kitchenLine;

	private Frame CurrentFrame;

	public delegate void ShotCueDelegate(Frame CurrentFrame,Vector3 orient, float force, float X, float Y);
	public event ShotCueDelegate OnShotCue;
	public delegate Frame RefreshAndReturnFrameDelegate();
	public event RefreshAndReturnFrameDelegate RefreshAndReturnFrame;
	public delegate bool GetPlayingStatusDelegate();
	public event GetPlayingStatusDelegate GetPlayingStatus;

	[SerializeField]
	private Texture2D[] ballTextures;
	[SerializeField]
	private Vector2[] deltaPositions;

	[System.NonSerialized]
	public int ballsCount = 16;

	[SerializeField]
	private Transform ballsParent;

	[SerializeField]
	private BallController ballControllerPrefab;
	[System.NonSerialized]
	public BallController ballController;


	[SerializeField]
	private Transform cuePivot; 

	[SerializeField]
	private Transform sideCueRight; 

	public Transform sideCueLeft;

	[SerializeField]
	private CueShotChecker cueShotChecker;
	
	[SerializeField]
	private Transform cueRotation;
	private Vector3 checkCuePosition = Vector3.zero;

	public BallPivot cueBallPivot;
	public float cueForceValue;
    public float cueForceProg = 2.0f;
	
	public float ballRadius = 0.35f;

	public float cueDisplacement = 0.0f;
	public float cueMaxDisplacement = 4.5f;
	public bool isShoting = false;
	private Vector3 cueStrPos = Vector3.zero;
	public Vector2 rotationDisplacement = Vector2.zero;
	[System.NonSerialized]
	public bool canControlled = false;  //when mobile , indicate cue rotate event
	public bool sideControlled = false;

	[System.NonSerialized]
	public bool canUpdate = true;
	public bool whiteballmoving = false;
	public GameObject hand;

	private BallController[] BallControllers;
    [HideInInspector]
	public BallController[] TopBarBalls;
	private int send_rate=0;

	private bool aiPlayerShooting = false;
	private int aiShootingAmount =0;

	private Vector3 shoot_dir=Vector3.zero;
	private Vector3 current_shootdir=Vector3.zero;
	private Vector3 direction_angle=Vector3.zero;
	private float ai_force=0.0f;
	private float current_aiforce=0.0f;
	private int angle_stepnum=0;
	private Vector3 ai_PivotPos=Vector3.zero;

	private bool is_firstshoot;
	public bool started=false;

	[HideInInspector]
	public float clock_left=Constant.play_time;
	[HideInInspector]
	public float clock_right=Constant.play_time;
		

	public static CueController instance;
	private int player1_win=0;
	private int player2_win=0;
	public static bool isFullBall;

#if UNITY_ANDROID || UNITY_IOS
    public GameObject CurNodeBack;
    public GameObject CurNodeBack1;
    public GameObject ControlCue;

    Vector3 ControlCuePosition0;
    Vector3 CurNodeBackPosition0;

    float MaxDelta = 254.1f; 
#endif

	void CreateAndSortBalls ()
	{
		BallControllers = new BallController[16];
		TopBarBalls = new BallController[16];
		ballRadius = 0.5f*ballControllerPrefab.transform.lossyScale.x;
		ballsCount = ballTextures.Length;

		for (int i = 0; i < ballsCount; i++) 
		{

			BallControllers[i] = BallController.Instantiate(ballControllerPrefab) as BallController;
			BallControllers[i].transform.parent = ballsParent;
			if(i == 0)
			{
				ballController = BallControllers[i];
			}
			BallControllers[i].ball.GetComponent<Renderer>().material.mainTexture = ballTextures[i];
			BallControllers[i].button.ballindex=i;

			TopBarBalls[i]=BallController.Instantiate(ballControllerPrefab) as BallController;			
			TopBarBalls[i].transform.parent=ballsParent;
			TopBarBalls[i].ball.GetComponent<Renderer>().material.mainTexture=ballTextures[i];
			TopBarBalls[i].button.ballindex=i;
            TopBarBalls[i].ball.localEulerAngles = new Vector3(0, 160, 180);
		}
		SetInitialPosition();
	}

    public void Init()
    {
        for (int i = 0; i < ballsCount; i++)
        {
            TopBarBalls[i].transform.position = new Vector3(1000, 0, 0);
        }

        lastSynchronizationTime = 0f;
        syncDelay = 0f;
        syncTime = 0f;
    }
	public void SetInitialPosition(){
		CurrentFrame=new Frame(true);
		for(int i=0;i<16;i++)
		{
			TopBarBalls[i].transform.position=new Vector3(1000,0,0);
			BallControllers[i].transform.position=CurrentFrame.Balls[i].r;			
		}
		cuePivot.LookAt(cuePivot.position+Vector3.right);
		
		GenerateCurrentFrame();
		
		if(Player.player_type==PLAYER_TYPE.MASTER)
		{
			SyncFrame ();
		}
	}

	public void ShowFrame(Frame frame=null) //Show balls , move balls.  rotation balls
	{
		if (frame==null)
			CurrentFrame=RefreshAndReturnFrame();
		else
			CurrentFrame=frame;
		for(int i=0;i<16;i++)
		{
			BallControllers[i].transform.position=CurrentFrame.Balls[i].r;
			BallControllers[i].ball.eulerAngles=CurrentFrame.Balls[i].rp;
			//BallControllers[i].transform.localScale=new Vector3(1.0f,1.0f,1.0f)*dis/(dis-CurrentFrame.Balls[i].r.y)*1.07f;
		}
	}

	void Awake ()
	{
		instance=this;
		Application.targetFrameRate=Constant.FRAMERATE;
		CreateAndSortBalls ();
		kitchenLine.SetPosition(0,new Vector3(-Constant.TABLE_W/2.0f,0,-Constant.TABLE_W/2.0f));
		kitchenLine.SetPosition(1,new Vector3(-Constant.TABLE_W/2.0f,0,Constant.TABLE_W/2.0f));
#if !MOBILE
		canControlled = true;
#endif

	}

	void Start()
	{
		is_firstshoot=true;

		clock_left=clock_right=Constant.play_time;

		GenerateCurrentFrame();

		if(Player.player_type==PLAYER_TYPE.MASTER)
		{
			SyncFrame ();
		}
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
#if UNITY_ANDROID || UNITY_IOS

		//iPhoneSettings.screenCanDarken = false;
        if (GameManager.isMobile){
            MaxDelta = (CurNodeBack.transform.position.y - CurNodeBack1.transform.position.y)*12/11;
            ControlCuePosition0 = ControlCue.transform.position;
            CurNodeBackPosition0 = CurNodeBack.transform.position;
        }
#endif
	}

	void UpdateTopUI(){		
		if(!GetPlayingStatus())
		{
			if(Player.IsPlayer())
			{
				clock_right=Constant.play_time;
				clock_left-=Time.deltaTime;
				if(clock_left<0.0f)
				{
                    Player.isTimeOut = true;
                    cueShotChecker.onPlayingEnded();
				/*	clock_left=Constant.play_time;*/
				}
			}
			else
			{
				clock_left=Constant.play_time;
				clock_right-=Time.deltaTime;
				if(clock_right<0.0f)
				{
					clock_right=Constant.play_time;
				}
			}
		}
	}

	void Update ()
	{
		if(!started || Player.win!=0 || GameManager.noPhysics)
		{
			return;
		}
		UpdateTopUI();
		Transform sideCue=Player.IsPlayer()?sideCueLeft:sideCueRight;
		Transform sideOppositeCue=Player.IsPlayer()?sideCueRight:sideCueLeft;
		sideCue.gameObject.SetActive(false);
		sideOppositeCue.gameObject.SetActive(true);
		//sideOppositeCue.GetComponent<CanvasGroup>().alpha=1.0f;
		if(send_rate++%15==0 && Player.player_type==PLAYER_TYPE.MASTER)
			SerializeCue();

		ballController.transform.position=CurrentFrame.Balls[0].r=cuePivot.position = BallControllers[0].transform.position;

		if(GetPlayingStatus())
		{
			ShowFrame ();
			//cueRotation.position=new Vector3(cueRotation.position.x,1000.0f,cueRotation.position.z);
			cueRotation.localScale=new Vector3(0.0f,0.0f,0.0f);
			return;
		}

		if(Player.player_type==PLAYER_TYPE.SLAVE)
		{
			SyncedMovement();
		}
		else if(Player.player_type==PLAYER_TYPE.AI)
		{
			AIUPdate();
		}//AI Action

		rotationDisplacement = (new Vector2(cueBallPivot.PositionX, cueBallPivot.PositionY));
		if(whiteballmoving)
		{
			AimLine.HideAimLines();
		}
		else
		{
			drawAimLine();
		}
		cueRotation.position=new Vector3(cueRotation.position.x,10.0f,cueRotation.position.z);

		Vector3 vec_MouseDown=Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //user Event
		if(Input.GetMouseButtonDown(0) && Player.IsPlayer())
		{
            cueDisplacement = 0.0f;
#if UNITY_ANDROID || UNITY_IOS                
                if (Mathf.Abs(vec_MouseDown.x) > 16.0f)
                    sideControlled = true;
                else
                    sideControlled = false;
#endif

            if (Mathf.Abs(vec_MouseDown.z) < 8.2f)
            {
                hand.SetActive(false);
                cueStrPos = GetWorldPointConvertedPosition();
#if UNITY_ANDROID || UNITY_IOS
                canControlled = true;                
#else
			sideControlled=true;
#endif
            }
		}

		if(Input.GetMouseButtonUp(0))
		{
			if(cueDisplacement != 0.0f && sideControlled && canUpdate && !whiteballmoving){
                Debug.Log(cueDisplacement);
				isShoting = true;                
			}
			//cueRotation.localPosition = checkCuePosition - cueDisplacement*Vector3.forward;
#if MOBILE
			canControlled = false;
#else
			else
				sideControlled=false;
#endif
			whiteballmoving = false;
#if UNITY_ANDROID || UNITY_IOS
            if (GameManager.isMobileScene)
            {
                CurNodeBack.transform.position = new Vector3(CurNodeBackPosition0.x, CurNodeBackPosition0.y, CurNodeBackPosition0.z);
                ControlCue.transform.position = new Vector3(ControlCuePosition0.x, ControlCuePosition0.y, ControlCuePosition0.z);
            }
#endif
		}
        if (!Player.IsPlayer())
            return;
		if(isShoting)
		{
            AnimateCue(); //shoot
        }
        else{
#if MOBILE
			if(canControlled)
			{
#endif
				if(sideControlled && !whiteballmoving)
				{
					if (canUpdate){
						float previousCueDisplacement=cueDisplacement;

						float Yclamp = Mathf.Clamp( cueBallPivot.PositionY, 0.0f, cueBallPivot.button.radius );
						float positionZ = -Mathf.Sqrt(Mathf.Clamp(  Mathf.Pow( ballRadius, 2.0f ) - ( Mathf.Pow( cueBallPivot.PositionX, 2.0f) +  Mathf.Pow(Yclamp, 2.0f )), 0.0f, Mathf.Pow( ballRadius, 2.0f ) ));

						checkCuePosition = ballRadius*(new Vector3(cueBallPivot.PositionX, Yclamp, positionZ));
						cueRotation.localPosition = checkCuePosition - cueDisplacement*Vector3.forward;
						sideCue.localPosition=new Vector3(-9,11,sideCue.localPosition.z)+cueDisplacement*Vector3.right;

						cueDisplacement=previousCueDisplacement;
					}
				}
				else if(!whiteballmoving && Maths.vec_abs (new Vector3(BallControllers[0].transform.position.x,0,BallControllers[0].transform.position.z)
				                                           -new Vector3(GetWorldPointConvertedPosition().x,0,GetWorldPointConvertedPosition().z))>BallControllers[0].radius
				        ){//&& Mathf.Abs(vec_MouseDown.z)<8.2f ){

					cuePivot.LookAt(GetWorldPointConvertedPosition ());
					cueDisplacement=0.0f;

				}else if(whiteballmoving){
					//AimLine.DrawAimLines(new Frame(CurrentFrame),cuePivot.forward, cueForceValue, rotationDisplacement.x, rotationDisplacement.y,false);
					cueDisplacement=0.0f;
				}
#if MOBILE
			}else{
				cueDisplacement=0.0f;			
			}
#endif
			if(Input.GetMouseButton(0) && sideControlled)
			{
#if MOBILE
    			cueDisplacement = Vector3.Dot(GetWorldPointConvertedPosition () - cueStrPos, Vector3.back.normalized);
				//cueDisplacement = Vector3.Dot(GetWorldPointConvertedPosition () - cueStrPos, Vector3.back.normalized);
#else
				cueDisplacement = Vector3.Dot(GetWorldPointConvertedPosition () - cueStrPos, -cuePivot.forward.normalized);
#endif
				float maxpower_rate;
				if(Player.shotpower_type==SHOTPOWER_TYPE.LITTLE)
					maxpower_rate=0.9f;
				else
					maxpower_rate=1.2f;
				cueDisplacement = Mathf.Clamp(cueDisplacement / cueForceProg, 0.0f, cueMaxDisplacement*maxpower_rate);
				cueForceValue = cueDisplacement/cueMaxDisplacement;
#if UNITY_ANDROID || UNITY_IOS
                if (GameManager.isMobileScene)
                {
                    float tempDelta = cueDisplacement * MaxDelta / cueMaxDisplacement;
                    float nodeDelta = MaxDelta / 12;
                    int i = (int)((tempDelta + nodeDelta / 2) / nodeDelta);
                    CurNodeBack.transform.position = new Vector3(CurNodeBackPosition0.x, CurNodeBackPosition0.y - nodeDelta * i, CurNodeBackPosition0.z);
//                    print(CurNodeBack.transform.position.y);
                    ControlCue.transform.position = new Vector3(ControlCuePosition0.x, ControlCuePosition0.y - tempDelta, ControlCuePosition0.z);
                }
#endif
			}
		}
	}

	void AIUPdate(){
		Transform sideCue=Player.IsPlayer()?sideCueLeft:sideCueRight;

		aiShootingAmount--;
		if(aiPlayerShooting==false)
		{
			shoot_dir=AIPlayer.ai_get_stroke_dir_8ball(CurrentFrame);
			/*if(AIPlayer.ai_level==AI_LEVEL.MEDIUM_LEVEL)
				{
					shoot_dir.x+=Random.Range (-0.05f,0.05f);
					shoot_dir.y+=Random.Range (-0.05f,0.05f);
				}else if(AIPlayer.ai_level==AI_LEVEL.LOW_LEVEL)
				{
					shoot_dir.x+=Random.Range (-0.1f,0.1f);
					shoot_dir.y+=Random.Range (-0.1f,0.1f);
						//alpha-=Random.Range (-0.05f,0.05f);
				}*/
			current_shootdir=Maths.vec_unit (new Vector3(Random.Range(-10.0f,-10.0f),0.0f,Random.Range(-10.0f,-10.0f)));
			aiPlayerShooting=true;
			aiShootingAmount=290;
			direction_angle=(shoot_dir-current_shootdir)/30.0f;
			//ai_force=Random.Range(5.0f,30.0f);
			ai_force=Random.Range(4.0f,8.0f);
			current_aiforce=0.0f;
			angle_stepnum=40;
			
			PositionXY=Vector3.zero;
			ai_PivotPos=Vector3.zero;
			if(AIPlayer.ai_level==AI_LEVEL.HIGH_LEVEL && Random.Range(0.0f,1.0f)>0.3f ||
			   AIPlayer.ai_level==AI_LEVEL.MEDIUM_LEVEL && Random.Range(0.0f,1.0f)>0.8f)
			{
				ai_PivotPos=Maths.vec_unit(new Vector3(Random.Range(-1.0f,1.0f)*Random.Range(-1.0f,1.0f)*Random.Range(-1.0f,1.0f),
				                                       0.0f,Random.Range(-1.0f,1.0f)))*Random.Range (0.5f,1.0f)/20.0f;
			}
		}
		if(aiShootingAmount>250){}
		else if(aiShootingAmount>250-angle_stepnum)
			current_shootdir+=direction_angle;
		else if(aiShootingAmount>180)
			current_shootdir-=direction_angle/3.0f;
		else if(aiShootingAmount>140){}
		else if(aiShootingAmount>120){
			//PositionXY+=ai_PivotPos;
			cueBallPivot.PositionX=PositionXY.x;
			cueBallPivot.PositionY=PositionXY.z;
			cueBallPivot.ballSphere.localPosition=new Vector3(PositionXY.x,PositionXY.z,0.0f)*cueBallPivot.button.radius;
		}
		else if(aiShootingAmount>100){}
		else if(aiShootingAmount>60)
			cueDisplacement+=(ai_force-current_aiforce)/40.0f;
		
		float Yclamp = Mathf.Clamp( cueBallPivot.PositionY, 0.0f, cueBallPivot.button.radius );
		float positionZ = -Mathf.Sqrt(Mathf.Clamp( Mathf.Pow( ballRadius, 2.0f ) - ( Mathf.Pow( cueBallPivot.PositionX, 2.0f) +  Mathf.Pow(Yclamp, 2.0f )), 0.0f, Mathf.Pow( ballRadius, 2.0f ) ));
		
		cueForceValue = Mathf.Clamp01(cueDisplacement/cueMaxDisplacement);
		checkCuePosition = ballRadius*(new Vector3(cueBallPivot. PositionX, Yclamp, positionZ));
		cueRotation.localPosition = checkCuePosition - cueDisplacement*Vector3.forward;
		sideCue.localPosition=new Vector3(-9,11,13.24f)+cueDisplacement*Vector3.right;
		
		if(aiShootingAmount<0)
		{
			GenerateCurrentFrame();
			OnShotCue(CurrentFrame,cuePivot.forward, cueForceValue, rotationDisplacement.x, rotationDisplacement.y); 
			cueDisplacement = 0.0f;  
			cueForceValue=0.0f;
			cueRotation.localPosition = checkCuePosition - cueDisplacement*Vector3.forward;
			sideCue.localPosition=new Vector3(-9,11,13.24f)+cueDisplacement*Vector3.right;
			PositionXY=Vector3.zero;
			cueBallPivot.PositionX=PositionXY.x;
			cueBallPivot.PositionY=PositionXY.z;
			cueBallPivot.ballSphere.localPosition=new Vector3(PositionXY.x,0.0f,PositionXY.y)*cueBallPivot.button.radius;
			aiPlayerShooting=false;
			return;
		}
		
		cuePivot.LookAt(cuePivot.position+current_shootdir);
	}
	public void drawAimLine(){
		cueRotation.localScale=new Vector3(1.0f,1.0f,1.0f);
		Cursor.SetCursor(null,Vector2.zero,CursorMode.ForceSoftware);
		GenerateCurrentFrame();
//		print (cueDisplacement);
		if(cueDisplacement < 0.15f*cueMaxDisplacement && Player.masse)
			AimLine.DrawAimLines(new Frame(CurrentFrame),cuePivot.forward, 1.0f, 0.0f,0.0f,false);
		else
		{
			AimLine.DrawAimLines(new Frame(CurrentFrame),cuePivot.forward, cueForceValue, rotationDisplacement.x, rotationDisplacement.y,Player.masse);
		}
	}

	private Vector3 GetWorldPointConvertedPosition ()
	{
		Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		return new Vector3(worldPoint.x, cuePivot.position.y, worldPoint.z);
	}

	private void GenerateCurrentFrame()
	{
		if(CurrentFrame==null)
		{	
			CurrentFrame=new Frame(true);
			for(int i=0;i<16;i++)
			{
				CurrentFrame.Balls[i].rp=BallControllers[i].ball.eulerAngles;
			}
		}
		for(int i=0;i<16;i++)
		{
			CurrentFrame.Balls[i].r=BallControllers[i].transform.position;
			CurrentFrame.Balls[i].rp=BallControllers[i].ball.eulerAngles;
			if(Mathf.Abs(BallControllers[i].transform.position.x)>20.0f)
				CurrentFrame.Balls[i].in_game=false;
		}
	}

	public void EnableBoosts(){
        if (GameManager.isMobileScene)
            return;
		GameObject.Find("Boost_WallAimLine").GetComponent<CanvasGroup>().interactable=true;
		GameObject.Find("Boost_LongAimLine").GetComponent<CanvasGroup>().interactable=true;
		GameObject.Find("Boost_PowerShot").GetComponent<CanvasGroup>().interactable=true;
		GameObject.Find("Boost_MasseShot").GetComponent<CanvasGroup>().interactable=true;
	}

	public void DisableBoosts(){
        if (GameManager.isMobileScene)
            return;
		GameObject.Find("Boost_WallAimLine").GetComponent<CanvasGroup>().interactable=false;
		GameObject.Find("Boost_LongAimLine").GetComponent<CanvasGroup>().interactable=false;
		GameObject.Find("Boost_PowerShot").GetComponent<CanvasGroup>().interactable=false;
		GameObject.Find("Boost_MasseShot").GetComponent<CanvasGroup>().interactable=false;
	}

	void RefreshAllBoosts(){
//		print ("refresh");
		Player.is_longline=false;
		Player.masse=false;
		Player.show_wallline=false;
		Player.shotpower_type=SHOTPOWER_TYPE.LITTLE;
        if (!GameManager.isMobileScene) { 
		    GameObject.Find("Boost_WallAimLine").transform.Find("BoostButton").Find("BoostGlow").gameObject.SetActive(false);
		    GameObject.Find("Boost_LongAimLine").transform.Find("BoostButton").Find("BoostGlow").gameObject.SetActive(false);
		    GameObject.Find("Boost_PowerShot").transform.Find("BoostButton").Find("BoostGlow").gameObject.SetActive(false);
		    GameObject.Find("Boost_MasseShot").transform.Find("BoostButton").Find("BoostGlow").gameObject.SetActive(false);

		    GameObject.Find("Boost_WallAimLine").GetComponent<CanvasGroup>().blocksRaycasts=true;
		    GameObject.Find("Boost_LongAimLine").GetComponent<CanvasGroup>().blocksRaycasts=true;
		    GameObject.Find("Boost_PowerShot").GetComponent<CanvasGroup>().blocksRaycasts=true;
		    GameObject.Find("Boost_MasseShot").GetComponent<CanvasGroup>().blocksRaycasts=true;
        }

		GameObject.Find ("CueTexture").transform.localEulerAngles=new Vector3(90,270,0);
		GameObject.Find ("CueTexture").transform.localPosition=new Vector3(0.0f,7.2f,-10.30129f);
	}

	void AnimateCue () //shoot
	{
		Transform sideCue=(Player.IsPlayer()?sideCueLeft:sideCueRight);
		cueDisplacement = Mathf.Lerp(cueDisplacement, 0.0f, Mathf.Clamp( cueForceValue, 0.15f, 1.0f )*50.0f*Time.deltaTime);
		if(!IsInvoking("RessetBallPivot"))
		{
			if(cueDisplacement < 0.05f*cueMaxDisplacement)
			{
				if(OnShotCue != null)
				{
					GenerateCurrentFrame();
					clock_left=clock_right=Constant.play_time;
					OnShotCue(CurrentFrame,cuePivot.forward, cueForceValue, rotationDisplacement.x, rotationDisplacement.y);
					RefreshAllBoosts();		
					is_firstshoot=false;
	#if !MOBILE
					sideControlled=false;
	#endif				
				}
				cueDisplacement = 0.0f;

				Invoke("RessetBallPivot", 0.5f);
			}
		}
		cueRotation.localPosition = checkCuePosition - cueDisplacement*Vector3.forward;
		sideCue.localPosition=new Vector3(-9,11,sideCue.localPosition.z)+cueDisplacement*Vector3.right;
		PositionXY=Vector3.zero;
	}

	[PunRPC] 
	public void RessetBallPivot ()
	{
//		Debug.Log("resetBallPivot"+cueDisplacement.ToString());
		if(Player.IsNet() && isShoting)
			GetComponent<PhotonView>().RPC ("RessetBallPivot",PhotonTargets.OthersBuffered);
		isShoting = false;
		cueBallPivot.Reset();
		checkCuePosition = ballRadius*(new Vector3(0.0f, 0.0f, -1.0f));
		cueRotation.localPosition = checkCuePosition - cueDisplacement*Vector3.forward;
	}

	Vector3 cue_localpos=Vector3.zero;
	Vector3 cue_pos=Vector3.zero;
	Vector3 cue_rot=Vector3.zero;
	Vector3 cue_ballpivot=Vector3.zero;
	Vector3 PositionXY=Vector3.zero;
	Vector3 startcue_localpos=Vector3.zero;
	Vector3 startcue_pos=Vector3.zero;
	Vector3 startcue_rot=Vector3.zero;
	Vector3 startcue_ballpivot=Vector3.zero;

	public bool white_moving = true;

	private float lastSynchronizationTime=0f;
	private float syncDelay=0f;
	private float syncTime=0f;

	[PunRPC] 
	private void SerializeCue(float cueDisplacement0=0f, Vector3 param_cue_localpos=default(Vector3),
        Vector3 param_cue_pos=default(Vector3),Vector3 param_cue_rot=default(Vector3),
        Vector3 param_cue_ballpivot=default(Vector3),Vector3 PositionXY0=default(Vector3),bool param_whiteballmoving=true)
	{
		if(Player.player_type==PLAYER_TYPE.MASTER)
		{
			if(Maths.vec_abs (cue_localpos-cueRotation.localPosition)<Constant.CUE_POS_MIN
			   && Maths.vec_abs (cue_pos-ballController.transform.position)<Constant.CUE_POS_MIN 
			   && Maths.vec_abs (cue_rot-cuePivot.localEulerAngles)<Constant.CUE_ROT_MIN
			   && Maths.vec_abs (cue_ballpivot-cueBallPivot.ballSphere.localPosition)<Constant.CUE_POS_MIN)
				return;
			cue_localpos=cueRotation.localPosition;
			cue_pos=ballController.transform.position;
			cue_rot=cuePivot.localEulerAngles;
			cue_ballpivot=cueBallPivot.ballSphere.localPosition;
			PositionXY0=new Vector3(cueBallPivot.PositionX,cueBallPivot.PositionY,cueForceValue);

			GetComponent<PhotonView>().RPC ("SerializeCue",PhotonTargets.OthersBuffered,cueDisplacement,cue_localpos,
                cue_pos,cue_rot,cue_ballpivot,PositionXY0,whiteballmoving);
		}
		else if(Player.player_type==PLAYER_TYPE.SLAVE)
		{
			cueDisplacement=cueDisplacement0;
			cue_localpos=param_cue_localpos;	cue_pos=param_cue_pos;			cue_rot=param_cue_rot;
			cue_ballpivot=param_cue_ballpivot;	

			cueBallPivot.PositionX=PositionXY0.x;
			cueBallPivot.PositionY=PositionXY0.y;
			cueForceValue=PositionXY0.z;

			whiteballmoving=param_whiteballmoving;

			/*if(lastSynchronizationTime<Time.time-1.5/PhotonNetwork.sendRate)*/
//             if (lastSynchronizationTime < Time.time - 2f)
// 				lastSynchronizationTime=Time.time;			
			syncTime=0f;
			syncDelay=1/(Time.time-lastSynchronizationTime);  //how long
            lastSynchronizationTime = Time.time;

			startcue_localpos=cueRotation.localPosition;
			startcue_pos=ballController.transform.position;
			startcue_rot=cuePivot.localEulerAngles;
			startcue_ballpivot=cueBallPivot.ballSphere.localPosition;
			
			if(Mathf.Abs (startcue_rot.x-cue_rot.x)>250 || Mathf.Abs (startcue_rot.y-cue_rot.y)>250 || Mathf.Abs (startcue_rot.z-cue_rot.z)>250)
				startcue_rot=cue_rot;			
		}
	}

	public void SetOtherElementsVisible(bool visible){
		TopBarUI.instance.nickName2.gameObject.SetActive(visible);
		TopBarUI.instance.nickName1.gameObject.SetActive(visible);

		GameObject.Find ("CueController").transform.Find("CuePivot").gameObject.SetActive(visible);
		GameObject.Find ("CueController").transform.Find("SideCueRight").gameObject.SetActive(visible);
		GameObject.Find ("CueController").transform.Find("SideCueLeft").gameObject.SetActive(visible);

        if (!GameManager.isMobileScene)
        {
            GameObject.Find("Boost_WallAimLine").transform.Find("BoostButton").gameObject.SetActive(visible);
            GameObject.Find("Boost_LongAimLine").transform.Find("BoostButton").gameObject.SetActive(visible);
            GameObject.Find("Boost_PowerShot").transform.Find("BoostButton").gameObject.SetActive(visible);
            GameObject.Find("Boost_MasseShot").transform.Find("BoostButton").gameObject.SetActive(visible);
        }
		for(int i=0;i<16;i++)
			BallControllers[i].gameObject.SetActive(visible);

		GameObject.Find ("TopBarCanvas").transform.Find("BundleOfCoins").gameObject.SetActive(visible);
        if (GlobalInfo.IsPlayGuest() || GlobalInfo.IsFriendMatch())
			GamePlayUI.instance.topCoins.SetActive(false);

		if(GlobalInfo.IsTournament())
		{
			GameObject.Find ("TopBarCanvas").transform.Find("BundleOfCoins").Find("img_ScoreBoard1").gameObject.SetActive(false);
			GameObject.Find ("TopBarCanvas").transform.Find("BundleOfCoins").Find("img_ScoreBoard2").gameObject.SetActive(false);
			GameObject.Find ("TopBarCanvas").transform.Find("BundleOfCoins").Find("Score:").gameObject.SetActive(false);
		}
	}

	private void SyncedMovement()  //When Slave
	{
		syncTime+=Time.deltaTime*syncDelay;
		cueRotation.localPosition=Vector3.Lerp (startcue_localpos,cue_localpos,syncTime);
		if(whiteballmoving)
			BallControllers[0].transform.position=Vector3.Lerp (startcue_pos,cue_pos,syncTime);

		cuePivot.localEulerAngles=Vector3.Lerp (startcue_rot,cue_rot,syncTime);
		cueBallPivot.ballSphere.localPosition =Vector3.Lerp (startcue_ballpivot,cue_ballpivot,syncTime);
	}

	public bool penetrated(Vector3 whiteball_pos)
	{
		for(int i=1;i<16;i++)
			if(Maths.vec_abs (whiteball_pos-BallControllers[i].transform.position)<Constant.BALL_D)
				return true;
		return false;
	}

	public bool outOfKitchenLine(Vector3 whiteball_pos)
	{
		if(!is_firstshoot)
			return false;
		if(whiteball_pos.x>-Constant.TABLE_W/2.0f)
			return true;
		return false; 
	}

	public void ShowHand(){
		hand.SetActive(true);
		hand.transform.position=new Vector3(BallControllers[0].transform.position.x+0.8f,3f,BallControllers[0].transform.position.z-0.2f);
	}

	[PunRPC]
	public void SyncFrame(string packet=null)
	{
		if(packet==null)
			GetComponent<PhotonView>().RPC ("SyncFrame",PhotonTargets.OthersBuffered,new Packet(CurrentFrame).m_packet);
		else
		{
			CurrentFrame=new Frame(packet);
			ShowFrame(CurrentFrame);
		}
	}

    public void SetTopBalls()
    {
        float dz = (GameManager.isMobileScene) ? 12f : 12.8f;
        float distance = (GameManager.isMobileScene) ? 1.35f : 1.47f;
        float startPos1 = (GameManager.isMobileScene) ? -16.7f : -17.73f;
        float startPos2 = (GameManager.isMobileScene) ? 5.3f : 6.18f;
        for (int i = 1; i < 8; i++)
        {
            if (CurrentFrame.Balls[i].in_game)
            {
                if (isFullBall)
                    TopBarBalls[i].transform.position = new Vector3(startPos1 + distance * i, 0, dz);
                else
                    TopBarBalls[i].transform.position = new Vector3(startPos2 + distance * i, 0, dz);
                //                print(TopBarBalls[i].transform.position);
            }
        }

        for (int i = 9; i < 16; i++)
        {
            if (CurrentFrame.Balls[i].in_game)
            {
                if (isFullBall)
                    TopBarBalls[i].transform.position = new Vector3(startPos2 + distance * (i - 8), 0, dz);
                else
                    TopBarBalls[i].transform.position = new Vector3(startPos1 + distance * (i - 8), 0, dz);
            }
        }
    }

    public void GameEnd()
    {
        cueShotChecker.GameEnd();
		white_moving = true;
		is_firstshoot = true;
    }
}
