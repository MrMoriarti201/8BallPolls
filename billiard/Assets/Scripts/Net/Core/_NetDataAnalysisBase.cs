using UnityEngine;
using System.Collections;

public abstract class NetDataAnalysisBase {

	protected int _flag;
	protected string _reason = "";
	
	public int flag{
		get { return _flag; }
		set { _flag = value; }
	}
	
	public string reason {
		get { return _reason; }
		set { _reason = value; }	
	}
		
	public static string urlbase
	{
		get
		{           
			return Net.url+"index.php/myapp/";
		}        
	}

	public abstract string url{ 
		get; 
	}
	public abstract string[] allparams{
		get;
	}
	
	public abstract bool Analysis ( NetIO nio );
}
