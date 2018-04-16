using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

namespace Xsolla {

	public class XsollaTextAll : IParseble {
		private List<XsollaError> errors;
		private List<XsollaInfo> info;
		private bool isFatal;

		public IParseble Parse(JSONNode textAllNode)
		{
			IEnumerator<JSONNode> errorEnumerator = textAllNode ["error"].Childs.GetEnumerator ();
			errors = new List<XsollaError> ();
			while (errorEnumerator.MoveNext()) {
				XsollaError currentError = new XsollaError();
				currentError.Parse(errorEnumerator.Current);
				errors.Add(currentError);
			}

			IEnumerator<JSONNode> infoEnumerator = textAllNode ["info"].Childs.GetEnumerator ();
			info = new List<XsollaInfo> ();
			while (infoEnumerator.MoveNext()) {
				XsollaInfo currentInfo = new XsollaInfo();
				currentInfo.Parse(infoEnumerator.Current);
				info.Add(currentInfo);
			}
			return this;
		}
	}

	public class XsollaInfo : IParseble {
		private string name;					//"name":"text"
		private string value;					//"value":"PLEASE NOTE: The total ord"
		private Attributes attributes;	//"attributes":{}

		public IParseble Parse(JSONNode xsollaInfoNode)
		{
			name = xsollaInfoNode ["name"].Value;
			value = xsollaInfoNode ["value"].Value;
			string key = xsollaInfoNode["attributes"]["key"];
			string pref = xsollaInfoNode["attributes"]["pref"];
			string parameter = xsollaInfoNode["attributes"]["parameter"];
			attributes = new Attributes (key, pref, parameter);
			return this;
		} 

		private struct Attributes {
			public string key { get; private set; }      	//"key":"out_change_message",
			public string pref { get; private set; }     	//"pref":"",
			public string parameter { get; private set; }	//"parameter":""

			public Attributes(string _key, string _pref, string _parameter) : this() {
				key = _key;
				pref = _pref;
				parameter = _parameter;
			}
		}
	}

}