using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;
using System;
using SimpleJSON;

namespace Xsolla {

	public class XsollaJsonGenerator {

		public User user;
		public Settings settings;

		public XsollaJsonGenerator(string userId, long projectId){
			user = new User ();
			settings = new Settings ();
			user.id = userId;
			settings.id = projectId;
		}

		public string GetPrepared(){
			StringBuilder builder = new StringBuilder();		
			builder.Append ("{")
				.Append ("\"user\":{")
					.Append ("\"id\":{").Append ("\"value\":\"").Append (user.id).Append ("\"}").Append(",");
					if(user.name != null)
						builder.Append("\"name\":{").Append("\"value\":\"").Append(user.name).Append("\"}").Append(",");
					if(user.email != null)
						builder.Append("\"email\":{").Append("\"value\":\"").Append(user.email).Append("\"}").Append(",");
					if (user.country != null) { 
						builder.Append ("\"country\":{")
							.Append ("\"value\":\"").Append (user.country).Append ("\"").Append (",")
								.Append ("\"allow_modify\":").Append (true)
								.Append ("}").Append (",");
					}
				builder.Length--;
				builder.Append ("}").Append(",");
				builder.Append ("\"settings\":{")
					.Append ("\"project_id\":").Append (settings.id).Append(",");
					if(settings.languge != null)
						builder.Append("\"language\":\"").Append(settings.languge).Append("\"").Append(",");
					if(settings.currency != null)
						builder.Append("\"currency\":\"").Append(settings.currency).Append("\"").Append(",");
					if (settings.mode == "sandbox")
						builder.Append ("\"mode\":\"sandbox\",");
					if(settings.secretKey != null)
						builder.Append("\"secretKey\":\"").Append(settings.secretKey).Append("\"").Append(",");
				builder.Length--;
				builder.Append("}")
			.Append("}");
			return builder.ToString();
		}

		public struct User {
			public string id;
			public string name;
			public string email;
			public string country;
		}

		public struct Settings {
			public long id;
			public string languge;
			public string currency;
			public string mode;
			public string secretKey;
		}

		public static IEnumerator FreshToken(Action<string> tokenCallback){
			XsollaJsonGenerator generator = new XsollaJsonGenerator ("user_1", 14004);//test 15764
			generator.user.name = "John Smith";
			generator.user.email = "support@xsolla.com";
			generator.user.country = "US";
			generator.settings.currency = "USD";
			generator.settings.languge = "en";
			string request = generator.GetPrepared ();
			string url = "https://livedemo.xsolla.com/sdk/token/";

			WWWForm form = new WWWForm ();
			form.AddField ("data", request);
			
//			byte[] body = Encoding.UTF8.GetBytes (request);
//			Dictionary<string, string> headers = new Dictionary<string, string> (2);
//			headers.Add("Content-Type", "application/json");                                                                                
//			headers.Add("Accept", "application/json");

			WWW www = new WWW(url, form);

			yield return www;
			if (string.IsNullOrEmpty(www.error)) {
				JSONNode rootNode = JSON.Parse(www.text);
				tokenCallback (rootNode["token"].Value);
			} else {
				tokenCallback(null);
			}
		}

		public static string Base64Encode(string plainText) {
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}

		static byte[] GetBytes(string str)
		{
			byte[] bytes = new byte[str.Length * sizeof(char)];
			System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
			return bytes;
		}
	}


	public class XsollaWallet : MonoBehaviour
	{
		
		private string token;
		
		private string userId;//"1234"
		private string userName;//"jhon"
		private string userEmail;//"a@b.ru"
		private string userCountry;//"US"
		private long projectId;// 15674
		private string language;//"en"
		private string currency;//"USD"
		
		private Dictionary<string, object> requestParams;
		
		private XsollaWallet(string token){
			this.token = token;
		}
		
		
		public string GetToken(){
			return token;
		}
		
		public string GetPrepared(){
			if(token != null) {
				return token;
			} else {
				StringBuilder builder = new StringBuilder();		
				builder.Append("{")
					.Append("\"user\":{")
						.Append("\"id\":{").Append("\"value\":").Append("").Append("}")
						.Append("\"email\":{").Append("\"value\":").Append("").Append("}")
						.Append("\"country\":{").Append("\"value\":").Append("").Append("}")
						.Append("}")
						.Append("\"settings\":{")
						.Append("\"project_id\":").Append("")
						.Append("\"language\":").Append("")
						.Append("\"currency\":").Append("")
						.Append("\"mode\":").Append("")
						.Append("}")
						.Append("}");
				return builder.ToString();
			}
		}
		
		public Dictionary<string, object> GetParams(){
			return this.requestParams;
		}
		
		public static class Factory
		{
			
			public static XsollaWallet CreateWallet(string access_token)
			{
				return new XsollaWallet (access_token);
			}
			
		}
		
	}
	
}
