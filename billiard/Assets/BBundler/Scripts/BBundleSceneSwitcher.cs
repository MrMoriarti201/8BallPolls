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

#region Preprocessor Defines

//#define NGUI_SUPPORT_357

#if UNITY_SUPPORTS_FTP_MOBILE
	#define UNITY_MOBILE_FTP_ENABLED
#endif

#endregion

using System;
using UnityEngine;

public class BBundleSceneSwitcher : MonoBehaviour
{
	public string levelName = string.Empty;
#if NGUI_SUPPORT_357
	public DownloadingScreenUI DownloadBar;
#endif

	private void OnEnable()
	{
		BBundleManager.Instance.AllDownloadsFinished += OnAllDownloadsFinished;
		BBundleManager.Instance.DownloadStarted += OnDownloadStarted;
	}

	private void OnDisable()
	{
		BBundleManager.Instance.AllDownloadsFinished -= OnAllDownloadsFinished;
	}

	private void OnDownloadStarted(object sender, EventArgs args)
	{
#if NGUI_SUPPORT_357
		if (!DownloadBar.gameObject.activeSelf)
			DownloadBar.gameObject.SetActive(true);
#endif
	}

	private void OnAllDownloadsFinished(object sender, EventArgs args)
	{
		ChangeLevel();
	}

	private void ChangeLevel()
	{
		if (!string.IsNullOrEmpty(levelName))
			Application.LoadLevel(levelName);
		else
			Application.LoadLevel(Application.loadedLevel + 1);
	}
}
