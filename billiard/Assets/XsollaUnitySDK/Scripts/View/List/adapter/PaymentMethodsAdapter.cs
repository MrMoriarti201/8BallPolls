using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Xsolla
{
	public class PaymentMethodsAdapter : IBaseAdapter {

		private GameObject paymentMethodPrefab;

		private List<XsollaPaymentMethod> paymentList;
		private XsollaPaymentMethods manager;
		public ImageLoader imageLoader;

		public void Awake()
		{
			paymentMethodPrefab = Resources.Load("Prefabs/SimpleView/ShopPaymentMethod") as GameObject;
			if(imageLoader == null)
				imageLoader = gameObject.AddComponent<ImageLoader>() as ImageLoader;
		}

		public override int GetCount ()
		{
			return paymentList.Count;//.GetCount ();
		}

		public override int GetElementType (int position)
		{
				return 0;
		}

		public XsollaPaymentMethod GetItem(int position)
		{
			return paymentList[position];
		}

		public override GameObject GetView (int position)
		{
			GameObject paymentItemInstance = Instantiate(paymentMethodPrefab) as GameObject;
			XsollaPaymentMethod item = GetItem (position);
			imageLoader.LoadImage(paymentItemInstance.GetComponentsInChildren<Image> ()[1], item.GetImageUrl());
			paymentItemInstance.GetComponentInChildren<Button> ().onClick.AddListener (() => {
				OnChoosePaymentMethod (item.id);});
			return paymentItemInstance;
		}

		public override GameObject GetNext ()
		{
			throw new System.NotImplementedException ();
		}

		public override GameObject GetPrefab ()
		{
			return paymentMethodPrefab;
		}

		public void OnChoosePaymentMethod(long paymentMethodId)
		{
			GetComponentInParent<PaymentListScreenController> ().ChoosePaymentMethod (paymentMethodId);
		}

		public void SetManager(XsollaPaymentMethods paymentMethods)
		{
			manager = paymentMethods;
			paymentList = paymentMethods.GetRecomendedItems();
		}

		public void UpdateElements(List<XsollaPaymentMethod> newPaymentList)
		{
			paymentList = newPaymentList;
		}

	}
}
