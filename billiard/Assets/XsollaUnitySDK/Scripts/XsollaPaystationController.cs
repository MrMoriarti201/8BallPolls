using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace Xsolla
{
	public class XsollaPaystationController : XsollaPaystation {

		public event Action<XsollaResult> OkHandler;
		public event Action<XsollaError> ErrorHandler;

		public Transform menuTransform;
		public GameObject mainScreen;
		public GameObject mainScreenContainer;
		public MyRotation progressBar;
		private bool isMainScreenShowed = false;

		public GameObject shopScreenPrefab;
		public GameObject paymentListScreebPrefab;
		public GameObject paymentScreenPrefab;

		private PaymentListScreenController _paymentListScreenController;
		private ShopViewController _shopViewController;

		private static ActiveScreen currentActive = ActiveScreen.UNKNOWN;

		enum ActiveScreen
		{
			SHOP, P_LIST, PAYMENT, STATUS, ERROR, UNKNOWN
		}

		void Start()
		{
			Resizer.ResizeToParrent (mainScreen);
			//OpenPaystation ("OOhhYf0lVZyUtsBZ3d4jV8KhZ1uSQdgO", true);//OOhhYf0lVZyUtsBZ3d4jV8KhZ1uSQdgO T1SdYOJJQNbaWDnyyuB05N1YDeqwgLlM 9TvKEQa71wMHrPN4u2arTMcYnqns7wFr
			// sandbox. NORMAL-WITH PURCHASE: yB9wwrtqanwlvatrNakqjRuI3qtuB0I0 NORMAL: jDN50MF8wzU3WVdpnd6RXT0r8bfeKRPa ZIP: nKPgvF6VGkmRQ7oGOsi9AVba2LMS08OG
		}

		protected override void RecieveUtils (XsollaUtils utils)
		{
			Resizer.ResizeToParrent (mainScreen);
			base.RecieveUtils(utils);
			InitHeader(utils);
			InitFooter (utils);
			if(utils.GetPurchase() == null || !utils.GetPurchase().IsPurchase())
				InitMenu(utils);
		}

		protected override void ShowPricepoints (XsollaUtils utils, XsollaPricepoints pricepoints)
		{
			Logger.Log ("Pricepoints recived");
			OpenPricepoints (utils, pricepoints);
			SetLoading (false);
		}

		protected override void ShowGoodsGroups (XsollaGroupsManager groups)
		{
			Logger.Log ("Goods Groups recived");
			OpenGoods (groups);
			SetLoading (false);
		}

		protected override void UpdateGoods (XsollaGoodsManager goods)
		{
			Logger.Log ("Goods recived");
			_shopViewController.UpdateGoods(goods);
			SetLoading (false);
		}

		protected override void ShowPaymentForm (XsollaUtils utils, XsollaForm form)
		{
			Logger.Log ("Payment Form recived");
			DrawForm (utils, form);
			SetLoading (false);
		}

		protected override void ShowPaymentStatus (XsollaTranslations translations, XsollaStatus status)
		{
			Logger.Log ("Status recived");
			SetLoading (false);
			OpenStatus (translations, status);
		}

		protected override void CheckUnfinishedPaymentStatus (XsollaStatus status)
		{
			Logger.Log ("Check Unfinished Payment Status");
			if (status.GetGroup () == XsollaStatus.Group.DONE) {
				var purchase = TransactionHelper.LoadPurchase();
				XsollaResult result = new XsollaResult(purchase);
				result.invoice = status.GetStatusData().GetInvoice ();
				result.status = status.GetStatusData().GetStatus ();
				Logger.Log("Ivoice ID " + result.invoice);
				Logger.Log("Bought", purchase);
				if(TransactionHelper.LogPurchase(result.invoice)) {
					if (OkHandler != null)
						OkHandler (result);
				} else {
						Logger.Log("Alredy added");
				}
				TransactionHelper.Clear();
			}
		}

		protected override void ShowPaymentError (XsollaError error)
		{
			Logger.Log ("Show Payment Error " + error);
			SetLoading (false);
			//OpenError (error);
		}

		// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
		// >>>>>>>>>>>>>>>>>>>>>>>>>>>> PAYMENT METHODS >>>>>>>>>>>>>>>>>>>>>>>>>>>> 
		// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
		private void InitPaymentListScreen(){
			currentActive = ActiveScreen.P_LIST;
			if (_paymentListScreenController == null) {
				GameObject paymentListScreen = Instantiate (paymentListScreebPrefab);
				_paymentListScreenController = paymentListScreen.GetComponent<PaymentListScreenController> ();
				_paymentListScreenController.transform.SetParent (mainScreenContainer.transform);
				_paymentListScreenController.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
				//mainScreenContainer.GetComponentInParent<ScrollRect> ().content = _paymentListScreenController.GetComponent<RectTransform> ();
			}
		}

		protected override void ShowQuickPaymentsList (XsollaUtils utils, XsollaQuickPayments quickPayments)
		{
			InitPaymentListScreen ();
			if (!_paymentListScreenController.IsQuiqckPayments()) {
				_paymentListScreenController.InitScreen (utils);
				_paymentListScreenController.SetQuickPayments (quickPayments);
//				LoadPaymentMethods ();
			} else {
				_paymentListScreenController.UpdateQuick(quickPayments);
			}
			if(_paymentListScreenController.IsAllLoaded())
				SetLoading (false);
		}

		protected override void ShowPaymentsList (XsollaPaymentMethods paymentMethods)
		{
			InitPaymentListScreen ();
			if (!_paymentListScreenController.IsAllPayments ()) {
				_paymentListScreenController.SetPaymentsMethods (paymentMethods);
//				LoadCountries ();
			} else {
				//_paymentListScreenController.SetPaymentsMethods(paymentMethods);
				_paymentListScreenController.UpdateRecomended(paymentMethods);
			}
			if(_paymentListScreenController.IsAllLoaded())
				SetLoading (false);
		}

		protected override void ShowCountries (XsollaCountries countries)
		{
			InitPaymentListScreen ();
			_paymentListScreenController.SetCountries (countries);
			if(_paymentListScreenController.IsAllLoaded())
				SetLoading (false);
//			throw new System.NotImplementedException ();
		}
		// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		// <<<<<<<<<<<<<<<<<<<<<<<<<<<< PAYMENT METHODS <<<<<<<<<<<<<<<<<<<<<<<<<<<< 
		// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

		// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
		// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> SHOP >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
		// >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
		private void InitShopScreen(){
			currentActive = ActiveScreen.SHOP;
			if (_shopViewController == null) {
				GameObject paymentListScreen = Instantiate (shopScreenPrefab);
				_shopViewController = paymentListScreen.GetComponent<ShopViewController> ();
				_shopViewController.transform.SetParent (mainScreenContainer.transform);
				_shopViewController.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
				//mainScreenContainer.GetComponentInParent<ScrollRect> ().content = _shopViewController.GetComponent<RectTransform> ();
			}
		}

		public void OpenPricepoints(XsollaUtils utils, XsollaPricepoints pricepoints)
		{
			InitShopScreen ();
			_shopViewController.OpenPricepoints(utils.GetTranslations().Get(XsollaTranslations.PRICEPOINT_PAGE_TITLE), pricepoints);
		}
		
		public void OpenGoods(XsollaGroupsManager groups)
		{
			InitShopScreen ();
			LoadGoods (groups.GetItemByPosition(0).id);
			_shopViewController.OpenGoods(groups);
		}

		// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< SHOP <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
		// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

		private void OpenStatus(XsollaTranslations translations, XsollaStatus status)
		{
			currentActive = ActiveScreen.STATUS;
			menuTransform.gameObject.SetActive (false);
			GameObject statusScreen = Instantiate (Resources.Load("Prefabs/SimpleView/_ScreenStatus/ScreenStatusNew")) as GameObject;
			statusScreen.transform.SetParent(mainScreenContainer.transform);
			statusScreen.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
			mainScreenContainer.GetComponentInParent<ScrollRect> ().content = statusScreen.GetComponent<RectTransform> ();
			StatusViewController controller = statusScreen.GetComponent<StatusViewController> ();
			controller.StatusHandler += OnUserStatusExit;
			controller.InitScreen(translations, status);
		}


		private void OpenError(XsollaError error)
		{
			currentActive = ActiveScreen.ERROR;
			GameObject errorScreen = Instantiate (Resources.Load("Prefabs/Screens/ScreenError")) as GameObject;
			errorScreen.transform.SetParent(mainScreenContainer.transform);
			errorScreen.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
			mainScreenContainer.GetComponentInParent<ScrollRect> ().content = errorScreen.GetComponent<RectTransform> ();
			ScreenErrorController controller = errorScreen.GetComponent<ScreenErrorController> ();
			controller.ErrorHandler += OnErrorRecivied;
			controller.DrawScreen(error);
		}

		private void DrawForm(XsollaUtils utils, XsollaForm form)
		{
			currentActive = ActiveScreen.PAYMENT;
			GameObject checkoutScreen = Instantiate (Resources.Load("Prefabs/SimpleView/_ScreenCheckout/ScreenCheckout")) as GameObject;
			checkoutScreen.transform.SetParent(mainScreenContainer.transform);
			checkoutScreen.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, 0);
			//scroll.content = paymentScreen.GetComponent<RectTransform> ();
			mainScreenContainer.GetComponentInParent<ScrollRect> ().content = checkoutScreen.GetComponent<RectTransform> ();
			ScreenCheckoutController controller = checkoutScreen.GetComponent<ScreenCheckoutController> ();
			controller.InitScreen(utils, form);
		}

		protected override void SetLoading (bool isLoading)
		{
			if (!isMainScreenShowed) {
				if (isLoading) {
					mainScreen.SetActive (false);
				} else {
					mainScreen.SetActive (true);
					isMainScreenShowed = true;
				}
			} else {
				if (isLoading) {
					Resizer.DestroyChilds(mainScreenContainer.transform);
				}
			}
			progressBar.SetLoading (isLoading);
		}

		private void InitHeader(XsollaUtils utils)
		{
			Text[] texts = mainScreen.GetComponentsInChildren<Text> ();
			//texts [0].text = utils.GetProject ().name;
		}

		//TODO minimize
		private void InitMenu(XsollaUtils utils)
		{
			GameObject menuItemPrefab = Resources.Load ("Prefabs/SimpleView/MenuItem") as GameObject;
//			menuTransform = mainScreen.GetComponentInChildren<HorizontalLayoutGroup> ().gameObject.transform;
			XsollaPaystation2 paystation2 = utils.GetSettings ().paystation2;
			if (paystation2.goodsAtFirst != null && paystation2.goodsAtFirst.Equals("1"))
			{
				GameObject menuItemGoods = Instantiate(menuItemPrefab) as GameObject;
				Text[] texts = menuItemGoods.GetComponentsInChildren<Text>();
				texts[0].text = "";
				texts[1].text = utils.GetTranslations().Get(XsollaTranslations.VIRTUALITEM_PAGE_TITLE);
				menuItemGoods.GetComponent<Button>().onClick.AddListener(delegate {LoadGoodsGroups();});
				menuItemGoods.transform.SetParent(menuTransform);
				
			}
			if (paystation2.pricepointsAtFirst != null && paystation2.pricepointsAtFirst.Equals("1"))
			{
				GameObject menuItemPricepoints = Instantiate(menuItemPrefab) as GameObject;
				Text[] texts = menuItemPricepoints.GetComponentsInChildren<Text>();
				texts[0].text = "";
				texts[1].text = utils.GetTranslations().Get(XsollaTranslations.PRICEPOINT_PAGE_TITLE);
				menuItemPricepoints.GetComponent<Button>().onClick.AddListener(delegate {LoadShopPricepoints();});
				menuItemPricepoints.transform.SetParent(menuTransform);
				
			}

		}

		private void InitFooter(XsollaUtils utils)
		{
			
		}

		private void OnUserStatusExit(XsollaStatus.Group group, XsollaStatusData statusData)
		{
			Logger.Log ("On user exit status screen");
			switch (group){
				case XsollaStatus.Group.DONE:
					Logger.Log ("Status Done");
					menuTransform.gameObject.SetActive (true);
					result.invoice = statusData.GetInvoice ();
					result.status = statusData.GetStatus ();
					Logger.Log("Ivoice ID " + result.invoice);
					Logger.Log("Status " + result.status);
					Logger.Log("Bought", result.purchases);
					TransactionHelper.Clear ();
					if (OkHandler != null)
						OkHandler (result);
					else 
						Logger.Log ("Have no OkHandler");
					break;
				case XsollaStatus.Group.TROUBLED:
					Logger.Log ("Status TROUBLED");
					TryAgain();
					break;
				case XsollaStatus.Group.INVOICE:
				case XsollaStatus.Group.UNKNOWN:
				default:
					Logger.Log ("Status in proccess");
					TryAgain();
					break;
			}
		}

		private void TryAgain(){
			SetLoading (true);
			menuTransform.gameObject.SetActive (true);
			Restart ();
		}
		
		
		private void OnErrorRecivied(XsollaError xsollaError)
		{
			Logger.Log("ErrorRecivied " + xsollaError.ToString());
			if (ErrorHandler != null)
				ErrorHandler (xsollaError);
			else 
				Logger.Log ("Have no ErrorHandler");
		}

		void OnDestroy(){
			Logger.Log ("User close window");
			switch (currentActive) 
			{
				case ActiveScreen.STATUS:
					Logger.Log ("Check payment status");
					GetComponentInChildren<StatusViewController>().statusViewExitButton.onClick.Invoke();
					break;
				default:
					Logger.Log ("Handle chancel");
					ErrorHandler (XsollaError.GetCancelError());
					break;
			}
		}
	}
}