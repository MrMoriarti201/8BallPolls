using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Linq;

namespace Xsolla {
	public class TransactionHelper {

		public const string KEY_TRANSICTION = "unfinished_transiction";
		public const string KEY_PURCHASE = "unfinished_purchase";

		public static void SaveRequest(Dictionary<string, object> dict){
			PlayerPrefs.SetString (KEY_TRANSICTION, DictToString(dict));
		}

		public static void SavePurchase(Dictionary<string, object> dict){
			PlayerPrefs.SetString (KEY_PURCHASE, DictToString(dict));
		}

		public static Dictionary<string, object> LoadRequest(){
			var s = PlayerPrefs.GetString (KEY_TRANSICTION);
			if (s != null && !"".Equals(s))
				return StringToDict (s);
			else
				return null;
		}

		public static Dictionary<string, object> LoadPurchase(){
			var s = PlayerPrefs.GetString (KEY_PURCHASE);
			if (s != null && !"".Equals(s))
				return StringToDict (s);
			else
				return null;
		}

		public static void Clear(){
			PlayerPrefs.DeleteKey (KEY_TRANSICTION);
			PlayerPrefs.DeleteKey (KEY_PURCHASE);
		}

		public static bool CheckUnfinished(){
			return PlayerPrefs.HasKey (KEY_TRANSICTION) && PlayerPrefs.HasKey (KEY_PURCHASE);
		}

		const string trackedKey = "tracked_purchase";
		public static bool LogPurchase(string invoiceID){
			if (PlayerPrefs.GetString (trackedKey, "FALSE") == "FALSE") {
				PlayerPrefs.SetString(trackedKey, "");
			}
			invoiceID = "<" + invoiceID + ">";
			if (PlayerPrefs.GetString (trackedKey).IndexOf (invoiceID) == -1) {
				PlayerPrefs.SetString (trackedKey, PlayerPrefs.GetString (trackedKey) + invoiceID);
				return true;
			} else {
				return false;
			}
		}

		private static string DictToString(Dictionary<string, object> dict){
			var res = string.Join("; ", dict.Select(
				p => string.Format(
				"{0}, {1}"
				,   p.Key
				,   p.Value != null ? p.Value.ToString() : ""
				)
				).ToArray());
			return res.ToString ();
		}

		private static Dictionary<string, object> StringToDict(string dictString){
			var dict = dictString.Split(';')
				.Select(s => s.Split(','))
					.ToDictionary(
						p => p[0].Trim()
					,   p => p[1].Trim() as object//.Equals("null") ? null : (bool?)(bool.Parse(p[1].Trim()))
					);
			return dict;
		}

//		public static string DictToJSONString(Dictionary<string, object> dict){
//			StringBuilder builder = new StringBuilder ();
//			builder.Append ("[");
//			foreach (var kv in dict) {
//				if(kv.Value != null)
//					builder.Append(" {\"").Append(kv.Key).Append("\" : ").Append(kv.Value.ToString().ToLower()).Append("},");
//				else
//					builder.Append("{\"").Append(kv.Key).Append("\":\"\"},");
//			}
//			builder.Remove (builder.Length - 1, 1);
//			builder.Append (" ]");
//			return builder.ToString ();
//		}
	}
}
