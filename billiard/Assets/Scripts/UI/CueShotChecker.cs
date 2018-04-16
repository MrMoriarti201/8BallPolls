using UnityEngine;
using System.Collections;
using System.Threading;

public class CueShotChecker : Photon.MonoBehaviour 
{

	private const float CUEBALL_MAXSPEED=100.0f;
	[SerializeField]
	private CueController cueController;
    [SerializeField]
	Buffer myBuffer;
	private Thread generateFrames_Server,generateFrames_Client;

	private bool is_playing = false;
	private Frame prev_Frame;

	private bool is_ended=false;

	public Audio audio_player;

	void Start () 
	{
	//	Borders.InitAllBorders();
		cueController.OnShotCue += OnShotCue;
		cueController.RefreshAndReturnFrame+=RefreshAndReturnFrame;
		cueController.GetPlayingStatus+=GetPlayingStatus;
		myBuffer=new Buffer();
		//if(Player.player_type==PLAYER_TYPE.MASTER || Player.player_type==PLAYER_TYPE.STANDALONE)
		//	OnShotCue(new Frame(true),new Vector3(1.0f,0.0f,0.0f), 0.1f, 0.0f, 0.0f);
	}

	void GenerateFrames_Client()
	{
		myBuffer.GenerateFrames(false);
		//StartCoroutine(myBuffer.GenerateFrames(false));
	}

	void GenerateFrames_Server()
	{
		myBuffer.GenerateFrames();
		//StartCoroutine(myBuffer.GenerateFrames());
	}

	void queue_shot(Ball m_Cue,Vector3 orient, float force,float X,float Y) {
		Vector3 dir=Vector3.zero;
		Vector3 nx,nz;
		Vector3 hitpoint;

		force*=force;

		if(Player.masse){
			dir=Maths.vec_unit(new Vector3(orient.x,2.0f,orient.z));
		}
		else{
			dir = orient;
		}
		nx = Maths.vec_unit(Maths.vec_cross(Vector3.up,dir));  /* parallel to table */
		nz = Maths.vec_unit(Maths.vec_cross(nx,dir));        /* orthogonal to dir and nx */
		hitpoint = X*nx+nz*Y*0.6f;
		m_Cue.v =  dir*CUEBALL_MAXSPEED*force;
		if(Maths.vec_abssq(hitpoint)==0.0f){
			m_Cue.w = Vector3.zero;
		} else {
			/* w = roll speed if hit 1/3of radius above center */
			//            balls.ball[cue_ball].w = vec_scale(vec_cross(dir,hitpoint),4.0*3.0*CUEBALL_MAXSPEED*queue_strength/balls.ball[cue_ball].d/balls.ball[cue_ball].d);
			/* hmm, this one works better */
			m_Cue.w = Maths.vec_cross(dir,hitpoint)*2.0f*3.0f*CUEBALL_MAXSPEED*force*force/m_Cue.d/m_Cue.d;
		}
	}

	void OnShotCue (Frame CurrentFrame,Vector3 orient, float force, float X, float Y)
	{
		GamePlayUI.instance.ShowBlink(false);
		if(CurrentFrame==null)
			CurrentFrame=myBuffer.GetCurrentFrame();
		Frame cur_Frame=GetFrameByShot(CurrentFrame,orient,force,X,Y);		
		cueController.DisableBoosts();
		if(Player.player_type==PLAYER_TYPE.MASTER)
		{
			Packet packet=new Packet(cur_Frame);
			GetComponent<PhotonView>().RPC ("OnPlayStarted",PhotonTargets.OthersBuffered,packet.m_packet);
		}
		cur_Frame.is_playing=true;
		myBuffer.StartPlaying(cur_Frame);
		prev_Frame=new Frame(cur_Frame);
		generateFrames_Server=new Thread(GenerateFrames_Server);
		generateFrames_Server.Start();
		is_playing=true;
		audio_player.PlayAudio("cue",1.0f);

	}
	
	private Frame GetFrameByShot(Frame CurrentFrame,Vector3 orient, float force, float X, float Y){
		//CurrentFrame.Cues[0].v=new Vector3(orient.x*force*50,orient.y*force*50,orient.z*force*50);
		queue_shot(CurrentFrame.Balls[0],orient,force,X,Y);
		return CurrentFrame;
	}

