using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Xsolla
{
	public class ShopScreenController : ScreenBaseConroller<object> {

	
		public override void InitScreen (XsollaTranslations translations, object model)
		{
			throw new System.NotImplementedException ();
		}

		public void InitScreen (XsollaUtils utils)
		{
			DrawScreen (utils);
		}

		public void DrawScreen(XsollaUtils utils)
		{
			InitTitle (utils);
		}

		private void InitTitle(XsollaUtils utils)
		{
			Text titleText = GetComponentInChildren<Text> ();
			titleText.text = utils.GetTranslations().Get(XsollaTranslations.PRICEPOINT_PAGE_TITLE);
		}

		public void OpenPricepoints(XsollaPricepoints pricepoints)
		{
			GridView gridView = GetComponentInChildren<GridView> ();
			PricePointsAdapter adapter = GetComponentInChildren<PricePointsAdapter>();
			adapter.SetManager (pricepoints);
			adapter.OnBuyPricepoints += (outAmount) => {
				Dictionary<string, object> map = new Dictionary<string, object> (1);
				map.Add ("out", outAmount);
				//				StartPayment (map);
				OpenPaymentMethods(map);
			};
			gridView.SetAdapter (adapter, 3);
			Resizer.ResizeToParrent (gameObject);
		}

		public void OpenSubscriptions(XsollaSubscriptions subscriptions)
		{
			GridView gridView = GetComponentInChildren<GridView> ();
			SubscriptionsAdapter adapter = GetComponentInChildren<SubscriptionsAdapter>();
			adapter.SetManager (subscriptions);
			gridView.SetAdapter (adapter, 1);
		}

		public void OpenGoods(XsollaGoodsManager goods)
		{
			GridView gridView = GetComponentInChildren<GridView> ();
			GoodsAdapter adapter = GetComponentInChildren<GoodsAdapter>();
			adapter.SetManager (goods);
			gridView.SetAdapter (adapter, 3);
		}

		private void OpenPaymentMethods(Dictionary<string, object> purchase)
		{
			gameObject.GetComponentInParent<XsollaPaystationController> ().ChooseItem (purchase);
		}


		public void OpenPaymentMethods(XsollaPaymentMethods paymentMethods)
		{
			GridView gridView = GetComponentInChildren<GridView> ();
			PaymentMethodsAdapter adapter = GetComponentInChildren<PaymentMethodsAdapter>();
			adapter.SetManager (paymentMethods);
			gridView.SetAdapter (adapter, 6);
		}


		private void StartPayment(Dictionary<string, object> map)
		{
			gameObject.GetComponentInParent<XsollaPaystationController> ().DoPayment (map);
		}

	
//		void Clear()
//		{
//			Lisst<GameObject> children = new List<GameObject>();
//			foreach (Transform child in container) children.Add(child.gameObject);
//			children.ForEach(child => Destroy(child));
//		}
	}
}
