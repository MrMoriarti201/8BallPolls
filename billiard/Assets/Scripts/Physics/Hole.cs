using UnityEngine;
using System.Collections;

public class Hole {

	public Vector3 pos{get; set;}           // pos
	public Vector3 aim{get; set;}            // position to aim for ai-player
	public float r{get; set;}            // radius of hole (wall-coll free zone)

	//Constructor
	public Hole()
	{
	}
	
	// Copy Constructor
	public Hole(Hole previousHole){
		SetHole (previousHole);
	}
	
	//Instance Constructor.
	//Here a_ means it's an argument
	public Hole(Vector3 a_pos, Vector3 a_aim, float a_r){
		SetHole(a_pos, a_aim, a_r);
	}
	
	//SetCue method
	public void SetHole(Hole previousHole){
		pos = previousHole.pos;
		aim = previousHole.aim;
		r = previousHole.r;
	}
	
	//SetCue instance method
	public void SetHole(Vector3 a_pos, Vector3 a_aim, float a_r){
		pos=a_pos;	aim=a_aim;	r=a_r;
	}
}
