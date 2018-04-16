using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Text;
using System.Linq;

namespace  Xsolla
{
	public abstract class XsollaPaystation : MonoBehaviour {

		private string BaseParams;
		private bool IsSandbox;
		private XsollaUtils Utils;
		private ActivePurchase currentPurchase;
		private bool chancelStatusCheck = false;

		private XsollaPaymentImpl __payment;
		private XsollaPaymentImpl Payment
		{
			get
			{
				if (__payment == null)
				{
					__payment = gameObject.GetComponent <XsollaPaymentImpl>();
					if (__payment == null)
					{
						__payment = gameObject.AddComponent <XsollaPaymentImpl>() as XsollaPaymentImpl;
					}
				}
				return __payment;
			}
			set
			{
				__payment = value;
			}
		}

		protected XsollaResult result;

		//{"user":{ "id":{ "value":"1234567","hidden":true},"email":{"value":"support@xsolla.com"},"name":{"value":"Tardis"},"country":{"value":"US"} },"settings":{"project_id":15764,"language":"en","currency":"USD"}}
		//jafS6nqbzRpZzA38
		// BGKkyK2VetScsLgOcnchTB3r1XdkQaW4 - sandbox
		//KVvI4jVlPaTbre4IAD2chJWTBRqQPkCD
		public void OpenPaystation (string accessToken, bool isSandbox)
		{
			SetLoading (true);
			Logger.isLogRequired = isSandbox;
			Logger.Log ("Paystation initiated current mode sandbox");
			currentPurchase = new ActivePurchase();
			JSONNode rootNode = JSON.Parse(accessToken);
			Dictionary<string, object> dict = new Dictionary<string, object> ();
			if (rootNode == null) {
				dict.Add ("access_token", accessToken);
				BaseParams = "access_token=" + accessToken;
			} else {
				JSONNode userNode = rootNode ["user"];
				JSONNode settingsNode = rootNode ["settings"];
				dict.Add("v1", userNode["id"]["value"].Value);
				dict.Add("project", settingsNode ["project_id"].AsInt);

				string vlaue = userNode["name"]["value"].Value;
				if(!"".Equals(vlaue))
					dict.Add("v2", vlaue);

				vlaue = userNode["email"]["value"].Value;
				if(!"".Equals(vlaue))
					dict.Add("email", vlaue);

				vlaue = userNode["country"]["value"].Value;
				if(!"".Equals(vlaue))
					dict.Add("country", vlaue);

				vlaue = settingsNode["language"].Value;
				if(!"".Equals(vlaue))
					dict.Add("local", vlaue);

				vlaue = settingsNode["currency"].Value;
				if(!"".Equals(vlaue))
					dict.Add("currency", vlaue);

				vlaue = settingsNode["mode"].Value;
				if(!"".Equals(vlaue)) {
					dict.Add("mode", vlaue);
					vlaue = settingsNode["secretKey"].Value;
					if(!"".Equals(vlaue)){
						StringBuilder sb = new StringBuilder("");
//						foreach(KeyValuePair<string, object> kv in dict.OrderBy(i => i.Key))
//						{
//							if(!kv.Key.Equals("country") && !kv.Key.Equals("local") && !kv.Key.Equals("mode"))
//								sb.Append(kv.Key).Append("=").Append(kv.Value.ToString());
//						}
						sb.Append("vok[*hjs34*p2khf2fi]#dsud:sH}UOHd[FJKhejdhu").Append(vlaue);

						string sign = MD5Helper.Md5Sum(sb.ToString());
						dict.Add("sign", sign);
					}
					dict.Add("apiMode", "sandbox");
				}
			
				BaseParams = "project=" + settingsNode ["project_id"] + "&v1=" + userNode["id"]["value"];
			}
			StartPayment (dict, isSandbox);
		}

