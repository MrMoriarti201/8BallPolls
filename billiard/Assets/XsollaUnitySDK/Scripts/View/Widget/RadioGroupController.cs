using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Xsolla {
	public class RadioGroupController : MonoBehaviour {

		public List<RadioButton> radioButtons;
		private int prevSelected = 0;

		public RadioGroupController(){
			radioButtons = new List<RadioButton>();
		}

		public void SetButtons(List<GameObject> objects)
		{
			foreach (GameObject go in objects) 
			{
				radioButtons.Add(go.GetComponent<RadioButton>());
			}
		}

		public void SelectItem(int position)
		{
			if(prevSelected >= 0)
				radioButtons [prevSelected].Deselect ();
			radioButtons [position].Select ();
			prevSelected = position;
		}

//		public void SelectItem(RadioButton radioButton)
//		{
//			if(prevSelected != null)
//				radioButton.Deselect ();
//			radioButton.Select ();
//			prevSelected = radioButton;
//		}

	}
}