using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class NetBase : MonoBehaviour
{

    protected Dictionary<string, NetDataAnalysisBase> analysisDict;
    protected virtual void Awake()
    {
        analysisDict = new Dictionary<string, NetDataAnalysisBase>();
        protocolList = new List<Protocol_Base>();
    }

    protected virtual void Update()
    {
        RecvMsg();
    }

    NetIO nio;
    Protocol_Base res;
    protected void RecvMsg()
    {
        for (int i = protocolList.Count - 1; i >= 0; --i)
        {
            protocolList[i].Update();
            if (protocolList[i].resultCode != ResultCodeType.None)
            {
                res = protocolList[i];
                if (res.resultCode == ResultCodeType.Success)
                {
                    nio = new NetIO(res.url, res.resultValue);
                    if (analysisDict.ContainsKey(nio.url))
                    {
                        NetDataAnalysisBase netAnalysis = analysisDict[nio.url];
                        analysisDict.Remove(nio.url);
                        Debug.Log(netAnalysis.GetType().Name + nio.msg);
                        netAnalysis.Analysis(nio);
                    }
                }
                protocolList.RemoveAt(i);
                GameManager.instance.ShowLoading(false);
            }
        }
    }

    public void SendMsg(NetDataAnalysisBase analysis)
    {
        GameManager.instance.ShowLoading(true);
        NetIO nio0 = new NetIO(analysis.url);
        foreach (string prm in analysis.allparams)
        {
            nio0.AddParam(prm);
        }
        if (nio0.msg != "")
        {
            nio0.url += "?" + nio0.msg;
            nio0.msg = "";
            nio0.url += "&userId=" + GlobalInfo.myProfile.user_id;
        }
        else
        {
            nio0.url += "?userId=" + GlobalInfo.myProfile.user_id;
        }
        
        if (!analysisDict.ContainsKey(nio0.url))
        {
            analysisDict.Add(nio0.url, analysis);
            Debug.Log(nio0.url);//
            //NetMessage.Instance.Send(nio0);
            SendProtocol(nio0.url, analysis.url);
        }
    }

    public static Protocol_Base protocolBase = null;
    public static List<Protocol_Base> protocolList = null;

    public static void SendProtocol(string url, string protocolEntityClassName)
    {
        protocolBase = (Protocol_Base)Protocol_Base.CreateInstance("Protocol_Base");
        if (null != protocolBase)
        {
            protocolBase.www = new WWW(url);
            protocolBase.url = url;
            protocolBase.className = protocolEntityClassName;
            protocolList.Add(protocolBase);
        }
        else
        {
            Debug.LogError("GameState entity is null : " + protocolEntityClassName);
        }
    }
}