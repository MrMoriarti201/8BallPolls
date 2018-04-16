using UnityEngine;
using System.Collections;

namespace Xsolla {
	abstract public class ValidatorBase : IValidator {

		protected string _errorMsg;

		public ValidatorBase()
		{
			_errorMsg = "";
		}

		public ValidatorBase(string errorMsg)
		{
			_errorMsg = errorMsg;
		}

		public string GetErrorMsg ()
		{
			return _errorMsg;
		}

		abstract public bool Validate (string s);
	}
}
