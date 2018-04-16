using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace Xsolla {
	public class XsollaSDK : MonoBehaviour {

		public bool isSandbox = false;
		public CCPayment payment;
		private string token;
	
		public string InitPaystation(string token)
		{
			var url = "https://secure.xsolla.com/paystation2/?access_token=" + token;
			Application.OpenURL (url);
			return url;
		}

		public void CreatePaymentForm()
		{
			GameObject newItem = Instantiate(Resources.Load("Prefabs/XsollaPaystation")) as GameObject;
			XsollaPaystationController formController = newItem.GetComponent<XsollaPaystationController> ();
			formController.OkHandler += (status) => {Debug.Log("OkHandler 1 " + status);};
			formController.ErrorHandler += (error) => {Debug.Log("ErrorHandler 2 " + error);};
			StartCoroutine (XsollaJsonGenerator.FreshToken ((token) => SetToken(formController, token)));
		}

		private void SetToken(XsollaPaystationController controller, string token){
			if(token != null) { 
				controller.OpenPaystation (token, false);
			}
		}

		public void CreatePaymentForm(InputField inputField)
		{
			GameObject newItem = Instantiate(Resources.Load("Prefabs/XsollaPaystation")) as GameObject;
			XsollaPaystationController formController = newItem.GetComponent<XsollaPaystationController> ();
			string accessToken = inputField.text;
			formController.OkHandler += (status) => {Debug.Log("OkHandler 1 " + status);};
			formController.ErrorHandler += (error) => {Debug.Log("ErrorHandler 2 " + error);};
			formController.OpenPaystation (accessToken, isSandbox);
		}

		public void CreatePaymentForm(string data)
		{
			GameObject newItem = Instantiate(Resources.Load("Prefabs/XsollaPaystation")) as GameObject;
			XsollaPaystationController formController = newItem.GetComponent<XsollaPaystationController> ();
			formController.OkHandler += (status) => {Debug.Log("OkHandler 1 " + status);};
			formController.ErrorHandler += (error) => {Debug.Log("ErrorHandler 2 " + error);};
			formController.OpenPaystation (data, isSandbox);
		}

		public void CreatePaymentForm(string data, Action<XsollaResult> actionOk, Action<XsollaError> actionError)
		{
			GameObject newItem = Instantiate(Resources.Load("Prefabs/XsollaPaystation")) as GameObject;
			XsollaPaystationController formController = newItem.GetComponent<XsollaPaystationController> ();
			formController.OkHandler += actionOk;
			formController.ErrorHandler += actionError;
			formController.OpenPaystation (data, isSandbox);
		}

		public void CreatePaymentForm(XsollaJsonGenerator generator, Action<XsollaResult> actionOk, Action<XsollaError> actionError)
		{
			CreatePaymentForm (generator.GetPrepared (), actionOk, actionError);
		}

		public void DirectPayment(CCPayment payment)
		{
			payment.InitPaystation ();
		}

		public void SetToken(string s)
		{
			this.token = s;
		}

		public void SetSandbox(bool b){
			this.isSandbox = b;
		}
	}
}