using UnityEngine;
using System.Collections;

namespace UserManager {
	public class ShopItem {
		public int num {get;set;}		
		public ShopItem () {}
	}
	public class Boosts {		
		public int quantity {get;set;}		
		public Boosts () {}
	}

	public class Trophy {		
		public int status {get;set;}		
		public Trophy () {
		}
	}
    
	public class UserLogin: NetDataAnalysisBase {
		private Login recvScript;
		public int id {get;set;}
		public string NickName{ get;set; }
		public int Country {get;set;}
		public int rank {get;set;}
		public int level {get;set;}
		public int point {get;set;}
		public int maxPoint {get;set;}
		public int coin {get;set;}
        public int cash { get; set; }
        public string avatar { get; set; }
        public int playCnt { get; set; }
		public int winCnt {get;set;}
		public int tournamentWin {get;set;}
		public int tournamentCnt {get;set;}

		public ShopItem[] tableColor {get;set;}
		public ShopItem[] tableFrame {get;set;}
		public ShopItem[] tablePattern {get;set;}
		public ShopItem[] data_cue {get;set;}
		public ShopItem[] data_avatar {get;set;}
		public Boosts[] boosts {get;set;}
		public Trophy[] trophyList {get;set;}
        public Trophy[] tournamentList { get; set;}
	
		public Profile[] friends {
			get{ return GlobalInfo.friends; }
			set{ GlobalInfo.friends = value; } 
		}

		public override string url {
			get {
				return urlbase+"checklogin";
			}
		}		
		string _username;
		public override string[] allparams {
			get {
				return new string[]{"username="+_username};
			}
		}
		
		public UserLogin( Login script,string username) {
            recvScript=script;
			_username = username;
			
			NickName="";
            avatar = "";

			tableColor=new ShopItem[1];
			tableColor[0]=new ShopItem();
			tableFrame=new ShopItem[1];
			tableFrame[0]=new ShopItem();
			tablePattern=new ShopItem[1];
			tablePattern[0]=new ShopItem();
			data_cue=new ShopItem[1];
			data_cue[0]=new ShopItem();
			data_avatar=new ShopItem[1];
			data_avatar[0]=new ShopItem();

			boosts=new Boosts[1];
			boosts[0]=new Boosts();

            trophyList = new Trophy[1];
            trophyList[0] = new Trophy();

            tournamentList = new Trophy[1];
            tournamentList[0] = new Trophy();

			friends=new Profile[1];
			friends[0]=new Profile();
		}
		
		public override bool Analysis (NetIO nio) {
            nio.ReadJsonValue( this );            
			if( recvScript != null ) {				
				recvScript.RecvLogin( this);
			}
			return true;
		}
	}

    public class GetUserInfo : NetDataAnalysisBase
    {
        private ProfileScene recvScript;
        public int id { get; set; }
        public string NickName { get; set; }
        public int Country { get; set; }
        public int rank { get; set; }
        public int level { get; set; }
        public int point { get; set; }
        public int maxPoint { get; set; }
        public int avatar { get; set; }
        public int playCnt { get; set; }
        public int winCnt { get; set; }
        public int tournamentWin { get; set; }
        public int tournamentCnt { get; set; }

        public Trophy[] trophyList { get; set; }
        public Trophy[] tournamentList { get; set; }

        public override string url
        {
            get
            {
                return urlbase + "GetUserInfo";
            }
        }

        public override string[] allparams
        {
            get
            {
                return new string[] { "id=" +ProfileOpen.id.ToString()};
            }
        }

        public GetUserInfo(ProfileScene script)
        {
            recvScript = script;            
            NickName = "";
        
            trophyList = new Trophy[1];
            trophyList[0] = new Trophy();

            tournamentList = new Trophy[1];
            tournamentList[0] = new Trophy();
        }

        public override bool Analysis(NetIO nio)
        {
            nio.ReadJsonValue(this);
            if (recvScript != null)
            {
                recvScript.GetUserInfo(this);
            }
            return true;
        }
    }

    public class GetCupInfo : NetDataAnalysisBase
    {
        private ProfileScene script;
        public Trophy[] tournamentList { get; set; }