		private void StartPayment(Dictionary<string, object> dict, bool isSandbox){
			Logger.Log ("Request prepared");
			currentPurchase.Add(ActivePurchase.Part.TOKEN, dict);
			IsSandbox = isSandbox;
			CheckUnfinished ();
			Payment.UtilsRecieved += RecieveUtils;
			
			Payment.FormReceived += (form) => ShowPaymentForm(Utils, form);
			Payment.StatusReceived += (status) => ShowPaymentStatus(Utils.GetTranslations(), status);
			Payment.StatusChecked += (status, elapsedTime) => WaitingStatus(status, elapsedTime);
			
			Payment.QuickPaymentMethodsRecieved += (quickpayments) => ShowQuickPaymentsList(Utils, quickpayments);
			Payment.PaymentMethodsRecieved += ShowPaymentsList;
			Payment.CountriesRecieved += ShowCountries;
			
			Payment.PricepointsRecieved += (pricepoints) => ShowPricepoints(Utils, pricepoints);
			Payment.GoodsGroupsRecieved += (goods) => ShowGoodsGroups(goods);
			Payment.GoodsRecieved += (goods) => UpdateGoods(goods);
			
			Payment.ErrorReceived += ShowPaymentError;
			Payment.SetModeSandbox (isSandbox);
			Payment.InitPaystation(currentPurchase.GetMergedMap());
		}

		private void CheckUnfinished(){
			Logger.Log ("Check unfinished payments");
			if (TransactionHelper.CheckUnfinished ()) {
				Logger.Log ("Have unfinished payments");
				Payment.StatusReceived += CheckUnfinishedPaymentStatus;
				var request = TransactionHelper.LoadRequest();
				if(request != null) {
					Payment.GetStatus(request);
				} else {
					TransactionHelper.Clear();
					Payment = null;
				}
			}
		}

		protected void NextPaymentStep(Dictionary<string, object> xpsMap)
		{
			Logger.Log ("Next Payment Step request");
			SetLoading (true);
			Payment.NextStep (xpsMap);
		}

		public void LoadShopPricepoints()
		{	
			Logger.Log ("Load Pricepoints request");
			SetLoading (true);
			Payment.GetPricePoints (currentPurchase.GetMergedMap());
		}
		
		public void LoadGoodsGroups()
		{
			Logger.Log ("Load Goods Groups request");
			SetLoading (true);
			Payment.GetItemsGrous (currentPurchase.GetMergedMap());
		}

		public void LoadGoods(long groupId)
		{
			Logger.Log ("Load Goods request");
			Payment.GetItems (groupId, currentPurchase.GetMergedMap());
		}

		public void LoadQuickPayment()
		{
			Logger.Log ("Load Quick Payments request");
			if (currentPurchase != null && currentPurchase.counter > 2) {
				currentPurchase.Remove (ActivePurchase.Part.PID);
				currentPurchase.Remove (ActivePurchase.Part.XPS);
			}
			LoadPaymentMethods ();
			LoadCountries ();
			SetLoading (true);
			Payment.GetQuickPayments (null, currentPurchase.GetMergedMap());
		}

		public void LoadPaymentMethods()
		{
			Logger.Log ("Load Payment Methods request");
			SetLoading (true);
			Payment.GetPayments (null, currentPurchase.GetMergedMap());
		}

		public void LoadCountries()
		{
			Logger.Log ("Load Countries request");
			SetLoading (true);
			Payment.GetCountries (currentPurchase.GetMergedMap());
		}

		public void UpdateCountries(string countryIso)
		{
			Logger.Log ("Update Countries request");
			Payment.GetQuickPayments (countryIso, currentPurchase.GetMergedMap());
			Payment.GetPayments (countryIso, currentPurchase.GetMergedMap());
		}

		public void ChooseItem(Dictionary<string, object> items)
		{
			Logger.Log ("Choose item request");
			TransactionHelper.SavePurchase (items);
			result = new XsollaResult (new Dictionary<string, object>(items));
			currentPurchase.Remove (ActivePurchase.Part.PID);
			currentPurchase.Remove (ActivePurchase.Part.XPS);
			FillPurchase (ActivePurchase.Part.ITEM, items);
			TryPay();
		}

		public void ChoosePaymentMethod(Dictionary<string, object> items)
		{
			Logger.Log ("Choose payment method request");
			items.Add ("returnUrl", "https://secure.xsolla.com/paystation3/#/desktop/return/?" + BaseParams);
			FillPurchase (ActivePurchase.Part.PID, items);
			TryPay();
		}

		public void DoPayment(Dictionary<string, object> items)
		{
			Logger.Log ("Do payment");
			currentPurchase.Remove (ActivePurchase.Part.INVOICE);
			FillPurchase (ActivePurchase.Part.XPS, items);
			TryPay();
		}

		public void GetStatus(Dictionary<string, object> items)
		{
			Logger.Log ("Get Status");
			FillPurchase (ActivePurchase.Part.INVOICE, items);
			Payment.NextStep (currentPurchase.GetMergedMap());
		}

