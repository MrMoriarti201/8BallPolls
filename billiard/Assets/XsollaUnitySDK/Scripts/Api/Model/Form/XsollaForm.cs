using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Xsolla
{
    public class XsollaForm : IParseble
    {
		
		private String xpsPrefix = "xps_";
		private int size = 0;

		private string account;
		private string accountXsolla;

        private String currentCommand;
        private String title;
        private String iconUrl;
        private String currency;
        private String sum;
        private String instruction;
        private int pid;

		private string checkoutToken;
		private XsollaTextAll textAll;
		private XsollaError xsollaError;
		private XsollaSummary summary;
		private XsollaCheckout chekout;
		private BuyData buyData;

		private List<XsollaFormElement> elements;
		private List<XsollaFormElement> elementsVisible;
		private Dictionary<string, XsollaFormElement> map;
		private Dictionary<string, object> xpsMap;

		public enum CurrentCommand {
			FORM, STATUS, CHECK, ACCOUNT, UNKNOWN
		}
		
		public XsollaForm() {
			elements = new List<XsollaFormElement>();
			elementsVisible = new List<XsollaFormElement>();
			map = new Dictionary<string, XsollaFormElement>();
			xpsMap = new Dictionary<string, object>();
        }

		/* * * * * * * * * * * * * * * * * * * * * * * * * * *
	    * PUBLIC METHODS
	    * * * * * * * * * * * * * * * * * * * * * * * * * * */
		public void AddElement(XsollaFormElement xsollaFormElement) {
			elements.Add(xsollaFormElement);
			if (xsollaFormElement.IsVisible() && !"couponCode".Equals(xsollaFormElement.GetName()))
				elementsVisible.Add(xsollaFormElement);
			map.Add(xsollaFormElement.GetName(), xsollaFormElement);
			if (xsollaFormElement.GetName () != null) 
			{	
				xpsMap.Add (xpsPrefix + xsollaFormElement.GetName (), xsollaFormElement.GetValue ());
			}
			size++;
		}

		
		public void UpdateElement(String name, String newValue) {
			if (map.ContainsKey (name)) {
				GetItem (name).SetValue (newValue);
				string key = xpsPrefix + name;
				xpsMap [key] = newValue;
			} else {
				string key = xpsPrefix + name;
				xpsMap.Add(key, newValue);
			}
		}

		public bool Contains(string name)
		{
			return map.ContainsKey (name);
		}

		public void Clear() {
			elements.Clear();
			elementsVisible.Clear();
			map.Clear ();
			xpsMap.Clear ();
		}

		public XsollaFormElement GetItem(string name)
		{
			if(map.ContainsKey(name))
				return map[name];
			return null;
		}

		public List<XsollaFormElement> GetVisible()
		{
			return elementsVisible;
		}

		public Dictionary<string, object> GetXpsMap()
		{
			return xpsMap;
		}

		public XsollaSummary GetSummary()
		{
			return summary;
		}

		public XsollaCheckout GetCheckout()
		{
			return chekout;
		}

		public bool IsValidPaymentSystem() {
			return pid == 26 || pid == 1380;
		}

		public string GetAccount()
		{
			return account;
		}

		public string GetAccountXsolla()
		{
			return accountXsolla;
		}

		public CurrentCommand GetCurrentCommand() 
		{
			switch (currentCommand) {
				case "form":
					return CurrentCommand.FORM;
				case "check":
					return CurrentCommand.CHECK;
				case "status":
					return CurrentCommand.STATUS;
				case "account":
					return CurrentCommand.ACCOUNT;
				default:
					return CurrentCommand.UNKNOWN;
			}
		}

		public string GetInstruction()
		{
			return instruction;
		}

		public XsollaError GetError ()
		{
			return xsollaError;
		}

		public string GetTitle()
		{
			return title;
		}

		public string GetSumTotal()
		{
			if(buyData != null && buyData.sum != null && buyData.currency != null){
				return PriceFormatter.Format(buyData.sum, buyData.currency);
			} else if (summary != null) {
				return PriceFormatter.Format(summary.GetFinance().total.amount, summary.GetFinance().total.currency);
			} else {
				if(sum != null && currency != null){
					return PriceFormatter.Format(sum, currency);
				} else {
					return "";
				}
			}
		}

		public string GetNextStepString()
		{
			if ("1".Equals(buyData.isVisible)) {
				return buyData.value;
			} else {
				return "Continue";
			}
		}

		public string GetIconUrl()
		{
			if(iconUrl.StartsWith("https:"))
				return iconUrl;
			else 
				return "https:" + iconUrl;
		}

		public static String FormatPrice(String currency, String amount){
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
					break;
			}
			return amount;
		}

		public string GetCheckoutToken()
		{
			if (checkoutToken == null)
				return "";
			else 
				return checkoutToken.ToString();
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * *
		* PRIVATE METHODS
		* * * * * * * * * * * * * * * * * * * * * * * * * * */
		private void SetAccount(string account)
		{
			this.account = account;
		}

		private void SetAccountXsolla(string accountXsolla)
		{
			this.accountXsolla = accountXsolla;
		}

		private void SetCurrentCommand(string currentCommand)
		{
            this.currentCommand = currentCommand;
        }

		private void SetTitle(string title)
		{
            this.title = title;
        }

		private void SetIconUrl(string iconUrl)
		{
            this.iconUrl = iconUrl;
        }

		private void SetCurrency(string currency)
		{
            this.currency = currency;
        }

		private void SetSum(string sum){
            this.sum = sum;
        }

		private void SetInstruction(string instruction)
		{
            this.instruction = instruction;
        }

		private void SetPid(int pid)
		{
            this.pid = pid;
        }


		/* * * * * * * * * * * * * * * * * * * * * * * * * * *
		* OVERRIDED METHODS
		* * * * * * * * * * * * * * * * * * * * * * * * * * */
		public IParseble Parse(JSONNode rootNode)
		{

//			textAll = new XsollaTextAll ();
//			textAll.Parse (rootNode[XsollaApiConst.R_TEXTALL]);
			JSONNode errorNode = rootNode [XsollaApiConst.ERROR_MSG];
			if (errorNode != null && errorNode.GetType() != typeof(JSONLazyCreator))
			{
				this.xsollaError = new XsollaError().Parse(errorNode) as XsollaError;
			}
			//errorNode = rootNode [XsollaApiConst.R_TEXTALL] [XsollaApiConst.ERROR_MSG][0];
			errorNode = rootNode ["errors"][0];
			if (errorNode != null) 
			{
				this.xsollaError = new XsollaError().Parse(errorNode) as XsollaError;
			}
			this.SetAccount (rootNode [XsollaApiConst.R_ACCOUNT]);
			this.SetAccountXsolla (rootNode [XsollaApiConst.R_ACCOUNTXSOLLA]);
			this.SetCurrentCommand (rootNode[XsollaApiConst.R_CURRENTCOMMAND]);
			this.SetTitle (rootNode[XsollaApiConst.R_TITLE]);
			this.SetIconUrl (rootNode[XsollaApiConst.R_ICONURL]);
			this.SetCurrency (rootNode[XsollaApiConst.R_CURRENCY]);
			this.SetSum (rootNode[XsollaApiConst.R_BUYDATA]["sum"]);
			this.SetInstruction (rootNode[XsollaApiConst.R_INSTRUCTION]);
			this.SetPid (rootNode[XsollaApiConst.R_PID].AsInt);

			checkoutToken = rootNode ["checkoutToken"];
			JSONNode buyDataNode = rootNode ["buyData"];
			if (buyDataNode != null && !"null".Equals (buyDataNode)) {
				buyData = new BuyData ().Parse (buyDataNode) as BuyData;
			}

			JSONNode summaryNode = rootNode ["summary"];
			if (summaryNode != null && !"null".Equals(summaryNode)) {
				summary = new XsollaSummary ().Parse (summaryNode) as XsollaSummary;
			}

			JSONNode checkoutNode = rootNode ["checkout"];
			if(checkoutNode != null && !"null".Equals(checkoutNode)){
				chekout = new XsollaCheckout ().Parse (checkoutNode) as XsollaCheckout;
			}

			JSONNode formNode = rootNode [XsollaApiConst.R_FORM];
			IEnumerable<JSONNode> formElements = formNode.Childs;
			IEnumerator<JSONNode> enumerator = formElements.GetEnumerator();
			this.Clear ();
			while(enumerator.MoveNext())
			{
				this.AddElement((XsollaFormElement)new XsollaFormElement().Parse(enumerator.Current));
			}
            return this;
        }

		public override string ToString ()
		{
			StringBuilder builder = new StringBuilder();
			foreach(XsollaFormElement e in elements){
				builder.Append("\n/").Append(e.ToString());
			}
			return string.Format ("[XsollaForm]"
			                      + "\n currentCommand= " + currentCommand
			                      + "\n title= " + title
			                      + "\n iconUrl= " + iconUrl
			                      + "\n currency= " + currency
			                      + "\n sum= " + sum
			                      + "\n instruction= " + instruction
			                      + "\n pid= " + pid
			                      + "\n xsollaError= " + xsollaError
			                      + "\n elements= " + builder.ToString());
		}
    }
}
