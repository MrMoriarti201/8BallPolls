using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using System.Text.RegularExpressions;

namespace Xsolla 
{

	public abstract class ScreenBaseConroller<T> : MonoBehaviour 
	{

		public delegate void RecieveError(XsollaError xsollaError);
		
		public event Action<XsollaError> ErrorHandler;

		protected Dictionary<string, GameObject> screenObjects;

		public ScreenBaseConroller(){
			screenObjects = new Dictionary<string, GameObject> ();
		}

		public abstract void InitScreen (XsollaTranslations translations, T model);
		private void DrawScreen (XsollaTranslations translations, T model){
		}

		protected void InitView()
		{
			foreach (KeyValuePair<string, GameObject> go in screenObjects) 
			{
				screenObjects[go.Key] = Instantiate(go.Value) as GameObject;
			}
		}

		protected GameObject GetObjectByTag(string tag)
		{
			if (screenObjects.ContainsKey (tag))
				return screenObjects [tag];
			return null;
		}

//		protected GameObject GetTitle(string titleText){
//
//			if (titleText != null) 
//			{
//				GameObject titleObj = Instantiate (Resources.Load("Prefabs/SimpleView/Title")) as GameObject;
//				SetText (titleObj, titleText);
//				return titleObj;
//			}
//			return null;
//		}

		protected GameObject GetOkStatus(string titleText){
			
			if (titleText != null) 
			{
				GameObject statusObj = Instantiate (Resources.Load("Prefabs/SimpleView/Status")) as GameObject;
				SetText (statusObj, titleText);
				return statusObj;
			}
			return null;
		}

		protected GameObject GetWaitingStatus(string titleText){
			
			if (titleText != null) 
			{
				GameObject statusObj = Instantiate (Resources.Load("Prefabs/SimpleView/StatusWaiting")) as GameObject;
				SetText (statusObj, titleText);
				return statusObj;
			}
			return null;
		}

		protected GameObject GetTitle(string titleText){
			
			if (titleText != null) 
			{
				GameObject titleObj = Instantiate (Resources.Load("Prefabs/SimpleView/TitleNoImg")) as GameObject;
				SetText (titleObj, titleText);
				return titleObj;
			}
			return null;
		}

		protected GameObject GetTitle(string titleText, string imgUrl){
			
			if (titleText != null) 
			{
				//bool isImageLoaderExist = ;
				if(GetComponent<ImageLoader>() == null)
					gameObject.AddComponent<ImageLoader>();
				GameObject titleObj = Instantiate (Resources.Load("Prefabs/SimpleView/Title")) as GameObject;
				SetText (titleObj, titleText);
				if(imgUrl != null && !"".Equals(imgUrl) && GetComponent<ImageLoader>() != null)
					SetImage(titleObj, imgUrl);
				return titleObj;
			}
			return null;
		}


		protected GameObject GetTitleWithButton(string titleText, string imgUrl, string accessToken, Action<string> onClickMore, bool isSandbox){
			
			if (titleText != null) 
			{
				//bool isImageLoaderExist = ;
				if(GetComponent<ImageLoader>() == null)
					gameObject.AddComponent<ImageLoader>();
				GameObject titleObj = Instantiate (Resources.Load("Prefabs/SimpleView/TitleWithButton")) as GameObject;
				SetText (titleObj, titleText);
				OpenPaystation openPaystaytion = titleObj.GetComponentInChildren<OpenPaystation>();
				openPaystaytion.TryOpenPaystation += onClickMore;
				openPaystaytion.isSandbox = isSandbox;
				openPaystaytion.token = accessToken;
				if(imgUrl != null && !"".Equals(imgUrl) && GetComponent<ImageLoader>() != null)
					SetImage(titleObj, imgUrl);
				return titleObj;
			}
			return null;
		}

		protected GameObject GetTitleWithImage(string titleText, string imgUrl){
			
			if (titleText != null) 
			{
				//bool isImageLoaderExist = ;
				if(GetComponent<ImageLoader>() == null)
					gameObject.AddComponent<ImageLoader>();
				GameObject titleObj = Instantiate (Resources.Load("Prefabs/SimpleView/TitleWithImage")) as GameObject;
				SetText (titleObj, titleText);
				if(imgUrl != null && !"".Equals(imgUrl) && GetComponent<ImageLoader>() != null)
					SetImage(titleObj, imgUrl);
				return titleObj;
			}
			return null;
		}

		protected GameObject GetTwoTextPlate(string titleText, string valueText){
			if (titleText != null) 
			{
				GameObject textPlate = Instantiate (Resources.Load("Prefabs/SimpleView/_ScreenCheckout/TwoTextGrayPlate")) as GameObject;
				Text[] texts = textPlate.GetComponentsInChildren<Text>();
				texts[0].text = titleText;
				texts[1].text = valueText;
				return textPlate;
			}
			return null;
		}

		protected GameObject GetErrorByString(string error)
		{
			bool isError = error != null;
			if (isError)
			{
				GameObject errorObj = Instantiate (Resources.Load("Prefabs/SimpleView/Error")) as GameObject;
				SetText (errorObj, error);
				return errorObj;
			}
			return null;
		}

