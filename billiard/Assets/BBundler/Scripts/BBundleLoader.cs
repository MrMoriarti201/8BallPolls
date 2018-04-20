#region Preprocessor Defines

//#define NGUI_SUPPORT_357

#if UNITY_SUPPORTS_FTP_MOBILE
#define UNITY_MOBILE_FTP_ENABLED
#endif

#endregion

using UnityEngine;
//using System;
//using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("BBundle/Loader")]
public enum BBundleType
{
    Prefab = 0,
    PrefabCreator,
    Object,
    AudioClip,
#if NGUI_SUPPORT_357
	Texture,
	Atlas
#else
    Texture
#endif
}

public enum BBundleLocationType
{
    Regular,
#if UNITY_MOBILE_FTP_ENABLED
	FTP,
#endif
    AWS
}

public enum BBundleFTPCacheType
{
    None,
    Anonymous
}

//To be added to
public enum BBundleAssetType
{
    Texture2D,
    GameObject,
    Transform
}

public enum BBundleTextureType
{
#if NGUI_SUPPORT_357
	Regular = 0,
	NGUISprite,
	NGUIUnity2DSprite,
	NGUILabel,
	NGUITexture
#else
    Regular = 0
#endif
}

[System.Serializable]
public class BBundlePlatformItem
{
    [SerializeField]
    public RuntimePlatform Platform;

    [SerializeField]
    public string LocationUrl;
}

public class BBundleLoader : MonoBehaviour
{

    #region Unity Built in API calls

    /// <summary>
    /// Check to see if there is a url to load, otherwise destroy this
    /// </summary>
    protected virtual void Start()
    {

        if (AssetUrl.Length > 0)
        {
            BBundleManager.Instance.LoadBundle(new BBundleObject(this));
        }
        else
        {
            Debug.Log("Asset doesn't have a URL, destroying this for it has no purpose in the scene");
            Destroy(this);
        }
    }

    #endregion

    #region Public Data

    /// <summary>
    /// The type of Bundle
    /// </summary>
    public BBundleType BundleType;

    /// <summary>
    /// The Texture type of bundle (Reg, NGUI, etc)
    /// </summary>
    public BBundleTextureType BundleTextureType;

    /// <summary>
    /// The type of asset
    /// </summary>
    public BBundleAssetType AssetType;

    /// <summary>
    /// The location of the bundle
    /// </summary>
    public BBundleLocationType LocationType;

    /// <summary>
    /// The cache type for an FTP
    ///		None = FTPWebRequest, no caching and always downloading the latest version
    ///		Anonymous = WWW Request, only allows anonymous downloads (http://docs.unity3d.com/ScriptReference/WWW.html)
    /// </summary>
    public BBundleFTPCacheType FtpCacheType;

    /// <summary>
    /// The cachedID for the ftp
    /// </summary>
    public int FTPid = 0;

    /// <summary>
    /// The cachedID for the ftp
    /// </summary>
    public int AWSid = 0;

    /// <summary>
    /// This is the ftp info of the bundle
    /// </summary>
    public BBundleFTPInfo FtpInfo;

    /// <summary>
    /// This is the aws info for the bundle
    /// </summary>
    public BBundleAWSInfo AWSInfo;

    /// <summary>
    /// Prefab to instantiate
    /// </summary>
    public Transform Prefab;

    /// <summary>
    /// Object to call back to
    /// </summary>
    public Transform Caller;

    /// <summary>
    /// Destroy this game object after loading?
    /// </summary>
    public bool DestroyOnLoaded;

    /// <summary>
    /// Is this asset type determined by something else if there is duplicate assets with the same name?
    /// </summary>
    public bool AssetTypeDetermined;

    /// <summary>
    /// The function name to call for Caller
    /// </summary>
    public string FunctionName;

    /// <summary>
    /// The asset to load
    /// </summary>
    public string AssetName;

    /// <summary>
    /// The URL of the asset bundle
    /// </summary>
    public string AssetUrl;

