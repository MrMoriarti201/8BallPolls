using UnityEngine;
using System.Collections;

public enum AI_LEVEL{HIGH_LEVEL, MEDIUM_LEVEL, LOW_LEVEL}

public class AIPlayer {
	public static AI_LEVEL ai_level=AI_LEVEL.HIGH_LEVEL;

	public static Vector3 ai_get_stroke_dir_8ball( Frame frame)
	{
		BALL_TYPE full_half=Player.ball_type;
		Vector3 r_hit;
		float angle;
		float minangle = Mathf.PI;
		float minangle_beta=Mathf.PI;
		Ball bhit, bcue;
		Hole hole;
		int minball=0, minball_beta=0;
		int minhole=-1, minhole_beta=-1;
		int i,j;
		Vector3 ai_err=Vector3.zero;

		if(ai_level==AI_LEVEL.LOW_LEVEL)
		{
			ai_err=Maths.vec_unit (new Vector3(Random.Range(-1.0f,1.0f),0,Random.Range(-1.0f,1.0f)))*Constant.HOLE1_R*1.5f;
			//ai_err=Maths.vec_unit (new Vector3(1.0f,0,-1.0f))*Constant.HOLE1_R*1.5f;
		}else if(ai_level==AI_LEVEL.MEDIUM_LEVEL){
			ai_err=Maths.vec_unit (new Vector3(Random.Range(-1.0f,1.0f),0,Random.Range(-1.0f,1.0f)))*Constant.HOLE1_R*1.0f;
		}
		bcue = new Ball(frame.Balls[0]);
		for( i=1; i<16; i++ ) if ( frame.Balls[i].in_game){
			if( ( full_half==BALL_TYPE.BALL_HALF && frame.Balls[i].nr>8 ) ||
			   ( full_half==BALL_TYPE.BALL_FULL && frame.Balls[i].nr<8 ) ||
			   ( full_half==BALL_TYPE.BALL_ANY  && frame.Balls[i].nr!=8 ) ||
			   ( frame.Balls[i].nr==8 && frame.Balls_in_game(full_half)==0 ) ){
				bhit = new Ball(frame.Balls[i]);
				for( j=0; j<Holes.GetHoleNum(Holes.BORDERS_8GAME); j++ ){
					hole = Holes.GetHole (Holes.BORDERS_8GAME,j);
					r_hit = Maths.vec_unit(bhit.r-hole.aim)*(bcue.d+bhit.d)/2.0f;
					r_hit = bhit.r+r_hit;
					if( !ball_in_way(0,r_hit,frame) && !ball_in_way(i,hole.aim,frame) ){
						angle = Mathf.Abs(Maths.vec_angle(r_hit-bcue.r,hole.aim-r_hit) );
						if( angle<minangle ){
							minball = i;
							minhole = j;
							minangle = angle;
						}
					} else if(!ball_in_way (0,r_hit,frame))
					{
						angle = Mathf.Abs(Maths.vec_angle(r_hit-bcue.r,hole.aim-r_hit) );
						if(angle<minangle_beta){
							minball_beta=i;
							minhole_beta=j;
							minangle_beta=angle;
						}
					}
				}
			}
		}
		
		if( minball==0 ){  /* no proper ball found */
			minball=minball_beta;
			minhole=minhole_beta;
			minangle=minangle_beta;
		/*	switch(full_half) {
			case BALL_TYPE.BALL_FULL:
				if( BallEvents.BM_get_balls_out_full()!=7 ){
					minball=1+my_rand(7-BallEvents.BM_get_balls_out_full());
				}
				break;
			case BALL_TYPE.BALL_HALF:
				if( BallEvents.BM_get_balls_out_half()!=7 ){
					minball=1+my_rand(7-BallEvents.BM_get_balls_out_half());
				}
				break;
			case BALL_TYPE.BALL_ANY:
				if( BallEvents.BM_get_balls_out_total()!=15 ){
					minball=1+my_rand(15-BallEvents.BM_get_balls_out_total());
				}
				break;
			}
			if( minball==0 ){
				minball = ind_ball_nr(8,frame);
			} else {
				minball = nth_in_game(minball,frame,full_half);
			}*/
		}
		
		bhit = frame.Balls[minball];
		if(minhole!=-1){
			hole = Holes.GetHole (Holes.BORDERS_8GAME,minhole);
			r_hit = Maths.vec_unit(bhit.r-hole.aim+ai_err*Maths.vec_abs(hole.aim-bhit.r)/10.0f)*(bcue.d+bhit.d)/2.0f;
			r_hit = bhit.r+r_hit-bcue.r;
		} else {  /* no proper ball found */
			r_hit = bhit.r-bcue.r;
			minhole=Random.Range(0,6);
		}
		GamePlayUI.instance.SetAIPocket(minhole);
		return Maths.vec_unit(r_hit);
	}

	private static bool ball_in_way( int ballnr, Vector3 aim, Frame frame )
	{
		Vector3 way, iball;
		float par, norm, lway;
		bool inway=false;
		int i;
		
		for(i=0;i<16;i++) {
			if( frame.Balls[i].in_game && i!=ballnr ){
				way   = aim-frame.Balls[ballnr].r;
				lway  = Maths.vec_abs(way);
				iball = frame.Balls[i].r-frame.Balls[ballnr].r;
				par   = Maths.vec_mul(Maths.vec_unit(way),iball);
				norm  = Maths.vec_abs(Maths.vec_cross(Maths.vec_unit(way),iball));
				if( par>0.0 && par<lway && norm<(frame.Balls[i].d+frame.Balls[ballnr].d)/2.0 ){
					inway=true;
					break;
				}
			}
		}
		return( inway );
	}

	private static int my_rand(int nr)
	{
		return Random.Range (0,nr);
	}

	private static float my_rand01()
	{
		return Random.Range (0.0f,1.0f);
	}

	private static int ind_ball_nr( int nr, Frame frame )
	{
		int i;
		for( i=0 ; i<16 ; i++ ){
			if( frame.Balls[i].nr == nr ) break;
		}
		return i;
	}
	
	private static int nth_in_game( int n, Frame frame, BALL_TYPE full_half )
	{
		int i;
		for( i=0; i<16 && n>=0; i++ ){
			if( full_half == BALL_TYPE.BALL_FULL && frame.Balls[i].nr<8 && frame.Balls[i].nr>0 ){
				n--;
			}
			if( full_half == BALL_TYPE.BALL_HALF && frame.Balls[i].nr>8 ){
				n--;
			}
			if( full_half == BALL_TYPE.BALL_ANY && ( frame.Balls[i].nr>8 || (frame.Balls[i].nr<8 && frame.Balls[i].nr>0) )){
				n--;
			}
		}
		return i;
	}
}