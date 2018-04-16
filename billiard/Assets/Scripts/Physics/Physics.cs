/*
	This is the Physics Class.
	This class is not used to declare some objects
	It just declares some functions which are used to generate the frame.
 */
using UnityEngine;
using System.Collections;

public class Physics0{
	//Some neccessary constants go here
	private static float SQRTM1=100000.0f;
	private static float SLIDE_THRESH_SPEED=1.0E-2f*Constant.rateWithOpenGL;
	private static float GRAVITY=9.81f;      /* m/s^2 */
	private static float MU_ROLL=0.03f*Constant.rateWithOpenGL;      /* table  roll-friction const */
	private static float MU_SLIDE=0.2f*Constant.rateWithOpenGL;       /* table slide-friction const */
	private static float SPOT_R =12.0e-3f*Constant.rateWithOpenGL;    /* 3mm radius der auflageflaeche - not used for rollmom (only rotational-friction around spot) */
	private static float OMEGA_MIN=SLIDE_THRESH_SPEED/SPOT_R;   /* 22.5°/s */
	private static int pocket_ball_gap=0;

	private static float logtime=0.0f;

	//This is the main function of the Physics class to generate the next frame from the previous frame
	//This function calls function "proceed_dt_euler" to get the fairly exact next frame except restriction and removing the ball from the game.
	//Called Functions : proceed_dt_euler, perimeter_speed, remove_balls_from_game
	//Return Value : null
	public static void proceed_dt(Frame frame, int border_type, float dt)
	{
		int i=0;
		Vector3 accel, waccel, uspeed, uspeed_eff, uspeed2, uspeed_eff2, fricaccel, fricmom, rollmom, totmom;
		float uspeed_eff_par, uspeed_eff2_par;
		int playcount=16;
		/* timestep with actual speeds, omegas,... */
		frame.ballballcollisionvolume=0;
		proceed_dt_euler(frame, border_type, dt, 0);
		
		/* calc new accelerations and speeds */
		for(i=0;i<16;i++) {
			if(frame.Balls[i].in_game){
				/* check if balls still moving */
				if(frame.Balls[i].r.y<-frame.Balls[i].d)
				{
					/*if(frame.Balls[i].r.y>-frame.Balls[i].d/2.0f)
						frame.Balls[i].r+=Vector3.up*-frame.Balls[i].d;*/
					if(frame.Balls[i].r.y<-frame.Balls[i].d*4.0f)
					{
						if(frame.Balls[i].v==Vector3.zero)
						{}
						else if(frame.Balls[i].r.x>Constant.POCKET_BALL_X-Constant.POCKET_CURVE_RADIUS)
						{
							frame.Balls[i].v=new Vector3(frame.Balls[i].v.x,0,0);
							//frame.Balls[i].v+=Vector3.right/10.0f;
						}else{
							frame.Balls[i].v=Maths.vec_cross (Vector3.down*3.0f,frame.Balls[i].r-new Vector3(Constant.POCKET_BALL_X-Constant.POCKET_CURVE_RADIUS,Constant.POCKET_BALL_Y,Constant.POCKET_BALL_Z));
						}
						frame.Balls[i].w = Maths.vec_cross( new Vector3(0.0f,2.0f/frame.Balls[i].d,0.0f),frame.Balls[i].v);
						/*if( Maths.vec_abs(frame.Balls[i].w) < OMEGA_MIN && Maths.vec_abs(frame.Balls[i].v) < SLIDE_THRESH_SPEED ) {
							frame.Balls[i].v = new Vector3(0,frame.Balls[i].v.y,0);
							frame.Balls[i].w =Vector3.zero;
							playcount--;
						}*/
					}else if(frame.Balls[i].r.y>-frame.Balls[i].d*3.0f){
						if(i==0)
						{
							frame.Balls[i].r=new Vector3(-100.0f,-100.0f,0);
						}else{
							frame.Balls[i].v+=Vector3.up*-GRAVITY*dt*20.0f;
						}
					}
				}
				else
				{
					if( Maths.vec_abs(frame.Balls[i].v)!=0 || Maths.vec_abs(frame.Balls[i].w)!=0 ){
						//		balls_moving=1;
						// we allways keep the moves, but only draws it, if options_balltrace is set
						//BM_add2path(&balls->ball[i] ); //draw the ball line
					}
					
					/* calc accel 3D */
					accel  = Vector3.zero; // init acceleration
					
					/* absolute and relative perimeter speed */
					uspeed = perimeter_speed(frame.Balls[i]);
					uspeed_eff = uspeed+frame.Balls[i].v;

					if( Maths.vec_abs( uspeed_eff ) > SLIDE_THRESH_SPEED ){  /* if sliding */
						/* acc caused by friction */
						fricaccel = Maths.vec_unit(uspeed_eff)*-MU_SLIDE*GRAVITY;
						accel += fricaccel;
						/* angular acc caused by friction */
						
						fricmom = Maths.vec_cross(fricaccel, new Vector3(0.0f,-frame.Balls[i].d/2.0f,0.0f))*frame.Balls[i].m;
						waccel = fricmom*-1.0f/frame.Balls[i].I ;
						//	if(i==0)
						/* perform accel */
						frame.Balls[i].w+= waccel*dt;
						frame.Balls[i].v+= accel*dt;
						uspeed2 = perimeter_speed(frame.Balls[i]);
						uspeed_eff2 = uspeed2+frame.Balls[i].v;
						/* if uspeed_eff passes 0 */
						uspeed_eff_par  = Maths.vec_mul( uspeed_eff, uspeed_eff-uspeed_eff2);
						uspeed_eff2_par = Maths.vec_mul( uspeed_eff2,uspeed_eff-uspeed_eff2);
						if( Maths.vec_ndist(Vector3.zero,uspeed_eff,uspeed_eff2) <= SLIDE_THRESH_SPEED && ((uspeed_eff_par >= 0.0f && uspeed_eff2_par <= 0.0f) || 
						                                                                                   (uspeed_eff2_par >= 0.0f && uspeed_eff_par <= 0.0f))){
							/* make rolling if uspeed_eff passed 0 */
							Vector3 md=Maths.vec_cross( frame.Balls[i].w, new Vector3(0.0f,frame.Balls[i].d/2.0f,0.0f));
							frame.Balls[i].v=(frame.Balls[i].v+md)/2.0f;
							frame.Balls[i].w = Maths.vec_cross( new Vector3(0.0f,2.0f/frame.Balls[i].d,0.0f),frame.Balls[i].v);
							frame.Balls[i].v = Maths.vec_cross( frame.Balls[i].w, new Vector3(0.0f,frame.Balls[i].d/2.0f,0.0f));
							
						}
						if( Maths.vec_abs(frame.Balls[i].w) < OMEGA_MIN && Maths.vec_abs(frame.Balls[i].v) < SLIDE_THRESH_SPEED ){
							frame.Balls[i].v = new Vector3(0,frame.Balls[i].v.y,0);
							frame.Balls[i].w = Vector3.zero;
						}
					} else { //If rolling
						fricmom = Vector3.zero;

						/* moment of rotation around ballspot */
						if( Mathf.Abs(Maths.vec_mul(Vector3.up,frame.Balls[i].w)) > OMEGA_MIN ){
							fricmom+=new Vector3(0.0f,frame.Balls[i].w.y,0.0f)*MU_SLIDE*frame.Balls[i].m*GRAVITY*SPOT_R;
						}

						/* wirkabstand von rollwid.-kraft */
						float ROLL_MOM_R=MU_ROLL*frame.Balls[i].I/frame.Balls[i].m/frame.Balls[i].d;
						
						rollmom = Maths.vec_cross(new Vector3(0.0f,frame.Balls[i].m*GRAVITY * ROLL_MOM_R*1.0f,0.0f), Maths.vec_unit(frame.Balls[i].v));
						
						totmom = fricmom+rollmom;
						waccel = totmom*-1.0f/frame.Balls[i].I;
						
						if(frame.Balls[i].w.y*(frame.Balls[i].w.y+waccel.y*dt)<0)
						{
							frame.Balls[i].w =frame.Balls[i].w+waccel*dt;
							frame.Balls[i].w=new Vector3(frame.Balls[i].w.x,0,frame.Balls[i].w.z);
						}else
							frame.Balls[i].w =frame.Balls[i].w+waccel*dt;

						/* align v with w to assure rolling */
						frame.Balls[i].v = Maths.vec_cross( frame.Balls[i].w, Vector3.up*frame.Balls[i].d/2.0f);
						if( Maths.vec_abs(frame.Balls[i].w) < OMEGA_MIN && Maths.vec_abs(frame.Balls[i].v) < SLIDE_THRESH_SPEED ) {
							frame.Balls[i].v = new Vector3(0,frame.Balls[i].v.y,0);
							frame.Balls[i].w =Vector3.zero;
							playcount--;
						}
					}
					/*if(frame.Balls[i].in_hole==2)
					{
						frame.Balls[i].r=new Vector3(frame.Balls[i].r.x,Constant.POCKET_BALL_Y,frame.Balls[i].r.z);
						frame.Balls[i].v=new Vector3(frame.Balls[i].v.x,Constant.POCKET_BALL_Y,frame.Balls[i].v.z);
					}*/
				}
			}
			else
				playcount--;
			if(frame.Balls[i].r.y>-frame.Balls[i].d)
			{
				frame.Balls[i].r=new Vector3(frame.Balls[i].r.x,0.0f,frame.Balls[i].r.z);
				frame.Balls[i].v=new Vector3(frame.Balls[i].v.x,0.0f,frame.Balls[i].v.z);
			}
		}
		if(playcount==0)
			frame.is_playing=false;
		remove_balls_from_game(frame);
		pocket_balls_from_game(frame);
	}
	//This is the function to get the next frame without restriction and removing balls from game
	//This function calls the function "proceed_dt_only" to get the next frame just with simple movement from speed and rotation speed.
	//It uses recursive call to generate the exact frame without penetration.
	//It calculates the wall_collision and ball_ball_collision part mainly
	//Called Functions : proceed_dt_only, calc_wall_collision_time, calc_ball_collision_time, ball_wall_interaction, ball_ball_interaction,
	//					 proceed_dt_euler, wall_dv_pos, ball_dv_pos
	//Return Value : null.
	private static void proceed_dt_euler(Frame frame, int border_type, float dt, int depth)
		/* this one does not remove fallen balls */
	{
		const int COLLTYPE_WALL=1;
		const int COLLTYPE_BALL=2;
		const int COLLTYPE_NONE=0;

		float dt1;
		float dtmin=0.0f;
		int i,j;
		int colltype=COLLTYPE_NONE;
		int collnr=-1;
		int collnr2=-1;
		
		if(depth==0){
			logtime=dt;
		} else {
			logtime+=dt;
		}
		
		/* move all balls */
		proceed_dt_only(frame,dt);		
		/* checks */
		for(i=0;i<16;i++){
			//if(frame.Balls[i].in_game){
			/* check wall collisions */
				for(j=0;j<Borders.GetBorderNum(Borders.BORDERS_8GAME);j++){
					dt1=calc_wall_collision_time(frame.Balls[i],Borders.GetBorder (Borders.BORDERS_8GAME,j));
					if( dt1<dtmin && dt1>-dt ){
						if( !wall_dv_pos(frame.Balls[i],Borders.GetBorder (Borders.BORDERS_8GAME,j),-dt*1.0f) ){ /* dont strobe apart */
							dtmin=dt1; colltype=COLLTYPE_WALL; collnr=j; collnr2=i;
						}
					}
				}
				/* check ball collisions */
				for(j=0;j<16;j++) if( j!=i){
					dt1=calc_ball_collision_time(frame.Balls[i],frame.Balls[j]); /* dt1 should be negative */
					if( dt1<dtmin && dt1>-dt ){
						if( !ball_dv_pos(frame.Balls[j],frame.Balls[i],-dt*1.0f) ){ /* dont strobe apart */
							dtmin=dt1; colltype=COLLTYPE_BALL; collnr=j; collnr2=i;
						}
					}
				}
			//}
		}		
		switch(colltype) {
		case COLLTYPE_WALL:
			logtime+=dtmin;
			proceed_dt_only(frame,dtmin);
			ball_wall_interaction(frame.Balls[collnr2],Borders.GetBorder (Borders.BORDERS_8GAME,collnr));
			BallEvents.collide_wall++;
			BallEvents.record_move_log_event( EventType0.BALL_WALL, collnr, frame.Balls[collnr2].nr, frame, frame.time);
			frame.ballwallcollisionvolume=(Maths.vec_abs (frame.Balls[collnr2].v))*0.05f;
//			MonoBehaviour.print(frame.ballwallcollisionvolume);
			proceed_dt_euler(frame,border_type,-dtmin,depth+1);
			break;
		case COLLTYPE_BALL:
			logtime+=dtmin;
			proceed_dt_only(frame,dtmin);
			ball_ball_interaction(frame.Balls[collnr],frame.Balls[collnr2]);

			BallEvents.record_move_log_event( EventType0.BALL_BALL, frame.Balls[collnr].nr, frame.Balls[collnr2].nr, frame, frame.time);
			frame.ballballcollisionvolume=(Maths.vec_abs (frame.Balls[collnr].v)+Maths.vec_abs(frame.Balls[collnr2].v))*0.05f;
			proceed_dt_euler(frame,border_type,-dtmin,depth+1);
			break;
		}
	}

