using UnityEngine;
using System.Collections;

public class Data_Daily
{
	int _id;
	int _daily_id;
	int _status;
	int _userid;
	int _dailydata;
	
	public int daily_date {
		get{ return _dailydata; }
		set{ _dailydata = value; }
	}
	
	public int user_id {
		get{ return _userid; }
		set{ _userid = value; }
	}
	
	public int id { 
		get { return _id; }
		set { _id = value; }
	}

	public int daily_id { 
		get { return _daily_id; }
		set{ _daily_id = value; }
	}

	public int status { 
		get { return _status; }
		set{ _status = value; }
	}
	
}
