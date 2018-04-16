using UnityEngine;
using System.Collections;

public class Maths {

	public static float vec_abs(Vector3 v){
		return( Mathf.Sqrt( v.x*v.x + v.y*v.y + v.z*v.z ));
	}

	public static float vec_abssq(Vector3 v )
	{
		return( v.x*v.x + v.y*v.y + v.z*v.z );
	}


	public static float vec_mul( Vector3 v1, Vector3 v2 )
	{
		return v1.x*v2.x + v1.y*v2.y + v1.z*v2.z;
	}

	public static Vector3 vec_proj( Vector3 v1, Vector3 v2 )
	{
		float v2ls;
		v2ls = v2.x*v2.x + v2.y*v2.y + v2.z*v2.z;
		if( v2ls > 0.0 ){
			return( v2*vec_mul(v1,v2)/v2ls );
		} else {
			return( v1 );
		}
	}

	public static Vector3 vec_cross( Vector3 v1, Vector3 v2 )
	{
		Vector3 vr;
		vr.x = v1.y * v2.z - v2.y * v1.z;
		vr.y = v1.z * v2.x - v2.z * v1.x;
		vr.z = v1.x * v2.y - v2.x * v1.y;
		return (vr);
	}

	public static Vector3 vec_unit( Vector3 v )
	{
		Vector3 vr;
		float l;
		l=vec_abs(v);
		if(Mathf.Abs(l)>1.0E-50){
			vr.x=v.x/l;
			vr.y=v.y/l;
			vr.z=v.z/l;
		} else {
			vr.x=0.0f;
			vr.y=0.0f;
			vr.z=0.0f;
		}
		return vr;
	}

	public static Vector3 vec_ncomp(Vector3 v1,Vector3 v2 )
	{
		return(v1-vec_proj(v1,v2) );
	}

	public static float vec_ndist ( Vector3 v, Vector3 v1, Vector3 v2 )
		/* normal distance of v to line(v1,v2) */
	{
		return( vec_abs(vec_ncomp(v-v1,v2-v1)) );
	}

	public static Vector3 rot_ax( Vector3 v, Vector3 ax, float phi )
	{
		Vector3 v_result=v;
		Vector3 bx,by,bz; // base
		Vector3 dp,dp2, nax=Vector2.zero;
		float sinphi,cosphi;
	
		if ( phi !=0.0 && vec_abs(ax)>0.0 ){
			
			bz = vec_unit( ax );
			if( Mathf.Abs(bz.x) <= Mathf.Abs(bz.y) && Mathf.Abs(bz.x) <= Mathf.Abs(bz.z) ) nax=Vector3.forward;
			if( Mathf.Abs(bz.y) <= Mathf.Abs(bz.z) && Mathf.Abs(bz.y) <= Mathf.Abs(bz.x) ) nax=Vector3.right;
			if( Mathf.Abs(bz.z) <= Mathf.Abs(bz.x) && Mathf.Abs(bz.z) <= Mathf.Abs(bz.y) ) nax=Vector3.up;
			bx = vec_unit( nax- bz*vec_mul(nax,bz));
			by = vec_cross(bz,bx);
			
			sinphi=Mathf.Sin(phi);
			cosphi=Mathf.Cos(phi);

			// transform into axis-system
			dp.x = vec_mul( v, bx );
			dp.y = vec_mul( v, by );
			dp.z = vec_mul( v, bz );
			
			dp2.x = dp.x*cosphi - dp.y*sinphi;
			dp2.y = dp.y*cosphi + dp.x*sinphi;
			dp2.z = dp.z;
			
			// retransform back
			v_result.x = dp2.x * bx.x + dp2.y * by.x + dp2.z * bz.x;
			v_result.y = dp2.x * bx.y + dp2.y * by.y + dp2.z * bz.y;
			v_result.z = dp2.x * bx.z + dp2.y * by.z + dp2.z * bz.z;
			
		}
		return v_result;
	}

	public static float plane_dist(Vector3 r, Vector3 rp, Vector3 n)
	{
		return( Maths.vec_mul(r-rp,n));
	}

	public static float vec_angle( Vector3 v1, Vector3 v2 )
		/* returns positive angle between 0 and M_PI */
	{
		return( Mathf.Acos(vec_mul( vec_unit(v1), vec_unit(v2) )) );
	}
	
}