	//This is the simple function to generate the next frame just with simple change of position and eulerAngle from the speed and the rotation speed.
	//Return Value : null
	private static void proceed_dt_only(Frame frame,float dt){
		Vector3 dx;
		float dphi;
		
		for(int i=0;i<16;i++){
			if(frame.Balls[i].in_game){
				/* translate ball */
				dx = frame.Balls[i].v*dt;
				frame.Balls[i].r += dx;
				
				/* perform rotation */
				dphi = Maths.vec_abs(frame.Balls[i].w)*dt;
				if( dphi > 1.0E-30f ){
					Quaternion rp=Quaternion.Euler(frame.Balls[i].rp);
					Quaternion w=Quaternion.AngleAxis(Maths.vec_abs(frame.Balls[i].w*dt*180.0f/Mathf.PI),frame.Balls[i].w);
					w=w*rp;
					frame.Balls[i].rp=w.eulerAngles;
				}
			}
		}
	}
	//This function removes balls from game by setting the value of in_game to false.
	//Ball is removed when it is pocketed.
	private static void remove_balls_from_game( Frame frame)
	{
		int i,playcount=16;
		for(i=0;i<16;i++){
			if(frame.Balls[i].in_game==true)
				playcount--;
		}
		for(i=0;i<16;i++){
			if(frame.Balls[i].r.z==12.0f)
				frame.Balls[i].in_game=false;
			if(!frame.Balls[i].in_game)
				continue;
			if( (Mathf.Abs(frame.Balls[i].r.z) >(Constant.TABLE_W/2) ||  Mathf.Abs(frame.Balls[i].r.x) >(Constant.TABLE_L/2)) &&
			   frame.Balls[i].r.y>-frame.Balls[i].d){//Ball Out!!! (r=position(vector3))
//				Debug.Log("Ball out:"+frame.Balls[i].nr+" Pos:"+frame.Balls[i].r);
				int holeNum=GamePlayUI.pocketId;
				if (holeNum>-1){ // call pocket mode
					if (holeNum==ball_in_hole(frame.Balls[i])){
						//Debug.Log("same hole:"+holeNum);
						holeNum=-1;
					}
					else{
						Debug.Log("Different Hole:"+holeNum+":"+ball_in_hole(frame.Balls[i]));
					}
				}

				frame.Balls[i].r=new Vector3(frame.Balls[i].r.x,-frame.Balls[i].d*2.0f,frame.Balls[i].r.z);
				if( frame.Balls[i].nr==0 ) 
					BallEvents.out_white=1;
				else if( frame.Balls[i].nr==8 ) 
					BallEvents.out_black=1;
				else if( frame.Balls[i].nr<8)
					BallEvents.out_full++;
				else if( frame.Balls[i].nr>8)
					BallEvents.out_half++;
				BallEvents.record_move_log_event( EventType0.BALL_OUT, frame.Balls[i].nr, holeNum, frame, 0.0f );
				frame.ballpocketvolume=1.0f;
			}

			if(frame.Balls[i].r.y<-frame.Balls[i].d*4.0f)
			{
				if(Mathf.Abs(frame.Balls[i].r.x)>Constant.TABLE_W/2.0f+Constant.BALL_D || frame.Balls[i].v==Vector3.zero)
				{
					frame.Balls[i].in_game=false;
					frame.Balls[i].v=Vector3.zero;
				}
			}
			else if(frame.Balls[i].r.y<-frame.Balls[i].d*3.0f)
			{
				frame.Balls[i].v=new Vector3(-frame.Balls[i].r.x/2.0f,0.0f,-frame.Balls[i].r.z/2.0f)/50.0f*Constant.FRAMERATE;
				frame.Balls[i].w = Maths.vec_cross( new Vector3(0.0f,2.0f/frame.Balls[i].d,0.0f),frame.Balls[i].v);
				if(Mathf.Abs (frame.Balls[i].r.x)<Constant.TABLE_L/3.0f && Mathf.Abs (frame.Balls[i].r.z)<Constant.TABLE_W/3.0f)
				{
					if(pocket_ball_gap==0)
					{
						frame.Balls[i].r=new Vector3(Constant.POCKET_BALL_X,Constant.POCKET_BALL_Y,Constant.POCKET_BALL_Z);
						frame.Balls[i].v=Vector3.back*3.0f;
						frame.Balls[i].w = Maths.vec_cross( new Vector3(0.0f,2.0f/frame.Balls[i].d,0.0f),frame.Balls[i].v);
						pocket_ball_gap=7;
					}else{
						pocket_ball_gap--;
					}
				} 
			}
		}
	}