	public Frame RefreshAndReturnFrame()
	{
		Frame buf_Frame;
		myBuffer.RefreshFrame();

		AimLine.ClearAimLines();

// 		if(Player.player_type==PLAYER_TYPE.MASTER)
// 			for(;(buf_Frame=myBuffer.PopBufferFrame())!=null;)
// 			{
// 				GetComponent<PhotonView>().RPC ("PushFrame",PhotonTargets.OthersBuffered,new Packet(buf_Frame,prev_Frame).m_packet);
// 				prev_Frame=new Frame(buf_Frame);
// 			}
		buf_Frame=myBuffer.GetCurrentFrame();
		if(!buf_Frame.is_playing)
			onPlayingEnded();
		if (buf_Frame.ballballcollisionvolume>0 )
			audio_player.PlayAudio("ball",buf_Frame.ballballcollisionvolume);
		if (buf_Frame.ballwallcollisionvolume>0 )
			audio_player.PlayAudio("wall",buf_Frame.ballwallcollisionvolume);
		if (buf_Frame.ballpocketvolume>0 ){
			audio_player.PlayAudio("pocket",buf_Frame.ballpocketvolume);
		}

		return buf_Frame;
	}

	public bool GetPlayingStatus()
	{	
		return is_playing;
	}

	public void onPlayingEnded()  //After one play End, (one Shoot)
	{
		if(Player.player_type==PLAYER_TYPE.MASTER || Player.player_type==PLAYER_TYPE.AI || Player.player_type==PLAYER_TYPE.STANDALONE)
		{
			Player.Should_change(myBuffer);  //check all in here.
			if(Player.win!=0)//Game End (Win or Lose)
			{
                GamePlayUI.instance.GameEnd();                 
			}
			if (Player.IsNet())
				GetComponent<PhotonView>().RPC ("PlayingEndedRPC",PhotonTargets.OthersBuffered,myBuffer.GetCurrentFrame().GetString(),Player.turn_change,Player.ball_type.ToString (),Player.win,Player.foul);
		}else if(Player.player_type==PLAYER_TYPE.SLAVE && is_ended){
//			print ("Slave Play End");
			EndPlaying(str_packet_temp,turn_change_temp,str_balltype_temp,win_or_lose_temp,foul_temp);
		}

		Player.shotpower_type=SHOTPOWER_TYPE.LITTLE;
		is_playing=false;
		cueController.clock_left=cueController.clock_right=Constant.play_time;
		//should be changed
		if(Player.player_type!=PLAYER_TYPE.AI)
		{
			cueController.EnableBoosts();
		}
	}

	[PunRPC]
	private void OnPlayStarted(string m_packet){		
//		print ("OnPlayerStarted");
		myBuffer.StartPlaying(new Frame(m_packet));
		prev_Frame=new Frame(m_packet);
		generateFrames_Client=new Thread(GenerateFrames_Client);
		generateFrames_Client.Start ();
		is_playing=true;
		is_ended=false;
		cueController.cueDisplacement = 0.0f;
		cueController.cueForceValue=0.0f;
	}
	
	[PunRPC]
	private void PushFrame(string m_packet){
        print("pushFrame");
		Frame frame=new Frame(m_packet,prev_Frame);
		frame.is_playing=true;
		myBuffer.PushSyncFrame(frame);
		prev_Frame=frame;
	}

	private string str_packet_temp;
	private bool turn_change_temp;
	private string str_balltype_temp;
	private int win_or_lose_temp;
	private int foul_temp;

    [PunRPC]  //After one play End, (one Shoot)
	private void PlayingEndedRPC(string str_packet,bool turn_change,string str_balltype,int win_or_lose,int foul){
		//print("PlayingEnded: is_Playing:"+is_playing);
		str_packet_temp=str_packet;
		turn_change_temp=turn_change;
		str_balltype_temp=str_balltype;
		win_or_lose_temp=win_or_lose;
		foul_temp=foul;        
		Frame frame=new Frame(str_packet);
		if(is_playing)
		{	
			myBuffer.SetLastFrame(frame);            
		}else{
			cueController.ShowFrame(frame);
			EndPlaying(str_packet_temp,turn_change_temp,str_balltype_temp,win_or_lose_temp,foul_temp);            
		}
		is_ended=true;
		cueController.clock_left=cueController.clock_right=Constant.play_time;
	}

	private void EndPlaying(string str_packet,bool turn_change,string str_balltype,int win_or_lose,int foul){
//		print ("EndPlaying:win-"+win_or_lose+"turn_change:"+turn_change);
		if(turn_change){
			Player.player_type=PLAYER_TYPE.MASTER;
            PopupUI.instance.UpdateTopMask();
			cueController.cueDisplacement=0f;
			Player.masse=false;
		}
		Player.ball_type=(str_balltype=="BALL_FULL"?BALL_TYPE.BALL_FULL:(str_balltype=="BALL_HALF"?BALL_TYPE.BALL_HALF:BALL_TYPE.BALL_ANY));
		Player.win=-win_or_lose;
		
		cueController.isShoting=false;
		cueController.clock_left=cueController.clock_right=Constant.play_time;
	}

    public void GameEnd()
    {
        myBuffer.ClearAll();
        is_playing = false;
    }
}
