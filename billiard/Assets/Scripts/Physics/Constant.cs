using UnityEngine;
using System.Collections;

public class Constant{

//	public const float options_table_size=6.7f*2.54f*12.0f/100.0f;
	public const float options_table_size=6.7f*2.54f*12.0f/100.0f;
	public const float rateWithOpenGL=16.9f;
	public const float TABLE_W= (options_table_size/2.0f)*rateWithOpenGL;
	public const float TABLE_L= (options_table_size)*rateWithOpenGL;
	public const float TABLE_H= 0.84f*rateWithOpenGL;      /* 84 cm */

	public const float TABLE_FRAME_W = 0.1f * rateWithOpenGL;      /* 84 cm */
	public const float TABLE_FRAME_H = 0.1f * rateWithOpenGL;      /* 84 cm */

	public const float BALL_M = 0.07f*rateWithOpenGL;   /* 170 g */
	public const float BALL_D = 62.15e-3f*rateWithOpenGL;  /* 57.15 mm */
	public const float HOLE1_R= (0.100f/2.0f)*rateWithOpenGL;  /* d=110 mm */
//	public const float HOLE1_R= (0.110f/2.0f)*rateWithOpenGL;  /* d=110 mm */
	public const float HOLE2_R= (0.100f/2.0f)*rateWithOpenGL;  /* d=115 mm */
	public const float QUEUE_L= 1.4f*rateWithOpenGL;     /* 1.4 m */
	public const float QUEUE_D1=0.035f*rateWithOpenGL;   /* 3.5cm */
	public const float QUEUE_D2=0.010f*rateWithOpenGL;   /* 1.0cm */
	public const float BANDE_D =0.035f*rateWithOpenGL;  /* 3.5cm to be conform with normed opening of middle pockets */
	public const float BANDE_D2RATIO = 0.5f*rateWithOpenGL;  /* (1-0.3)*BANDE_D */
	public const float BANDE_D2=(BANDE_D*(1.0f-0.3f));  /* (1-0.3)*BANDE_D */
	public const float HOLE1_W =(2.0f*HOLE1_R);  /* */
	public const float HOLE2_W =HOLE2_R*2.0f;  /* */

	public const float FRAME_D =(2.0f*HOLE2_R+0.05f*rateWithOpenGL);   /* d_hole+5cm */
	public const float FRAME_H = 0.09f*rateWithOpenGL;   /*  9cm */
	public const float FRAME_H2= 0.16f*rateWithOpenGL;   /* 16cm */
	public const float FRAME_DH=0.010f*rateWithOpenGL;          /* +7mm */
	public const float FRAME_PHASE=0.01f*rateWithOpenGL;       /* 1cm */
	public const float FRAME_DW=   0.017f*rateWithOpenGL;       /* 1.7cm */
	public const float WOOD_D  =(FRAME_D-BANDE_D);  /* */
	public const float HOLE1_TAN= 1/1.4142f; /* cotan(35�) */
	public const float HOLE2_TAN= 0.10397f; /* tan(20�) */
	public const float HOLE1_XYOFFS=(0.04f/1.4142f)*rateWithOpenGL;  /* */
	public const float HOLE2_XYOFFS=(HOLE2_R+0.005f*rateWithOpenGL);  /* */
	public const float HOLE1_AIMOFFS=0.02f*rateWithOpenGL;  /* */
	public const float HOLE2_AIMOFFS=0.02f*rateWithOpenGL;  /* */
	public const float HOLE1_PHASE=  0.005f*rateWithOpenGL;  /* */
	public const float HOLE2_PHASE=  0.005f*rateWithOpenGL;  /* */
	public const float BALL_FULL=1;
	public const float BALL_HALF=2;
	public const float BALL_ANY =0;

	public const float CUE_ROT_MIN=0.1f; //0.01
	public const float CUE_POS_MIN=0.1f;

	public const float TABLE_WALL_HEIGHT=10000.0f;

	public const float POCKET_BALL_X=-TABLE_L/7.0f;
	public const float POCKET_BALL_Y=-BALL_D*5.0f;
	public const float POCKET_BALL_Z=-TABLE_W/2.0f-TABLE_FRAME_H+BALL_D/2.0f;
	public const float POCKET_CURVE_RADIUS=-TABLE_L/14.0f;

	public const int FRAMERATE=50;

	public const float play_time=25;
	public const float play_time_blink=10;

	public static Vector3 ballEffectPos=new Vector3(-0.1654353f,-0.02624985f,0.5620149f);
//	public static Quaternion ballEffectAng=new Vector3(-2.575928f,165.2178f,-179.6658f);
}
