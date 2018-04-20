/*
http://www.cgsoso.com/forum-211-1.html

CGæœæœ Unity3d æ¯æ—¥Unity3dæ’ä»¶å…è´¹æ›´æ–° æ›´æœ‰VIPèµ„æºï¼

CGSOSO ä¸»æ‰“æ¸¸æˆå¼€å‘ï¼Œå½±è§†è®¾è®¡ç­‰CGèµ„æºç´ æã€‚

æ’ä»¶å¦‚è‹¥å•†ç”¨ï¼Œè¯·åŠ¡å¿…å®˜ç½‘è´­ä¹°ï¼

daily assets update for try.

U should buy the asset from home store if u use it in your project!
*/

//////////////////////////////////////////////////////
//
// Copyright Â© 2014 Brett Faulds
//
// Author
//	Brett Faulds - brfaulds@gmail.com
//
// For help and support please email the author above 
// or visit http://www.brettfaulds.com
//
//////////////////////////////////////////////////////

#region Preprocessor Defines

#endregion

using UnityEngine;
//using System;
//using System.Collections;
//using System.Collections.Generic;

public class TexturedObject : MonoBehaviour 
{

	#region Unity Built in API calls

    public void Start()
    {
        //Set this to false if you don't want the debug information to show.
        //BBundleManager.Instance.DebugEnabled = false;
    }

	#endregion
	
	#region Constructors
	
	//public TexturedObject ()
	//{
	//
	//}
	
	#endregion
	
	#region Events
	
	#endregion
	
	#region Properties

    #endregion
	
	#region Public Data

    /// <summary>
    /// The asset name of what I want to load, in this case will always be a texture
    /// </summary>
    public string AssetName = "";

	#endregion
	
	#region Protected Data
	
	#endregion
	
	#region Private Data
	
	#endregion
	
	#region Public API

    /// <summary>
    /// Function to call that will let me know that the asset bundle is ready to be pulled from
    /// </summary>
    public void LoadedTexture()
    {
        this.GetComponent<Renderer>().sharedMaterial.mainTexture = BBundleManager.Instance.Bundles[AssetName] as Texture2D;
    }

	#region Overrides
	
	#endregion
	
	#endregion
	
	#region Protected API
	
	#endregion
	
	#region Private API
	
	#endregion

}