	private static void pocket_balls_from_game( Frame frame)
	{
		/*int i;
		for(i=0;i<16;i++){
			if(frame.Balls[i].in_hole==1 && (Mathf.Abs(frame.Balls[i].r.x)<Constant.TABLE_L/3.0f && Mathf.Abs(frame.Balls[i].r.z)<Constant.TABLE_W/3.0f))
			{
				frame.Balls[i].in_hole=2;
				frame.Balls[i].r=new Vector3(Constant.POCKET_BALL_X,Constant.POCKET_BALL_Y,Constant.POCKET_BALL_Z);
				frame.Balls[i].v=Vector3.back*3.0f;
				frame.Balls[i].w=Vector3.left*6.0f;
			}
			if(frame.Balls[i].in_hole==2)
			{
				if(Mathf.Abs (frame.Balls[i].r.z)>Constant.TABLE_L/1.5f)
					frame.Balls[i].in_game=false;
			}
		}*/
	}
	//It calculates how long it takes for ball to collide with the wall.
	//If it returns minus value it means it had to be already reflected.
	//Called Functions : inrange_advborder
	//Return Value : float type value. Collisiont time of ball and wall.
	private static float calc_wall_collision_time(Ball ball,Border border){
		float h, vn, rval, ph, q, t1,t2;
		Vector3   dr, r, v=Vector3.zero;
		rval=0.0f;
		switch (border.pnr) {
		case 4:
		case 3:
			dr = ball.r-border.r1;
			h  = Maths.vec_mul(dr,border.n) - ball.d/2.0f;
			vn = Maths.vec_mul(ball.v,border.n);
			rval = -h/vn;
			break;
		case 2:
			/* del all comps par to cylinder */
			dr = border.r2-border.r1;
			r =  ball.r-border.r1;
			r -= Maths.vec_proj(r,dr);
			v =  ball.v;
			v -= Maths.vec_proj(v,dr);
			ph = Maths.vec_mul (v,r)/Maths.vec_abssq(v);
			q  = (Maths.vec_abssq(r) - ball.d*ball.d/4.0f)/Maths.vec_abssq(v);
			if(ph*ph>q){
				t1 = -ph+Mathf.Sqrt(ph*ph-q);
				t2 = -ph-Mathf.Sqrt(ph*ph-q);
			} else {
				t1 = SQRTM1;
				t2 = SQRTM1;
			}
			/* solve |r+vt|=d/2 */
			rval = (t1<t2)?t1:t2;
			break;
		case 1:
			r = ball.r-border.r1;
			ph = Maths.vec_mul(ball.v,r)/Maths.vec_abssq(ball.v);
			q  = (Maths.vec_abssq(r) - ball.d*ball.d/4.0f)/Maths.vec_abssq(v);
			if(ph*ph>q){
				t1 = -ph+Mathf.Sqrt(ph*ph-q);
				t2 = -ph-Mathf.Sqrt(ph*ph-q);
			} else {
				t1 = SQRTM1;
				t2 = SQRTM1;
			}
			rval = (t1<t2)?t1:t2;
			break;
		}
		if( !inrange_advborder( ball, border ) ){
			rval=1E20f;
		}
		return(rval);
	}
	//It calculates how long it takes for ball to collide with other ball.
	//If it returns minus value it means it had to be already collided.
	//Return Value : float type value. Collisiont time of two balls.
	private static float calc_ball_collision_time( Ball b1, Ball b2 )
	{
		float p, q, vs, rs, t1, t2, ds;
		Vector3 dv, dr;
		dv = b1.v-b2.v;
		dr = b1.r-b2.r;
		vs = dv.x*dv.x + dv.y*dv.y + dv.z*dv.z ;
		rs = dr.x*dr.x + dr.y*dr.y + dr.z*dr.z ;
		ds = (b1.d+b2.d)/2.0f;
		ds *= ds;
		p  = ( dv.x*dr.x + dv.y*dr.y + dv.z*dr.z )/vs;
		q  = (rs-ds)/vs;
		q  = (p*p>q)?Mathf.Sqrt(p*p-q):SQRTM1;
		t1 = -p + q;
		t2 = -p - q;
		return( (t1<t2)?t1:t2 );
	}

