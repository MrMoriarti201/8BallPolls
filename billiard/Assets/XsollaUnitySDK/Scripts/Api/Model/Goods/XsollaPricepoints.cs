
using SimpleJSON;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine;

namespace Xsolla 
{
	public class XsollaPricepoints : XsollaObjectsManager<XsollaPricepoint>, IParseble 
	{

//		private List<XsollaPricepoint> list;// 			"list":[],
		private Dictionary<string, object> formParams;//"formParams":[],
		private string projectCurrency;// 				"projectCurrency":"Coins",
//		private XsollaApi api;// 						"api":{}

		public string GetProjectCurrency(){
			return projectCurrency;
		}

		public IParseble Parse (JSONNode pricepointsNode)
		{
			var listNode = pricepointsNode ["list"];
			var pricepointsEnumerator = listNode.Childs.GetEnumerator ();
			while (pricepointsEnumerator.MoveNext()) 
			{
				AddItem(new XsollaPricepoint().Parse(pricepointsEnumerator.Current) as XsollaPricepoint);
			}

			JSONNode formParamsNode = pricepointsNode ["formParams"];
			formParams = new Dictionary<string, object> (formParamsNode.Count);
			IEnumerator<JSONNode> formParamsEnumerator = formParamsNode.Childs.GetEnumerator ();
			while (formParamsEnumerator.MoveNext()) 
			{
				JSONNode current = formParamsEnumerator.Current;
				formParams.Add(current["name"], current["value"]);
			}

			projectCurrency = pricepointsNode ["projectCurrency"];

//			api = new XsollaApi ().Parse (pricepointsNode [XsollaApiConst.R_API]) as XsollaApi;

			return this;
		}

		public struct FormParam
		{
			public string name { get; private set;}// "name":"theme",
			public object value { get; private set;}// "value":100
			
			public FormParam(string newName, object newValue):this()
			{
				name = newName;
				value = newValue;
			} 
		}
	}

	public class XsollaPricepoint : IXsollaObject, IParseble
	{
		public int outAmount { get; private set;}// 					"out":100,
		public int outWithoutDiscount { get; private set;}// 			"outWithoutDiscount":100,
		public int bonusOut { get; private set;}// 						"bonusOut":0,
		public float sum { get; private set;}// 						"sum":0.99,
		public float sumWithoutDiscount { get; private set;}// 			"sumWithoutDiscount":0.99,
		public string currency { get; private set;}// 					"currency":"USD",
		public string image { get; private set;}// 						"image":"\/\/livedemo.xsolla.com\/paystation\/img\/1.png",
		public string desc { get; private set;}// 						"desc":"",
		public List<XsollaBonusItem> bonusItems { get; private set;}//	"bonusItems":[],
		public string label { get; private set;}// 						"label":"",
		public string advertisementType { get; private set;}// 			"advertisementType":null,
		public bool selected { get; private set;}// 					"selected":true

		public string GetImageUrl()
		{
			if(image.StartsWith("https:"))
				return image;
			else 
				return "https:" + image;
		}


		public string GetPriceString()
		{
			if (sum == sumWithoutDiscount) {
				return CurrencyFormatter.FormatPrice(currency, sum.ToString());
			} 
			else 
			{
				string oldPrice = CurrencyFormatter.FormatPrice(currency, sumWithoutDiscount.ToString());
				string newPrice = CurrencyFormatter.FormatPrice(currency, sum.ToString());
				return "<size=10><color=#a7a7a7>" + oldPrice + "</color></size>" + " " + newPrice;
			}

		}

		public bool IsSpecialOffer()
		{
			return sum != sumWithoutDiscount || bonusItems.Count > 0;
		}

		public string GetKey()
		{
			return outAmount.ToString ();
		}

		public string GetName()
		{
			return outAmount.ToString ();
		}

		public IParseble Parse (JSONNode pricepointNode)
		{
			outAmount = pricepointNode["out"].AsInt;
			outWithoutDiscount = pricepointNode["outWithoutDiscount"].AsInt;
			bonusOut = pricepointNode["bonusOut"].AsInt;
			sum = pricepointNode["sum"].AsFloat;
			sumWithoutDiscount = pricepointNode["sumWithoutDiscount"].AsFloat;
			currency = pricepointNode["currency"];
			image = pricepointNode["image"];
			desc = pricepointNode["description"];
			bonusItems = XsollaBonusItem.ParseMany (pricepointNode ["bonusItems"]);
			label = pricepointNode["label"];
			advertisementType = pricepointNode["advertisementType"];
			selected = pricepointNode["selected"].AsBool;
			return this;
		}
	}


}
