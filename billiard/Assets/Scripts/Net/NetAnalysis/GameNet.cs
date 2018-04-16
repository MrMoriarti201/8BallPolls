using UnityEngine;
using System.Collections;

namespace Game {
	public class GameEnd: NetDataAnalysisBase {
		
		public int coin_bonus {get;set;}
		public int mis_id {get;set;}
		public int levelBonus {get;set;}
		public int rankBonus {get;set;}
		public int rankBonusType {get;set;}
		//Profile Informations.
		public int crown {get;set;}
		public int level {get;set;}
		public int rank {get;set;}
		public int point {get;set;}
		public int maxPoint {get;set;}
		public int coin {get;set;}
		public int cash {get;set;}
		public string rankName {get;set;}

		private GamePlayUI recvScript;

		public override string url {
			get {
				return urlbase+"GameEnd";
			}
		}
		
		public override string[] allparams {
			get {
                return new string[]{ "isWin="+((Player.win==1)?"1":"0"),"isTournament="+((GlobalInfo.IsTournament())?"1":"0")
                ,"trophy_num="+trophy_num.ToString(),"entrance="+bet.ToString(), "isFriendMatch=" + ((GlobalInfo.IsFriendMatch()) ? "1" : "0")};//,"r="+ (Tournament.rn++).ToString()};
			}
		}
		int trophy_num;
        int bet;
		public GameEnd(GamePlayUI script) {
			recvScript=script;
			trophy_num=0;
			rankName="Newbie";
			if (!GlobalInfo.trophyList[14] && Player.trophy==Trohpy_Type.Pot7Ball){
				trophy_num=15;
			}
			else if (!GlobalInfo.trophyList[15] && Player.trophy==Trohpy_Type.Pot8Ball){
				trophy_num=16;
			}
			else if (!GlobalInfo.trophyList[17] && Player.trophy==Trohpy_Type.Pocket2Balls){
				trophy_num=18;
			}

            bet = GlobalInfo.bet_index;
            if (GlobalInfo.IsTournament())
            {
                bet = 0;
                if (Player.win != 1)
                {
                    bet = GlobalInfo.bet_index;  // When Lose, - coin.
                    GlobalInfo.tournamentCnt++;
                }
                else if (Tournament.TournamentMatch == 2)  //Tournament Final Win
                {
                    bet = GlobalInfo.bet_index;
                    ((TableItem)(GlobalInfo.cue_List[GlobalInfo.tournamentCue[bet]])).purchase_mode = PURCHASE_MODE.BOUGHT;
                    GlobalInfo.tournamentCnt++;
                    GlobalInfo.tournamentWin++;
                }                
            }
		}
		/// <summary>
		/// After it 's time for finish counting.
		/// </summary>
		public override bool Analysis (NetIO nio) {
			nio.ReadJsonValue( this );

			GlobalInfo.coin=coin;
			GlobalInfo.cash=cash;
            GlobalInfo.myProfile.point = point;

			if (levelBonus>0){
				GlobalInfo.myProfile.level=level;
                GlobalInfo.myProfile.maxPoint = maxPoint;
				if (rankBonus>0){
					GlobalInfo.myProfile.rank=rank;			
					GlobalInfo.rankName=rankName;

					GlobalInfo.crown=(crown>0);
				}
			}

			if( recvScript != null ) {
				recvScript.RecvGameEnd( this );
			}
			return true;
		}
	}

    public class GetMaxTournamentId : NetDataAnalysisBase
    {
        public int t_id { get; set;}        
        public override string url
        {
            get
            {
                return urlbase + "GetMaxTournamentId";
            }
        }
        public override string[] allparams
        {
            get
            {
                return new string[] {};
            }
        }
        public GetMaxTournamentId() {}

        public override bool Analysis(NetIO nio)
        {
            nio.ReadJsonValue(this);
            TournamentChart.instance.GetComponent<PhotonView>().RPC("SetT_ID", PhotonTargets.AllBuffered, t_id + 1);
            return true;
        }
    }

    public class SetTournamentBall : NetDataAnalysisBase
    {
        public override string url
        {
            get
            {
                return urlbase + "SetTournamentBall";
            }
        }
        public override string[] allparams
        {
            get
            {
                return new string[]{ "t_id="+Tournament.t_id.ToString(),"pos="+pos.ToString()
                    ,"i="+i.ToString(),"num="+num.ToString()};//,"g="+ (Tournament.rn++).ToString()};
            }
        }

        int i;
        int num;
        int pos;
        public SetTournamentBall(int pos0,int i0, int num0)
        {
            pos = pos0;
            i = i0;
            num = num0;
        }
        public override bool Analysis(NetIO nio)
        {
            nio.ReadJsonValue(this);         
            return true;
        }
    }

    public class BallStatus
    {
        public int id { get; set; }
        public int pos { get; set; }
        public int i { get; set; }
        public int num { get; set; }
        public BallStatus() { }
    }
    public class GetTournamentBall : NetDataAnalysisBase
    {
        public BallStatus[] ballStatus{ get; set; }
        public override string url
        {
            get
            {
                return urlbase + "GetTournamentBall";
            }
        }
        public override string[] allparams
        {
            get
            {
                return new string[]{ "t_id="+Tournament.t_id.ToString()};//,"r="+ (Tournament.rn++).ToString()};
            }
        }
        public GetTournamentBall() {
            ballStatus= new BallStatus[1];
            ballStatus[0] = new BallStatus();
        }

        public override bool Analysis(NetIO nio)
        {
            nio.ReadJsonValue(this);
            for (int i=0;i<ballStatus.Length;i++){
                TournamentChart.instance.GetComponent<PhotonView>().RPC("SetBalls", PhotonTargets.AllBuffered
                    , ballStatus[i].pos,ballStatus[i].i,ballStatus[i].num);
            }
            return true;
        }
    }

    public class SetTournamentWinner : NetDataAnalysisBase
    {
        public override string url
        {
            get
            {
                return urlbase + "SetTournamentWinner";
            }
        }
        public override string[] allparams
        {
            get
            {
                return new string[]{ "t_id="+Tournament.t_id.ToString(),"pos="+pos.ToString(),"pos0="+pos0.ToString()};
            }
        }

        int pos0;
        int pos;
        public SetTournamentWinner(int po, int winnerId)
        {
            pos = po;
            pos0 = winnerId;            
        }
        public override bool Analysis(NetIO nio)
        {
            nio.ReadJsonValue(this);
            return true;
        }
    }

    public class WinnerList
    {
        public int pos { get; set; }
        public int pos0 { get; set; }
        public WinnerList() { }
    }
    public class GetTournamentWinner: NetDataAnalysisBase
    {
        public WinnerList[] winnerList { get; set; }
        public override string url
        {
            get
            {
                return urlbase + "GetTournamentWinner";
            }
        }
        public override string[] allparams
        {
            get
            {
                return new string[] { "t_id=" + Tournament.t_id.ToString(), "tournamentMatch=" + Tournament.TournamentMatch.ToString() };
            }
        }
        public GetTournamentWinner()
        {
            winnerList = new WinnerList[1];
            winnerList[0] = new WinnerList();
        }

        public override bool Analysis(NetIO nio)
        {
            nio.ReadJsonValue(this);
            for (int i = 0; i < winnerList.Length; i++)
            {
                TournamentChart.instance.GetComponent<PhotonView>().RPC("SetWinner", PhotonTargets.AllBuffered
                    , winnerList[i].pos, winnerList[i].pos0);
            }
            return true;
        }
    }
}