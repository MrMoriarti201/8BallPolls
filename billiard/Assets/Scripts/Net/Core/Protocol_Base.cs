using UnityEngine;
using System.Collections;

public enum ResultCodeType
{
    None,
    Success,
    Server_Connect_Error
}

public class Protocol_Base : ScriptableObject
{
    public WWW www = null;
    public string url = "";
    public ResultCodeType resultCode = ResultCodeType.None;
    public string resultValue = "";
    public string className = "";

    public virtual void Update()
    {
        if (www.isDone)
        {
            if (string.IsNullOrEmpty(www.error))
            {
                resultCode = ResultCodeType.Success;
                resultValue = www.text;
            }
            else
            {
                resultCode = ResultCodeType.Server_Connect_Error;
                Debug.LogError("server error: " + www.error);
            }
        }
    }
}