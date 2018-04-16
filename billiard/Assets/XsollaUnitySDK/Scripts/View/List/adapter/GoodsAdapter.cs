

using UnityEngine;
using UnityEngine.UI;
using System;

namespace Xsolla 
{
	public class GoodsAdapter : IBaseAdapter 
	{
		private GameObject shopItemPrefab;

		private XsollaGoodsManager manager;
		private string textValue = "Coins";
		int current = 0;

		public Action<string> OnBuy; 

		public void Awake()
		{
			shopItemPrefab = Resources.Load("Prefabs/SimpleView/_ScreenShop/ShopItemGood") as GameObject;
		}
		
		public override int GetElementType(int id)
		{
			return 0;
		}
		
		public override int GetCount() 
		{
			return manager.GetCount ();
		}

		
		public override GameObject GetView(int position)
		{
			GameObject shopItemInstance = Instantiate(shopItemPrefab) as GameObject;
			XsollaShopItem item = manager.GetItemByPosition (position);//manager.GetItemByPosition (position);
			ShopItemViewAdapter itemAdapter = shopItemInstance.GetComponent<ShopItemViewAdapter>();
			itemAdapter.SetRealPrice (item.GetPriceString());
			itemAdapter.SetSpecial (item.GetBounusString());
			itemAdapter.SetCoins (item.GetDescription());
			itemAdapter.SetCoinsAmount (item.GetName());
			itemAdapter.SetImage (item.GetImageUrl());
			itemAdapter.SetOnClickListener(() => OnClickBuy("sku[" + item.GetKey() + "]"));
			return shopItemInstance;
		}

		private void OnClickBuy (string sku){
			if (OnBuy != null) 
			{
				OnBuy(sku);
			}
		}

		public override GameObject GetPrefab ()
		{
			return shopItemPrefab;
		}

		public void SetManager(XsollaGoodsManager pricepoints)
		{
			manager = pricepoints;
		}

		public override GameObject GetNext ()
		{
			if (current < manager.GetCount ()) 
			{
				GameObject go = GetView (current);
				current ++;
				return go;
			}
			return null;
		}
	}
}
