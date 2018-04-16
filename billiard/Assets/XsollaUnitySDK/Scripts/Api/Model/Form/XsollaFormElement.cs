using System.Collections.Generic;
using SimpleJSON;
using System.Collections;
using System.Text;
using UnityEngine;

namespace Xsolla
{
	public class XsollaFormElement : IParseble
	{
//		private static string ELEM_NAME = "name";

		/* * * * * * * * * * * * * * * * * * * * * * * * * * *
	    * CONSTANTS
	    * * * * * * * * * * * * * * * * * * * * * * * * * * */
		public const int TYPE_UNKNOWN = -1;
		public const int TYPE_HIDDEN = 0;
		public const int TYPE_TEXT = 1;
		public const int TYPE_SELECT = 2;
		public const int TYPE_VISIBLE = 3;
		public const int TYPE_TABLE = 4;
		public const int TYPE_CHECK = 5;
		public const int TYPE_HTML = 6;
		public const int TYPE_LABEL = 7;//HTML

		private string name;
		private string title;
		private string type;
		private string example; // placeholder
		private string value;
		//TODO  Type.SELECT - массив обьектов; Type.TABLE где то обьект объектов
		private List<Option> options;
		private bool isMandatory;
		private bool isReadonly;
		private bool isVisible;
		private bool isPakets;
		private string tooltip;
		//TODO  комплексный объект
		private string javascript;

		public XsollaFormElement ()
		{
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * *
	    * PUBLIC METHODS
	    * * * * * * * * * * * * * * * * * * * * * * * * * * */
		public string GetName()
		{
			return name;
		}

		public string GetTitle()
		{
			return title;
		}

		public string GetValue()
		{
			return value;
		}

		public string GetExample()
		{
			return !"null".Equals(example) ? example : title;
		}

		public List<Option> GetOptions()
		{
			return options;
		}

		public bool IsVisible()
		{
			return isVisible;
		}

		public bool IsPackets()
		{
			return isPakets;
		}

		public int GetElementType() 
		{
			switch (type) {
				case "hidden":
					return TYPE_HIDDEN;
				case "text":
					return TYPE_TEXT;
				case "select":
					return TYPE_SELECT;
				case "isVisible":
					return TYPE_VISIBLE;
				case "table":
					return TYPE_TABLE;
				case "check":
					return TYPE_CHECK;
				case "html":
					return TYPE_HTML;
				case "label":
					return TYPE_LABEL;
				default:
					return TYPE_UNKNOWN;
			}
		}

		
		public void SetValue(string value){
			this.value = value;
		}

		public static string FormatPrice(string currency, string amount){
			switch (currency) {
				case "USD":
					amount = "$" + amount;
					break;
				case "EUR":
					amount = "&euro;" + amount;
					break;
				case "GBP":
					amount = "&pound;" + amount;
					break;
				case "BRL":
					amount = "R$" + amount;
					break;
				case "RUB":
					amount = amount + "₽";
					break;//&#8399;
				default:
					amount = amount + currency;
					break;//&#8399;
			}
			return amount;
		}
		
		/* * * * * * * * * * * * * * * * * * * * * * * * * * *
		* PRIVATE METHODS
		* * * * * * * * * * * * * * * * * * * * * * * * * * */
		private List<Option> ParseOptions(JSONNode optionsNode) {
			List<Option> options = new List<Option>();
			return options;
		}

		private void SetName(string name){
			this.name = name;
		}
		
		private void SetTitle(string title){
			this.title = title;
		}
		
		private void SetType(string type){
			this.type = type;
		}
		
		private void SetExample(string example){
			this.example = example;
		}

		private void SetOptions(JSONNode optionsNode){
			List<Option> options = new List<Option>(optionsNode.Count);
			IEnumerable<JSONNode> optionsEnumerable = optionsNode.Childs;
			IEnumerator<JSONNode> optionsEnumerator = optionsEnumerable.GetEnumerator ();
			while(optionsEnumerator.MoveNext()){
				JSONNode optionNode = optionsEnumerator.Current;
				options.Add(new Option(optionNode["value"], optionNode["label"]));
			}
			this.options = options;
		}

		private void SetMandatory(bool isMandatory){
			this.isMandatory = isMandatory;
		}

		private void SetReadonly(bool isReadonly){
			this.isReadonly = isReadonly;
		}

		private void SetVisible(bool isVisible){
			this.isVisible = isVisible;
		}

		private void SetPakets(bool isPakets){
			this.isPakets = isPakets;
		}

		private void SetTooltip(string tooltip){
			this.tooltip = tooltip;
		}

		private void SetJavascript(string javascript){
			this.javascript = javascript;
		}		
		
		/* * * * * * * * * * * * * * * * * * * * * * * * * * *
		* OVERRIDED METHODS
		* * * * * * * * * * * * * * * * * * * * * * * * * * */
		public IParseble Parse(JSONNode obj) 
		{
			this.SetName (obj ["name"]);
			this.SetTitle (obj ["title"]);
			this.SetType (obj ["type"]);
			this.SetExample (obj ["example"]);
			this.SetValue (obj ["value"]);
			this.SetOptions (obj ["options"]);
			this.SetMandatory (obj ["isMandatory"].AsInt > 0);
			this.SetReadonly (obj ["isReadonly"].AsInt > 0);
			this.SetVisible (obj ["isVisible"].AsInt > 0);
			this.SetPakets (obj ["isPakets"].AsInt > 0);
			this.SetTooltip (obj ["tooltip"]);
			this.SetJavascript (obj ["javascript"]);
			return this;
		}
		


		public override string ToString ()
		{
			StringBuilder builder = new StringBuilder();
			foreach(Option o in options){
				builder.Append("\u0009/").Append(o.ToString());
			}

			return string.Format ("[XsollaFormElement]"
			                      + "\n name= " + name 
			                      + "\n title= " + title 
			                      + "\n type=" + type 
			                      + "\n options=" + builder.ToString() 
			                      + "\n isMandatory=" + isMandatory 
			                      + "\n isReadonly=" + isReadonly 
			                      + "\n isVisible=" + isVisible 
			                      + "\n tooltip=" + tooltip 
			                      + "\n javascript=" + javascript);
		}


		
		/* * * * * * * * * * * * * * * * * * * * * * * * * * *
		* INNER CLASS
		* * * * * * * * * * * * * * * * * * * * * * * * * * */
		public class Option {
			private string value;
			private string label;
			
			Option() {
			}
			
			public Option(string value, string label) {
				this.value = value;
				this.label = label;
			}
			
			public string GetValue() {
				return value;
			}
			
			public void SetValue(string value) {
				this.value = value;
			}
			
			public string GetLabel() {
				return label;
			}
			
			public void SetLabel(string label) {
				this.label = label;
			}
			
			public override string ToString ()
			{
				return string.Format ("[Option]"
				                      + "\n value= " + value 
				                      + "\n label= " + label );
			}
			
		}
	}


}

