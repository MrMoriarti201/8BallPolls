using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class BaseValidator : ILocalValidator
{
    protected string errorMsg;

    void setError(string err)
    {
        Debug.Log(err);
    }

    protected BaseValidator()
    {
        errorMsg = "Error";
    }

    protected BaseValidator(string errorMsg)
    {
        this.errorMsg = errorMsg;
    }

    public string getErrorMsg()
    {
        return errorMsg;
    }

    public void setErrorMsg(string errorMsg)
    {
        this.errorMsg = errorMsg;
    }

    public bool validate(InputField editText)
    {
        string text = editText.ToString();
        bool isValid = validate(text);
        if (!isValid)
            setError(errorMsg);
        else
            setError(null);
        return isValid;
    }

    public virtual bool validate(string s)
    {
        throw new System.NotImplementedException();
    }
}