        public override string url
        {
            get
            {
                return urlbase + "GetCupInfo";
            }
        }

        public override string[] allparams
        {
            get
            {
                return new string[] {};
            }
        }

        public GetCupInfo (ProfileScene script0)
        {
            script = script0;
            tournamentList = new Trophy[1];
            tournamentList[0] = new Trophy();            
        }

        public override bool Analysis(NetIO nio)
        {
            nio.ReadJsonValue(this);
            if (script != null)
            {
                script.ReceiveCupList(this);
            }
            return true;
        }
    }
	public class AddCoin: NetDataAnalysisBase {
		public int coin {get;set;}
		int value;
		public override string url {
			get {
				return urlbase+"AddCoin";
			}
		}		
		public override string[] allparams {
			get {
				return new string[]{"value="+value.ToString()};
			}
		}		
		public AddCoin(int val) {
			value=val;
		}
		
		public override bool Analysis (NetIO nio) {
			nio.ReadJsonValue( this );
			GlobalInfo.coin=coin;
			if (CashUI.instance)
				CashUI.instance.UpdateValue();
			return true;
		}
	}

	public class AddCash: NetDataAnalysisBase {
		public int cash {get;set;}
		int value;
		public override string url {
			get {
				return urlbase+"AddCash";
			}
		}		
		public override string[] allparams {
			get {
				return new string[]{"value="+value.ToString()};
			}
		}		
		public AddCash(int val) {
			value=val;
		}
		
		public override bool Analysis (NetIO nio) {
			nio.ReadJsonValue( this );
			GlobalInfo.cash=cash;
			if (CashUI.instance)
				CashUI.instance.UpdateValue();
			return true;
		}
	}

    public class AddFriend: NetDataAnalysisBase
    {
        public override string url
        {
            get
            {
                return urlbase + "AddFriend";
            }
        }
        public override string[] allparams
        {
            get
            {
                return new string[] { "id=" + GlobalInfo.opponentProfile.user_id.ToString() };
            }
        }
        public AddFriend()
        {   
        }

        public override bool Analysis(NetIO nio)
        {
            nio.ReadJsonValue(this);         
            return true;
        }
    }

    public class SearchFriend : NetDataAnalysisBase
    {
        public Profile searchRes { get; set; }
        public override string url
        {
            get
            {
                return urlbase + "SearchFriend";
            }
        }
        public override string[] allparams
        {
            get
            {
                return new string[] { "searchTxt=" +searchTxt };
            }
        }
        string searchTxt;
        WithFriends script;
        public SearchFriend(WithFriends scri,    string str)
        {
            script = scri;
            searchTxt=str;
            searchRes = new Profile();
        }

        public override bool Analysis(NetIO nio)
        {
            nio.ReadJsonValue(this);
            if (script!=null)
            {
                script.OnSearchReceive(searchRes);
            }
            return true;
        }
    }

    public class OnlineStatus : NetDataAnalysisBase
    {
        public override string url
        {
            get
            {
                return urlbase + "OnlineStatus";
            }
        }
        public override string[] allparams
        {
            get
            {
                return new string[] {"status=" + GameManager.status.ToString() };
            }
        }
        int status;
        public OnlineStatus()
        {         
        }

        public override bool Analysis(NetIO nio)
        {
            nio.ReadJsonValue(this);
            return true;
        }
    }
    public class GetFriendOnlineStatus : NetDataAnalysisBase
    {
        public Trophy[] statusList { get; set; }
        public override string url
        {
            get
            {
                return urlbase + "GetFriendOnlineStatus";
            }
        }
        public override string[] allparams
        {
            get
            {
                return new string[] { };
            }
        }
        string searchTxt;
        WithFriends script;
        public GetFriendOnlineStatus(WithFriends scri)
        {
            script = scri;
            statusList = new Trophy[1];
            statusList[0] = new Trophy();
        }

        public override bool Analysis(NetIO nio)
        {
            nio.ReadJsonValue(this);
            if (script != null)
            {
				script.OnOnlineStatusReceive(this);
            }
            return true;
        }
    }
}