	//This function calculates the reflected speed and rotation speed of the ball against the wall
	//It changes the speed and rotation speed after calculation
	//Return Value : null
	private static void ball_wall_interaction( Ball ball, Border border )
	{
		float CUSHION_LOSS_O0=border.loss0;       /* const loss of energy by cushion */
		float CUSHION_MAX_LOSS=border.loss_max;    /* const loss of energy by cushion */
		float CUSHION_LOSS_WSPEED=border.loss_wspeed; /* prop. halbwertsgeschwindigkeit */
		float MU_BANDE=border.mu;          /* friction const between cusion an ball */
		
		Vector3 dv, dw, dr, hit_normal;
		// myvec n;
		Vector3 vp, vn;
		float loss;
		Vector3 dw2;
		
		switch (border.pnr) {
		case 4:
			hit_normal=border.n;
			break;
		case 3:
			hit_normal=border.n;
			break;
		case 2:
			dr = border.r2-border.r1;
			hit_normal = ball.r-border.r1;
			hit_normal = Maths.vec_unit( hit_normal-Maths.vec_proj(hit_normal,dr) );
			break;
		case 1:
			hit_normal = Maths.vec_unit( ball.r-border.r1);
			break;
		default:
			hit_normal=Vector3.zero;
			break;
		}
		
		if (border.pnr==4 || border.pnr==3 || border.pnr==2 || border.pnr==1){
			
			vn = Maths.vec_proj(ball.v, hit_normal);
			vp = ball.v-vn;
			
			/* normal component */
			loss = CUSHION_LOSS_O0+(CUSHION_MAX_LOSS-CUSHION_LOSS_O0)*(1.0f-Mathf.Exp(-Maths.vec_abs(vn)/CUSHION_LOSS_WSPEED));
			dv = vn*-(1.0f+Mathf.Sqrt(1.0f-loss) );
			//dv= vn*-(1.0f+Mathf.Sqrt(1.0f-) );
			ball.v +=dv;
			
			/* parallel component */
			dv = Maths.vec_unit(perimeter_speed_normal(ball, hit_normal)+vp)*-Maths.vec_abs(dv)*MU_BANDE;
			dw = Maths.vec_cross(dv*ball.m/2.0f/ball.I,hit_normal*ball.d);
			dw2 =dw+Maths.vec_proj(ball.w,dw);
			if( Maths.vec_mul(dw2,ball.w) < 0.0f ){
				dw-=dw2;
				dv = Maths.vec_unit(dv)*Maths.vec_abs(dw)*2.0f*ball.I/ball.m/ball.d;
			}
			ball.w +=dw;
			dv.y = 0.0f;
			ball.v -=dv;

			if(border.pnr==4)
				ball.v=Vector3.zero;
		}
	}
	//This function calculates the reflected speed and rotation speed of the both balls.
	//It changes the speed and rotation speed of two collided balls after calculation.
	//Return Value : null
	private static void ball_ball_interaction( Ball b1, Ball b2 )
	{
		Ball b1s, b2s;  /* balls in coord system of b1 */
		Vector3 dvec, duvec, dvn, dvp, dw1, dw2;
		Vector3 dw1max, dw2max;
		Vector3 dpp, dpn, fric_dir,perimeter_speed_b1, perimeter_speed_b2;
		Vector3 dv1, dv2;
		float MU_BALL=0.1f;     /* friction const between ball and ball */

		dvec = b1.r-b2.r;
		duvec = Maths.vec_unit(dvec);
		
		/* stoss */
		b1s=new Ball(b1); b2s=new Ball(b2);
		b2s.v-=b1s.v;
		b1s.v=Vector3.zero;
		
		dvn   = duvec*Maths.vec_mul(duvec,b2s.v);
		dvp   = b2s.v-dvn;
		
		b2s.v = b2s.v-dvn;
		b2s.v = b2s.v+dvn*(b2s.m-b1s.m)/(b1s.m+b2s.m);
		b1s.v = dvn*2.0f*b2s.m/(b1s.m+b2s.m);  /* (momentum transfer)/m1 */
		b2.v =b1.v+b2s.v;
		b1.v+=b1s.v;
		
		/* angular momentum transfer */
		dpn  = b1s.v*b1s.m; /* momentum transfer from ball2 to ball1 */
		perimeter_speed_b1=Maths.vec_cross( b1.w, duvec*-b1.d/2.0f);
		perimeter_speed_b2=Maths.vec_cross( b2.w, duvec*b2.d/2.0f);
		fric_dir = Maths.vec_unit(perimeter_speed_b2-perimeter_speed_b1+dvp);
		dpp = fric_dir*-Maths.vec_abs(dpn)*MU_BALL;  /* dp parallel of ball2 */
		dw2 = Maths.vec_cross(dpp,duvec)* b2.d/b2.I;
		dw1 = Maths.vec_cross(dpp,duvec)* b1.d/b1.I;
		dw2max = Maths.vec_proj(b2.w-b1.w,dw2)*0.5f;
		dw1max = Maths.vec_proj(b1.w-b2.w,dw2)*0.5f;
		if( Maths.vec_abs(dw1)>Maths.vec_abs(dw1max) || Maths.vec_abs(dw2)>Maths.vec_abs(dw2max) ){
			/* correct momentum transfer to max */
			dpp *=Maths.vec_abs(dw2max)/Maths.vec_abs(dw2);
			/* correct amg mom transfer to max */
			dw2=dw2max;
			dw1=dw1max;
		}
		//b1.w = b1.w-dw1;
		//b2.w = b2.w-dw2;
		
		/* parallel momentum transfer due to friction between balls */
		dv1 = dpp*-b1.m;
		dv2 = dpp*b2.m;
		dv1.y=0.0f;
		dv2.y=0.0f;
		b1.v -= dv1;
		b2.v -= dv2;

		if(b1.r.y<-b1.d*4.0f)
		{
			b1.v=b2.v=Vector3.zero;
			b1.w=b2.w=Vector3.zero;
			//b1.in_game=b2.in_game=false;
		}
		
	}
	//This function shows if ball is in range of the border so that it could be reflected or not.
	//Return Value : true if ball is in range of the border.
	//				 false if ball is not in range of the border.
	private static bool inrange_advborder( Ball b, Border w )
	{
		Vector3 r,dr,dr1,dr2,dr3,n;
		float d,dra;
		if (w.pnr==3){
			dr1 = w.r2-w.r1;
			dr2 = w.r3-w.r2;
			dr3 = w.r1-w.r3;
			n   = Maths.vec_unit(Maths.vec_cross(dr1,dr2));
			return( Maths.plane_dist( b.r, w.r1, Maths.vec_unit(Maths.vec_cross(n,dr1)) ) >= 0.0f &&
			       Maths.plane_dist( b.r, w.r2, Maths.vec_unit(Maths.vec_cross(n,dr2)) ) >= 0.0f &&
			       Maths.plane_dist( b.r, w.r3, Maths.vec_unit(Maths.vec_cross(n,dr3)) ) >= 0.0f );
		} else if(w.pnr==2){
			r   = b.r-w.r1;
			dr  = w.r2-w.r1;
			dra = Maths.vec_abs(dr);
			d   = Maths.vec_mul(r,dr)/dra;
			return (d>=0.0f && d<dra);
		} else if(w.pnr==1){
			return true;
		}
		return true;
	}

