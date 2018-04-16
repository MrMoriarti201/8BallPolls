using UnityEngine;
using System.Collections;
using System;

namespace Xsolla 
{
	public class SubscriptionsAdapter : IBaseAdapter {
		
		public Action<string> OnBuySubscription;

		private GameObject subscriptionPrefab;
		private GameObject subscriptionSpecialPrefab;
		private XsollaSubscriptions manager;

		public void Awake()
		{
			subscriptionPrefab = Resources.Load("Prefabs/SimpleView/_ScreenShop/ShopItemSubscription") as GameObject;
			subscriptionSpecialPrefab = Resources.Load("Prefabs/SimpleView/_ScreenShop/ShopItemSubscriptionSpecial") as GameObject;
		}
		
		public override int GetElementType(int id)
		{
			return 0;
		}
		
		public override int GetCount() 
		{
			return manager.GetCount ();
		}
		
		public XsollaSubscription GetItem (int position)
		{
			return manager.GetItemByPosition (position);
		}
		
		public XsollaSubscription GetItemById (int position)
		{
			return null;
		}
		
		public override GameObject GetView(int position)
		{
			XsollaSubscription subscription = GetItem (position);
			GameObject subcriptionInstance;
			if (subscription.IsSpecial ()) {
				subcriptionInstance = Instantiate (subscriptionSpecialPrefab);
			} else {
				subcriptionInstance = Instantiate (subscriptionPrefab);
			}
			ShopItemViewAdapter itemAdapter = subcriptionInstance.GetComponentInChildren<ShopItemViewAdapter>();
			itemAdapter.SetRealPrice(subscription.name);//1
			itemAdapter.SetSpecial(subscription.description);//2 //GetBounusString()
			itemAdapter.SetCoins(subscription.GetPriceString());//3
			itemAdapter.SetCoinsAmount(subscription.GetPeriodString("per"));//4
			itemAdapter.SetOnClickListener (() => OnClickBuy(subscription.id));
			return subcriptionInstance;
		}
		
		private void OnClickBuy (string subscriptionId){
			if (OnBuySubscription != null) 
			{
				OnBuySubscription(subscriptionId);
			}
		}
		
		public override GameObject GetPrefab ()
		{
			return subscriptionPrefab;
		}
		
		public void SetManager(XsollaSubscriptions pricepoints)
		{
			manager = pricepoints;
		}
		
		public override GameObject GetNext ()
		{
			return null;
		}
	
	}

}