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
public class BBundleAWSInfo 
{
	#region Public Data

	public string AWSName;
	public string AWSUrl;
	public string Bucket;
	public string FilePath;

	#endregion

	public BBundleAWSInfo() { }

	public BBundleAWSInfo(BBundleAWSInfo other)
	{
		AWSName = other.AWSName;
		AWSUrl = other.AWSUrl;
		Bucket = other.Bucket;
		FilePath = other.FilePath;
	}
}