	private static Vector3 perimeter_speed( Ball ball )
	{
		return(Maths.vec_cross(ball.w,new Vector3(0.0f,-ball.d/2.0f,0.0f)));
	}

	private static Vector3 perimeter_speed_normal( Ball ball, Vector3 normal )
	{
		return(Maths.vec_cross(ball.w,normal*-ball.d/2.0f));
	}
	
	private static bool wall_dv_pos( Ball ball, Border border, float dt )
		/* returns 1 if border and ball strobe away from each other, 0 else (at time dt) */
	{
		Vector3 ballpos;
		switch(border.pnr) {
		case 4:
		case 3:
			return( Maths.vec_mul(ball.v,border.n) > 0.0f );
		case 2:
			ballpos = ball.r+ball.v*dt;
			return(Maths.vec_mul(ball.v,Maths.vec_ncomp(ballpos-border.r1,border.r2-border.r1)) > 0.0f);
		case 1:
			ballpos = ball.r+ball.v*dt;
			return( Maths.vec_mul(ballpos-border.r1,ball.v) > 0.0f );
		}
		return true;
	}

	private static bool ball_dv_pos( Ball b1, Ball b2, float dt )
		/* returns 1 if balls strobe away from each other, 0 else (at time dt) */
	{
		Vector3 b1pos, b2pos;
		b1pos = b1.r+b1.v*dt;
		b2pos = b2.r+b2.v*dt;
		return( Maths.vec_mul(b2pos-b1pos,b2.v-b1.v) > 0.0f );
	}

	//It checks if ball is in a hole
	//Return Value : 1 if ball is in a hole; 0 if ball is not in any hole.
	private static int ball_in_hole(Ball ball, int hole_type=Holes.BORDERS_8GAME)
	{
		for(int i=0;i<Holes.GetHoleNum (hole_type);i++)
			if( Maths.vec_abs(Holes.GetHole(hole_type,i).pos-ball.r) < Holes.GetHole(hole_type,i).r*3 )
				return i;
		return -1;
	}
}
