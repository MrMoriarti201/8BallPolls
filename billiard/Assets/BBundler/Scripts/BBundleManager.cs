#region Preprocessor Defines

#if DEBUG
	#define DEBUGING_ENABLED
#endif

#if UNITY_SUPPORTS_FTP_MOBILE
	#define UNITY_MOBILE_FTP_ENABLED
#endif

#endregion

using System;
using System.IO;
using System.Net;
using UnityEngine;
//using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

/// <summary>
/// Purpose of this class is to load all bundles in one area so that it can be referenced throughout the scene
///     This class will be automatically made if it doesn't exist, no need to add it to the scene.
/// </summary>
public class BBundleManager : MonoBehaviour 
{
	#region Unity Built in API calls

    /// <summary>
    /// To remove memory of all asset bundles loaded
    /// </summary>
    private void OnDestroy()
    {
        //foreach (KeyValuePair<string, object> bundle in _bundles)
        //{
        //    DestroyImmediate(bundle.Value as Object, true);
        //}
		_beingDestroyed = true;
        _bundles.Clear();
        Resources.UnloadUnusedAssets();
    }

	#endregion
    
	#region Events

	public EventHandler DownloadStarted;
	public EventHandler DownloadPercentageChanged;
	public EventHandler DownloadCompleted;
	public EventHandler DownloadFailed;
	public EventHandler AllDownloadsFinished;

	#endregion

	#region Properties

