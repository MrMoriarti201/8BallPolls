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

#if DEBUG
	#define DEBUGING_ENABLED
#endif

#endregion

using System.Collections.Generic;
using UnityEngine;

public class BBundleObject
{
	#region Public Data

	public GameObject Object { get; private set; }

	public string _url;
	public string Url
	{
		get
		{
			if (PlatformIndependant)
			{
				for (int i = 0; i < PlatformItems.Length; ++i)
				{
					if (PlatformItems[i].Platform == Application.platform)
					{
						_url = PlatformItems[i].LocationUrl;
					}
				}
			}

			return _url;
		}
	}
	public string Callback { get;  set; }
	public int Version { get; private set; }
	public bool PlatformIndependant { get; private set; }
	public BBundlePlatformItem[] PlatformItems { get; private set; } 
	public BBundleFTPInfo FTPInfo { get; private set; }
	public BBundleAWSInfo AWSInfo { get; private set; }
	public BBundleLocationType Location { get; private set; }
	public BBundleFTPCacheType FTPCache { get; private set; }

	#endregion

	public BBundleObject(BBundleLoader loader)
	{
		Object = loader.gameObject;
		_url = loader.AssetUrl;

		Callback = BBundleLoader.AssetLoadedFunction;
		Version = loader.AssetVersion;
		FTPInfo = loader.FtpInfo;
		AWSInfo = loader.AWSInfo;
		Location = loader.LocationType;
		FTPCache = loader.FtpCacheType;
		PlatformIndependant = loader.PlatformIndependant;
		if (PlatformIndependant)
			PlatformItems = loader.PlatformItems;
	}

	public void InitObject()
	{
		if (Object != null)
		{
			Object.SendMessage(Callback);
		}
		else
		{
#if DEBUGING_ENABLED
           Debug.LogError("Couldn't find object to send message to. Url: " + Url);
#endif
		}
	}

	public override string ToString()
	{
		return "Bundle Object { \"object\": \"" + Object.name + "\", \"Url\":\"" + Url + "\",  \"Platform Dependant\": \"" + PlatformIndependant + "\"}";
	}
}
