#region Preprocessor Defines

#endregion

using System;
using UnityEngine;
//using System;
//using System.Collections;
//using System.Collections.Generic;

/// <summary>
/// Just a generic event listener for you to tell if stuff is finished
/// </summary>
public class BBundleEventListener : MonoBehaviour 
{

	#region Unity Built in API calls

	private void OnEnable()
	{
		BBundleManager.Instance.DownloadStarted += BBundleDownloadStarted;
		BBundleManager.Instance.DownloadPercentageChanged += BBundleDownloadPercentChanged;
		BBundleManager.Instance.DownloadCompleted += BBundleDownloadFinished;
		BBundleManager.Instance.DownloadFailed += BBundleDownloadFailed;
		BBundleManager.Instance.AllDownloadsFinished += BBundleAllDownloadsFinished;
	}

	private void OnDisable()
	{
		if (BBundleManager.Instance)
		{
			BBundleManager.Instance.DownloadStarted -= BBundleDownloadStarted;
			BBundleManager.Instance.DownloadPercentageChanged -= BBundleDownloadPercentChanged;
			BBundleManager.Instance.DownloadCompleted -= BBundleDownloadFinished;
			BBundleManager.Instance.DownloadFailed -= BBundleDownloadFailed;
			BBundleManager.Instance.AllDownloadsFinished -= BBundleAllDownloadsFinished;
		}
	}

	#endregion
	
	#region Private API

	private void BBundleDownloadStarted(object sender, EventArgs args)
	{
		Debug.Log("Download Started");
	}

	private void BBundleDownloadPercentChanged(object sender, EventArgs args)
	{
		Debug.Log("Download Percent: " + BBundleManager.Instance.DownloadPercent);
	}

	private void BBundleDownloadFinished(object sender, EventArgs args)
	{
		Debug.Log("Download Finished");
	}

	private void BBundleDownloadFailed(object sender, EventArgs args)
	{
		Debug.Log("Download Failed");
	}

	private void BBundleAllDownloadsFinished(object sender, EventArgs args)
	{
		Debug.Log("All Downloads Finished");
	}

	#endregion

}
