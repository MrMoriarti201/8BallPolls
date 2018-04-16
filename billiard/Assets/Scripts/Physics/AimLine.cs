using UnityEngine;
using System.Collections;

public class AimLine{

	private static ArrayList Line1;
	private static ArrayList Line2;
	private static Vector3 collision_pos;
	private static ArrayList CircleLine;

	private static LineRenderer lineRenderer1;
	private static LineRenderer lineRenderer2;
	private static LineRenderer circleLineRenderer;

	private static float short_line=1.5f;
	private static float long_line=2.66f;
	private static bool is_wall=false;

	static AimLine(){
		Line1= new ArrayList();
		Line2= new ArrayList();
		CircleLine= new ArrayList();

		//DrawCircle(new Vector3(0.0f,10.0f,0.0f),2.0f,10);
	}

	public static void ClearAimLines()
	{
		CueController.instance.line1.SetVertexCount(0);
		CueController.instance.line2.SetVertexCount(0);
		CueController.instance.line3.SetVertexCount(0);
		CueController.instance.circleLine.SetVertexCount(0);
	}

	private static void DrawCircle(Vector3 center, float radius, int slice_num){

		if(slice_num<6)
			slice_num=6;
		else if(slice_num>100)
			slice_num=100;
		for(int i=0;i<slice_num+1;i++)
		{
			float angle=(i%slice_num)/(float)slice_num*2.0f*Mathf.PI;
			Vector3 point=new Vector3(Mathf.Sin (angle)*radius,0.0f,Mathf.Cos (angle)*radius);
			CircleLine.Add(center+point);
		}
	}

	private static void DrawLines(){
		lineRenderer1=CueController.instance.line1;
		lineRenderer2=CueController.instance.line2;
		if (GlobalInfo.bet_index>6)
			lineRenderer2.enabled=false;
		circleLineRenderer=CueController.instance.circleLine;

		lineRenderer1.SetVertexCount(Line1.Count-1);
		CueController.instance.line3.SetVertexCount(2);
		lineRenderer2.SetVertexCount(Line2.Count);
		circleLineRenderer.SetVertexCount(CircleLine.Count);
		if(Line1.Count>1){
			CueController.instance.line3.SetPosition(0,(Vector3)Line1[Line1.Count-2]);
			for(int i=0;i<Line1.Count;i++)
			{
				if(i==Line1.Count-1){
					if (!is_wall || Player.show_wallline)
					{
						Vector3 vec_start=(Vector3)Line1[i-1];
						Vector3 vec_end=(Vector3)Line1[i];
						Line1[i]=Maths.vec_unit (vec_end-vec_start)*(Player.is_longline?long_line:short_line)+vec_start;
						if (GlobalInfo.bet_index>6){ //no GuideLine
	//						lineRenderer1.SetVertexCount(Line1.Count-1);
							CueController.instance.line3.SetVertexCount(0);
							break;
						}
					}
					CueController.instance.line3.SetPosition(1,(Vector3)Line1[i]);
				}
				else{
					lineRenderer1.SetPosition(i,(Vector3)Line1[i]);
				}
			}
		}
		if(Line2.Count>1){
			for(int i=0;i<Line2.Count;i++)
			{
				if(i==Line2.Count-1)
				{
					Vector3 vec_start=(Vector3)Line2[i-1];
					Vector3 vec_end=(Vector3)Line2[i];
					Line2[i]=Maths.vec_unit (vec_end-vec_start)*(Player.is_longline?long_line:short_line)+vec_start;
				}
				lineRenderer2.SetPosition(i,(Vector3)Line2[i]);
			}
		}
		if(CircleLine.Count>1){
			for(int i=0;i<CircleLine.Count;i++)
			{
				circleLineRenderer.SetPosition(i,(Vector3)CircleLine[i]);
			}
		}
	}

