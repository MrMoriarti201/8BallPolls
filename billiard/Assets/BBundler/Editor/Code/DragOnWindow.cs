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

using UnityEngine;
using UnityEditor;
using System;
//using System.Collections;
//using System.Collections.Generic;

public static class DragOnWindow
{

	#region Unity Built in API calls

	#endregion
	
	#region Constructors
	
	//public DragOnWindow ()
	//{
	//
	//}
	
	#endregion
	
	#region Events
	
	#endregion
	
	#region Properties

    #endregion
	
	#region Public Data

    public static string[] objectPaths;
    public static UnityEngine.Object[] objects;

	#endregion
	
	#region Protected Data
	
	#endregion
	
	#region Private Data

    private static bool isDragging = false;

	#endregion
	
	#region Public API

    public static bool ShowDragging(this EditorWindow editorWindow, bool displayOverlay, GUIStyle style)
    {
        Rect dropZone = new Rect(0, 0, editorWindow.position.width, editorWindow.position.height);
        if (isDragging && displayOverlay)
        {
            GUI.Box(dropZone, "Add", style);
        }

        Event currentEvent = Event.current;
        switch (currentEvent.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                CheckWindow(editorWindow, currentEvent, dropZone);
                break;
            case EventType.DragExited:
                isDragging = false;
                editorWindow.Repaint();
                break;
        }

        return isDragging;
    }

	#region Overrides
	
	#endregion
	
	#endregion
	
	#region Protected API
	
	#endregion
	
	#region Private API

    private static void CheckWindow(EditorWindow editorWindow, Event current, Rect dropZone)
    {
        if (current.type == EventType.DragUpdated)
            isDragging = true;

        if (!dropZone.Contains(current.mousePosition))
            return;

        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

        if (current.type == EventType.DragPerform)
        {
            DragAndDrop.AcceptDrag();

            objectPaths = DragAndDrop.paths;
            objects = DragAndDrop.objectReferences;
        }
        else
        {
            objectPaths = null;
            objects = null;
        }

        editorWindow.Repaint();
    }
	
	#endregion

}
