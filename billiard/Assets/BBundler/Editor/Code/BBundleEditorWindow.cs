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

#if UNITY_SUPPORTS_FTP_MOBILE
	#define UNITY_MOBILE_FTP_ENABLED
#endif

#endregion

using System.Net;
using System.Text;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public class BBundleEditorWindow : EditorWindow 
{
    /// <summary>
    /// Current menu state
    /// </summary>
    private enum CurrentBBundleMenu
    {
        Loading,
        Main,
        Create,
        AssetDatabase,
        Internet,
        Building,
        Documentation,
		Upload,
		UploadFTP,
		UploadAWS,
		FTPAWS,
		FTPList,
		AWS,
		Settings,
        Destroy
    };

	#region Unity Built in API calls

    /// <summary>
    /// Show the editor, otherwise close it
    /// </summary>
    [MenuItem("Window/BBundle/Editor %g")]
    public static void ShowEditor()
    {
        if (Instance.IsVisible)
        {
			EditorPrefs.SetBool("BBundleEditorOpened", false);
            Instance.Close();
        }
        else
        {
	        EditorPrefs.SetBool("BBundleEditorOpened", true);
            Instance.Show();
            Instance.Focus();
            Instance.IsVisible = true;
        }
    }

	[MenuItem("AssetBundles/Clear Cache")]
	public static void ClearAssetCache()
	{
		Caching.ClearCache();
		Debug.Log("Asset bundle Cache cleared");
	}

    /// <summary>
    /// Draw the menus
    /// </summary>
    private void OnGUI()
    {
        DrawMenus();
    }

    /// <summary>
    /// Update the menus if needed
    /// </summary>
    private void Update()
    {
        UpdateMenus();
    }

    /// <summary>
    /// Unload all assets from the editor for memory leak purposes
    /// </summary>
    private void OnDestroy()
    {
        if ((Application.isPlaying == false) && (Application.isEditor == true) && (Application.isLoadingLevel == false))
        {
			EditorPrefs.SetBool("BBundleEditorOpened", false);
	        if (_instance != null)
	        {
		        _instance.UnloadAssets();
	        }
        }
    }
	
	#endregion
	
	#region Properties

    /// <summary>
    /// Get the current BBundle Editor Instance
    /// </summary>
    private static BBundleEditorWindow _instance;
    public static BBundleEditorWindow Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = EditorWindow.GetWindow(typeof(BBundleEditorWindow)) as BBundleEditorWindow;
                _instance.UnloadAssets();
                _instance.Initialize();
				System.Net.ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => { return true; };
            }

            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    #endregion
	
	#region Public Data

    /// <summary>
    /// Is this window visible?
    /// </summary>
    public bool IsVisible = false;

	#endregion
	
	#region Private Data

    /// <summary>
    /// Private list of all the textures loaded
    /// </summary>
    private Dictionary<string, object> _textureData = new Dictionary<string, object>();

    /// <summary>
    /// Current menu state
    /// </summary>
    private CurrentBBundleMenu _currentMenu = CurrentBBundleMenu.Loading;

    /// <summary>
    /// Just some styles that are being used
    /// </summary>
    private GUIStyle _emtpy = new GUIStyle();
    private GUIStyle _boldWhite = new GUIStyle();

    /// <summary>
    /// Check if the window needs to be repainted
    /// </summary>
    private bool _firstRepaint = false;

    /// <summary>
    /// Allow build options with the bundle being built
    /// </summary>
    private bool _changeBuildOptions = false;

    /// <summary>
    /// Check if the user is able to build
    /// </summary>
    private bool _buildReady = false;

    /// <summary>
    /// Check to see if the user is browsing an asset bundle
    /// </summary>
    private bool _browsing = false;

    /// <summary>
    /// All the asset bundle objects that is either loaded or to be saved
    /// </summary>
    private List<UnityEngine.Object> _assetBundleObjects = new List<UnityEngine.Object>();

    /// <summary>
    /// The designated main asset
    /// </summary>
    private UnityEngine.Object _mainAsset;

    /// <summary>
    /// The asset names
    /// </summary>
    private List<string> _assetNames = new List<string>();

    /// <summary>
    /// The scrolling window position
    /// </summary>
    private Vector2 _scrollPos = Vector2.zero;
    
    /// <summary>
    /// Target platform that the asset bundle is going to be built for
    /// </summary>
	private BuildTarget _targetPlatform = BuildTarget.WebGL;

    /// <summary>
    /// The current build options for this bundle, disabled by default
    /// </summary>
    private BuildAssetBundleOptions _currentOptions = BuildAssetBundleOptions.CollectDependencies;

    /// <summary>
    /// Textures of the BBundle Editor
    /// </summary>
    private Texture2D _add;
    private Texture2D _subtract;
    private Texture2D _internet;

	/// <summary>
	/// Asset we are going to upload
	/// </summary>
	private Object _assetToUpload;

    /// <summary>
    /// The asset path that it will be saved to
    /// </summary>
    private string _assetPath = "";

    /// <summary>
    /// The files in the asset path
    /// </summary>
    private string[] _assetFiles;

    /// <summary>
    /// The documentation notes
    /// </summary>
    private string _documentation = "";

    /// <summary>
    /// The asset bundle name
    /// </summary>
    private string _assetBundleName = "";

	/// <summary>
	/// The url for where the documentation is located
	/// </summary>
	private const string DOCUMENTATION_URL = "https://dl.dropboxusercontent.com/u/73856822/BBundle%20Editor%20Manual.pdf";

	#region AMAZON S3

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

	/// <summary>
	/// AWS Default URL
	/// </summary>
	public const string AWS_DEFAULT_URL = "https://s3.amazonaws.com/";

	/// <summary>
	/// This is the access key for s3
	/// </summary>
	private string _amazonLoginAccessKey = "";

	/// <summary>
	/// This is the secret key for s3
	/// </summary>
	private string _amazonLoginSecretKey = "";

	/// <summary>
	/// Region of Amazon services
	/// </summary>
	//private string _amazonRegion = "s3";

	/// <summary>
	/// Bucket directory name
	/// </summary>
	//private string _amazonBucket = "";

	/// <summary>
	/// Determine if the user is truly logged in
	/// </summary>
	private bool _AWSloginSuccessful = false;

	/// <summary>
	/// The list of buckets on the given S3
	/// </summary>
	private List<string> _AWSBucketList;

	/// <summary>
	/// ID of the current bucket
	/// </summary>
	private int _AWSBucketID;

	/// <summary>
	/// The id of the current uploading user
	/// </summary>
	private string _AWSOwnerID;

	#endregion

	#region FTP Info

	/// <summary>
	/// The ftp url to upload to
	/// </summary>
	private string _ftpHostUrl;

	/// <summary>
	/// Username for this session
	/// </summary>
	private string _ftpUsername;

	/// <summary>
	/// Password for this user
	/// </summary>
	private string _ftpPassword;

	/// <summary>
	/// Directory to upload the file to
	/// </summary>
	private string _ftpDirectory;

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

	#endregion

	/// <summary>
    /// The add name reference
    /// </summary>
    private const string AddImageName = "Add";

    /// <summary>
    /// The subtract name reference
    /// </summary>
    private const string SubtractImageName = "Subtract";

    /// <summary>
    /// The internet name reference
    /// </summary>
    private const string InternetImageName = "Internet";

	#endregion
	
	#region Private API

    /// <summary>
    /// Initialize the window
    /// </summary>
    private void Initialize()
    {
        this.title = "BBundle Editor";
        minSize = new Vector2(400, 400);
        _textureData.Clear();
        _scrollPos = Vector2.zero;
        _boldWhite = new GUIStyle();
        _boldWhite.normal.textColor = Color.white;
        _boldWhite.fontStyle = FontStyle.Bold;
        _boldWhite.fontSize = 14;
        _boldWhite.alignment = TextAnchor.MiddleCenter;
		_AWSBucketList = new List<string>();
        
        wantsMouseMove = true;

        GetEditorResourcesArtPath();
        //GetEditorDocumentationPath();
    }

    /// <summary>
    /// Get the art directory filepath
    /// </summary>
    /// <returns></returns>
    private string GetEditorResourcesArtPath()
    {
        string result = "";

		result += Application.dataPath + "/BBundler/Editor/Art/";

        if (Directory.Exists(result))
        {
            LoadEditorArtFiles(result); //Trim off (.png)
        }
        else
        {
            Debug.LogError("Invalid path! Did you move the BBundlerAsset Folder?");
            result = "";
        }

        return result;
    }

    /// <summary>
    /// Documentation text
    /// </summary>
    /// <returns></returns>
    private string GetEditorDocumentationPath()
    {
		string dir = Application.dataPath + "/BBundler/Editor/Documentation/";
		string result = Application.dataPath + "/BBundler/Editor/Documentation/doc.txt";

        if (Directory.Exists(dir))
        {
            WWW fileNode = new WWW("file://" + result);

            while (!fileNode.isDone)
            {
                if (EditorUtility.DisplayCancelableProgressBar("Loading BBundler Editor Art Assets", "Loading... " + result, fileNode.progress))
                {
                    Debug.Log("Canceled loading BBundler Editor, if you have any problems please email: brfaulds@gmail.com");
                    break;
                }
            }

            if (EditorUtility.DisplayCancelableProgressBar("Loading BBundler Editor Art Assets", "Loading... " + fileNode.texture.name, fileNode.progress))
            {
                Debug.Log("Canceled loading BBundler Editor, if you have any problems please email: brfaulds@gmail.com");
            }

            _documentation = fileNode.text;

            EditorUtility.ClearProgressBar();
            //LoadEditorArtFiles(result); //Trim off (.txt)
        }
        else
        {
            Debug.LogError("Invalid path! Did you move the BBundlerAsset Folder?");
            result = "";
        }

        return result;
    }

    /// <summary>
    /// Load the art files from the asset path
    /// </summary>
    /// <param name="assetPath"></param>
    private void LoadEditorArtFiles(string assetPath)
    {
        string[] aFilePaths = Directory.GetFiles(assetPath);
        _textureData = new Dictionary<string, object>();

        bool canceled = false;
        foreach (string asset in aFilePaths)
        {
            if (asset.Contains("meta")) //Skip meta files
                continue;

            WWW fileNode = new WWW("file://"+asset);
            
            while (!fileNode.isDone)
            {
                if (EditorUtility.DisplayCancelableProgressBar("Loading BBundler Editor Art Assets", "Loading... " + asset, fileNode.progress))
                {
                    Debug.Log("Canceled loading BBundler Editor, if you have any problems please email: brfaulds@gmail.com");
                    canceled = true;
                    break;
                }
            }

            if (EditorUtility.DisplayCancelableProgressBar("Loading BBundler Editor Art Assets", "Loading... " + fileNode.texture.name, fileNode.progress))
            {
                Debug.Log("Canceled loading BBundler Editor, if you have any problems please email: brfaulds@gmail.com");
                canceled = true;
            }

            if (canceled)
                break;

            string finalName = fileNode.url.Remove(0, assetPath.Length + 7);
            finalName = finalName.Remove(finalName.Length - 4);
            
            _textureData.Add(finalName, fileNode.texture);
        }
        EditorUtility.ClearProgressBar();

        if (canceled)
        {
            Close();
        }

        _add = _textureData[AddImageName] as Texture2D;
        _subtract = _textureData[SubtractImageName] as Texture2D;
        _internet = _textureData[InternetImageName] as Texture2D;

        _scrollPos = Vector2.zero;

        _currentMenu = CurrentBBundleMenu.Main;
    }

    /// <summary>
    /// Get the art files from an asset path with a trim off the end (.png, etc)
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="trimOff"></param>
    /// <returns></returns>
    private string GetArtFileNames(string assetPath, int trimOff)
    {
        string final = "";

        //string sDataPath = Application.dataPath;
        //string sFolderPath = sDataPath.Substring(0, sDataPath.Length - 6) + assetPath;

        string[] aFilePaths = Directory.GetFiles(assetPath);

        if (aFilePaths.Length > 0)
        {
            for (int i = 0; i < aFilePaths.Length; ++i)
            {
                string sAssetPath = aFilePaths[i].Remove(0, assetPath.Length);//sFilePath.Substring(sDataPath.Length - 6);
                if (sAssetPath.Contains(".meta")) //Ignore meta files, don't include them in the bundle (unless you want to include them)
                    continue;
                
				sAssetPath = sAssetPath.Remove(sAssetPath.Length - trimOff, trimOff); //Trim off the .png part

                //Debug.Log(sFilePath);
                if (i + 1 < aFilePaths.Length - 1)
                    final += sAssetPath + ", ";
                else
                    final += sAssetPath;
            }
        }
        else
        {
            Debug.Log("No files in path");
        }

        return final;
    }

    /// <summary>
    /// Get the filenames in one string
    /// </summary>
    /// <param name="assetPath">Location</param>
    /// <param name="trimOff">Trim off amount</param>
    /// <returns></returns>
    private static string GetFiles(string assetPath, int trimOff)
    {
        string final = "";

        string sDataPath = Application.dataPath;
        string sFolderPath = sDataPath.Substring(0, sDataPath.Length - 6) + assetPath;

        string[] aFilePaths = Directory.GetFiles(sFolderPath);

        foreach (string sFilePath in aFilePaths)
        {
            string sAssetPath = sFilePath.Remove(0, sFolderPath.Length + 1);//sFilePath.Substring(sDataPath.Length - 6);
            if (sAssetPath.Contains(".meta")) //Ignore meta files, don't include them in the bundle (unless you want to include them)
                continue;
            else
                sAssetPath = sAssetPath.Remove(sAssetPath.Length - trimOff, trimOff); //Trim off the .png part
            //Debug.Log(sFilePath);
            final += '"' + sAssetPath + '"' + ", ";
        }

        return final;
    }

    /// <summary>
    /// Get the files located in a directory folder path
    /// </summary>
    /// <param name="assetPath">Directory Filepath</param>
    /// <returns></returns>
    private static string[] GetFiles(string assetPath)
    {
        string sDataPath = Application.dataPath;
        string sFolderPath = sDataPath + assetPath;

        string[] aFilePaths = Directory.GetFiles(sFolderPath);
        List<string> finalResult = new List<string>();
        if (aFilePaths.Length > 0)
        {
            for (int i = 0; i < aFilePaths.Length; ++i)
            {
                if (!aFilePaths[i].Contains(".meta"))
                    finalResult.Add(aFilePaths[i]);
            }
        }

        return finalResult.ToArray();
    }

    /// <summary>
    /// Draw the menus depending on the current state
    /// </summary>
    private void DrawMenus()
    {
        switch (_currentMenu)
        {
            case CurrentBBundleMenu.Loading:
                LoadingMenu();
                break;
            case CurrentBBundleMenu.Main:
                MainMenu();
                break;
            case CurrentBBundleMenu.Create:
                Creation();
                break;
            case CurrentBBundleMenu.AssetDatabase:
                EditAssetBundlesMenu();
                break;
            case CurrentBBundleMenu.Internet:
                LoadingMenu();
                break;
            case CurrentBBundleMenu.Building:
                BuildingMenu();
                break;
            case CurrentBBundleMenu.Documentation:
                DocumentationMenu();
                break;
			case CurrentBBundleMenu.Upload:
		        UploadMenu();
		        break;
			case CurrentBBundleMenu.UploadFTP:
				UploadFTPMenu();
				break;

			case CurrentBBundleMenu.FTPAWS:
		        FTPAWSListMenu();
		        break;
			case CurrentBBundleMenu.FTPList:
		        FTPListMenu();
		        break;
			case CurrentBBundleMenu.AWS:
		        AWSMenu();
		        break;
			case CurrentBBundleMenu.Settings:
		        SettingsMenu();
		        break;
        }
    }

    /// <summary>
    /// Update the menus depending on the state
    /// </summary>
    private void UpdateMenus()
    {
        switch (_currentMenu)
        {
            case CurrentBBundleMenu.Building:
                BuildingMenuUpdate();
                break;
        }
    }

    /// <summary>
    /// The Loading menu GUI
    /// </summary>
    private void LoadingMenu()
    {
        GUILayout.Label("Loading Editor Art Assets...", EditorStyles.boldLabel);
    }

    /// <summary>
    /// The Building menu GUI
    /// </summary>
    private void BuildingMenu()
    {
        GUILayout.Label("Building bundle assets...", EditorStyles.boldLabel);
    }

    /// <summary>
    /// The building menu update
    /// </summary>
    private void BuildingMenuUpdate()
    {
        if (_buildReady)
        {
            _buildReady = false;
            _assetPath = Application.dataPath + "/BBundler/Export/" + _assetBundleName + ".unity3d";
            Debug.Log("Building to: " + _assetPath);
			//_originalPath = "Assets/BBundler/Export/" + _assetBundleName + ".unity3d";

            //Create a backup just incase the user wants to revert
            if (File.Exists(_assetPath))
            {
				string backupPath = Application.dataPath + "/BBundler/Export/Backups/" + _assetBundleName + ".unity3d.bak";
				string backupDirectory = Application.dataPath + "/BBundler/Export/Backups/";
                if (!Directory.Exists(backupDirectory))
                    Directory.CreateDirectory(backupDirectory);
                File.Copy(_assetPath, backupPath, true);
            }

            BuildAssetBundleOptions options = BuildAssetBundleOptions.CollectDependencies;

            if (_changeBuildOptions)
                options = _currentOptions;

            if (BuildPipeline.BuildAssetBundle(_mainAsset, _assetBundleObjects.ToArray(), _assetPath, options, _targetPlatform))
            {
                Debug.Log("Build Succeeded");

				string versionPath = Application.dataPath + "/BBundler/Export/" + _assetBundleName + "_ver.txt";
	            if (File.Exists(versionPath)) //Update the version since it exists
	            {
		            int versionNumber = 0;
					using (StreamReader sr = new StreamReader(File.OpenRead(versionPath)))
					{
						string line = sr.ReadToEnd();
						int.TryParse(line, out versionNumber);
						versionNumber++;
					}

		            using (StreamWriter sw = new StreamWriter(File.OpenWrite(versionPath)))
		            {
			            sw.Write(versionNumber);
		            }
	            }
	            else
	            {
		            //Create first iteration of text
		            using (StreamWriter sw = new StreamWriter(File.Create(versionPath)))
		            {
						sw.Write(0);
		            }
	            }

                _currentMenu = CurrentBBundleMenu.Main;
                _mainAsset = null;
                _assetBundleObjects.Clear();
                _assetNames.Clear();
                _browsing = false;
                _assetPath = "";
                _assetBundleName = "";
                _changeBuildOptions = false;
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.Log("Build failed");
                _currentMenu = CurrentBBundleMenu.Create;
            }
        }
    }

    /// <summary>
    /// The creation window
    /// </summary>
    private void Creation()
    {
        if (!_browsing)
        {
            _assetBundleName = EditorGUILayout.TextField("Bundle Save Name: ", _assetBundleName);
            _targetPlatform = (BuildTarget)EditorGUILayout.EnumPopup("Target Platform: ", _targetPlatform);

            if (_assetBundleName.Length < 1)
            {
                EditorGUILayout.LabelField("Must have a save name before dragging assets here!", _boldWhite);
                _scrollPos = GUILayout.BeginScrollView(_scrollPos, _emtpy);
                GUILayout.EndScrollView();
                if (GUILayout.Button("Back", GUILayout.Height(75)))
                {
                    _currentMenu = CurrentBBundleMenu.Main;
                    _mainAsset = null;
                    _assetBundleObjects = new List<UnityEngine.Object>();
                    _assetNames = new List<string>();
                    _browsing = false;
                    _assetPath = "";
                }

                return;
            }
        }

        if (!_browsing && this.ShowDragging(true, _boldWhite))
        {
            //Objects being dragged
            _firstRepaint = true;
        }
        else
        {
            if (_firstRepaint)
            {
                //Add objects here and check what was loaded!
                if (DragOnWindow.objects != null)
                {
                    foreach (object loaded in DragOnWindow.objects)
                    {
                        //Debug.Log(loaded);
                        _assetBundleObjects.Add(loaded as UnityEngine.Object);
                        if (!_mainAsset)
                            _mainAsset = _assetBundleObjects[_assetBundleObjects.Count - 1];
                        //LoadedAsset = loaded as Texture2D;
                        //Debug.Log(AssetDatabase.GetAssetPath(loaded as UnityEngine.Object));
                    }
                }

                _firstRepaint = false;
                Repaint();
                return;
            }

            _scrollPos = GUILayout.BeginScrollView(_scrollPos, _emtpy);

            if (_assetBundleObjects.Count > 0)
            {
                for (int i = 0; i < _assetBundleObjects.Count; ++i)
                {
                    GUILayout.Label(_assetBundleObjects[i].name + " - " + _assetBundleObjects[i].GetType().Name.ToString());
                    GUILayout.BeginHorizontal();

                    if (!_browsing && GUILayout.Button("?", GUILayout.Width(20)))
                    {
                        Debug.Log("Path: " + AssetDatabase.GetAssetPath(_assetBundleObjects[i]));
                    }

                    if (!_browsing && _mainAsset == _assetBundleObjects[i])
                    {
                        GUILayout.Toggle(true, "Main Asset");
                    }
                    else if (!_browsing)
                    {
                        if (GUILayout.Toggle(false, "Main Asset"))
                            _mainAsset = _assetBundleObjects[i];
                    }

                    GUILayout.Space(this.position.width / 2.5f);

                    if (!_browsing && GUILayout.Button(_subtract, GUILayout.Width(100)))
                    {
                        //Debug.Log("Removed - " + _assetBundleObjects[i].name);

                        if (_mainAsset == _assetBundleObjects[i])
                        {
                            _mainAsset = null;
                            _assetBundleObjects.Remove(_assetBundleObjects[i]);
                            if (_assetBundleObjects.Count > 0)
                            {
                                _mainAsset = _assetBundleObjects[_assetBundleObjects.Count - 1];
                            }
                        }
                        else
                            _assetBundleObjects.Remove(_assetBundleObjects[i]);

                        return;
                    }
                    GUILayout.EndHorizontal();

                    if (i + 1 < _assetBundleObjects.Count)
                        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
                }
            }
            else
                GUILayout.Label("Drag and drop assets into this window to add them.", EditorStyles.boldLabel);

            GUILayout.EndScrollView();

            if (_mainAsset != null && !_browsing && _assetBundleName.Length > 0)
            {
                _changeBuildOptions = GUILayout.Toggle(_changeBuildOptions, "Enable Build Options");

                if (_changeBuildOptions)
                    _currentOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup("Build Options: ", _currentOptions);
            }

            if (_mainAsset != null && !_browsing && _assetBundleName.Length > 0 && GUILayout.Button("Build Bundle", GUILayout.Height(50)))
            {
                _currentMenu = CurrentBBundleMenu.Building;
                _buildReady = true;
            }

            if (GUILayout.Button("Back", GUILayout.Height(75)))
            {
                _currentMenu = CurrentBBundleMenu.Main;
                _mainAsset = null;
                _assetBundleObjects = new List<UnityEngine.Object>();
                _assetNames = new List<string>();
                _browsing = false;
                _assetPath = "";
            }
        }
    }

    /// <summary>
    /// The Main menu
    /// </summary>
    private void MainMenu()
    {
        if (GUILayout.Button("Create", GUILayout.Height(50)))
        {
            _scrollPos = Vector2.zero;
            _mainAsset = null;
            _assetBundleObjects = new List<UnityEngine.Object>();
            _currentMenu = CurrentBBundleMenu.Create;
        }

        if (GUILayout.Button("View Bundle", GUILayout.Height(50)))
        {
            //Get filepaths for bundles
			_assetPath = "/BBundler/Export/";
            _assetFiles = GetFiles(_assetPath);

            _scrollPos = Vector2.zero;
            _currentMenu = CurrentBBundleMenu.AssetDatabase;
        }

		// FTP & AmazonAWS (s3)
		if (GUILayout.Button("Upload Bundle (FTP/S3)", GUILayout.Height(50)))
		{
			_scrollPos = Vector2.zero;
			_mainAsset = null;
			_assetBundleObjects = new List<UnityEngine.Object>();

			//if (!string.IsNullOrEmpty(_originalPath))
			//{
			//	_assetToUpload = GetAssetFromPath(_originalPath);
			//	_originalPath = string.Empty;
			//}

			_currentMenu = CurrentBBundleMenu.Upload;
		}

#if UNITY_MOBILE_FTP_ENABLED
		if (GUILayout.Button("FTP/AWS Setup", GUILayout.Height(50)))
#else
		if (GUILayout.Button("AWS Setup", GUILayout.Height(50)))
#endif
		{
			_scrollPos = Vector2.zero;
			_currentMenu = CurrentBBundleMenu.FTPAWS;
		}

        if (GUILayout.Button("Documentation", GUILayout.Height(50)))
        {
            //_scrollPos = Vector2.zero;
            //_currentMenu = CurrentBBundleMenu.Documentation;
			Application.OpenURL(DOCUMENTATION_URL);
        }
		
        _scrollPos = GUILayout.BeginScrollView(_scrollPos, _emtpy);
        GUILayout.EndScrollView();

		if (GUILayout.Button("Settings", GUILayout.Height(50)))
		{
			_scrollPos = Vector2.zero;
			_currentMenu = CurrentBBundleMenu.Settings;
		}

        if (GUILayout.Button("Close BBundle Editor", GUILayout.Height(75)))
        {
			CleanupEditor();
        }
    }

	/// <summary>
	/// Get the filepath for a given asset
	/// </summary>
	/// <param name="path"></param>
	private Object GetAssetFromPath(string path)
	{
		Object selection = null;

		foreach (Object obj in Resources.FindObjectsOfTypeAll<Object>())
		{
			if (obj == null || string.IsNullOrEmpty(obj.name) || obj.name.Length <= 0)
				continue;

			string assetPath = AssetDatabase.GetAssetPath(obj);

			if (assetPath.Length <= 0)
				continue;
			else if (!assetPath.Contains(".unity3d"))
				continue;

			if (assetPath.Equals(path))
			{
				selection = obj;
				break;
			}
		}

		if (selection == null)
			return null;

		Selection.objects = new Object[] { selection };
		return selection;
	}

	private bool IsUnityBundledObject(Object obj)
	{
		string assetPath = AssetDatabase.GetAssetPath(obj);

		if (!assetPath.Contains(".unity3d"))
			return false;

		return true;
	}

    /// <summary>
    /// The View/(Maybe in the future Edit) window
    /// </summary>
    private void EditAssetBundlesMenu()
    {
        _scrollPos = GUILayout.BeginScrollView(_scrollPos, _emtpy);

        if (_assetFiles.Length > 0)
        {
            for (int i = 0; i < _assetFiles.Length; ++i)
            {
                if (GUILayout.Button(_assetFiles[i], GUILayout.Height(30)))
                {
                    //Load this asset bundle
                    _currentMenu = CurrentBBundleMenu.Loading;
                    LoadAssets(_assetFiles[i]);
                    break;
                }
            }
        }

        GUILayout.EndScrollView();

        if (GUILayout.Button("Back", GUILayout.Height(75)))
        {
            _currentMenu = CurrentBBundleMenu.Main;
        }
    }

    /// <summary>
    /// The documentation window
    /// </summary>
    private void DocumentationMenu()
    {
        _scrollPos = GUILayout.BeginScrollView(_scrollPos, _emtpy);

        GUILayout.TextArea(_documentation, _boldWhite);

        GUILayout.EndScrollView();

        if (GUILayout.Button("Back", GUILayout.Height(75)))
        {
            _currentMenu = CurrentBBundleMenu.Main;
        }
    }

	/// <summary>
	/// The upload window
	/// </summary>
	private void UploadMenu()
	{
		if (GUILayout.Button("FTP Upload", GUILayout.Height(75)))
		{
			_ftpHostUrl = EditorPrefs.GetString("BBundleEditorFTPURL", "ftp://127.0.0.1/");
			_ftpUsername = EditorPrefs.GetString("BBundleEditorFTPUSR", string.Empty);
			_ftpPassword = EditorPrefs.GetString("BBundleEditorFTPPWD", string.Empty);
			_ftpDirectory = EditorPrefs.GetString("BBundleEditorFTPDIR", string.Empty);

			_currentMenu = CurrentBBundleMenu.UploadFTP;
		}

		if (GUILayout.Button("Amazon AWS (s3)", GUILayout.Height(75)))
		{
			_amazonLoginAccessKey = EditorPrefs.GetString("BBundleEditorAWSAcessKey", string.Empty);
			_amazonLoginSecretKey = EditorPrefs.GetString("BBundleEditorAWSSecretKey", string.Empty);

			_currentMenu = CurrentBBundleMenu.UploadAWS;
		}

		_scrollPos = GUILayout.BeginScrollView(_scrollPos, _emtpy);
		GUILayout.EndScrollView();

		if (GUILayout.Button("Back", GUILayout.Height(75)))
		{
			_currentMenu = CurrentBBundleMenu.Main;
		}
	}

	/// <summary>
	/// The upload (FTP) window
	/// </summary>
	private void UploadFTPMenu()
	{
		_assetToUpload = EditorGUILayout.ObjectField(_assetToUpload, typeof (Object), true);

		if (_assetToUpload != null && !IsUnityBundledObject(_assetToUpload))
		{
			_assetToUpload = null;
			Debug.LogWarning("Not a asset bundle");
		}
		else if (_assetToUpload != null)
		{
			_ftpHostUrl = EditorGUILayout.TextField(new GUIContent("Ftp URL", "i.e. ftp://127.0.0.1/"), _ftpHostUrl);
			_ftpDirectory = EditorGUILayout.TextField(new GUIContent("Ftp Directory", "Directory to save to"), _ftpDirectory);
			_ftpUsername = EditorGUILayout.TextField(new GUIContent("Username", "Username to login with"), _ftpUsername);
			_ftpPassword = EditorGUILayout.PasswordField(new GUIContent("Password", "Password to login with"), _ftpPassword);

			if (!string.IsNullOrEmpty(_ftpHostUrl) && !string.IsNullOrEmpty(_ftpDirectory) &&
				!string.IsNullOrEmpty(_ftpUsername) && !string.IsNullOrEmpty(_ftpPassword))
			{
				GUI.color = Color.green;
				if (GUILayout.Button("Upload", GUILayout.Height(50)))
				{
					SaveFTPPrefs();
					//All is good, time to upload
					bool success = UploadFTPFile(_assetToUpload);
					if (success)
					{
						Debug.Log("Uploaded Succeeded");
						_assetToUpload = null;
					}
					else
						Debug.LogError("Upload Failed, please check the url, username, password, and directory is set correctly");
				}
			}
			else
			{
				GUI.color = Color.grey;
				if (GUILayout.Button("Upload", GUILayout.Height(50)))
				{
					SaveFTPPrefs();
					if (IsDataGood("Ftp URL", _ftpHostUrl) && IsDataGood("Directory", _ftpDirectory) && 
						IsDataGood("Username", _ftpUsername) && IsDataGood("Password", _ftpPassword))
					{
					}
				}
			}

			GUI.color = Color.white;
		}
		else
		{
			GUI.color = Color.grey;
			if (GUILayout.Button("Upload", GUILayout.Height(50)))
			{

			}
			GUI.color = Color.white;
		}

		_scrollPos = GUILayout.BeginScrollView(_scrollPos, _emtpy);
		GUILayout.EndScrollView();
		if (GUILayout.Button("Back", GUILayout.Height(75)))
		{
			_assetToUpload = null;
			_currentMenu = CurrentBBundleMenu.Upload;
		}
	}

	/// <summary>
	/// Saves the editor prefs for the ftp data
	/// </summary>
	private void SaveFTPPrefs()
	{
		EditorPrefs.SetString("BBundleEditorFTPURL", _ftpHostUrl);
		EditorPrefs.SetString("BBundleEditorFTPUSR", _ftpUsername);
		EditorPrefs.SetString("BBundleEditorFTPPWD", _ftpPassword);
		EditorPrefs.SetString("BBundleEditorFTPDIR", _ftpDirectory);
	}

	/// <summary>
	/// Deletes the FTP editor prefs
	/// </summary>
	private void DeleteFTPPrefs()
	{
		EditorPrefs.DeleteKey("BBundleEditorFTPURL");
		EditorPrefs.DeleteKey("BBundleEditorFTPUSR");
		EditorPrefs.DeleteKey("BBundleEditorFTPPWD");
		EditorPrefs.DeleteKey("BBundleEditorFTPDIR");

		int ftpCount = EditorPrefs.GetInt(FTP_COUNT);
		for (int i = 0; i < ftpCount; ++i)
		{
			EditorPrefs.DeleteKey(FTP_NAME + i);
			EditorPrefs.DeleteKey(FTP_URL + i);
			EditorPrefs.DeleteKey(FTP_USR + i);
			EditorPrefs.DeleteKey(FTP_PWD + i);
		}
		EditorPrefs.DeleteKey(FTP_COUNT);
	}

	/// <summary>
	/// Saves the editor prefs for the AWS data
	/// </summary>
	private void SaveAWSPrefs()
	{
		 EditorPrefs.SetString("BBundleEditorAWSAcessKey", _amazonLoginAccessKey);
		 EditorPrefs.SetString("BBundleEditorAWSSecretKey", _amazonLoginSecretKey);
	}

	/// <summary>
	/// Deletes the AWS editor prefs
	/// </summary>
	private void DeleteAWSPrefs()
	{
		EditorPrefs.DeleteKey("BBundleEditorAWSAcessKey");
		EditorPrefs.DeleteKey("BBundleEditorAWSSecretKey");
		int awsCount = EditorPrefs.GetInt(AWS_COUNT);
		for (int i = 0; i < awsCount; ++i)
		{
			EditorPrefs.DeleteKey(AWS_NAME + i);
			EditorPrefs.DeleteKey(AWS_URL + i);
			EditorPrefs.DeleteKey(AWS_BUCKET + i);
		}
		EditorPrefs.DeleteKey(AWS_COUNT);
	}


	/// <summary>
	/// For choosing FTP or AWS setup
	/// </summary>
	private void FTPAWSListMenu()
	{
#if UNITY_MOBILE_FTP_ENABLED
		if (GUILayout.Button("FTP Servers (Add/Remove)", GUILayout.Height(75)))
		{
			_currentMenu = CurrentBBundleMenu.FTPList;
		}
#endif
		if (GUILayout.Button("Amazon AWS (s3) (Setup)", GUILayout.Height(75)))
		{
			_currentMenu = CurrentBBundleMenu.AWS;
		}

		_scrollPos = GUILayout.BeginScrollView(_scrollPos, _emtpy);
		GUILayout.EndScrollView();
		if (GUILayout.Button("Back", GUILayout.Height(75)))
		{
			_currentMenu = CurrentBBundleMenu.Main;
		}
	}

	/// <summary>
	/// For choosing FTP or AWS setup
	/// </summary>
	private void SettingsMenu()
	{
		if (GUILayout.Button("Clear Asset Bundle Cache", GUILayout.Height(50)))
		{
			Caching.ClearCache();
			Debug.Log("Asset bundle Cache cleared");
		}

		if (GUILayout.Button("Clear FTP Prefs", GUILayout.Height(50)))
		{
			DeleteFTPPrefs();
			Debug.Log("FTP prefs cleared");
		}

		if (GUILayout.Button("Clear AWS Prefs", GUILayout.Height(50)))
		{
			DeleteAWSPrefs();
			Debug.Log("AWS prefs cleared");
		}

		if (GUILayout.Button("Clear All Prefs", GUILayout.Height(50)))
		{
			DeleteFTPPrefs();
			DeleteAWSPrefs();
			Debug.Log("All BBundle prefs cleared");
		}

		_scrollPos = GUILayout.BeginScrollView(_scrollPos, _emtpy);
		GUILayout.EndScrollView();
		
		if (GUILayout.Button("Back", GUILayout.Height(75)))
		{
			_currentMenu = CurrentBBundleMenu.Main;
		}
	}

	/// <summary>
	/// For choosing FTP or AWS setup
	/// </summary>
	private void FTPListMenu()
	{
		Selection.activeObject = null;
		int ftpListcount = EditorPrefs.GetInt(FTP_COUNT);
		_scrollPos = GUILayout.BeginScrollView(_scrollPos, _emtpy);
		string startString = string.Empty;
		int initialLength = 0;

		for (int i = 0; i < ftpListcount; ++i)
		{
			GUILayout.Label(EditorPrefs.GetString(FTP_NAME + i));
			startString = EditorPrefs.GetString(FTP_NAME + i);
			initialLength = startString.Length;
			startString = EditorGUILayout.TextField("FTP Name", startString);
			if (startString.Length != initialLength)
			{
				//Got to make sure to remove any of these because it will then folder into that popup
				EditorPrefs.SetString(FTP_NAME + i, startString.Replace("/", string.Empty).Replace("//", string.Empty));
			}

			startString = EditorPrefs.GetString(FTP_URL + i);
			initialLength = startString.Length;
			startString = EditorGUILayout.TextField("FTP URL", startString);
			if (startString.Length != initialLength)
			{
				EditorPrefs.SetString(FTP_URL + i, startString);
			}

			bool useSSL = EditorPrefs.GetBool(FTP_SSL + i);
			bool previous = useSSL;
			useSSL = EditorGUILayout.Toggle(new GUIContent("Use SSL", "If this is an ssl connection, check this"), useSSL);
			if (useSSL != previous)
			{
				EditorPrefs.SetBool(FTP_SSL + i, useSSL);
			}

			startString = EditorPrefs.GetString(FTP_USR + i);
			initialLength = startString.Length;
			startString = EditorGUILayout.TextField("FTP Username", startString);
			if (startString.Length != initialLength)
			{
				EditorPrefs.SetString(FTP_USR + i, startString);
			}

			startString = EditorPrefs.GetString(FTP_PWD + i);
			initialLength = startString.Length;
			startString = EditorGUILayout.TextField("FTP Password", startString);
			if (startString.Length != initialLength)
			{
				EditorPrefs.SetString(FTP_PWD + i, startString);
			}

			GUILayout.BeginHorizontal();
			GUILayout.Space(this.position.width / 2.5f);

			if (GUILayout.Button(_subtract, GUILayout.Width(100)))
			{
				for (int j = i; j < ftpListcount; ++j)
				{
					if (j + 1 < ftpListcount)
					{
						EditorPrefs.SetString(FTP_NAME + j, EditorPrefs.GetString(FTP_NAME + j + 1));
						EditorPrefs.SetString(FTP_URL + j, EditorPrefs.GetString(FTP_URL + j + 1));
						EditorPrefs.SetString(FTP_USR + j, EditorPrefs.GetString(FTP_USR + j + 1));
						EditorPrefs.SetString(FTP_PWD + j, EditorPrefs.GetString(FTP_PWD + j + 1));
					}
					else
					{
						EditorPrefs.DeleteKey(FTP_NAME + j);
						EditorPrefs.DeleteKey(FTP_URL + j);
						EditorPrefs.DeleteKey(FTP_USR + j);
						EditorPrefs.DeleteKey(FTP_PWD + j);
					}
				}
				
				EditorPrefs.SetInt(FTP_COUNT, ftpListcount - 1);
			}
			GUILayout.EndHorizontal();

			if (i + 1 < ftpListcount)
				GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
		}

		if (GUILayout.Button(new GUIContent("Add", "Add a new FTP connection to the FTP List"), GUILayout.Height(50)))
		{
			EditorPrefs.SetInt(FTP_COUNT, ftpListcount + 1);
		}

		GUILayout.EndScrollView();
		if (GUILayout.Button("Back", GUILayout.Height(75)))
		{
			_currentMenu = CurrentBBundleMenu.FTPAWS;
		}
	}

	/// <summary>
	/// The AWS setup menu
	/// </summary>
	private void AWSMenu()
	{
		Selection.activeObject = null;
		int awsListcount = EditorPrefs.GetInt(AWS_COUNT);
		_scrollPos = GUILayout.BeginScrollView(_scrollPos, _emtpy);
		string startString = string.Empty;
		int initialLength = 0;

		for (int i = 0; i < awsListcount; ++i)
		{
			GUILayout.Label(EditorPrefs.GetString(AWS_NAME + i));
			startString = EditorPrefs.GetString(AWS_NAME + i);
			initialLength = startString.Length;
			startString = EditorGUILayout.TextField("AWS Name", startString);
			if (startString.Length != initialLength)
			{
				//Got to make sure to remove any of these because it will then folder into that popup
				EditorPrefs.SetString(AWS_NAME + i, startString.Replace("/", string.Empty).Replace("//", string.Empty));
			}

			startString = EditorPrefs.GetString(AWS_URL + i);
			initialLength = startString.Length;
			if (initialLength == 0)
			{
				startString = AWS_DEFAULT_URL;
			}
			startString = EditorGUILayout.TextField("AWS URL", startString);
			if (startString.Length != initialLength)
			{
				EditorPrefs.SetString(AWS_URL + i, startString);
			}

			startString = EditorPrefs.GetString(AWS_BUCKET + i);
			initialLength = startString.Length;
			startString = EditorGUILayout.TextField("AWS Bucket", startString);
			if (startString.Length != initialLength)
			{
				EditorPrefs.SetString(AWS_BUCKET + i, startString);
			}

			GUILayout.Label("Final Url: " + EditorPrefs.GetString(AWS_URL + i) + EditorPrefs.GetString(AWS_BUCKET + i));

			GUILayout.BeginHorizontal();
			GUILayout.Space(this.position.width / 2.5f);

			if (GUILayout.Button(_subtract, GUILayout.Width(100)))
			{
				for (int j = i; j < awsListcount; ++j)
				{
					if (j + 1 < awsListcount)
					{
						EditorPrefs.SetString(AWS_NAME + j, EditorPrefs.GetString(AWS_NAME + j + 1));
						EditorPrefs.SetString(AWS_URL + j, EditorPrefs.GetString(AWS_URL + j + 1));
					}
					else
					{
						EditorPrefs.DeleteKey(AWS_NAME + j);
						EditorPrefs.DeleteKey(AWS_URL + j);
						EditorPrefs.DeleteKey(AWS_BUCKET + j);
					}
				}
				
				EditorPrefs.SetInt(AWS_COUNT, awsListcount - 1);
			}
			GUILayout.EndHorizontal();

			if (i + 1 < awsListcount)
				GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
		}

		if (GUILayout.Button(new GUIContent("Add", "Add a new AWS connection to the AWS List"), GUILayout.Height(50)))
		{
			EditorPrefs.SetInt(AWS_COUNT, awsListcount + 1);
		}

		GUILayout.EndScrollView();
		if (GUILayout.Button("Back", GUILayout.Height(75)))
		{
			_currentMenu = CurrentBBundleMenu.FTPAWS;
		}
	}

    /// <summary>
    /// Load the assets of a bundle to be viewed
    /// </summary>
    /// <param name="path"></param>
    private void LoadAssets(string path)
    {
        _assetBundleObjects = new List<UnityEngine.Object>();
        _assetNames = new List<string>();
        _mainAsset = null;
        _assetPath = "";

        _browsing = true;

        string finalPath = "";
        if (Application.isEditor)
            finalPath = "file://" + path;
        else
            finalPath = path;

        Debug.Log(finalPath);

        WWW loadedBundle = new WWW(finalPath);
//
		if (!string.IsNullOrEmpty(loadedBundle.error))
            throw new Exception("WWW download had an error:" + loadedBundle.error);

        AssetBundle bundle = loadedBundle.assetBundle;

		UnityEngine.Object[] allObjs = bundle.LoadAllAssets();

        foreach (UnityEngine.Object Obj in allObjs)
        {
            _assetBundleObjects.Add(Obj);
            _assetNames.Add(Obj.name);
        }

        _mainAsset = bundle.mainAsset;

        // Unload the AssetBundles compressed contents to conserve memory
        bundle.Unload(false);

        _currentMenu = CurrentBBundleMenu.Create;
    }

    /// <summary>
    /// Unload all asset references before destroying the menu
    /// </summary>
    private void UnloadAssets()
    {
        _currentMenu = CurrentBBundleMenu.Destroy;
        foreach (KeyValuePair<string, object> temp in _textureData)
        {
            UnityEngine.Object obj = temp.Value as UnityEngine.Object;
            DestroyImmediate(obj, true);
            obj = null;
            Resources.UnloadAsset(obj);
        }

        foreach (UnityEngine.Object obj in _assetBundleObjects)
        {
            UnityEngine.Object tObj = obj;
            DestroyImmediate(tObj, true);
            tObj = null;
            Resources.UnloadAsset(tObj);
        }

        DestroyImmediate(_add);
        DestroyImmediate(_subtract);
        DestroyImmediate(_internet);
        DestroyImmediate(_mainAsset);

        _add = null;
        _subtract = null;
        _internet = null;
        _mainAsset = null;
	    _assetToUpload = null;

        Resources.UnloadAsset(_add);
        Resources.UnloadAsset(_subtract);
        Resources.UnloadAsset(_internet);
        Resources.UnloadAsset(_mainAsset);
        Resources.UnloadUnusedAssets();

        _textureData.Clear();
        _assetBundleObjects.Clear();
        _assetNames.Clear();
    }

	private bool IsDataGood(string dataType, string data)
	{
		if (string.IsNullOrEmpty(data))
		{
			Debug.LogError(dataType + " is an empty value");
			return false;
		}
		return true;
	}

	#region FTP Upload

	/// <summary>
	/// Pass an object to be uploaded
	/// </summary>
	/// <param name="obj"></param>
	/// <returns>Returns whether it succeeded in uploading</returns>
	private bool UploadFTPFile(Object obj)
	{
		string filePath = AssetDatabase.GetAssetPath(obj);
		return UploadFTPFile(filePath);
	}

	/// <summary>
	/// Pass in the path to upload to the ftp using credentials provided
	/// </summary>
	/// <param name="path"></param>
	/// <returns>Returns whether it succeeded in uploading</returns>
	private bool UploadFTPFile(string path)
	{
		string filename = Path.GetFileName(path);
		//string responseString = string.Empty;
		bool uploadSuccess = false;

		string uri = _ftpHostUrl + _ftpDirectory + '/' + filename;
		Debug.Log(uri);
		FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
		request.Method = WebRequestMethods.Ftp.UploadFile;
		request.Credentials = new NetworkCredential(_ftpUsername, _ftpPassword);

		using (StreamReader sourceStream = new StreamReader(path))
		{
			byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());

			request.ContentLength = fileContents.Length;

			using (Stream requestStream = request.GetRequestStream())
			{
				requestStream.Write(fileContents, 0, fileContents.Length);
			}
		}

		using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
		{
			switch (response.StatusCode)
			{
				case FtpStatusCode.CommandOK:
				case FtpStatusCode.ClosingControl:
				case FtpStatusCode.ClosingData:
				case FtpStatusCode.FileActionOK:
					uploadSuccess = true;
					break;
			}
			//responseString = response.StatusCode.ToString();
		}

		return uploadSuccess;
	}

	#endregion

	private void CleanupEditor()
	{
		EditorPrefs.SetBool("BBundleEditorOpened", false);

		UnloadAssets();
		Close();
	}

	#endregion

}