		protected GameObject GetError(XsollaError error)
		{
			bool isError = error != null;
			if (isError)
			{
				GameObject errorObj = Instantiate (Resources.Load("Prefabs/SimpleView/Error")) as GameObject;
				SetText (errorObj, error.GetMessage());
				return errorObj;
			}
			return null;
		}
		
		protected GameObject GetList(IBaseAdapter adapter)
		{

			if (adapter != null) 
			{
				GameObject listViewObj = Instantiate (Resources.Load ("Prefabs/SimpleView/ListView")) as GameObject;
				ListView listView = listViewObj.GetComponent<ListView> ();
				//adapter.SetElements (status.GetStatusText ().GetStatusTextElements());
				listView.SetAdapter(adapter);
				listView.DrawList ();
				return listViewObj;
			} 
			return null;
		}

		protected GameObject GetTextPlate(string s)
		{
			if (s != null) 
			{
				int start = s.IndexOf("<a");
				int end = s.IndexOf("a>");
				string taggedText = s.Substring(start, end - start + 2);
				string[] linkedText = taggedText.Split(new Char [] {'<', '>'});
				string newString = "<color=#a38dd8>" + linkedText[2] + "</color>";
				s = s.Replace(taggedText, newString);
				GameObject textPlate = Instantiate (Resources.Load("Prefabs/SimpleView/Instructions")) as GameObject;
				SetText(textPlate, s);
				return textPlate;
			} 
			return null;
		}

		protected GameObject GetSumTotal(string text){
			
			if (text != null) 
			{
				GameObject totalSumObj = Instantiate (Resources.Load("Prefabs/SimpleView/TotalSum")) as GameObject;
				SetText (totalSumObj, text);
				return totalSumObj;
			}
			return null;
		}

		protected GameObject GetButton(string text, UnityAction onClick)
		{
			if (text != null)
			{ 
				GameObject buttonObj = Instantiate (Resources.Load ("Prefabs/SimpleView/Button")) as GameObject;
				SetText (buttonObj, text);
				buttonObj.GetComponentInChildren<Button> ().onClick.AddListener (onClick);
				return buttonObj;
			}
			return null;
		}

		protected GameObject GetHelp(XsollaTranslations translations)
		{
			if (translations != null)
			{ 
				GameObject helpObj = Instantiate (Resources.Load ("Prefabs/SimpleView/Help")) as GameObject;
				Text[] texsts = helpObj.GetComponentsInChildren<Text>();
				texsts[0].text = translations.Get(XsollaTranslations.SUPPORT_PHONE);
				texsts[1].text = translations.Get(XsollaTranslations.SUPPORT_NEED_HELP);
				texsts[2].text = "support@xsolla.com";
				texsts[3].text = translations.Get(XsollaTranslations.SUPPORT_CUSTOMER_SUPPORT);
				return helpObj;
			}
			return null;
		}

		protected GameObject GetClose(UnityAction onClick)
		{
			if (onClick != null)
			{ 
				GameObject buttonObj = Instantiate (Resources.Load ("Prefabs/SimpleView/Close")) as GameObject;
				buttonObj.GetComponentInChildren<Button> ().onClick.AddListener (onClick);
				return buttonObj;
			}
			return null;
		}

		protected GameObject GetEmpty()
		{
			return Instantiate (Resources.Load ("Prefabs/SimpleView/Empty")) as GameObject;
		}

		protected void OnErrorRecived(XsollaError error)
		{
			if(ErrorHandler != null)
				ErrorHandler(error);
		}

		public string  GetFirstAHrefText(string s){
			int start = s.IndexOf("<a");
			int end = s.IndexOf("a>");
			string taggedText = s.Substring(start, end - start + 2);
			string[] text = taggedText.Split(new Char [] {'<', '>'});
			return text [2];
		}

		protected void SetImage(GameObject go, string imgUrl)
		{
			Image[] i2 = go.GetComponentsInChildren<Image>();
			GetComponent<ImageLoader>().LoadImage(i2[1], imgUrl);
		}

		protected void SetText(GameObject go, string s)
		{
			go.GetComponentInChildren<Text>().text = s;
		}

		protected void SetText(Text text, string s)
		{
			text.text = s;
		}

		protected void ResizeToParent()
		{
			RectTransform containerRectTransform = GetComponent<RectTransform>();
			RectTransform parentRectTransform = transform.parent.gameObject.GetComponent<RectTransform> ();
			float parentHeight = parentRectTransform.rect.height;
			float parentWidth = parentRectTransform.rect.width;
			float parentRatio = parentWidth/parentHeight;// > 1 horizontal
			float width = containerRectTransform.rect.width;
			if (parentRatio < 1) {
				containerRectTransform.offsetMin = new Vector2 (-parentWidth/2, -parentHeight/2);
				containerRectTransform.offsetMax = new Vector2 (parentWidth/2, parentHeight/2);
			} else {
				float newWidth = parentWidth/3;
				if(width < newWidth){
					containerRectTransform.offsetMin = new Vector2 (-newWidth/2, -parentHeight/2);
					containerRectTransform.offsetMax = new Vector2 (newWidth/2, parentHeight/2);
				}
			}
		}

	}

}
