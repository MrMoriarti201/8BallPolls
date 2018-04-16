using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;
using System.Text;

namespace Xsolla
{
	public class XsollaCheckout : IParseble {

		private string action; //	action: "https://www.paypal.com/cgi-bin/webscr"
		private Dictionary<string, string> data;//	data: {useraction: "commit", cmd: "_express-checkout", token: "EC-7WD71503Y43901830", force_sa: "true"}
		private string method;//	method: "GET"

		public string GetLink()
		{
			var builder = new StringBuilder ();
			builder.Append (action).Append ("/?");
			foreach(KeyValuePair<string, string> elem in data)
			{
				builder.Append (elem.Key).Append("=").Append(elem.Value).Append("&");
			}
			return builder.ToString ();
		}

		public string GetMethod()
		{
			return method;
		}

		public IParseble Parse(JSONNode checkoutNode)
		{
			if (checkoutNode ["action"] != null && !"null".Equals (checkoutNode ["action"])) {
				action = checkoutNode ["action"];
				data = new Dictionary<string, string> ();
				var dataNode = checkoutNode ["data"] as JSONClass;
				IEnumerator<KeyValuePair<string, JSONNode>> enumerator = dataNode.GetKeyValueDict ().GetEnumerator ();
				while (enumerator.MoveNext()) {
					data.Add (enumerator.Current.Key, enumerator.Current.Value);
				}
				method = checkoutNode ["method"];
				return this;
			} else {
				return null;
			}
		}
	}


}