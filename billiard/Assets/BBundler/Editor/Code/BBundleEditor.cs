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
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// The purpose of this is to have a robust system that users can easily add/update their game objects in a given scene
/// </summary>
[CustomEditor(typeof(BBundleLoader))]
public class BBundleEditor : Editor 
{
	#region Unity Built in API calls

    public override void OnInspectorGUI()
    {
        BBundleLoader current = target as BBundleLoader;

        current.BundleType = (BBundleType)EditorGUILayout.EnumPopup(new GUIContent("Bundle Type", "What type of bundle information we are going to pull"), current.BundleType);
        switch (current.BundleType)
        {
            case BBundleType.Prefab:
                current.Prefab = EditorGUILayout.ObjectField(new GUIContent("Create Prefab", "The prefab we are going to instantiate from our project"), current.Prefab, typeof(Transform), true) as Transform;
                current.FunctionName = EditorGUILayout.TextField(new GUIContent("Function to call after loaded", "This will allow you to make sure your function gets called only after being loaded"), current.FunctionName);
				current.CallFunctionAfter = EditorGUILayout.Toggle(new GUIContent("Call function after?", "Choose to call or ignore the function after"), current.CallFunctionAfter);
		        if (current.ExtraOptions)
		        {
					current.OtherFunctionedObject = EditorGUILayout.ObjectField(new GUIContent("Game Object to Notify", "Notify another game object that is put here"), current.OtherFunctionedObject, typeof(GameObject), true) as GameObject;
					current.FunctionToCallOnOther = EditorGUILayout.TextField(new GUIContent("Game Object function to call after loaded", "This is the function that will call on the other game object"), current.FunctionToCallOnOther);
			        if (GUILayout.Button(new GUIContent("Collapse Extras", "Hide extra features")))
			        {
				        current.ExtraOptions = false;
			        }
		        }
		        else
		        {
					if (GUILayout.Button(new GUIContent("Expand Extras", "Show extra features")))
					{
						current.ExtraOptions = true;
					}
		        }

                break;
            case BBundleType.PrefabCreator:
                current.AssetName = EditorGUILayout.TextField(new GUIContent("Prefab Name", "Name of the prefab we are going to pull from the bundle"), current.AssetName);
                current.FunctionName = EditorGUILayout.TextField(new GUIContent("Function to call after loaded", "This will allow you to make sure your function gets called only after being loaded"), current.FunctionName);

                current.AssetTypeDetermined = EditorGUILayout.Toggle(new GUIContent("Asset Type Determined?", "Used for assets with the same name, grabs an asset of a given type"), current.AssetTypeDetermined);
                if (current.AssetTypeDetermined)
                {
                    current.AssetType = (BBundleAssetType)EditorGUILayout.EnumPopup(new GUIContent("Asset Type", "The type of asset this is"), current.AssetType);
                }
				current.CallFunctionAfter = EditorGUILayout.Toggle(new GUIContent("Call function after?", "Choose to call or ignore the function after"), current.CallFunctionAfter);
				if (current.ExtraOptions)
				{
					current.OtherFunctionedObject = EditorGUILayout.ObjectField(new GUIContent("Game Object to Notify", "Notify another game object that is put here"), current.OtherFunctionedObject, typeof(GameObject), true) as GameObject;
					current.FunctionToCallOnOther = EditorGUILayout.TextField(new GUIContent("Game Object function to call after loaded", "This is the function that will call on the other game object"), current.FunctionToCallOnOther);
					if (GUILayout.Button(new GUIContent("Collapse Extras", "Hide extra features")))
					{
						current.ExtraOptions = false;
					}
				}
				else
				{
					if (GUILayout.Button(new GUIContent("Expand Extras", "Show extra features")))
					{
						current.ExtraOptions = true;
					}
				}
                break;
            case BBundleType.Object:
                current.Caller = EditorGUILayout.ObjectField(new GUIContent("Notify Object", "Notify this object after bundle being loaded"), current.Caller, typeof(Transform), allowSceneObjects: true) as Transform;
				if (current.Caller != null)
					current.FunctionName = EditorGUILayout.TextField(new GUIContent("Function to call after loaded", "This will allow you to make sure your function gets called only after being loaded"), current.FunctionName);
				break;
			case BBundleType.AudioClip:
				current.AssetName = EditorGUILayout.TextField(new GUIContent("AudioClip Name", "Name of the AudioClip we are going to pull from the bundle"), current.AssetName);
				current.Caller = EditorGUILayout.ObjectField(new GUIContent("Notify Object", "Notify this object after bundle being loaded"), current.Caller, typeof(Transform), allowSceneObjects: true) as Transform;
                
				if (current.Caller != null)
					current.FunctionName = EditorGUILayout.TextField(new GUIContent("Function to call after loaded", "This will allow you to make sure your function gets called only after being loaded"), current.FunctionName);
				
				current.LoadAudioClip = EditorGUILayout.Toggle(new GUIContent("Load Audio Clip?", "Load this AudioClip in this game object"), current.LoadAudioClip);
		        if (current.LoadAudioClip)
		        {
					current.AutostartAudioClip = EditorGUILayout.Toggle(new GUIContent("Auto Play Audio", "Play this audio clips as soon as it is loaded"), current.AutostartAudioClip);
					current.LoopAudio = EditorGUILayout.Toggle(new GUIContent("Loop Audio", "Loop this audio clip as it is played"), current.LoopAudio);
		        }
		        break;
            case BBundleType.Texture:
                current.BundleTextureType = (BBundleTextureType)EditorGUILayout.EnumPopup(new GUIContent("Bundle Texture Type", "The type of texture we are going to load on this object"), current.BundleTextureType);

                #if NGUI_SUPPORT_357

                if (current.BundleTextureType == BBundleTextureType.NGUISprite)
                {
                    current.AtlasName = EditorGUILayout.TextField(new GUIContent("Atlas Name", "The name of the atlas for this sprite"), current.AtlasName);
                    current.AssetName = EditorGUILayout.TextField(new GUIContent("Texture Name", "The name of the texture we are going to load"), current.AssetName);
                }
                else if (current.BundleTextureType == BBundleTextureType.NGUILabel)
                {
                    current.AssetName = EditorGUILayout.TextField(new GUIContent("Font Name", "The name of the font we are going to load"), current.AssetName);
                }
                else
                {
                    current.AssetName = EditorGUILayout.TextField(new GUIContent("Texture Name", "The name of the texture we are going to load"), current.AssetName);
                }

                #else

                current.AssetName = EditorGUILayout.TextField(new GUIContent("Texture Name", "The name of the texture we are going to load"), current.AssetName);

                #endif

				if (current.ExtraOptions)
				{
					current.OtherFunctionedObject = EditorGUILayout.ObjectField(new GUIContent("Game Object to Notify", "Notify another game object that is put here"), current.OtherFunctionedObject, typeof(GameObject), true) as GameObject;
					current.FunctionToCallOnOther = EditorGUILayout.TextField(new GUIContent("Game Object function to call after loaded", "This is the function that will call on the other game object"), current.FunctionToCallOnOther);
					if (GUILayout.Button(new GUIContent("Collapse Extras", "Hide extra features")))
					{
						current.ExtraOptions = false;
					}
				}
				else
				{
					if (GUILayout.Button(new GUIContent("Expand Extras", "Show extra features")))
					{
						current.ExtraOptions = true;
					}
				}

                break;
        }

		current.PlatformIndependant = EditorGUILayout.Toggle(new GUIContent("Platform Independant", "Do you want to make urls platform independant?"), current.PlatformIndependant);
		if (current.PlatformIndependant)
		    PlatformDependantLocationType(current);
		else
			LocationType(current);

	    if (current.BundleType != BBundleType.Texture && current.BundleType != BBundleType.AudioClip)
            current.DestroyOnLoaded = EditorGUILayout.Toggle(new GUIContent("Destroy once loaded?", "Do we want to destroy this game object after the asset is loaded?"), current.DestroyOnLoaded);

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

	#endregion

	#region Constants

	/// <summary>
	/// FTP List Count(Editor Only)
	/// </summary>
	public const string FTP_COUNT = "FTPListCount";

	/// <summary>
	/// FTP Url
	/// </summary>
	public const string FTP_NAME = "FTPNAME";

	/// <summary>
	/// FTP Url
	/// </summary>
	public const string FTP_URL = "FTPURL";

	/// <summary>
	/// Ftp Password
	/// </summary>
	public const string FTP_PWD = "FTPPWD";

	/// <summary>
	/// FTP Username
	/// </summary>
	public const string FTP_USR = "FTPUSR";

	/// <summary>
	/// FTP SSL
	/// </summary>
	public const string FTP_SSL = "FTPSSL";

	/// <summary>
	/// AWS List Count(Editor Only)
	/// </summary>
	public const string AWS_COUNT = "AWSListCount";

	/// <summary>
	/// AWS Url
	/// </summary>
	public const string AWS_NAME = "AWSNAME";

	/// <summary>
	/// The AWS Bucket
	/// </summary>
	public const string AWS_BUCKET = "AWSBUCKET";

	/// <summary>
	/// AWS Url
	/// </summary>
	public const string AWS_URL = "AWSURL";

	#endregion

	#region Protected API

	protected void PlatformDependantLocationType(BBundleLoader current)
	{
		EditorGUILayout.HelpBox("Please not that this may not work in the editor, and instead may only be viewed on the device, recommended to test without this first, then switch to after. \nAlso note that having two of the same platform will only pick one.", MessageType.Warning);
		current.LocationType = BBundleLocationType.Regular;
		if (current.PlatformItems != null)
		{
			for (int i = 0; i < current.PlatformItems.Length; ++i)
			{
				GUILayout.Box("", new GUILayoutOption[] {GUILayout.ExpandWidth(true), GUILayout.Height(1)});
				current.PlatformItems[i].Platform =
					(RuntimePlatform)
						EditorGUILayout.EnumPopup(new GUIContent("Platform", "Which platform you will be making this url for"),
							current.PlatformItems[i].Platform);
				current.PlatformItems[i].LocationUrl =
					EditorGUILayout.TextField(new GUIContent("Url", "The location for this asset bundle download"),
						current.PlatformItems[i].LocationUrl);
				GUILayout.Box("", new GUILayoutOption[] {GUILayout.ExpandWidth(true), GUILayout.Height(1)});
			}
		}

		if (GUILayout.Button("Add Platform Dependant Item"))
		{
			List<BBundlePlatformItem> currentItems = new List<BBundlePlatformItem>(current.PlatformItems);
			currentItems.Add(new BBundlePlatformItem());
			current.PlatformItems = currentItems.ToArray();
		}
		if (current.PlatformItems.Length > 0 && GUILayout.Button("Remove Platform Dependant Item"))
		{
			List<BBundlePlatformItem> currentItems = new List<BBundlePlatformItem>(current.PlatformItems);
			currentItems.RemoveAt(currentItems.Count - 1); //Remove the last item
			current.PlatformItems = currentItems.ToArray();
		}
		if (current.PlatformItems.Length > 0 && GUILayout.Button("Clear List"))
		{
			current.PlatformItems = null;
		}
	}

	protected void LocationType(BBundleLoader current)
	{
		current.LocationType = (BBundleLocationType)EditorGUILayout.EnumPopup(new GUIContent("Bundle Location Type", "The type of location we are going to load from (Default: Regular)"), current.LocationType);
		if (current.LocationType == BBundleLocationType.Regular)
		{
			current.AssetUrl =
				EditorGUILayout.TextField(new GUIContent("Asset Bundle URL", "The URL for this asset bundle to load from"),
					current.AssetUrl);
			current.AssetVersion =
				EditorGUILayout.IntField(
					new GUIContent("Asset Bundle Version", "The version of this asset bundle being loaded"), current.AssetVersion);
		}
#if UNITY_MOBILE_FTP_ENABLED
	    else if (current.LocationType == BBundleLocationType.FTP)
	    {
			current.FtpCacheType = (BBundleFTPCacheType)EditorGUILayout.EnumPopup(new GUIContent("Bundle FTP Cache Type", "The type of caching we will be using (Default: None)"), current.FtpCacheType);

		    if (current.FtpCacheType == BBundleFTPCacheType.None)
		    {
				int ftpListCount = EditorPrefs.GetInt(FTP_COUNT);

				if (ftpListCount > 0)
				{
					if (EditorPrefs.GetBool("BBundleEditorOpened"))
					{
						GUI.color = Color.red;
						EditorGUILayout.LabelField("BBundle Editor Opened - Cannot view list atm");
						GUI.color = Color.white;
						return;
					}
					string ftpURL = EditorPrefs.GetString(FTP_URL + current.FTPid);
					string ftpUSR = EditorPrefs.GetString(FTP_USR + current.FTPid);
					string ftpPWD = EditorPrefs.GetString(FTP_PWD + current.FTPid);
					bool useSSL = EditorPrefs.GetBool(FTP_SSL + current.FTPid);
					EditorGUILayout.LabelField(new GUIContent("Asset FTP URL: " + ftpURL, "The FTP Info for this asset bundle to load from"));

					current.FtpInfo.HostUSR = ftpUSR;
					current.FtpInfo.HostPWD = ftpPWD;
					current.FtpInfo.HostURL = ftpURL;
					if (string.IsNullOrEmpty(current.FtpInfo.Filename))
						current.FtpInfo.Filename = string.Empty;

					current.AssetUrl = current.FtpInfo.HostURL + current.FtpInfo.Filename;

					List<string> CurrentFTPList = new List<string>();
					for (int i = 0; i < ftpListCount; ++i)
					{
						CurrentFTPList.Add(EditorPrefs.GetString(FTP_NAME + i));
					}

					bool previous = useSSL;
					useSSL = EditorGUILayout.Toggle(new GUIContent("Use SSL", "If this is an ssl connection, check this"), useSSL);
					if (useSSL != previous)
					{
						EditorPrefs.SetBool(FTP_SSL + current.FTPid, useSSL);
					}

					current.FTPid = EditorGUILayout.Popup(current.FTPid, CurrentFTPList.ToArray());

					current.FtpInfo.Filename =
						EditorGUILayout.TextField(new GUIContent("Filename", "The filename of the asset bundle on this ftp server"),
							current.FtpInfo.Filename);

					EditorGUILayout.LabelField(new GUIContent("Final Path: " + current.AssetUrl, "The actual path we will be loading"));
				}
				else
				{
					EditorGUILayout.LabelField(new GUIContent("No FTP data setup (Check BBundle Editor)", "In BBundle Editor you can setup your connections to ftps"));
				}
		    }
		    else
		    {
				current.AssetUrl =
					EditorGUILayout.TextField(new GUIContent("Asset Bundle URL", "The URL for this asset bundle to load from"),
						current.AssetUrl);

				current.AssetVersion =
				EditorGUILayout.IntField(
					new GUIContent("Asset Bundle Version", "The version of this asset bundle being loaded"), current.AssetVersion);
		    }
	    }
#endif
		else if (current.LocationType == BBundleLocationType.AWS)
		{
			int awsListCount = EditorPrefs.GetInt(AWS_COUNT);

			if (awsListCount > 0)
			{
				if (EditorPrefs.GetBool("BBundleEditorOpened"))
				{
					EditorGUILayout.LabelField("BBundle Editor Opened - Cannot view list atm");
					return;
				}

				string awsName = EditorPrefs.GetString(AWS_NAME + current.AWSid);
				string awsUrl = EditorPrefs.GetString(AWS_URL + current.AWSid);
				string awsBucket = EditorPrefs.GetString(AWS_BUCKET + current.AWSid);
				EditorGUILayout.LabelField(new GUIContent("AWS Name: " + awsName, "The AWS name"));
				EditorGUILayout.LabelField(new GUIContent("AWS URL: " + awsUrl, "The AWS url"));
				EditorGUILayout.LabelField(new GUIContent("AWS Bucket: " + awsBucket, "The AWS bucket"));

				current.AWSInfo.AWSName = awsName;
				current.AWSInfo.AWSUrl = awsUrl;
				current.AWSInfo.Bucket = awsBucket;
				if (string.IsNullOrEmpty(current.AWSInfo.FilePath))
					current.AWSInfo.FilePath = string.Empty;

				current.AssetUrl = current.AWSInfo.AWSUrl + current.AWSInfo.Bucket + "/" + current.AWSInfo.FilePath;

				List<string> CurrentAWSList = new List<string>();
				for (int i = 0; i < awsListCount; ++i)
				{
					CurrentAWSList.Add(EditorPrefs.GetString(AWS_NAME + i));
				}

				int prevId = current.AWSid;
				current.AWSid = EditorGUILayout.Popup(current.AWSid, CurrentAWSList.ToArray());
				if (current.AWSid != prevId)
				{
					current.AWSInfo.AWSName = EditorPrefs.GetString(AWS_NAME + current.AWSid);
					current.AWSInfo.AWSUrl = EditorPrefs.GetString(AWS_URL + current.AWSid);
					current.AWSInfo.Bucket = EditorPrefs.GetString(AWS_BUCKET + current.AWSid);
					if (string.IsNullOrEmpty(current.AWSInfo.FilePath))
						current.AWSInfo.FilePath = string.Empty;
					current.AssetUrl = current.AWSInfo.AWSUrl + current.AWSInfo.Bucket + "/" + current.AWSInfo.FilePath;
				}

				current.AWSInfo.FilePath =
					EditorGUILayout.TextField(new GUIContent("Filename", "The filename of the asset bundle on this ftp server"),
						current.AWSInfo.FilePath);

				EditorGUILayout.LabelField(new GUIContent("Final Path: " + current.AssetUrl, "The actual path we will be loading"));
			}
			else
			{
				EditorGUILayout.LabelField(new GUIContent("No AWS data setup (Check BBundle Editor)", "In BBundle Editor you can setup your connections to aws"));
			}
		}
	}

	#endregion
}

#if NGUI_SUPPORT_357
[CustomEditor(typeof(BBundleAtlasReferenceLoader))]
public class BBundleAtlasEditor : BBundleEditor
{
	public override void OnInspectorGUI()
	{
		BBundleAtlasReferenceLoader current = target as BBundleAtlasReferenceLoader;
		current.BundleType = BBundleType.Atlas;
		if (string.IsNullOrEmpty(current.FunctionName))
			current.FunctionName = "LoadedAsset";

		if (current.Caller == null)
			current.Caller = current.transform;

		current.Caller = EditorGUILayout.ObjectField(new GUIContent("Notify Object", "Notify this object after bundle being loaded"), current.Caller, typeof(Transform), allowSceneObjects: true) as Transform;
		current.FunctionName = EditorGUILayout.TextField(new GUIContent("Function to call after loaded", "This will allow you to make sure your function gets called only after being loaded"), current.FunctionName);
		current.AtlasReplacementName = EditorGUILayout.TextField(new GUIContent("Atlas name to be replaced with", "The atlas to be swapped to"), current.AtlasReplacementName);

		current.PlatformIndependant = EditorGUILayout.Toggle(new GUIContent("Platform Independant", "Do you want to make urls platform independant?"), current.PlatformIndependant);
		if (current.PlatformIndependant)
			PlatformDependantLocationType(current);
		else
			LocationType(current);

		current.DestroyOnLoaded = EditorGUILayout.Toggle(new GUIContent("Destroy once loaded?", "Do we want to destroy this game object after the asset is loaded?"), current.DestroyOnLoaded);

		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}
#endif

[CustomEditor(typeof(BBundleTextAssetLoader))]
public class BBundleTextAssetEditor : BBundleEditor
{
	public override void OnInspectorGUI()
	{
		BBundleTextAssetLoader current = target as BBundleTextAssetLoader;
		if (string.IsNullOrEmpty(current.FunctionName))
			current.FunctionName = "LoadedAsset";

		if (current.Caller == null)
			current.Caller = current.transform;

		current.Caller = EditorGUILayout.ObjectField(new GUIContent("Notify Object", "Notify this object after bundle being loaded"), current.Caller, typeof(Transform), allowSceneObjects: true) as Transform;
		current.FunctionName = EditorGUILayout.TextField(new GUIContent("Function to call after loaded", "This will allow you to make sure your function gets called only after being loaded"), current.FunctionName);
		current.TextAssetName = EditorGUILayout.TextField(new GUIContent("Asset name to be loaded", "The name of the text asset as you uploaded to the bundle"), current.TextAssetName);
		
		current.PlatformIndependant = EditorGUILayout.Toggle(new GUIContent("Platform Independant", "Do you want to make urls platform independant?"), current.PlatformIndependant);
		if (current.PlatformIndependant)
			PlatformDependantLocationType(current);
		else
			LocationType(current);

		current.DestroyOnLoaded = EditorGUILayout.Toggle(new GUIContent("Destroy once loaded?", "Do we want to destroy this game object after the asset is loaded?"), current.DestroyOnLoaded);

		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}
