using UnityEngine;
using System.Collections;

public class Borders {

	public const int BORDERS_8GAME=0;
	const float xBorder=0.2f;
	private static Border[] borders_8;

	static Borders(){
		InitAllBorders();
	}

	public static void InitAllBorders(){
		Init8GameBorders();
	}

	public static void Init8GameBorders(){
		borders_8=new Border[50];
		for(int i=0;i<50;i++)
			borders_8[i]=new Border();
		
		/* borders */
		//    walls->nr=30;
		/* bonds */
		borders_8[0].pnr = 3;
		borders_8[0].r1 = new Vector3( +Constant.TABLE_W/2.0f, +Constant.TABLE_L/2.0f-Constant.HOLE1_W/1.3f, -Constant.BALL_D/2.0f );
		borders_8[0].r2 = new Vector3( +Constant.TABLE_W/2.0f,              +Constant.HOLE1_W/1.75f, 0.0f );
		borders_8[0].r3 = new Vector3( +Constant.TABLE_W/2.0f, +Constant.TABLE_L/2.0f-Constant.HOLE1_W/1.3f, Constant.BALL_D/2.0f );
		borders_8[0].n  = new Vector3( -1.0f, 0.0f, 0.0f );

		borders_8[1].pnr = 3;
		borders_8[1].r1 = new Vector3( +Constant.TABLE_W/2.0f, -Constant.TABLE_L/2.0f+Constant.HOLE1_W/1.3f, -Constant.BALL_D/2.0f );
		borders_8[1].r2 = new Vector3( +Constant.TABLE_W/2.0f,              -Constant.HOLE1_W/1.75f , 0.0f );
		borders_8[1].r3 = new Vector3( +Constant.TABLE_W/2.0f, -Constant.TABLE_L/2.0f+Constant.HOLE1_W/1.3f, Constant.BALL_D/2.0f );
		borders_8[1].n  = new Vector3( -1.0f, 0.0f, 0.0f );
		
		borders_8[2].pnr = 3;
		borders_8[2].r1 = new Vector3( -Constant.TABLE_W/2.0f, +Constant.TABLE_L/2.0f-Constant.HOLE1_W/1.3f, -Constant.BALL_D/2.0f );
		borders_8[2].r2 = new Vector3( -Constant.TABLE_W/2.0f,              +Constant.HOLE1_W/1.75f, 0.0f );
		borders_8[2].r3 = new Vector3( -Constant.TABLE_W/2.0f, +Constant.TABLE_L/2.0f-Constant.HOLE1_W/1.3f, Constant.BALL_D/2.0f );
		borders_8[2].n  = new Vector3( +1.0f, 0.0f, 0.0f );
		
		borders_8[3].pnr = 3;
		borders_8[3].r1 = new Vector3( -Constant.TABLE_W/2.0f, -Constant.TABLE_L/2.0f+Constant.HOLE1_W/1.3f, -Constant.BALL_D/2.0f );
		borders_8[3].r2 = new Vector3( -Constant.TABLE_W/2.0f,              -Constant.HOLE1_W/1.75f, 0.0f );
		borders_8[3].r3 = new Vector3( -Constant.TABLE_W/2.0f, -Constant.TABLE_L/2.0f+Constant.HOLE1_W/1.3f, Constant.BALL_D/2.0f );
		borders_8[3].n  = new Vector3( +1.0f, 0.0f, 0.0f );
		
		borders_8[4].pnr = 3;
		borders_8[4].r1 = new Vector3( -Constant.TABLE_W/2.0f+Constant.HOLE1_W/1.3f-xBorder, +Constant.TABLE_L/2.0f, -Constant.BALL_D/2.0f );
		borders_8[4].r2 = new Vector3( +Constant.TABLE_W/2.0f-Constant.HOLE1_W/1.3f+xBorder, +Constant.TABLE_L/2.0f, 0.0f );
		borders_8[4].r3 = new Vector3( -Constant.TABLE_W/2.0f+Constant.HOLE1_W/1.3f-xBorder, +Constant.TABLE_L/2.0f, Constant.BALL_D/2.0f );
		borders_8[4].n  = new Vector3( 0.0f, -1.0f, 0.0f );
		
		borders_8[5].pnr = 3;
		borders_8[5].r1 = new Vector3( -Constant.TABLE_W/2.0f+Constant.HOLE1_W/1.3f-xBorder, -Constant.TABLE_L/2.0f, -Constant.BALL_D/2.0f );
		borders_8[5].r2 = new Vector3( +Constant.TABLE_W/2.0f-Constant.HOLE1_W/1.3f+xBorder, -Constant.TABLE_L/2.0f, 0.0f );
		borders_8[5].r3 = new Vector3( -Constant.TABLE_W/2.0f+Constant.HOLE1_W/1.3f-xBorder, -Constant.TABLE_L/2.0f, Constant.BALL_D/2.0f );
		borders_8[5].n  = new Vector3( 0.0f, +1.0f, 0.0f );
		
		/* edges */
		/* upper right */
		borders_8[6].pnr = 2;
		borders_8[6].r1 = new Vector3( +Constant.TABLE_W/2.0f, +Constant.TABLE_L/2.0f-Constant.HOLE1_W/1.3f, -Constant.BALL_D/2.0f );
		borders_8[6].r2 = new Vector3( +Constant.TABLE_W/2.0f, +Constant.TABLE_L/2.0f-Constant.HOLE1_W/1.3f, Constant.BALL_D/2.0f );
		borders_8[18].pnr = 3;
		borders_8[18].r1 = new Vector3( +Constant.TABLE_W/2.0f, +Constant.TABLE_L/2.0f-Constant.HOLE1_W/1.3f, -Constant.BALL_D/2.0f );
		borders_8[18].r2 = new Vector3( +Constant.TABLE_W/2.0f, +Constant.TABLE_L/2.0f-Constant.HOLE1_W/1.3f, Constant.BALL_D/2.0f );
		borders_8[18].r3 = new Vector3( +Constant.TABLE_W/2.0f+Constant.HOLE1_TAN/1.2f, +Constant.TABLE_L/2.0f-Constant.HOLE1_W/1.3f+Constant.HOLE1_TAN, 0.0f );
		borders_8[18].n  = Maths.vec_unit(Maths.vec_cross(borders_8[18].r2-borders_8[18].r1,borders_8[18].r3-borders_8[18].r1));
		
		/* upper right */
		borders_8[7].pnr = 2;
		borders_8[7].r1 = new Vector3( +Constant.TABLE_W/2.0f-Constant.HOLE1_W/1.3f+xBorder, +Constant.TABLE_L/2.0f, -Constant.BALL_D/2.0f );
		borders_8[7].r2 = new Vector3( +Constant.TABLE_W/2.0f-Constant.HOLE1_W/1.3f+xBorder, +Constant.TABLE_L/2.0f, Constant.BALL_D/2.0f );
		borders_8[19].pnr = 3;
		borders_8[19].r1 = new Vector3( +Constant.TABLE_W/2.0f-Constant.HOLE1_W/1.3f+xBorder, +Constant.TABLE_L/2.0f, -Constant.BALL_D/2.0f );
		borders_8[19].r2 = new Vector3( +Constant.TABLE_W/2.0f-Constant.HOLE1_W/1.3f+xBorder, +Constant.TABLE_L/2.0f, Constant.BALL_D/2.0f );
		borders_8[19].r3 = new Vector3( +Constant.TABLE_W/2.0f-Constant.HOLE1_W/1.3f+xBorder+Constant.HOLE1_TAN, +Constant.TABLE_L/2.0f+Constant.HOLE1_TAN/1.2f, 0.0f );
		borders_8[19].n  = Maths.vec_unit(Maths.vec_cross(borders_8[19].r1-borders_8[19].r2,borders_8[19].r3-borders_8[19].r1));
		
		/* upper left */
		borders_8[8].pnr = 2;
		borders_8[8].r1 = new Vector3( -Constant.TABLE_W/2.0f, +Constant.TABLE_L/2.0f-Constant.HOLE1_W/1.3f, -Constant.BALL_D/2.0f );
		borders_8[8].r2 = new Vector3( -Constant.TABLE_W/2.0f, +Constant.TABLE_L/2.0f-Constant.HOLE1_W/1.3f, Constant.BALL_D/2.0f );
		borders_8[20].pnr = 3;
		borders_8[20].r1 = new Vector3( -Constant.TABLE_W/2.0f, +Constant.TABLE_L/2.0f-Constant.HOLE1_W/1.3f, -Constant.BALL_D/2.0f );
		borders_8[20].r2 = new Vector3( -Constant.TABLE_W/2.0f, +Constant.TABLE_L/2.0f-Constant.HOLE1_W/1.3f, Constant.BALL_D/2.0f );
		borders_8[20].r3 = new Vector3( -Constant.TABLE_W/2.0f-Constant.HOLE1_TAN/1.2f, +Constant.TABLE_L/2.0f-Constant.HOLE1_W/1.3f+Constant.HOLE1_TAN, 0.0f );
		borders_8[20].n  = Maths.vec_unit(Maths.vec_cross(borders_8[20].r1-borders_8[20].r2,borders_8[20].r3-borders_8[20].r1));
		
		/* upper left */
		borders_8[9].pnr = 2;
		borders_8[9].r1 = new Vector3( -Constant.TABLE_W/2.0f+Constant.HOLE1_W/1.3f-xBorder-xBorder, +Constant.TABLE_L/2.0f, -Constant.BALL_D/2.0f );
		borders_8[9].r2 = new Vector3( -Constant.TABLE_W/2.0f+Constant.HOLE1_W/1.3f-xBorder, +Constant.TABLE_L/2.0f, Constant.BALL_D/2.0f );
		borders_8[21].pnr = 3;
		borders_8[21].r1 = new Vector3( -Constant.TABLE_W/2.0f+Constant.HOLE1_W/1.3f-xBorder, +Constant.TABLE_L/2.0f, -Constant.BALL_D/2.0f );
		borders_8[21].r2 = new Vector3( -Constant.TABLE_W/2.0f+Constant.HOLE1_W/1.3f-xBorder, +Constant.TABLE_L/2.0f, Constant.BALL_D/2.0f );
		borders_8[21].r3 = new Vector3( -Constant.TABLE_W/2.0f+Constant.HOLE1_W/1.3f-xBorder-Constant.HOLE1_TAN, +Constant.TABLE_L/2.0f+Constant.HOLE1_TAN/1.2f, 0.0f );
		borders_8[21].n  = Maths.vec_unit(Maths.vec_cross(borders_8[21].r2-borders_8[21].r1,borders_8[21].r3-borders_8[21].r1));
		
		/* lower right */
		borders_8[10].pnr = 2;
		borders_8[10].r1 = new Vector3( +Constant.TABLE_W/2.0f, -Constant.TABLE_L/2.0f+Constant.HOLE1_W/1.3f, -Constant.BALL_D/2.0f );
		borders_8[10].r2 = new Vector3( +Constant.TABLE_W/2.0f, -Constant.TABLE_L/2.0f+Constant.HOLE1_W/1.3f, Constant.BALL_D/2.0f );
		borders_8[22].pnr = 3;
		borders_8[22].r1 = new Vector3( +Constant.TABLE_W/2.0f, -Constant.TABLE_L/2.0f+Constant.HOLE1_W/1.3f, -Constant.BALL_D/2.0f );
		borders_8[22].r2 = new Vector3( +Constant.TABLE_W/2.0f, -Constant.TABLE_L/2.0f+Constant.HOLE1_W/1.3f, Constant.BALL_D/2.0f );
		borders_8[22].r3 = new Vector3( +Constant.TABLE_W/2.0f+Constant.HOLE1_TAN/1.2f, -Constant.TABLE_L/2.0f+Constant.HOLE1_W/1.3f-Constant.HOLE1_TAN, 0.0f );
		borders_8[22].n  = Maths.vec_unit(Maths.vec_cross(borders_8[22].r1-borders_8[22].r2,borders_8[22].r3-borders_8[22].r1));
		
		/* lower right */
		borders_8[11].pnr = 2;
		borders_8[11].r1 = new Vector3( +Constant.TABLE_W/2.0f-Constant.HOLE1_W/1.3f+xBorder, -Constant.TABLE_L/2.0f, -Constant.BALL_D/2.0f );
		borders_8[11].r2 = new Vector3( +Constant.TABLE_W/2.0f-Constant.HOLE1_W/1.3f+xBorder, -Constant.TABLE_L/2.0f, Constant.BALL_D/2.0f );
		borders_8[23].pnr = 3;
		borders_8[23].r1 = new Vector3( +Constant.TABLE_W/2.0f-Constant.HOLE1_W/1.3f+xBorder, -Constant.TABLE_L/2.0f, -Constant.BALL_D/2.0f );
		borders_8[23].r2 = new Vector3( +Constant.TABLE_W/2.0f-Constant.HOLE1_W/1.3f+xBorder, -Constant.TABLE_L/2.0f, Constant.BALL_D/2.0f );
		borders_8[23].r3 = new Vector3( +Constant.TABLE_W/2.0f-Constant.HOLE1_W/1.3f+xBorder+Constant.HOLE1_TAN, -Constant.TABLE_L/2.0f-Constant.HOLE1_TAN/1.2f, 0.0f );
		borders_8[23].n  = Maths.vec_unit(Maths.vec_cross(borders_8[23].r2-borders_8[23].r1,borders_8[23].r3-borders_8[23].r1));
		
		/* lower left */
		borders_8[12].pnr = 2;
		borders_8[12].r1 = new Vector3( -Constant.TABLE_W/2.0f, -Constant.TABLE_L/2.0f+Constant.HOLE1_W/1.3f, -Constant.BALL_D/2.0f );
		borders_8[12].r2 = new Vector3( -Constant.TABLE_W/2.0f, -Constant.TABLE_L/2.0f+Constant.HOLE1_W/1.3f, Constant.BALL_D/2.0f );
		borders_8[24].pnr = 3;
		borders_8[24].r1 = new Vector3( -Constant.TABLE_W/2.0f, -Constant.TABLE_L/2.0f+Constant.HOLE1_W/1.3f, -Constant.BALL_D/2.0f );
		borders_8[24].r2 = new Vector3( -Constant.TABLE_W/2.0f, -Constant.TABLE_L/2.0f+Constant.HOLE1_W/1.3f, Constant.BALL_D/2.0f );
		borders_8[24].r3 = new Vector3( -Constant.TABLE_W/2.0f-Constant.HOLE1_TAN/1.2f, -Constant.TABLE_L/2.0f+Constant.HOLE1_W/1.3f-Constant.HOLE1_TAN, 0.0f );
		borders_8[24].n  = Maths.vec_unit(Maths.vec_cross(borders_8[24].r2-borders_8[24].r1,borders_8[24].r3-borders_8[24].r1));
		
		/* lower left */
		borders_8[13].pnr = 2;
		borders_8[13].r1 = new Vector3( -Constant.TABLE_W/2.0f+Constant.HOLE1_W/1.3f-xBorder, -Constant.TABLE_L/2.0f, -Constant.BALL_D/2.0f );
		borders_8[13].r2 = new Vector3( -Constant.TABLE_W/2.0f+Constant.HOLE1_W/1.3f-xBorder, -Constant.TABLE_L/2.0f, Constant.BALL_D/2.0f );
		borders_8[25].pnr = 3;
		borders_8[25].r1 = new Vector3( -Constant.TABLE_W/2.0f+Constant.HOLE1_W/1.3f-xBorder, -Constant.TABLE_L/2.0f, -Constant.BALL_D/2.0f );
		borders_8[25].r2 = new Vector3( -Constant.TABLE_W/2.0f+Constant.HOLE1_W/1.3f-xBorder, -Constant.TABLE_L/2.0f, Constant.BALL_D/2.0f );
		borders_8[25].r3 = new Vector3( -Constant.TABLE_W/2.0f+Constant.HOLE1_W/1.3f-xBorder-Constant.HOLE1_TAN, -Constant.TABLE_L/2.0f-Constant.HOLE1_TAN/1.2f, 0.0f );
		borders_8[25].n  = Maths.vec_unit(Maths.vec_cross(borders_8[25].r1-borders_8[25].r2,borders_8[25].r3-borders_8[25].r1));
		
		/* middle left */
		borders_8[14].pnr = 2;
		borders_8[14].r1 = new Vector3( -Constant.TABLE_W/2.0f, Constant.HOLE1_W/1.75f, -Constant.BALL_D/2.0f );
		borders_8[14].r2 = new Vector3( -Constant.TABLE_W/2.0f, Constant.HOLE1_W/1.75f, Constant.BALL_D/2.0f );
		borders_8[26].pnr = 3;
		borders_8[26].r1 = new Vector3( -Constant.TABLE_W/2.0f, Constant.HOLE1_W/1.75f, -Constant.BALL_D/2.0f );
		borders_8[26].r2 = new Vector3( -Constant.TABLE_W/2.0f, Constant.HOLE1_W/1.75f, Constant.BALL_D/2.0f );
		borders_8[26].r3 = new Vector3( -Constant.TABLE_W/2.0f-Constant.HOLE1_TAN/1.2f, Constant.HOLE1_W/1.75f-Constant.HOLE2_TAN, 0.0f );
		borders_8[26].n  = Maths.vec_unit(Maths.vec_cross(borders_8[26].r2-borders_8[26].r1,borders_8[26].r3-borders_8[26].r1));
		
		/* middle left */
		borders_8[15].pnr = 2;
		borders_8[15].r1 = new Vector3( -Constant.TABLE_W/2.0f, -Constant.HOLE1_W/1.75f, -Constant.BALL_D/2.0f );
		borders_8[15].r2 = new Vector3( -Constant.TABLE_W/2.0f, -Constant.HOLE1_W/1.75f, Constant.BALL_D/2.0f );
		borders_8[27].pnr = 3;
		borders_8[27].r1 = new Vector3( -Constant.TABLE_W/2.0f, -Constant.HOLE1_W/1.75f, -Constant.BALL_D/2.0f );
		borders_8[27].r2 = new Vector3( -Constant.TABLE_W/2.0f, -Constant.HOLE1_W/1.75f, Constant.BALL_D/2.0f );
		borders_8[27].r3 = new Vector3( -Constant.TABLE_W/2.0f-Constant.HOLE1_TAN/1.2f, -Constant.HOLE1_W/1.75f+Constant.HOLE2_TAN, 0.0f );
		borders_8[27].n  = Maths.vec_unit(Maths.vec_cross(borders_8[27].r1-borders_8[27].r2,borders_8[27].r3-borders_8[27].r1));
		
		/* middle right */
		borders_8[16].pnr = 2;
		borders_8[16].r1 = new Vector3( +Constant.TABLE_W/2.0f, Constant.HOLE1_W/1.75f, -Constant.BALL_D/2.0f );
		borders_8[16].r2 = new Vector3( +Constant.TABLE_W/2.0f, Constant.HOLE1_W/1.75f, Constant.BALL_D/2.0f );
		borders_8[28].pnr = 3;
		borders_8[28].r1 = new Vector3( +Constant.TABLE_W/2.0f, Constant.HOLE1_W/1.75f, -Constant.BALL_D/2.0f );
		borders_8[28].r2 = new Vector3( +Constant.TABLE_W/2.0f, Constant.HOLE1_W/1.75f, Constant.BALL_D/2.0f );
		borders_8[28].r3 = new Vector3( +Constant.TABLE_W/2.0f+Constant.HOLE1_TAN/1.2f, Constant.HOLE1_W/1.75f-Constant.HOLE2_TAN, 0.0f );
		borders_8[28].n  = Maths.vec_unit(Maths.vec_cross(borders_8[28].r1-borders_8[28].r2,borders_8[28].r3-borders_8[28].r1));
		
		/* middle right */
		borders_8[17].pnr = 2;
		borders_8[17].r1 = new Vector3( +Constant.TABLE_W/2.0f, -Constant.HOLE1_W/1.75f, -Constant.BALL_D/2.0f );
		borders_8[17].r2 = new Vector3( +Constant.TABLE_W/2.0f, -Constant.HOLE1_W/1.75f, Constant.BALL_D/2.0f );
		borders_8[29].pnr = 3;
		borders_8[29].r1 = new Vector3( +Constant.TABLE_W/2.0f, -Constant.HOLE1_W/1.75f, -Constant.BALL_D/2.0f );
		borders_8[29].r2 = new Vector3( +Constant.TABLE_W/2.0f, -Constant.HOLE1_W/1.75f, Constant.BALL_D/2.0f );
		borders_8[29].r3 = new Vector3( +Constant.TABLE_W/2.0f+Constant.HOLE1_TAN/1.2f, -Constant.HOLE1_W/1.75f+Constant.HOLE2_TAN, 0.0f );
		borders_8[29].n  = Maths.vec_unit(Maths.vec_cross(borders_8[29].r2-borders_8[29].r1,borders_8[29].r3-borders_8[29].r1));
		
		/* friction constants and loss factors */
		for(int i=0;i<32;i++){
			borders_8[i].mu          = 0.1f;
			borders_8[i].loss0       = 0.2f;
			borders_8[i].loss_max    = 0.5f;
			borders_8[i].loss_wspeed = 4.0f;  /* [m/s] */
		}
		
		/* table surface */
		//#define TABLE_W2 (TABLE_W-BALL_D*0.9)
		//#define TABLE_L2 (TABLE_L-BALL_D*0.9)

		//#undef TABLE_W2
		//#undef TABLE_L2

		borders_8[32].pnr = 4;
		borders_8[32].r1 = new Vector3( +Constant.TABLE_W/2.0f+Constant.TABLE_FRAME_H, +Constant.TABLE_L/2.0f+Constant.TABLE_FRAME_W, -Constant.TABLE_WALL_HEIGHT);
		borders_8[32].r2 = new Vector3( +Constant.TABLE_W/2.0f+Constant.TABLE_FRAME_H, -Constant.TABLE_L/2.0f-Constant.TABLE_FRAME_W, 0.0f );
		borders_8[32].r3 = new Vector3( +Constant.TABLE_W/2.0f+Constant.TABLE_FRAME_H, +Constant.TABLE_L/2.0f+Constant.TABLE_FRAME_W, Constant.TABLE_WALL_HEIGHT);
		borders_8[32].n  = new Vector3( -1.0f, 0.0f, 0.0f );

		borders_8[33].pnr = 4;
		borders_8[33].r1 = new Vector3( -Constant.TABLE_W/2.0f-Constant.TABLE_FRAME_H, +Constant.TABLE_L/2.0f+Constant.TABLE_FRAME_W, -Constant.TABLE_WALL_HEIGHT);
		borders_8[33].r2 = new Vector3( -Constant.TABLE_W/2.0f-Constant.TABLE_FRAME_H, -Constant.TABLE_L/2.0f-Constant.TABLE_FRAME_W, 0.0f );
		borders_8[33].r3 = new Vector3( -Constant.TABLE_W/2.0f-Constant.TABLE_FRAME_H, +Constant.TABLE_L/2.0f+Constant.TABLE_FRAME_W, Constant.TABLE_WALL_HEIGHT);
		borders_8[33].n  = new Vector3( 1.0f, 0.0f, 0.0f );

		borders_8[30].pnr = 4;
		borders_8[30].r1 = new Vector3( +Constant.TABLE_W/2.0f+Constant.TABLE_FRAME_H, +Constant.TABLE_L/2.0f+Constant.TABLE_FRAME_W, -Constant.TABLE_WALL_HEIGHT);
		borders_8[30].r2 = new Vector3( -Constant.TABLE_W/2.0f-Constant.TABLE_FRAME_H, +Constant.TABLE_L/2.0f+Constant.TABLE_FRAME_W, 0.0f );
		borders_8[30].r3 = new Vector3( +Constant.TABLE_W/2.0f+Constant.TABLE_FRAME_H, +Constant.TABLE_L/2.0f+Constant.TABLE_FRAME_W, Constant.TABLE_WALL_HEIGHT);
		borders_8[30].n  = new Vector3( 0.0f, -1.0f, 0.0f );

		borders_8[31].pnr = 4;
		borders_8[31].r1 = new Vector3( +Constant.TABLE_W/2.0f+Constant.TABLE_FRAME_H, -Constant.TABLE_L/2.0f-Constant.TABLE_FRAME_W, -Constant.TABLE_WALL_HEIGHT);
		borders_8[31].r2 = new Vector3( -Constant.TABLE_W/2.0f-Constant.TABLE_FRAME_H, -Constant.TABLE_L/2.0f-Constant.TABLE_FRAME_W, 0.0f );
		borders_8[31].r3 = new Vector3( +Constant.TABLE_W/2.0f+Constant.TABLE_FRAME_H, -Constant.TABLE_L/2.0f-Constant.TABLE_FRAME_W, Constant.TABLE_WALL_HEIGHT);
		borders_8[31].n  = new Vector3( 0.0f, 1.0f, 0.0f );

		/*borders_8[36].pnr = 4;
		borders_8[36].r1 = new Vector3( +Constant.TABLE_W/2.0f+Constant.TABLE_FRAME_H, +Constant.TABLE_L/2.0f+Constant.TABLE_FRAME_W, -Constant.TABLE_WALL_HEIGHT);
		borders_8[36].r2 = new Vector3( -Constant.TABLE_W/2.0f-Constant.TABLE_FRAME_H, +Constant.TABLE_L/2.0f+Constant.TABLE_FRAME_W, 0.0f );
		borders_8[36].r3 = new Vector3( +Constant.TABLE_W/2.0f+Constant.TABLE_FRAME_H, +Constant.TABLE_L/2.0f+Constant.TABLE_FRAME_W, Constant.TABLE_WALL_HEIGHT);
		borders_8[36].n  = new Vector3( 0.0f, -1.0f, 0.0f );*/

	/*	borders_8[36].pnr = 3;
		borders_8[36].r1 = new Vector3( (-Constant.TABLE_W/2.0f-Constant.TABLE_FRAME_H)*10.0f, (-Constant.TABLE_L/2.0f-Constant.TABLE_FRAME_W)*10.0f, -Constant.BALL_D*3.0f);
		borders_8[36].r2 = new Vector3( 0, (+Constant.TABLE_L/2.0f+Constant.TABLE_FRAME_W)*10.0f, -Constant.BALL_D*3.0f );
		borders_8[36].r3 = new Vector3( (+Constant.TABLE_W/2.0f+Constant.TABLE_FRAME_H)*10.0f, (-Constant.TABLE_L/2.0f-Constant.TABLE_FRAME_W)*10.0f, -Constant.BALL_D*3.0f);
		borders_8[36].n  = new Vector3( 0.0f, 0.0f, 1.0f );*/

	/*	borders_8[36].pnr = 3;
		borders_8[36].r1 = new Vector3( +Constant.TABLE_W/2.0f+Constant.TABLE_FRAME_H, -Constant.TABLE_L/2.0f-Constant.TABLE_FRAME_W, -Constant.BALL_D*3.0f);
		borders_8[36].r2 = new Vector3( -Constant.TABLE_W/2.0f-Constant.TABLE_FRAME_H, -Constant.TABLE_L/2.0f-Constant.TABLE_FRAME_H, -Constant.BALL_D*3.0f );
		borders_8[36].r3 = new Vector3( +Constant.TABLE_W/2.0f+Constant.TABLE_FRAME_H, -Constant.TABLE_L/2.0f-Constant.TABLE_FRAME_H, -Constant.BALL_D*3.0f);
		borders_8[36].n  = new Vector3( 0.0f, 0.0f, 1.0f );*/

		/*borders_8[33].pnr = 3;
		borders_8[33].r1 = new Vector3( +Constant.TABLE_W/2.0f-Constant.TABLE_FRAME_W, +Constant.TABLE_L/2.0f-Constant.HOLE1_W/1.3f, -Constant.BALL_D/2.0f );
		borders_8[33].r2 = new Vector3( +Constant.TABLE_W/2.0f-Constant.TABLE_FRAME_W,              +Constant.HOLE1_W/1.75f, 0.0f );
		borders_8[33].r3 = new Vector3( +Constant.TABLE_W/2.0f-Constant.TABLE_FRAME_W, +Constant.TABLE_L/2.0f-Constant.HOLE1_W/1.3f, Constant.BALL_D/2.0f );
		borders_8[33].n  = new Vector3( -1.0f, 0.0f, 0.0f );

		borders_8[34].pnr = 3;
		borders_8[34].r1 = new Vector3( +Constant.TABLE_W/2.0f+Constant.TABLE_FRAME_W, +Constant.TABLE_L/2.0f-Constant.HOLE1_W/1.3f, -Constant.BALL_D/2.0f );
		borders_8[34].r2 = new Vector3( +Constant.TABLE_W/2.0f+Constant.TABLE_FRAME_W,              +Constant.HOLE1_W/1.75f, 0.0f );
		borders_8[34].r3 = new Vector3( +Constant.TABLE_W/2.0f+Constant.TABLE_FRAME_W, +Constant.TABLE_L/2.0f-Constant.HOLE1_W/1.3f, Constant.BALL_D/2.0f );
		borders_8[34].n  = new Vector3( -1.0f, 0.0f, 0.0f );

		borders_8[35].pnr = 3;
		borders_8[35].r1 = new Vector3( +Constant.TABLE_W/2.0f+Constant.TABLE_FRAME_W, +Constant.TABLE_L/2.0f-Constant.HOLE1_W/1.3f, -Constant.BALL_D/2.0f );
		borders_8[35].r2 = new Vector3( +Constant.TABLE_W/2.0f+Constant.TABLE_FRAME_W,              +Constant.HOLE1_W/1.75f, 0.0f );
		borders_8[35].r3 = new Vector3( +Constant.TABLE_W/2.0f+Constant.TABLE_FRAME_W, +Constant.TABLE_L/2.0f-Constant.HOLE1_W/1.3f, Constant.BALL_D/2.0f );
		borders_8[35].n  = new Vector3( -1.0f, 0.0f, 0.0f );*/

		for(int i=0;i<34;i++)
		{
			borders_8[i].r1=new Vector3(borders_8[i].r1.y,borders_8[i].r1.z,borders_8[i].r1.x);
			borders_8[i].r2=new Vector3(borders_8[i].r2.y,borders_8[i].r2.z,borders_8[i].r2.x);
			borders_8[i].r3=new Vector3(borders_8[i].r3.y,borders_8[i].r3.z,borders_8[i].r3.x);
			borders_8[i].n=new Vector3(borders_8[i].n.y,borders_8[i].n.z,borders_8[i].n.x);
		}

	}

	public static Border[] GetBorders(int type){
		switch(type){
		case BORDERS_8GAME:
			return borders_8;
		default:
			break;
		}
		return null;
	}

	public static int GetBorderNum(int type){
		switch(type){
		case BORDERS_8GAME:
			return 34;
		default:
			break;
		}
		return 0;
	}

	public static Border GetBorder(int type,int index){
		switch(type){
		case BORDERS_8GAME:
			return borders_8[index];
		default:
			break;
		}
		return null;
	}

}