		protected void Restart (){
			Logger.Log ("Restart payment");
			currentPurchase.RemoveAllExceptToken ();
			chancelStatusCheck = true;
		}

		public void RetryPayment()
		{
			Logger.Log ("Retry payment");
			TryPay();
		}
		
		private void FillPurchase(ActivePurchase.Part part, Dictionary<string, object> items)
		{
			if (currentPurchase == null) {
				currentPurchase = new ActivePurchase();
				currentPurchase.Add(part, new Dictionary<string, object>(items));
			} else {
				currentPurchase.Remove(part);
				currentPurchase.Add(part, new Dictionary<string, object>(items));
			}
		}

		private void TryPay()
		{
			Logger.Log ("Try pay");
			if (Utils.GetPurchase () != null) {
				if (currentPurchase.counter >= 2) {
					NextPaymentStep (currentPurchase.GetMergedMap());
				} else {
					LoadQuickPayment ();
				}
			} else {
				if (currentPurchase.counter >= 3) {
					NextPaymentStep (currentPurchase.GetMergedMap());
				} else {
					LoadQuickPayment ();
				}
			}
		}

		protected virtual void RecieveUtils (XsollaUtils utils){
			Logger.Log ("Utils recived");
			Utils = utils;
			XsollaPurchase xsollaPurchase = utils.GetPurchase ();
			if (xsollaPurchase != null) 
			{
				bool isPurchase = xsollaPurchase.IsPurchase();
				if(xsollaPurchase.paymentSystem != null && isPurchase){
					NextPaymentStep(currentPurchase.GetMergedMap());
				} else if(isPurchase){ 
					LoadQuickPayment();
				} else {
					LoadShop(utils);
				}
			} 
			else 
			{
				LoadShop(utils);
			}
			SetLoading (false);
		}

		private void LoadShop(XsollaUtils utils){
			Logger.Log ("Load Shop request");
			XsollaPaystation2 paystation2 = utils.GetSettings ().paystation2;
			if (paystation2.goodsAtFirst != null && paystation2.goodsAtFirst.Equals("1"))
			{
				LoadGoodsGroups();
			} else 
			if (paystation2.pricepointsAtFirst != null && paystation2.pricepointsAtFirst.Equals("1")){
				LoadShopPricepoints();
			}
		}

		public void LoadShop(){
			Logger.Log ("Load Shop request");
			if (Utils != null) {
				XsollaPaystation2 paystation2 = Utils.GetSettings ().paystation2;
				if (paystation2.goodsAtFirst != null && paystation2.goodsAtFirst.Equals ("1")) {
					LoadGoodsGroups ();
				} 
				else if (paystation2.pricepointsAtFirst != null && paystation2.pricepointsAtFirst.Equals ("1"))
				{
					LoadShopPricepoints ();
				}
			}
		}

		protected abstract void ShowPricepoints (XsollaUtils utils, XsollaPricepoints pricepoints);
		
		protected abstract void ShowGoodsGroups (XsollaGroupsManager groups);
		protected abstract void UpdateGoods (XsollaGoodsManager goods);
		
		protected abstract void ShowQuickPaymentsList (XsollaUtils utils, XsollaQuickPayments paymentMethods);
		protected abstract void ShowPaymentsList (XsollaPaymentMethods paymentMethods);
		protected abstract void ShowCountries (XsollaCountries paymentMethods);

		protected abstract void ShowPaymentForm (XsollaUtils utils, XsollaForm form);

		protected abstract void ShowPaymentStatus (XsollaTranslations translations, XsollaStatus status);
		protected abstract void CheckUnfinishedPaymentStatus (XsollaStatus status);

		protected void WaitingStatus (string status, int elapsedTime) {
			Logger.Log ("Waiting payment status");
			if (!"done".Equals (status) && !"cancel".Equals (status) && elapsedTime < 1200) {
				if(chancelStatusCheck){
//					Payment.InitPaystation(currentPurchase.GetMergedMap());
					LoadShopPricepoints();
					chancelStatusCheck = false;
				} else {
					StartCoroutine (Test ());
				}
			} else {
				currentPurchase.Remove(ActivePurchase.Part.INVOICE);
				TryPay();
			}
		}

		private IEnumerator Test(){
			yield return new WaitForSeconds(2);
			Payment.NextStep (currentPurchase.GetMergedMap());
		}

		protected abstract void ShowPaymentError (XsollaError error);

		protected abstract void SetLoading (bool isLoading);
	}
}