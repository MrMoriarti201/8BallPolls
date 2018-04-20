//////////////////////////////////////////////////////
//
// Copyright © 2014 Brett Faulds
//
// Author
//	Brett Faulds - brfaulds@gmail.com
//
// For help and support please email the author above `
// or visit http://www.brettfaulds.com
//
//////////////////////////////////////////////////////

using UnityEngine;

public class BBundleTextAssetLoader : BBundleLoader
{

	#region Unity Built in API calls

	protected override void Start()
	{
		BBundleManager.Instance.LoadBundle(new BBundleObject(this));
	}

	#endregion

	#region Events

	public delegate void LoadedTextAsset (string text);
	public event LoadedTextAsset OnLoadedTextAsset;

	#endregion

	#region Public API

	public string TextAssetName;
	public string Data;

	#endregion

	#region Protected API

	protected override void LoadedAsset()
	{
		TextAsset textAsset = (TextAsset)BBundleManager.Instance.GetAssetOfType(TextAssetName, typeof(TextAsset));
		Data = textAsset.text;

		if (OnLoadedTextAsset != null)
			OnLoadedTextAsset(Data);

#if DEBUGING_ENABLED
		Debug.Log("Text asset loaded");
#endif
	}

	#endregion
}