	private static Frame GetFrameByShot(Frame frame,Vector3 orient,float force,float X,float Y, bool is_masse) {
		Vector3 dir=Vector3.zero;
		Vector3 nx,nz;
		Vector3 hitpoint;
		float CUEBALL_MAXSPEED=100.0f;
		if(!is_masse)
			force=1.0f;

		force*=force;

		if(is_masse){
			dir=Maths.vec_unit(new Vector3(orient.x,2.0f,orient.z));
		}
		else{
			dir = orient;
		}
		nx = Maths.vec_unit(Maths.vec_cross(Vector3.up,dir));  /* parallel to table */
		nz = Maths.vec_unit(Maths.vec_cross(nx,dir));        /* orthogonal to dir and nx */
		hitpoint = X*nx+nz*Y*0.6f;
		frame.Balls[0].v =  dir*CUEBALL_MAXSPEED*force;
		if(Maths.vec_abssq(hitpoint)==0.0f){
			frame.Balls[0].w = Vector3.zero;
		} else {
			/* w = roll speed if hit 1/3of radius above center */
			//            balls.ball[cue_ball].w = vec_scale(vec_cross(dir,hitpoint),4.0*3.0*CUEBALL_MAXSPEED*queue_strength/balls.ball[cue_ball].d/balls.ball[cue_ball].d);
			/* hmm, this one works better */
			frame.Balls[0].w = Maths.vec_cross(dir,hitpoint)*2.0f*3.0f*CUEBALL_MAXSPEED*force*force/frame.Balls[0].d/frame.Balls[0].d;
		}
		return frame;
	}

	public static void HideAimLines()
	{
		Line1.Clear ();
		Line2.Clear ();
		CircleLine.Clear();
		ClearAimLines();
//		DrawLines();
	}

	public static void DrawAimLines(Frame frame,Vector3 orient,float force,float X,float Y, bool is_masse){	
//		X=0.99f;
		Frame original_frame=new Frame(frame);
		BallEvents.ClearAllEvents();
		Line1.Clear ();
		Line2.Clear ();
		CircleLine.Clear ();

		frame=GetFrameByShot (frame,orient,force,X,Y,is_masse);
		frame.is_playing=true;
		int j=1000;
		ArrayList _EventList=BallEvents.GetEventList();
		Vector3 delta_vec=new Vector3(0,10.0f,0.0f);
		Line1.Add (frame.Balls[0].r+delta_vec);
		int i=0; 
		for(i=0;frame.is_playing && j>0;i++,j--)
		{ 
			if(_EventList.Count>0 && j>1)
			{
				j=1;
				if(frame.Balls[0].in_game==false || frame.Balls[0].r.y<-frame.Balls[0].d)
				{
					is_wall=true;
					break;
				}
				BallEvent be=(BallEvent)_EventList[0];
				Line1.Add (be.pos2+delta_vec);
				if(be._event!=EventType0.BALL_WALL)
				{
					is_wall=false;
					int hit_ball=BallEvents.BM_get_1st_ball_hit();
					Line2.Add(original_frame.Balls[hit_ball].r+delta_vec);
					Line2.Add(be.v+original_frame.Balls[hit_ball].r+delta_vec);
					collision_pos=be.pos2+delta_vec;
					DrawCircle (collision_pos,Constant.BALL_D/2.0f,16);

				}else{
					collision_pos=be.pos2+delta_vec;
					DrawCircle (collision_pos,Constant.BALL_D/2.0f,16);
					if(Player.show_wallline)
						Line1.Add (frame.Balls[0].r+delta_vec);
					is_wall=true;
					break;
				}
			}
			if (i>0)
				Line1.Add (frame.Balls[0].r+delta_vec);
			else 
				Line1.Add (frame.Balls[0].r+delta_vec);
			frame=frame.GetNextFrame();
			if(frame.is_playing==false && is_masse==true)
			{
				Vector3 lastSpeed=Vector3.zero;
				i=Line1.Count-1;

				lastSpeed=(Vector3)Line1[i]-(Vector3)Line1[i-1];
				frame.Balls[0].v=Maths.vec_unit(lastSpeed)*100.0f;
				frame.is_playing=true;
				is_masse=false;
//				Debug.Log("is_masse=fales");
			}
		}
//		if(i==1)
//			MonoBehaviour.print ("error");
		DrawLines();
	}

}
