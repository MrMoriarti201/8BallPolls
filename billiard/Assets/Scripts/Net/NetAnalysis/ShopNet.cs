using UnityEngine;
using System.Collections;

namespace ShopNet {

	public class BuyTable: NetDataAnalysisBase {
		private TableTab recvScript;
		public override string url {
			get {
				return urlbase+"BuyTable";
			}
		}
		public override string[] allparams {
			get {
				return new string[]{ "table="+tableName,"price="+item.price.ToString(),"id="+id.ToString(),"type="+type.ToString()};
			}
		}
		string tableName;
		TableItem item;
		int type;
		int id;
		CuesTab cueScript;
		AvatarTab avatarScript;
		public BuyTable( CuesTab script,string name,TableItem item0,int id0) {
			tableName=name;
			item=item0;
			cueScript= script;
			type=(int)(item.price_type);
			id=id0;
		}
		public BuyTable( AvatarTab script,string name,TableItem item0,int id0) {
			tableName=name;
			item=item0;
			avatarScript= script;
			type=(int)(item.price_type);
			id=id0;
		}
		public BuyTable( TableTab script,string name,TableItem item0,int id0) {
			tableName=name;
			item=item0;
			recvScript = script;
			type=(int)(item.price_type);
			id=id0;
		}
		public int value {get;set;}
		public override bool Analysis (NetIO nio) {
			nio.ReadJsonValue( this );
			if (type==1){//coin
				GlobalInfo.coin=value;
			}
			else
				GlobalInfo.cash=value;
			if (CashUI.instance)
				CashUI.instance.UpdateValue();
			item.purchase_mode=PURCHASE_MODE.BOUGHT;
			if (tableName=="tablecolor"){
				recvScript.colorBuyBtn.SetActive(false);
				recvScript.colorUseBtn.SetActive(true);
			}
			else if (tableName=="tableframe"){
				recvScript.ShowTableSegment(id);
			}
			else if (tableName=="tablepattern"){
				recvScript.patternBuyBtn.SetActive(false);
				recvScript.patternUseBtn.SetActive(true);
			}
			else if (tableName=="data_cue"){
				cueScript.ShowCue(id);
			}
			else if (tableName=="data_avatar"){
				avatarScript.ShowAvatar(id);
			}
			return true;
		}
	}

	public class SetBoosts: NetDataAnalysisBase {
		public override string url {
			get {
				return urlbase+"SetBoosts";
			}
		}
		int id,val;
		public override string[] allparams {
			get {
				return new string[]{ "id="+id.ToString(),"value="+val.ToString()};
			}
		}

		public SetBoosts(int id0,int val0) {
			id=id0;
			val=val0;
		}

		public int value {get;set;}

		public override bool Analysis (NetIO nio) {
			nio.ReadJsonValue( this );
			GlobalInfo.cash=value;
			if (CashUI.instance)
				CashUI.instance.UpdateValue();
			return true;
		}
	}

	public class SetAvatar: NetDataAnalysisBase {
		public override string url {
			get {
				return urlbase+"SetAvatar";
			}
		}

		public override string[] allparams {
			get {
                return new string[] { "avatar=" + GlobalInfo.myProfile.avatar };
			}
		}
		
		public SetAvatar() {
		}

		public override bool Analysis (NetIO nio) {
			nio.ReadJsonValue( this );		
			return true;
		}
	}

}