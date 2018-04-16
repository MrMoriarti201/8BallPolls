using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Xsolla {
	public class RadioButton : MonoBehaviour, IRadioButton {

		public Image image;
		public Text text;
		public Color activeImage;
		public Color normalImage;
		public Color activeText;
		public Color normalText;


		public void Select()
		{
			image.color = activeImage;
			text.color = activeText;
		}

		public void Deselect()
		{
			image.color = normalImage;
			text.color = normalText;
		}

	}
}
