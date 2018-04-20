/*
http://www.cgsoso.com/forum-211-1.html

CG搜搜 Unity3d 每日Unity3d插件免费更新 更有VIP资源！

CGSOSO 主打游戏开发，影视设计等CG资源素材。

插件如若商用，请务必官网购买！

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

//////////////////////////////////////////////////////
//
// Copyright © 2014 Brett Faulds
//
// Author
//	Brett Faulds - brfaulds@gmail.com
//
// For help and support please email the author above 
// or visit http://www.brettfaulds.com
//
//////////////////////////////////////////////////////

/// <summary>
/// The purpose of this class is to hold all information
///		that pertains to FTP
/// </summary>
[System.Serializable]
public class BBundleFTPInfo 
{
#if !UNITY_WEBPLAYER
	#region Public Data

	public string HostURL;
	public string HostUSR;
	public string HostPWD;
	public string Filename;
	public bool UseSSL;

	#endregion

	public bool IsEmpty()
	{
		bool returnValue = true;

		if (HostURL != null && HostURL.Length > 0)
			returnValue = false;

		return returnValue;
	}

	public BBundleFTPInfo() { }

	public BBundleFTPInfo(BBundleFTPInfo other)
	{
		HostURL = other.HostURL;
		HostUSR = other.HostUSR;
		HostPWD = other.HostPWD;
		Filename = other.Filename;
		UseSSL = other.UseSSL;
	}
#endif
}
