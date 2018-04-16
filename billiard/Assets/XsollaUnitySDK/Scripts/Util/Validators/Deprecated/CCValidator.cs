using UnityEngine;
using System.Collections;
using System.Linq;

public class CCValidator : MainValidator
{

    public CCValidator() {
        errorMsg = "Please enter a valid credit card number";
    }

    public CCValidator(string errorMsg) {
        base.setErrorMsg(errorMsg);
    }

    public override bool validate(string s) {
// 		transform.GetComponent<CCEditText>().Correct();
        s = s.Replace("\\s", "");
		s = s.Replace(" ", "");
		return checkLuhn(strToIntArr(s));
    }

    public int[] strToIntArr(string intString) {
		if (intString.Length < 12)
			return null;
		int n;
		int[] digits = intString.ToCharArray().Select(s => int.TryParse(s.ToString(), out n) ? n : 0).ToArray();
        return digits;
    }

    public static bool checkLuhn(int[] digits) {
        if (digits==null || digits.Length < 12)
            return false;

        int sum = 0;
        int length = digits.Length;
        for (int i = 0; i < length; i++) {

            // get digits in reverse order
			int digit = digits[length - i - 1];

            // every 2nd number multiply with 2
            if (i % 2 == 1) {
                digit *= 2;
            }
            sum += digit > 9 ? digit - 9 : digit;
        }
        return sum % 10 == 0;
    }
}