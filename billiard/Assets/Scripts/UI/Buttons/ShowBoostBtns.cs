using UnityEngine;
using System.Collections;

public class ShowBoostBtns : MonoBehaviour {

    public GameObject boostBtns;
    public GameObject pivotDlg;
    public GameObject chatDlg;
    public GameObject chatDlgBack;
    
    public static bool showBoost = true;
    public static bool showPivotDlg = true;
    public static bool showChatDlg = true;

    public static bool statusShowAnother = true;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ShowBoostBtnsDlg()
    {   
        if (statusShowAnother || !showBoost) { 
            boostBtns.SetActive(showBoost);
            showBoost = !showBoost;
            statusShowAnother = !statusShowAnother;
        }
    }

    public void ShowPivotSetting()
    {
        if (statusShowAnother || !showPivotDlg)
        { 
            pivotDlg.SetActive(showPivotDlg);
            showPivotDlg = !showPivotDlg;
            statusShowAnother = !statusShowAnother;
        }
        
    }

    public void ShowChattingDlg()
    {
        if (statusShowAnother || !showChatDlg)
        {
            chatDlg.SetActive(showChatDlg);
            chatDlgBack.SetActive(showChatDlg);
            showChatDlg = !showChatDlg;
            statusShowAnother = !statusShowAnother;
        }
    }
}
