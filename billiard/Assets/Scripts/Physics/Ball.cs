/*Cue

 This is a structure of a cue.
 Here are properties of the cue like mass, diameter, position, speed, ...
 */

using UnityEngine;
using System.Collections;

public class Ball {
	
	public float m { get; set;}		//mass
	public float I { get; set;}		//massentraegheitsmom
	public float d { get; set;}		//diameter
	public Vector3 r { get; set;}	//position
	public Vector3 v { get; set;}	//speed
	public Vector3 w { get; set;}	//rotation speed and axe
	public Vector3 rp {get;set;}	//rotation position
	
	public int nr { get; set;}		//0=white, ...
	public bool in_game {get; set;}			//ball in game
	private int in_fov;				//ball in field of view;
	public int in_hole{get; set;}			//ball still in game but already in hole
	private Vector3 [] path;			//path of ball
	private int pathsize;			//number of reserved path points
	private int pathcnt;			//counter of path points
	private int soundplayed;		//Sound only once for in_hole;
	
	//Constructor
	public Ball()
	{
	}
	
	// Copy Constructor
	public Ball(Ball previousBall){
		SetBall (previousBall);
	}
	
	//Instance Constructor.
	//Here a_ means it's an argument
	public Ball(int a_nr, float a_m, float a_I, float a_d, Vector3 a_r, Vector3 a_v, Vector3 a_w, Vector3 a_rp){
		SetBall(a_nr, a_m, a_I, a_d, a_r, a_v, a_w, a_rp);
	}
	
	//SetCue method
	public void SetBall(Ball previousBall){
		nr = previousBall.nr;
		m = previousBall.m;
		I = previousBall.I;
		d = previousBall.d;
		r = previousBall.r;
		v = previousBall.v;
		w = previousBall.w;
		rp= previousBall.rp;
		in_game=previousBall.in_game;
		in_hole=previousBall.in_hole;
	}
	
	//SetCue instance method
	public void SetBall(int a_nr, float a_m, float a_I, float a_d, Vector3 a_r, Vector3 a_v, Vector3 a_w, Vector3 a_rp){
		nr=a_nr;	m=a_m;	I=a_I;	d=a_d;	r=a_r;	v=a_v;	w=a_w; rp=a_rp;
	}

	public bool isEqualTo(Ball ball)
	{
		if(r==ball.r && v==ball.v && rp==ball.rp && w==ball.w)
			return true;
		return false;
	}
}