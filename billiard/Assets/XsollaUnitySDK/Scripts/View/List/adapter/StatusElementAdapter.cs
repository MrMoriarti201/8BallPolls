using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Xsolla {

	public class StatusElementAdapter : IBaseAdapter {
		
		private GameObject statusPrefab;

		private List<XsollaStatusText.StatusTextElement> elements;

		public void Awake()
		{
			statusPrefab = Resources.Load ("Prefabs/SimpleView/_StatusElements/XsollaStatusElement") as GameObject;
		}

		public override int GetElementType(int id)
		{
			return 0;
		}

		public override int GetCount()
		{
			return elements.Count;
		}

		public override GameObject GetView(int position)
		{
			XsollaStatusText.StatusTextElement element = elements[position];
			//create a new item, name it, and set the parent
			GameObject newItem = Instantiate(statusPrefab) as GameObject;
			Text[] texts = newItem.GetComponentsInChildren<Text> ();
			texts [0].text = element.GetPref();
			texts [1].text = element.GetValue();
			return newItem;
		}

	
		
		public override GameObject GetPrefab()
		{
			return statusPrefab;
		}

		
		public void SetElements(List<XsollaStatusText.StatusTextElement> elements)
		{
			this.elements = elements;
		}

		public override GameObject GetNext ()
		{
			throw new NotImplementedException ();
		}

	}

}