	/// <summary>
    /// Grab the current BBundle Manager Instance, used for loading asset bundles
    /// </summary>
    private static BBundleManager _instance;
    public static BBundleManager Instance
    {
        get
        {
			if (_instance == null && !_beingDestroyed)
            {
                _instance = new GameObject("BBundle Manager").AddComponent<BBundleManager>();
				_instance.InitializeBBundleManager();
				DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    /// <summary>
    /// A public way to call any asset after it has been loaded to an asset
    /// </summary>
    private Dictionary<string, object> _bundles = new Dictionary<string,object>();
    public Dictionary<string, object> Bundles
    {
        get
        {
            return _bundles;
        }
    }

	public T GetAssetOfType<T>(string AssetName)
	{
		return (T)GetAssetOfType(AssetName, typeof(T));
	}

    public object GetAssetOfType(string AssetName, System.Type type)
    {
        object value = null;

        IEnumerator oEm = _bundles.GetEnumerator();
        while (oEm.MoveNext())
        {
            KeyValuePair<string, object> kV = (KeyValuePair<string, object>)oEm.Current;

            if ((kV.Key.Replace(type.ToString(), "").Equals(AssetName) || kV.Key.Equals(AssetName)) && kV.Value.GetType().Equals(type))
            {
                value = kV.Value;
                break;
            }
        }

        return value;
    }

	public bool BundleDownloading { get; private set; }
	public int DownloadedAssets { get; private set; }
	public float DownloadPercent { get; private set; }
	public string DownloadingAsset { get; private set; }

    #endregion
	
	#region Private Data

    //These are private variables that will store the asset urls as well as what is currently qued
	private List<BBundleObject> _que = new List<BBundleObject>();
    private List<string> _readyUrls = new List<string>();
	private List<string> _pendingUrls = new List<string>();
    private List<BBundleObject> _bundledObjects = new List<BBundleObject>();
	private static bool _beingDestroyed = false;

	#endregion
	
	#region Public API

	/// <summary>
	/// To initialize the BBundleManager
	/// </summary>
	public void InitializeBBundleManager()
	{
		BundleDownloading = false;
		DownloadedAssets = 0;
		DownloadPercent = 0;
		DownloadingAsset = string.Empty;
	}

    /// <summary>
    /// A public function for all game objects to call with the instance of BBundle Manager
    /// </summary>
	/// <param name="currentBundleObject">This will be the current bundle we are loading/queueing</param>
    public void LoadBundle(BBundleObject currentBundleObject)
    {
        string platform = "Windows/";
#if (UNITY_IPHONE)
        platform="iOS/";
#elif (UNITY_ANDROID)
        platform="Android/";
#endif

        currentBundleObject._url = Net.url + "Asset/" + platform+currentBundleObject._url + ".unity3d";

		if (_readyUrls.Contains(currentBundleObject.Url))
        {
			currentBundleObject.Object.SendMessage(currentBundleObject.Callback);
        }
		else if (!_que.Contains(currentBundleObject)) //Starts a new coroutine whenever the url isn't part of the assets
        {
#if DEBUGING_ENABLED
			Debug.Log("Starting Que download");
#endif
			//Que the download here
	        QueDownload(currentBundleObject);
        }
        else
        {
#if DEBUGING_ENABLED
			Debug.Log("Adding to existing Que download");
#endif
			_bundledObjects.Add(currentBundleObject);
        }
    }

	#endregion

	#region Protected API

	protected virtual void OnDownloadStarted()
	{
		EventHandler handler = DownloadStarted;
		if (handler != null)
		{
			handler(this, EventArgs.Empty);
		}
	}

	protected virtual void OnDownloadPercentageChanged()
	{
		EventHandler handler = DownloadPercentageChanged;
		if (handler != null)
		{
			handler(this, EventArgs.Empty);
		}
	}

	protected virtual void OnDownloadComplete()
	{
		EventHandler handler = DownloadCompleted;
		if (handler != null)
		{
			handler(this, EventArgs.Empty);
		}
	}

	protected virtual void OnDownloadFailed()
	{
		EventHandler handler = DownloadFailed;
		if (handler != null)
		{
			handler(this, EventArgs.Empty);
		}
	}

	protected virtual void OnAllDownloadsFinished()
	{
		EventHandler handler = AllDownloadsFinished;
		if (handler != null)
		{
			handler(this, EventArgs.Empty);
		}
	}

	#endregion

	#region Private API

	/// <summary>
	/// Function to call that will que up the current bundle to download
	/// </summary>
	/// <param name="quedBundle"></param>
	private void QueDownload(BBundleObject quedBundle)
	{
#if DEBUGING_ENABLED
		Debug.Log("Attempting to download: " + quedBundle.ToString());
#endif

		if (!_pendingUrls.Contains(quedBundle.Url))
		{
			//Add to que to be downloaded
			_pendingUrls.Add(quedBundle.Url);
			_que.Add(quedBundle);

			FlushQue();
		}
		else
		{
#if DEBUGING_ENABLED
			Debug.Log("Que already contains url, setting aside instead");
#endif

			_bundledObjects.Add(quedBundle);
		}
	}

	/// <summary>
	/// Make sure to flush the que
	/// </summary>
	private void FlushQue()
	{
		if (_que.Count > 0 && !BundleDownloading)
		{
#if DEBUGING_ENABLED
			Debug.Log("Flushing bundle que");
#endif
			BundleDownloading = true;
			DownloadPercent = 0;
			DownloadingAsset = String.Empty;
			StartCoroutine(QuedDownloads(_que[0]));
		}
		else if (_que.Count <= 0)
		{
#if DEBUGING_ENABLED
			Debug.Log("Downloads finished");
#endif
			//Finished all downloads
			OnAllDownloadsFinished();
		}
	}

    /// <summary>
    /// Que up each download so that they are not all downloading at once
    /// </summary>
	/// <param name="currentBundleObject">This will be the current bundle we are loading/queueing</param>
    private IEnumerator QuedDownloads(BBundleObject currentBundleObject)
    {
#if DEBUGING_ENABLED
		Debug.Log("Que started download");
#endif
		OnDownloadStarted();
        yield return new WaitForEndOfFrame();

// 	    string versionURL = currentBundleObject.Url.Remove(currentBundleObject.Url.Length - ".unity3d".Length);
// 	    versionURL += "_ver.txt";
// 		Debug.Log("Version url: " + versionURL);
// 
// 		//TODO: Possibly timeout timer
// 		WWW versionCheck = new WWW(versionURL);
// 
// 	    yield return new WaitForEndOfFrame();
// 
// 	    yield return versionCheck;

	    int versionNumber = 0;

// 	    if (string.IsNullOrEmpty(versionCheck.error))
// 	    {
// 		    if (int.TryParse(versionCheck.text, out versionNumber))
// 		    {
// 			    Debug.Log("Version number successfully pulled: " + versionNumber);
// 		    }
// 	    }
// 	    else
// 	    {
// 		    Debug.LogWarning("Error: " + versionCheck.error);
// 			Debug.Log("Defaulting to bundle object's version");
// 		    versionNumber = currentBundleObject.Version;
// 	    }

	    if (Caching.IsVersionCached(currentBundleObject.Url, versionNumber))
		{
			DownloadingAsset = currentBundleObject.Url;
#if DEBUGING_ENABLED
			Debug.Log(string.Format("Loading asset bundle from cached version on {0}", currentBundleObject.Url));
#endif
		}

#if UNITY_MOBILE_FTP_ENABLED && UNITY_WEBPLAYER
		if (!currentBundleObject.FTPInfo.IsEmpty() && currentBundleObject.Location == BBundleLocationType.FTP && currentBundleObject.FTPCache == BBundleFTPCacheType.None)
		{
			//There is ftp info available, should download from there instead

			if (string.IsNullOrEmpty(currentBundleObject.FTPInfo.Filename))
			{
				//There is no filename, so there cannot be a file to download then
				_pendingUrls.Remove(currentBundleObject.Url);
				_que.Remove(currentBundleObject);
				if (DebugEnabled)
				{
#if DEBUGING_ENABLED
					Debug.LogError("Failed to load bundle at url: " + currentBundleObject.Url);
#endif
				}
				BundleDownloading = false;
				OnDownloadFailed();
				FlushQue();
				yield break;
			}

			string finalPath = currentBundleObject.FTPInfo.HostURL + currentBundleObject.FTPInfo.Filename;
			Debug.Log("Downloading form : " + finalPath);
			FtpWebRequest request =
				(FtpWebRequest)WebRequest.Create(finalPath);
			request.Method = WebRequestMethods.Ftp.DownloadFile;
			request.Credentials = new NetworkCredential(currentBundleObject.FTPInfo.HostUSR,
				currentBundleObject.FTPInfo.HostPWD);
			request.EnableSsl = currentBundleObject.FTPInfo.UseSSL;
			request.KeepAlive = true;
			request.UsePassive = true;
			//request.ServicePoint.ConnectionLeaseTimeout = 1000 * 30;
			//request.ServicePoint.ConnectionLimit = 8;
			request.ReadWriteTimeout = 1000 * 60;
			AssetBundle downloadedAssetBundle;

			//Ensures that we close the response after done
			using (FtpWebResponse response = (FtpWebResponse) request.GetResponse())
			{
				using (Stream responseStream = response.GetResponseStream())
				{
					yield return responseStream;

					List<byte> bytes = new List<byte>(1024*1024);
					byte[] buffer = new byte[2048];
					int readCount = 0;
					while (true)
					{
						readCount = responseStream.Read(buffer, 0, buffer.Length);

						if (readCount == 0)
							break;

						for (int i = 0; i < readCount; i++)
							bytes.Add(buffer[i]);
					}

					downloadedAssetBundle = AssetBundle.CreateFromMemoryImmediate(bytes.ToArray());
					yield return downloadedAssetBundle;
				}
			}
			if (downloadedAssetBundle != null)
			{
				FlushBundle(currentBundleObject, downloadedAssetBundle);
			}
			else
			{
				_pendingUrls.Remove(currentBundleObject.Url);
				_que.Remove(currentBundleObject);
				if (DebugEnabled)
				{
#if DEBUGING_ENABLED
					Debug.LogError("Asset FTP download error: Cannot reach destination or returned back null");
#endif
				}
				BundleDownloading = false;
				OnDownloadFailed();
				FlushQue();
			}
		}
		else
		{
			//No ftp info found, downloading the regular method
#endif
#if DEBUGING_ENABLED
		Debug.Log("Downloading asset: " + currentBundleObject.Url);
#endif
	    DownloadingAsset = currentBundleObject.Url;

		using (WWW bundleDownload = WWW.LoadFromCacheOrDownload(currentBundleObject.Url, versionNumber))
		{
				while (!bundleDownload.isDone)
				{
					if (bundleDownload.progress != DownloadPercent)
					{
						DownloadPercent = bundleDownload.progress;
						OnDownloadPercentageChanged();
					}

					yield return new WaitForEndOfFrame();
				}
				yield return new WaitForEndOfFrame();
				if (!string.IsNullOrEmpty(bundleDownload.error))
				{
					_pendingUrls.Remove(currentBundleObject.Url);
					_que.Remove(currentBundleObject);
#if DEBUGING_ENABLED
#if UNITY_MOBILE_FTP_ENABLED
						if (currentBundleObject.Location == BBundleLocationType.FTP &&
						    currentBundleObject.FTPCache == BBundleFTPCacheType.Anonymous)
						{
							Debug.LogWarning("Are you sure the ftp allows annonymous downloading?");
						}
#endif
					Debug.Log("Asset loaded error: " + bundleDownload.error);
#endif
					BundleDownloading = false;
					OnDownloadFailed();
					FlushQue();
				}
				else
				{
					print (bundleDownload.url);

					if(bundleDownload.assetBundle != null) {
						print (bundleDownload.assetBundle.ToString());
						FlushBundle(currentBundleObject, bundleDownload.assetBundle);
					}
				}
			}
#if UNITY_MOBILE_FTP_ENABLED
		}
#endif
    }

	/// <summary>
	/// Helps remove any bundles that are waiting
	/// </summary>
	private void RemoveOverheadBundles()
	{
		if (_bundledObjects.Count > 0)
		{
			for (int i = 0; i < _bundledObjects.Count; ++i)
			{
				if (_readyUrls.Contains(_bundledObjects[i].Url))
				{
					BBundleObject obj = _bundledObjects[i];
					obj.InitObject();
					_bundledObjects.Remove(obj);
					i--;
				}
			}
		}
	}

#if UNITY_MOBILE_FTP_ENABLED && UNITY_WEBPLAYER
	/// <summary>
	/// Download from an FTP
	/// </summary>
	/// <param name="info">The ftp info we will be using</param>
	private IEnumerator DownloadFromFtp(BBundleObject currentBundleObject)
	{
		AssetBundle downloadedAssetBundle = null;

		if (currentBundleObject.FTPInfo.Filename.Length == 0)
			yield break;

		FtpWebRequest request = (FtpWebRequest)WebRequest.Create(currentBundleObject.FTPInfo.HostURL + currentBundleObject.FTPInfo.Filename);
		request.Method = WebRequestMethods.Ftp.DownloadFile;
		request.Credentials = new NetworkCredential(currentBundleObject.FTPInfo.HostUSR, currentBundleObject.FTPInfo.HostPWD);
		
		//Ensures that we close the response after done
		using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
		{
			yield return response;

			using (Stream responseStream = response.GetResponseStream())
			{
				yield return responseStream;

				using (StreamReader reader = new StreamReader(responseStream))
				{
					byte[] bytes = System.Text.Encoding.UTF8.GetBytes(reader.ReadToEnd());
					downloadedAssetBundle = AssetBundle.CreateFromMemoryImmediate(bytes);
				}
			}
		}

		if (downloadedAssetBundle != null)
		{
			FlushBundle(currentBundleObject, downloadedAssetBundle);
		}
		else if (DebugEnabled)
		{
#if DEBUGING_ENABLED
			Debug.LogError("Asset FTP download error: Cannot reach destination or returned back null");
#endif
		}
	}
#endif

	/// <summary>
	/// Makes sure to flush out any bundles that have been loaded
	/// </summary>
	/// <param name="currentBundleObject"></param>
	/// <param name="downloadedAssetBundle"></param>
	private void FlushBundle(BBundleObject currentBundleObject, AssetBundle downloadedAssetBundle)
	{
		Object[] assets = downloadedAssetBundle.LoadAllAssets();
		foreach (Object innerAsset in assets)
		{
			if (!_bundles.ContainsKey(innerAsset.name))
			{
				_bundles.Add(innerAsset.name, innerAsset);
			}
			else
			{
				_bundles.Add(innerAsset.name + innerAsset.GetType().ToString(), innerAsset);
                Debug.LogWarning("Asset bundle has two assets with the same name (" + innerAsset.name + ") , please check your asset bundle!");
			}
		}

		downloadedAssetBundle.Unload(false);
		_pendingUrls.Remove(currentBundleObject.Url);
		_que.Remove(currentBundleObject);
		_readyUrls.Add(currentBundleObject.Url);
		currentBundleObject.Object.SendMessage(currentBundleObject.Callback);

#if DEBUGING_ENABLED
		Debug.Log("Asset Bundle loaded from url: " + currentBundleObject.Url);
#endif

		DownloadedAssets++;
		RemoveOverheadBundles(); 
		BundleDownloading = false;
		OnDownloadComplete();
		FlushQue();
	}

	private IEnumerator DownloadFromAWS(BBundleObject currentBundleObject)
	{
		AssetBundle downloadedAssetBundle = null;

		WWW awsRequest = new WWW(currentBundleObject.Url); //new WWW(info.Bucket + '.' + info.AWSHost + '/' + info.FilePath);

		yield return awsRequest;

		if (!string.IsNullOrEmpty(awsRequest.error))
		{
			//Errors
			Debug.LogError(awsRequest.error);
		}
		else
		{
			//No Errors, load the bundle
			downloadedAssetBundle = awsRequest.assetBundle;

			if (downloadedAssetBundle != null)
			{
				Object[] assets = downloadedAssetBundle.LoadAllAssets();
				foreach (Object innerAsset in assets)
				{
					if (!_bundles.ContainsKey(innerAsset.name))
					{
						_bundles.Add(innerAsset.name, innerAsset);
					}
					else
					{
						_bundles.Add(innerAsset.name + innerAsset.GetType().ToString(), innerAsset);
					}
				}

				downloadedAssetBundle.Unload(false);
				_readyUrls.Add(currentBundleObject.Url);
				currentBundleObject.Object.SendMessage(currentBundleObject.Callback);

#if DEBUGING_ENABLED
				Debug.Log("Asset Bundle loaded from url: " + currentBundleObject.Url);
#endif

				RemoveOverheadBundles();
			}
#if DEBUGING_ENABLED
			Debug.LogError("Asset AWS download error: Cannot reach destination or returned back null");
#endif
		}
	}

	#endregion
}
