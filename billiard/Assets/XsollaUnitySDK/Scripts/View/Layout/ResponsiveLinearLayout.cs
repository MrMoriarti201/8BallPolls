using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Xsolla 
{
	public class ResponsiveLinearLayout : MonoBehaviour 
		{

		public List<GameObject> objects;
		
		private float parentHeight = 0;
		private float totalHeight = 0;
		private float containerFinalHeight = 0;
		private RectTransform containerRectTransform;

		public void AddObject(GameObject go){
			if(go != null)
				objects.Add (go);
		}

		public void Invalidate(){
			// parentHeight = gameObject.GetComponentInParent<RectTransform> ().rect.height;
			containerRectTransform = GetComponent<RectTransform>();
			parentHeight = GameObject.FindGameObjectWithTag ("Container").GetComponent<RectTransform>().rect.height;
			GetTotlHeight ();
			DrawLayout ();
		}

		float GetTotlHeight()
		{
			foreach (GameObject go in objects) 
			{
				RectTransform goRectTransform = go.GetComponent<RectTransform>();
				
				//calculate the width and height of each child item.
				float width = containerRectTransform.rect.width;
				float ratio = width / goRectTransform.rect.width;
				float height = goRectTransform.rect.height * ratio;
				
				//adjust the height of the container so that it will just barely fit all its children
				containerFinalHeight += height;
			}
			return containerFinalHeight;
		}

		void DrawObject(GameObject go){
			RectTransform goRectTransform = go.GetComponent<RectTransform>();
			
			//calculate the width and height of each child item.
			float width = containerRectTransform.rect.width;
			float ratio = width / goRectTransform.rect.width;
			float height = goRectTransform.rect.height * ratio;
			
			//adjust the height of the container so that it will just barely fit all its children
			totalHeight += height;
			go.transform.SetParent (gameObject.transform);
			
			//move and size the new item
			RectTransform rectTransform = go.GetComponent<RectTransform>();
			
			float x = -containerRectTransform.rect.width / 2;
			float y = containerFinalHeight / 2 - totalHeight;
			rectTransform.offsetMin = new Vector2(x, y);
			
			x = rectTransform.offsetMin.x + width;
			y = rectTransform.offsetMin.y + height;
			rectTransform.offsetMax = new Vector2(x, y);
		}

		public void DrawLayout(){
			foreach (GameObject go in objects) 
			{
				DrawObject(go);
			}
			if (totalHeight > parentHeight) 
			{
				// offsetMin = Lower Left Corner
				containerRectTransform.offsetMin = new Vector2 (containerRectTransform.offsetMin.x, (-totalHeight + parentHeight / 2));
				// offsetMax = Upper Right Corner
				containerRectTransform.offsetMax = new Vector2 (containerRectTransform.offsetMax.x, parentHeight / 2);//totalHeight / 2
			}
		}
	}
}