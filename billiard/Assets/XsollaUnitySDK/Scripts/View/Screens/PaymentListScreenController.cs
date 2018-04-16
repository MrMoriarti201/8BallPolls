using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Xsolla
{
	public class PaymentListScreenController : ScreenBaseConroller<object> 
	{

		public Text titleText;
		public QuickPaymentsController quickController;
		public AllPaymentsController allController;
		public ImageLoader imageLoader;
		public GameObject screenHider;

		private string startCountryIso;
		private XsollaQuickPayments _quickPayments;
		private XsollaPaymentMethods _paymentMethods;
		private XsollaCountries _countries;
		private int loadingProgress = 0; // 3 - full loaded
		private XsollaUtils utilsLink;

		public override void InitScreen (XsollaTranslations translations, object model)
		{
			throw new System.NotImplementedException ();
		}
	
		public void InitScreen(XsollaUtils utils)
		{
			utilsLink = utils;
			titleText.text = utils.GetTranslations().Get(XsollaTranslations.PAYMENT_METHODS_PAGE_TITLE);
			startCountryIso = utils.GetUser ().GetCountryIso ();
		}

		public void SetQuickPayments(XsollaQuickPayments quickPayments)
		{
			_quickPayments = quickPayments;
			LoadElement ();
		}

		public void SetPaymentsMethods(XsollaPaymentMethods paymentMethods)
		{
			_paymentMethods = paymentMethods;
			LoadElement ();
		}

		public void SetCountries(XsollaCountries countries)
		{
			_countries = countries;
			LoadElement ();
//			OpenAllPayments ();
		}

		public void UpdateQuick(XsollaQuickPayments quickPayments)
		{
			_quickPayments = quickPayments;
			quickController.SetQuickMethods (quickPayments);
		}

		public void UpdateRecomended(XsollaPaymentMethods paymentMethods)
		{
			_paymentMethods = paymentMethods;
			quickController.SetAllMethods (paymentMethods);
			allController.UpdatePaymentMethods (paymentMethods);
		}

		private void InitChildView(){
//			Resizer.ResizeToParrent (gameObject);
			quickController.SetQuickMethods (_quickPayments);
			quickController.SetAllMethods (_paymentMethods);
			allController.SetPaymentMethods (_paymentMethods);
			if (utilsLink.GetUser ().IsAllowChangeCountry ()) { 
				allController.SetCountries (startCountryIso, _countries);
			}
			screenHider.SetActive (false);
		}

		public void OpenQuickPayments()
		{
			allController.gameObject.SetActive(false);
			quickController.gameObject.SetActive(true);
		}

		public void OpenAllPayments()
		{
			quickController.gameObject.SetActive(false);
			allController.gameObject.SetActive(true);

		}

		private void LoadElement(){
			loadingProgress++;
			if(IsAllLoaded()){
				InitChildView ();
				OpenQuickPayments ();
			}
		}

		public bool IsAllLoaded()
		{
			return loadingProgress == 3;
		}

		public void ChoosePaymentMethod(long paymentMethodId)
		{
//			if (_purchase.ContainsKey ("pid"))
//				_purchase ["pid"] = paymentMethodId;
//			else
//				_purchase.Add ("pid", paymentMethodId);
//
//			if (!_purchase.ContainsKey ("hidden"))
//				_purchase.Add ("hidden", "out");
			
			Dictionary<string, object> purchase = new Dictionary<string, object>();
			purchase.Add ("pid", paymentMethodId);
			purchase.Add ("hidden", "out");
			GetComponentInParent<XsollaPaystationController> ().ChoosePaymentMethod (purchase);
		}

		public void ChangeCountry(string countryIso)
		{
			GetComponentInParent<XsollaPaystationController> ().UpdateCountries (countryIso);
		}

		public bool IsQuiqckPayments()
		{
			return _quickPayments != null;
		}

		public bool IsAllPayments()
		{
			return _paymentMethods != null;
		}

	}
}
