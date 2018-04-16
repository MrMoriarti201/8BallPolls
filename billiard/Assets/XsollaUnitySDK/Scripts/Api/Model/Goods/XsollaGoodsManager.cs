using SimpleJSON;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace Xsolla
{
	public class XsollaGoodsManager : XsollaObjectsManager<XsollaShopItem>, IParseble {

		//		private List<XsollaGoodsGroup>//			"items":[];

//		public int GetCount(){
//
//			return itemsList.GetItemsList ().Count;
//		}
//
//		public XsollaShopItem GetItemByPosition(int position)
//		{
//			return itemsList[0].GetItemsList ()[position];
//		}

		public IParseble Parse (JSONNode goodsNode)
		{

			JSONNode itemsNode = goodsNode ["virtual_items"];//virtual_items <- NEW | OLD -> items
			IEnumerator<JSONNode> goodsEnumerator = itemsNode.Childs.GetEnumerator ();
			while(goodsEnumerator.MoveNext())
			{
				AddItem(new XsollaShopItem().Parse(goodsEnumerator.Current) as XsollaShopItem);
			}
			return this;
		}
	}

	public class XsollaGroupsManager : XsollaObjectsManager<XsollaGoodsGroup>, IParseble
	{
		public IParseble Parse (JSONNode groupsNode)
		{
			JSONNode goodsGroupsNode = groupsNode["groups"];//["goodsgroups"];
			IEnumerator<JSONNode> goodsGroupsEnumerator = goodsGroupsNode.Childs.GetEnumerator ();
			while(goodsGroupsEnumerator.MoveNext()){
				AddItem(new XsollaGoodsGroup().Parse(goodsGroupsEnumerator.Current) as XsollaGoodsGroup);
			}
			return this;
		}
	}

	public class XsollaGoodsGroup : IXsollaObject, IParseble
	{
		public long id {get; private set;}// "id":"119",
		public string name {get; private set;}// "name":"Top Items",
//		public List<XsollaShopItem> goodsList {get; private set;}// "goods":[]

		public string GetKey()
		{
			return id.ToString ();
		}

		public string GetName()
		{
			return name;
		}

		public IParseble Parse (JSONNode goodsGroupNode)
		{
			id = goodsGroupNode ["id"].AsInt;
			name = goodsGroupNode ["name"];
			return this;
		}
	}

	public class XsollaShopItem : IXsollaObject, IParseble
	{
			
//		public long id {get; private set;}// 							"id":"1468",
//		public string name {get; private set;}//						"name":"Rabbit",
//		public string description {get; private set;}//					"description":"Rabbits are small mammals in the family Leporidae.",
//		public string image {get; private set;}//						"image":"https:\/\/xsolla.cachefly.net\/img\/misc\/merchant-digital-goods\/4376c004fa33dc74483906561d617cb3.png",
//		public float amount {get; private set;}//						"amount":0.39,
//		public float amountWithoutDiscount {get; private set;}//		"amountWithoutDiscount":0.39,
//		public string currency {get; private set;}//					"currency":"USD",
//		public int bonusOut {get; private set;}//						"bonusOut":0,
//		public List<XsollaBonusItem> bonusItems {get; private set;}//	"bonusItems":[],
//		public string label {get; private set;}//						"label":"",
//		public string advertisementType {get; private set;}//			"advertisementType":null
//
		private long id;									//	id: 1468,
		private string sku;									//	sku: "1468",
		private string name;								//	name: "Кролик",
		private string imageUrl;							//	image_url: "https://xsolla.cachefly.net/img/3906561d617cb3.png",
		private string description;							//	description: "Кролики — это маленькие млекопитающие семейства зайцевых.",
		private string descriptionLong;						//	long_description: "There are eight different genera in the family classified as rabbits...",
		private string currency;							//	currency: "USD",
		private float amount;								//	amount: 0.39,
		private float amountWithoutDiscount;				//	amount_without_discount: 0.39,
		private float vcAmount;								//	vc_amount: 0,
		private float vcAmountWithoutDiscount;				//	vc_amount_without_discount: 0,
		private string advertisementType;					//	advertisement_type: null,
		private List<XsollaBonusItem> bonusVirtualItems;	//	bonus_virtual_items: [],
		private XsollaBonusItem bonusVirtualCurrency;		//	bonus_virtual_currency: {},
		private string label;								//	label: null,
		private string offerLabel;							//	offer_label: null,
		private int quantityLimit;							//	quantity_limit: 1,
		private string isFavorite;							//	is_favorite: 0,
		private string[] unsatisfiedUserAttributes;			//	unsatisfied_user_attributes: []


		public string GetBounusString()
		{
			if (bonusVirtualItems.Count > 0) {
				StringBuilder stringBuilder = new StringBuilder ();
				stringBuilder.Append ("<color=#2DAE7B>");
				stringBuilder.Append ("+ ");
				foreach (XsollaBonusItem bonusItem in bonusVirtualItems) {
					stringBuilder.Append (bonusItem.name).Append (" free ");
				}
				stringBuilder.Append ("</color>");
				return stringBuilder.ToString ();
			} else if (bonusVirtualCurrency != null && bonusVirtualCurrency.quantity != null && !"".Equals(bonusVirtualCurrency.quantity)) {
				StringBuilder stringBuilder = new StringBuilder ();
				stringBuilder.Append ("<color=#2DAE7B>");
				stringBuilder.Append ("+ ");
				stringBuilder.Append (bonusVirtualCurrency.quantity).Append (bonusVirtualCurrency.name).Append (" free ");
				stringBuilder.Append ("</color>");
				return stringBuilder.ToString ();
			} else {
				return "";
			}
		}

		public string GetImageUrl()
		{
			if (imageUrl != null) {
				if (imageUrl.StartsWith ("https:"))
					return imageUrl;
				else 
					return "https:" + imageUrl;
			} else {
				return null;
			}
		}

		public string GetPriceString()
		{
			if (amount == amountWithoutDiscount) {
				return CurrencyFormatter.FormatPrice(currency, amount.ToString());
			} 
			else 
			{
				string oldPrice = CurrencyFormatter.FormatPrice(currency, amountWithoutDiscount.ToString());
				string newPrice = CurrencyFormatter.FormatPrice(currency, amount.ToString());
				return "<size=10><color=#a7a7a7>" + oldPrice + "</color></size>" + " " + newPrice;
			}
			
		}

		public string GetSku(){
			return sku;
		}

		public string GetAdvertisementType(){
			return advertisementType;
		}

		public string GetKey()
		{
			return sku.ToString ();//sku <- NEW | OLD -> id
		}

		public string GetName()
		{
			return name;
		}

		public string GetDescription(){
			return description;
		}

		public IParseble Parse (JSONNode shopItemNode)
		{
			id = shopItemNode ["id"].AsInt;
			sku = shopItemNode["sku"];
			name = shopItemNode ["name"];
			description = shopItemNode ["description"];
			imageUrl = shopItemNode ["image_url"];//image_url <- NEW | OLD -> image
			amount = shopItemNode ["amount"].AsFloat;
			amountWithoutDiscount = shopItemNode ["amount_without_discount"].AsFloat;//amount_without_discount <- NEW | OLD -> amountWithoutDiscount
			currency = shopItemNode ["currency"];
			bonusVirtualItems = XsollaBonusItem.ParseMany (shopItemNode ["bonus_virtual_items"]);
			var bvc = new XsollaBonusItem ();
			bvc.Parse (shopItemNode ["bonus_virtual_currency"]);
			bonusVirtualCurrency = bvc;
//			bonusOut = shopItemNode ["bonusOut"].AsInt;
//			bonusItems = 
			advertisementType = null;//shopItemNode ["advertisementType"];
			return this;
		}
	}

}