    /// <summary>
    /// The version of the asset bundle
    /// </summary>
    public int AssetVersion = 0;

    /// <summary>
    /// Call the function after the asset is loaded
    /// </summary>
    public bool CallFunctionAfter = true;

    /// <summary>
    /// Show the extra options for the user
    /// </summary>
    public bool ExtraOptions = false;

    /// <summary>
    /// In case you want to call another object when it is loaded
    /// </summary>
    public GameObject OtherFunctionedObject;

    /// <summary>
    /// The function to call on something else
    /// </summary>
    public string FunctionToCallOnOther;

    /// <summary>
    /// This is to know if this url is platform dependant (If so, then use the platform independant distinction)
    /// </summary>
    public bool PlatformIndependant;

    /// <summary>
    /// This is the list of urls for that specific platform
    /// </summary>
    public BBundlePlatformItem[] PlatformItems = { };

    /// <summary>
    /// This is a constant variable for this function that will be used for loading the assets
    /// </summary>
    public const string AssetLoadedFunction = "LoadedAsset";

    //Audio clip Stuff!
    /// <summary>
    /// Load the audio clip with an audio source if existing
    /// </summary>
    public bool LoadAudioClip = false;

    /// <summary>
    /// Should the audio clip loop?
    /// </summary>
    public bool LoopAudio = false;

    /// <summary>
    /// Should we autoplay it?
    /// </summary>
    public bool AutostartAudioClip = false;

#if NGUI_SUPPORT_357

	/// <summary>
	/// The name of the NGUI Atlas
	/// </summary>
	public string AtlasName;

#endif

    #endregion

    #region Protected API

