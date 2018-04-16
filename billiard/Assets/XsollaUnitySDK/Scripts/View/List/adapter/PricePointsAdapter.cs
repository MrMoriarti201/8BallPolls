

using UnityEngine;
using UnityEngine.UI;
using System;

namespace Xsolla 
{
	public class PricePointsAdapter : IBaseAdapter
	{
		private GameObject shopItemPrefab, shopItemPrefabRecomended, shopItemPrefabBest, shopItemPrefabOffer;

		private const int NORMAL = 0;
		private const int RECOMMENDED = 1;
		private const int BEST_DEAL = 2;
		private const int SPECIAL_OFFER = 3;

		public Action<int> OnBuyPricepoints;

		private XsollaPricepoints manager;
		private string textValue = "Coins";
		int current = 0;

		private ImageLoader imageLoader;

		public void Awake()
		{
			shopItemPrefab = Resources.Load("Prefabs/SimpleView/_ScreenShop/SimpleShopItem") as GameObject;
			shopItemPrefabRecomended = Resources.Load("Prefabs/SimpleView/_ScreenShop/ShopItemRecomended") as GameObject;
			shopItemPrefabBest = Resources.Load("Prefabs/SimpleView/_ScreenShop/ShopItemBest") as GameObject;
			shopItemPrefabOffer = Resources.Load("Prefabs/SimpleView/_ScreenShop/ShopItemOffer") as GameObject;
		}
		
		public override int GetElementType(int id)
		{
			XsollaPricepoint pricepoint = GetItem (id);
			switch (pricepoint.label) {
			case null:
			case "null":
			case "":
				if(pricepoint.IsSpecialOffer())
					return SPECIAL_OFFER;
				return NORMAL;
			case "RECOMMENDED":
				return RECOMMENDED;
			case "BEST DEAL":
				return BEST_DEAL;
			default:
				return NORMAL;
			}
		}
		
		public override int GetCount() 
		{
			return manager.GetCount ();
		}

		public XsollaPricepoint GetItem (int position)
		{
			return manager.GetItemByPosition (position);
		}

		public XsollaPricepoint GetItemById (int position)
		{
			return null;
		}

		public override GameObject GetView(int position)
		{
			int type = GetElementType (position);
			GameObject shopItemInstance;
			switch (type) 
			{
				case NORMAL:
				shopItemInstance = Instantiate(shopItemPrefab) as GameObject;
					break;
				case RECOMMENDED:
				shopItemInstance = Instantiate(shopItemPrefabRecomended) as GameObject;
					break;
				case BEST_DEAL:
				shopItemInstance = Instantiate(shopItemPrefabBest) as GameObject;
					break;
				case SPECIAL_OFFER:
				shopItemInstance = Instantiate(shopItemPrefabOffer) as GameObject;
					break;
				default:
				shopItemInstance = Instantiate(shopItemPrefab) as GameObject;
					break;
			}
			XsollaPricepoint pricepoint = GetItem (position);
			ShopItemViewAdapter itemAdapter = shopItemInstance.GetComponent<ShopItemViewAdapter>();
			itemAdapter.SetRealPrice (pricepoint.GetPriceString());
			itemAdapter.SetSpecial (pricepoint.desc);
			if("".Equals(pricepoint.label) || "null".Equals(pricepoint.label))
				itemAdapter.SetCoins (textValue);
			else
				itemAdapter.SetCoins (pricepoint.label);
			itemAdapter.SetCoinsAmount ( pricepoint.outAmount.ToString());
			itemAdapter.SetImage (pricepoint.GetImageUrl());
			itemAdapter.SetOnClickListener(() => OnClickBuy(pricepoint.outAmount));
//			ImageLoader imageLoader = GetComponent<ImageLoader> ();
//			imageLoader.LoadImage (shopItemInstance.GetComponent<Image>(), url);

			return shopItemInstance;
		}

		private void OnClickBuy (int i){
			if (OnBuyPricepoints != null) 
			{
				OnBuyPricepoints(i);
			}
		}

		public override GameObject GetPrefab ()
		{
			return shopItemPrefab;
		}

		public void SetManager(XsollaPricepoints pricepoints)
		{
			manager = pricepoints;
		}

		public void SetManager(XsollaPricepoints pricepoints, ImageLoader loader)
		{
			SetManager(pricepoints);
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
