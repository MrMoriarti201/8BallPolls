using UnityEngine;
using System.Collections;

public class Border {

	public int   pnr {get; set;}          // 4=arc 3=triangle 2=line 1=point
	public Vector3 r1 {get; set;}            // pos
	public Vector3 r2 {get; set;}            // pos
	public Vector3 r3 {get; set;}            // pos (tangent vec for arc)
	public Vector3 n {get; set;}             // normal vector
	public float mu {get; set;}           // friction const
	public float loss0 {get; set;}       // const loss per hit (0th order in speed)
	public float loss_max {get; set;}     // max loss
	public float loss_wspeed {get; set;}  // width of higher order loss curve

	//Constructor
	public Border()
	{
	}
	
	// Copy Constructor
	public Border(Border previousBorder){
		SetBorder (previousBorder);
	}
	
	//Instance Constructor.
	//Here a_ means it's an argument
	public Border(int a_pnr, Vector3 a_r1, Vector3 a_r2, Vector3 a_r3, Vector3 a_n, float a_mu, float a_loss0, float a_loss_max, float a_loss_wspeed){
		SetBorder(a_pnr, a_r1, a_r2, a_r3, a_n, a_mu, a_loss0, a_loss_max, a_loss_wspeed);
	}
	
	//SetCue method
	public void SetBorder(Border previousBorder){
		pnr = previousBorder.pnr;
		r1 = previousBorder.r1;
		r2 = previousBorder.r2;
		r3 = previousBorder.r3;
		n = previousBorder.n;
		mu = previousBorder.mu;
		loss0 = previousBorder.loss0;
		loss_max= previousBorder.loss_max;
		loss_wspeed= previousBorder.loss_wspeed;
	}
	
	//SetCue instance method
	public void SetBorder(int a_pnr, Vector3 a_r1, Vector3 a_r2, Vector3 a_r3, Vector3 a_n, float a_mu, float a_loss0, float a_loss_max, float a_loss_wspeed){
		pnr=a_pnr;	r1=a_r1;	r2=a_r2;	r3=a_r3;	n=a_n;	mu=a_mu;	loss0=a_loss0; loss_max=a_loss_max; loss_wspeed=a_loss_wspeed;
	}
	
}