    /// <summary>
    /// Function that is called after loading an asset through BBundleLoader
    /// </summary>
    protected virtual void LoadedAsset()
    {
        switch (BundleType)
        {
            case BBundleType.Object:
                if (FunctionName.Length > 0)
                {
                    if (Caller != null)
                    {
                        Caller.gameObject.SendMessage(FunctionName);
                    }
                }
                else
                {
                    Debug.Log("Asset was loaded, but there was no object attached to send the asset to");
                }
                break;
            case BBundleType.AudioClip:
                if (FunctionName.Length > 0)
                {
                    if (Caller != null)
                    {
                        Caller.gameObject.SendMessage(FunctionName);
                    }
                }
                else
                {
                    Debug.Log("Asset was loaded, but there was no object attached to send the asset to");
                }

                if (LoadAudioClip)
                {
                    AudioSource thisSource = GetComponent<AudioSource>();
                    if (thisSource == null)
                    {
                        thisSource = gameObject.AddComponent<AudioSource>();
                    }

                    thisSource.clip = BBundleManager.Instance.GetAssetOfType<AudioClip>(AssetName);

                    if (AutostartAudioClip)
                    {
                        thisSource.Play();
                    }
                }
                break;
            case BBundleType.Texture:
                switch (BundleTextureType)
                {
                    case BBundleTextureType.Regular:
                        Texture tempTexture = BBundleManager.Instance.Bundles[AssetName] as Texture;
                        if (tempTexture != null)
                        {
                            gameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = tempTexture;

                            if (OtherFunctionedObject && FunctionToCallOnOther.Length > 0)
                            {
                                OtherFunctionedObject.SendMessage(FunctionToCallOnOther);
                            }
                        }
                        else
                        {
                            Debug.LogWarning(string.Format("Missing texture in asset bundle: {0} (Regular) for object {1}", AssetName, gameObject.name));
                        }
                        break;
#if NGUI_SUPPORT_357

					case BBundleTextureType.NGUISprite:
						UISprite thisSprite = gameObject.GetComponent<UISprite>();
						if (thisSprite != null)
						{
							Transform go = BBundleManager.Instance.GetAssetOfType(AtlasName, typeof(Transform)) as Transform;
							if (go != null)
							{
								UIAtlas atlas = go.GetComponent<UIAtlas>();
								thisSprite.atlas = atlas;
								thisSprite.spriteName = AssetName;

								if (OtherFunctionedObject && FunctionToCallOnOther.Length > 0)
								{
									OtherFunctionedObject.SendMessage(FunctionToCallOnOther);
								}
							}
							else
							{
								Debug.LogError(string.Format("Missing atlas in asset bundle: {0} (NGUISprite) for object {1}", AtlasName, gameObject.name));
							}
						}
						else
						{
							Debug.LogError("Missing UISprite component on object (NGUISprite)");
						}

						break;
					case BBundleTextureType.NGUILabel:
						UILabel thisLabel = this.gameObject.GetComponent<UILabel>();
						if (thisLabel != null)
						{
							Transform go = BBundleManager.Instance.GetAssetOfType(AssetName, typeof(Transform)) as Transform;
							if (go != null)
							{
								UIFont font = go.GetComponent<UIFont>();
								thisLabel.bitmapFont = font;

								if (OtherFunctionedObject && FunctionToCallOnOther.Length > 0)
								{
									OtherFunctionedObject.SendMessage(FunctionToCallOnOther);
								}
							}
							else
							{
								Debug.LogError(string.Format("Missing font in asset bundle: {0} (NGUILabel) for object {1}", AssetName, gameObject.name));
							}
						}
						else
						{
							Debug.LogError("Missing UILabel Component (NGUILabel)");
						}
						break;
					case BBundleTextureType.NGUITexture:
						UITexture thisTexture = this.gameObject.GetComponent<UITexture>();
						if (thisTexture != null)
						{
							Texture temp = BBundleManager.Instance.Bundles[AssetName] as Texture;
							if (temp != null)
							{
								thisTexture.mainTexture = temp;

								if (OtherFunctionedObject && FunctionToCallOnOther.Length > 0)
								{
									OtherFunctionedObject.SendMessage(FunctionToCallOnOther);
								}
							}
							else
							{
								Debug.LogWarning(string.Format("Missing texture in asset bundle: {0} (NGUITexture) for object {1}", AssetName, gameObject.name));
							}
						}
						else
						{
							Debug.LogError("Missing UITexture Component (NGUITexture)");
						}
						break;
					case BBundleTextureType.NGUIUnity2DSprite:
						UI2DSprite this2DSprite = gameObject.GetComponent<UI2DSprite>();
						if (this2DSprite != null)
						{
							Sprite temp = BBundleManager.Instance.GetAssetOfType(AssetName, typeof(Sprite)) as Sprite;
							if (temp != null)
							{
								this2DSprite.sprite2D = temp;

								if (OtherFunctionedObject && FunctionToCallOnOther.Length > 0)
								{
									OtherFunctionedObject.SendMessage(FunctionToCallOnOther);
								}
							}
							else
							{
								Debug.LogWarning(string.Format("Missing sprite in asset bundle: {0} (NGUIUnity2DSprite) for object {1}", AssetName, gameObject.name));
							}
						}
						else
						{
							Debug.LogError("Missing UI2DSprite Component (NGUIUnity2DSprite)");
						}
						break;

#endif
                }
                break;
        }


//         //Destroy the gameobject on loaded
//         if (DestroyOnLoaded)
//             Destroy(gameObject);
//         else //Else destroy the monobehaviour attached to it.
//             Destroy(this);
    }

    #endregion

    #region Private API

    private System.Type GetTypeOfAsset(BBundleAssetType type)
    {
        System.Type returnValue;
        switch (type)
        {
            case BBundleAssetType.GameObject:
                returnValue = typeof(GameObject);
                break;
            case BBundleAssetType.Texture2D:
                returnValue = typeof(Texture2D);
                break;
            case BBundleAssetType.Transform:
                returnValue = typeof(Transform);
                break;
            default:
                returnValue = typeof(object);
                break;
        }
        return returnValue;
    }

    #endregion

    void LoadImage()
    {
        GameManager.instance.ShowImage();
    }

    void LoadTexture()
    {
        GameManager.instance.ShowTexture();
    }

    void OnLoadBundle()
    {
        //Progress.instance.LoadBundles();
    }

}
