using UnityEngine;
using System.Collections;

public class Holes {
	public const int BORDERS_8GAME=0;
	private static Hole[] holes_8;

	static Holes(){
		InitAllHoles();
	}

	public static void InitAllHoles(){
		Init8GameHoles();
	}
	public static void Init8GameHoles(){
		holes_8=new Hole[32];
		for(int i=0;i<32;i++)
			holes_8[i]=new Hole();
		/* middle right */
		holes_8[0].aim = new Vector3( +Constant.TABLE_L/4.0f-Constant.HOLE2_AIMOFFS, 0.0f, 0.0f );
		holes_8[0].pos = new Vector3( +Constant.TABLE_L/4.0f+Constant.HOLE2_XYOFFS, 0.0f, 0.0f );
		holes_8[0].r   = Constant.HOLE2_R;
		/* middle left */
		holes_8[1].aim = new Vector3( -Constant.TABLE_L/4.0f+Constant.HOLE2_AIMOFFS, 0.0f, 0.0f );
		holes_8[1].pos = new Vector3( -Constant.TABLE_L/4.0f-Constant.HOLE2_XYOFFS, 0.0f, 0.0f );
		holes_8[1].r   = Constant.HOLE2_R;
		/* upper right */
		holes_8[2].aim = new Vector3( +Constant.TABLE_L/4.0f-Constant.HOLE1_AIMOFFS, +Constant.TABLE_L/2.0f-Constant.HOLE1_AIMOFFS, 0.0f );
		holes_8[2].pos = new Vector3( +Constant.TABLE_L/4.0f+Constant.HOLE1_XYOFFS, +Constant.TABLE_L/2.0f+Constant.HOLE1_XYOFFS, 0.0f );
		holes_8[2].r   = Constant.HOLE1_R;
		/* upper left */
		holes_8[3].aim = new Vector3( -Constant.TABLE_L/4.0f+Constant.HOLE1_AIMOFFS, +Constant.TABLE_L/2.0f-Constant.HOLE1_AIMOFFS, 0.0f );
		holes_8[3].pos = new Vector3( -Constant.TABLE_L/4.0f-Constant.HOLE1_XYOFFS, +Constant.TABLE_L/2.0f+Constant.HOLE1_XYOFFS, 0.0f );
		holes_8[3].r   = Constant.HOLE1_R;
		/* lower left */
		holes_8[4].aim = new Vector3( -Constant.TABLE_L/4.0f+Constant.HOLE1_AIMOFFS, -Constant.TABLE_L/2.0f-Constant.HOLE1_AIMOFFS, 0.0f );
		holes_8[4].pos = new Vector3( -Constant.TABLE_L/4.0f-Constant.HOLE1_XYOFFS, -Constant.TABLE_L/2.0f-Constant.HOLE1_XYOFFS, 0.0f );
		holes_8[4].r   = Constant.HOLE1_R;
		/* lower right */
		holes_8[5].aim = new Vector3( +Constant.TABLE_L/4.0f-Constant.HOLE1_AIMOFFS, -Constant.TABLE_L/2.0f+Constant.HOLE1_AIMOFFS, 0.0f );
		holes_8[5].pos = new Vector3( +Constant.TABLE_L/4.0f+Constant.HOLE1_XYOFFS, -Constant.TABLE_L/2.0f-Constant.HOLE1_XYOFFS, 0.0f );
		holes_8[5].r   = Constant.HOLE1_R;

		for(int i=0;i<6;i++)
		{
			holes_8[i].aim=new Vector3(holes_8[i].aim.y,holes_8[i].aim.z,holes_8[i].aim.x);
			holes_8[i].pos=new Vector3(holes_8[i].pos.y,holes_8[i].pos.z,holes_8[i].pos.x);
		}
	}

	public static Hole[] GetHoles(int type){
		switch(type){
		case BORDERS_8GAME:
			return holes_8;
		default:
			break;
		}
		return null;
	}
	
	public static int GetHoleNum(int type){
		switch(type){
		case BORDERS_8GAME:
			return 6;
		default:
			break;
		}
		return 0;
	}
	
	public static Hole GetHole(int type,int index){
		switch(type){
		case BORDERS_8GAME:
			return holes_8[index];
		default:
			break;
		}
		return null;
	}
}