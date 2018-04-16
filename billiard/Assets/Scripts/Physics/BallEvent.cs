/*
 BallEvents is the class which logs all the events like Ball_out, Ball_wall_interaction, Ball_ball_interaction
 */
//
using UnityEngine;
using System.Collections;
/// This is an enumerator which presents if it's ball_out, ball_wall or ball_ball
public enum EventType0{
	BALL_OUT,BALL_WALL,BALL_BALL
}

/// <summary>
/// This is a struct which contains the event
/// Here we logs nr of balls, positions, speeds, ...
/// </summary>
public struct BallEvent {
	public int	ballnr;
	public int     ballnr2;
	public Vector3  pos;
	public Vector3  pos2;
	public Vector3  v;
	public Vector3  v2;
	public Vector3  w;
	public Vector3  w2;
	public int     timestep_nr;
	public float timeoffs;
	public EventType0 _event;
}

/// <summary>
/// This is a class which logs all the Events
/// </summary>
public class BallEvents{
	private static int timestep_nr;
	//private static float duration;
	//private static float duration_last;
	public static int out_half;

	public static int out_full;
	public static int out_white;
	public static int out_black;
	public static int collide_wall;
	//private static int eventnr;
	private static ArrayList _EventList;

	public static int BM_get_balls_out_half()  { return out_half; }
	public static int BM_get_balls_out_full()  { return out_full; }
	public static int BM_get_balls_out_total() { return out_full+out_half; }
	public static int BM_get_balls_out_all() { return out_full+out_half+out_white+out_black; }
	public static int BM_get_white_out() { return out_white; }

	public static int BM_get_balls_walls(){return collide_wall;}
	
	/// <summary>
	/// This is a constructor which initializes all  the values.
	/// Return value : null
	/// </summary>
	static BallEvents(){
		_EventList=new ArrayList();
		ClearAllEvents();
	}
	
	/// <summary>
	/// This is a function which clears all the events
	/// </summary>
	public static void ClearAllEvents()
	{
		out_half=0;
		out_full=0; 
		out_white=0;
		out_black=0;
//		eventnr=0;
		timestep_nr=0;
		collide_wall=0;
//		duration=0.0f;
//		duration_last=0.0f;
		_EventList.Clear();
	}

	public static int BM_get_nth_ball_out(int n)
	{
		int i,ballout=-1;
		for(i=0;i<_EventList.Count && n!=0;i++){
			BallEvent be=(BallEvent)_EventList[i];
			if( be._event==EventType0.BALL_OUT){
				n--;
				if( n==0 ) ballout=be.ballnr;
			}
		}
		return ballout;
	}

	public static int GetOutPocket(int n)
	{
		for(int i=0;i<_EventList.Count && n!=0;i++){
			BallEvent be=(BallEvent)_EventList[i];
			if( be._event==EventType0.BALL_OUT){
				n--;
				if( n==0 )
					return be.ballnr2;					
			}
		}
		return -2;
	}

	public static int BM_get_min_ball_out() {
		int minnr=100;
		
		for(int i=0;i<_EventList.Count;i++){
			BallEvent be=(BallEvent)_EventList[i];
			if( be._event==EventType0.BALL_OUT && be.ballnr<minnr )
				minnr=be.ballnr;
		}
		return minnr;
	}

	/// Get the index of first ball hit event
	public static int BM_get_1st_ball_hit()
	{
		int hitball=-1;
		
		for(int i=0;i<_EventList.Count;i++){
			BallEvent be=(BallEvent)_EventList[i];
			if( be._event==EventType0.BALL_BALL ){
				if( be.ballnr!=0 ){
					hitball=be.ballnr;
				} else {
					hitball=be.ballnr2;
				}
				break;
			}
		}
		return hitball;
	}

	/// <summary>
	/// It is the function which returns the ball with it's number.
	/// </summary>
	/// <returns>the ball</returns>
	/// <param name="nr">number of ball</param>
	/// <param name="frame">Frame.</param>
	public static Ball BM_get_ball_by_nr( int nr, Frame frame )
	{
		int i;
		for(i=0;i<16;i++)
			if( frame.Balls[i].nr == nr ) break;
		return (i<16) ? frame.Balls[i] : null ;
	}

	/// <summary>
	/// Record all the ball event.
	/// </summary>
	/// <param name="_event">the type of event</param>
	/// <param name="nr">number of first ball</param>
	/// <param name="nr2">number of second ball</param>
	/// <param name="frame">Frame</param>
	/// <param name="timeoffs">Timeoffset</param>
	public static void record_move_log_event( EventType0 _event, int nr, int nr2, Frame frame, float timeoffs )
	{
		BallEvent be=new BallEvent();
		be._event=_event;
		
		be.ballnr=nr;
		be.ballnr2=nr2;

		Ball ball1=BM_get_ball_by_nr(nr,frame);
		Ball ball2=BM_get_ball_by_nr(nr2,frame);

		if(_event==EventType0.BALL_BALL){

			be.pos =ball1.r;
			be.pos2=ball2.r;
			be.v   =ball1.v;
			be.v2  =ball2.v;
			be.w   =ball1.w;
			be.w2  =ball2.w;
		}else if(_event==EventType0.BALL_WALL){
			be.pos2=ball2.r;
			be.v2  =ball2.v;
			be.w2  =ball2.w;
		}else if(_event==EventType0.BALL_OUT){
			be.pos =ball1.r;
		}
		
		be.timestep_nr = timestep_nr;
		be.timeoffs    = timeoffs;

		_EventList.Add(be);
	}

	public static int BM_get_balls_hit()  {
		int i,hits=0;		
		for(i=0;i<_EventList.Count;i++){
			BallEvent be=(BallEvent)_EventList[i];
			if( be._event==EventType0.BALL_BALL ){
				if( be.ballnr==0 || be.ballnr2==0 ){
					hits++;
				}
			}
		}
		return hits;
	}

	public static int BM_get_ball_out(int nr) {
		for(int i=0;i<_EventList.Count;i++){
			BallEvent be=(BallEvent)_EventList[i];
			if( be._event==EventType0.BALL_OUT && be.ballnr==nr ){
				return be.ballnr2;  //-1: Ok. 0~5: wrong pocket
			}
		}
		return -2; //no out
	}

	public static ArrayList GetEventList(){
		return _EventList;
	}

    public static void UpLoadBallStatus()
    {
        int id,pos;        
        for (int i = 0; i < _EventList.Count; i++)
        {
            BallEvent be = (BallEvent)_EventList[i];
            if (be._event == EventType0.BALL_OUT && be.ballnr !=0)
            {
                if (be.ballnr != 8)  // top ball
                {
                    GamePlayUI.instance.SetBallOut(be.ballnr);
                }
                //Tournament Ball
                if (!GlobalInfo.IsTournament())
                    continue;
                if (Tournament.IsMatch && Tournament.pos > Tournament.opponentPos)
                    continue;
                if (Player.IsPlayer()){
                    Tournament.ballNum++;
                    id = Tournament.ballNum;
                    pos = Tournament.pos;
                }   
                else{
                    Tournament.opponentBallNum++;
                    id = Tournament.opponentBallNum;
                    pos = Tournament.opponentPos;
                }
                    
                Net.instance.SendMsg(new Game.SetTournamentBall(pos,id,be.ballnr));
            }
        }
    }
}
