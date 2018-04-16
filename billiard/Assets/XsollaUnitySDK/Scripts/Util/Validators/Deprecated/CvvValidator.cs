using UnityEngine;
using System.Collections;

public class CvvValidator : MainValidator
{

    private CCEditText mCcEditText;
    private bool mIsMandatory;

    public CvvValidator() {
        errorMsg = "CVV/CVV2 is invalid";
    }

    public CvvValidator(string errorMsg) {
        base.setErrorMsg(errorMsg);
    }


    public CvvValidator(CCEditText ccEditText, bool isMandatory) {
        this.mCcEditText = ccEditText;
        this.mIsMandatory = isMandatory;
    }

    public CvvValidator(string errorMsg, CCEditText ccEditText, bool isMandatory) {
        base.setErrorMsg(errorMsg);
        this.mCcEditText = ccEditText;
        this.mIsMandatory = isMandatory;
    }

    public override bool validate(string s) {
        if (mCcEditText != null) {
            return mCcEditText.getCurrentType() == CCEditText.Type.MAESTRO || s.Length > 2 && s.Length < 5;
        } else {
            return s.Length > 2 && s.Length < 5;
        }
    }
}