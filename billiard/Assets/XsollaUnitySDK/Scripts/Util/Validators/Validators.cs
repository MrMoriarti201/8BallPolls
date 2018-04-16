
using System;
using System.Linq;

namespace Xsolla {

	public static class ValidatorFactory {

		public enum ValidatorType {
			EMPTY, MONTH, YEAR, CVV, CREDIT_CARD
		}

		public static IValidator GetByType (ValidatorType type) {
			IValidator validator;
			switch (type) {
				case ValidatorType.EMPTY:
					validator = new ValidatorEmpty();
					break;
				case ValidatorType.MONTH:
					validator = new ValidatorMonth();
					break;
				case ValidatorType.YEAR:
					validator = new ValidatorYear();
					break;
				case ValidatorType.CVV:
					validator = new ValidatorCvv();
					break;
				case ValidatorType.CREDIT_CARD:
					validator = new ValidatorCreditCard();
					break;
				default: 
					validator = new ValidatorEmpty();
					break;
			}
			return validator;
		}

	}



	public class ValidatorEmpty : ValidatorBase {

		public ValidatorEmpty()
		{
			_errorMsg = "Can't be empty";
		}

		public ValidatorEmpty(string s) : base(s){}

		public override bool Validate (string s)
		{
			return !"".Equals (s);
		}
	}

	public class ValidatorMonth : ValidatorBase {
		
		public ValidatorMonth()
		{
			_errorMsg = "Invalid month";
		}

		public ValidatorMonth(string s) : base(s){}
		
		public override bool Validate (string s)
		{
			if (intValidator.isInteger(s))
			{
				int month = int.Parse(s);
				return month <= 12 && month > 0;
			}
			else
			{
				return false;
			}
		}

	}

	public class ValidatorYear : ValidatorBase {

		public ValidatorYear()
		{
			_errorMsg = "Invalid Year";
		}

		public ValidatorYear(string s) : base(s){}

		public override bool Validate (string s)
		{
			if (intValidator.isInteger(s))
			{
				int year = int.Parse(s);
				year = year < 1000 ? 2000 + year : year;
				DateTime calendar = DateTime.Now;
				int currentYear = calendar.Year;
				int validYear = currentYear + 15;
				return year >= currentYear && year <= validYear;
			}
			else
			{
				return false;
			}
		}
		
	}

	public class ValidatorCvv : ValidatorBase {

		private bool _isMaestro = false;

		public ValidatorCvv()
		{
			_errorMsg = "Invalid Cvv";
		}

		public ValidatorCvv(string s) : base(s){}

		public ValidatorCvv(string s, bool isMaestro) : base(s){
			_isMaestro = isMaestro;
		}
		
		public override bool Validate (string s)
		{
			return _isMaestro || s.Length > 2 && s.Length < 5;
		}
		
	}

	public class ValidatorCreditCard : ValidatorBase {
		
		public ValidatorCreditCard()
		{
			_errorMsg = "Invalid Credit Card";
		}
		
		public ValidatorCreditCard(string s) : base(s){}

		public override bool Validate (string s)
		{
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
}
