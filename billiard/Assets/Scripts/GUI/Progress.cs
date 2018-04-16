using System;
using UnityEngine;
using System.Collections;

public class Progress : MonoBehaviour {

    static public Progress instance;
	private float progress=0.0f;	
	private string[] country_list = {
		"Afghanistan", "Albania", "Algeria", "American Samoa", "Andorra", "Angola", "Anguilla", "Antigua and Barbuda", "Argentina",
		"Armenia", "Aruba", "Australia", "Austria", "Azerbaijan", "Bahrain", "Bangladesh", "Barbados", "Belgium", "Belize", "Benin",
		"Bermuda", "Bhutan", "Bolivia", "Bosnia and Herzegovina", "Bostswana", "Bouvet Island", "Brazil", "British Indian Ocean Territory",
		"British Virgin Islands", "Brunei", "Bulgaria", "Burkina Faso", "Burundi", "Cambodia", "Cameroon", "Canada",
		"Cape Verde", "Cayman Islands", "Central African Republic", "Chad", "Chile", "China", "Christmas Islands",
		"Cocos (Keeling) Islands", "Colombia", "Comoros", "Cook Islands", "Costa Rica", "Cote d'Ivoire", "Croatia",	"Cuba", "Cyprus",
		"Czech Republic", "Democratic Republic of the Congo", "Denmark", "Djibouti", "Dominica", "Dominican Republic", "Ecuador", "Egypt",
		"El Salvador", "England", "Equatorial Guinea", "Eritrea", "Estonia", "Ethiopia", "European Union",
		"Falkland Islands (Islas Malvinas)", "Faroe Islands", "Fiji", "Finland", "France", "French Poynesia", "Gabon", "Georgia",
		"Germany", "Ghana", "Gibraltar", "Greece", "Greenland", "Grenada", "Guam", "Guatemala", "Guernsey", "Guinea Blissau", "Guinea",
		"Guyana", "Haiti", "Holy See (Vatican City)", "Honduras", "Hong Kong", "Hungary", "Iceland", "India", "Indonesia", "Iran", "Iraq",
		"Ireland", "Isle of Man", "Israel", "Italy", "Jamaica", "Japan", "Jordan", "Kazahstan", "Kenya", "Kiribati", "Kosovo",
		"Kurdistan Nation",	"Kuwait", "Kyrgyzstan", "Laos", "Latvia", "Lebanon", "Lesotho", "Liberia", "Libya", "Liechtenstein",
		"Lithuania", "Luxembourg", "Macau", "Macedonia", "Madagascar", "Malawi", "Malaysia", "Maldives", "Mali", "Malta",
		"Marschal Islands", "Mauritania", "Mauritius", "Mayotte", "Mexico", "Micronesia", "Moldavia", "Monaco", "Mongolia", "Montenegro",
		"Montserrat", "Morocco", "Mozambique", "Myanmar", "Namibia", "Nauru", "Nepal", "Netherlands Anthilles", "Netherlands", "New Zealand",
		"Nicaragua", "Niger", "Nigeria", "Niue", "Norfolk Island", "North Korea", "Northern Mariana Islands", "Norway", "Oman", "Pakistan",
		"Palau", "Palestine", "Panama", "Papua New Guinea", "Paraguay", "Peru", "Philippines", "Pitcairn Islands", "Poland", "Portugal",
		"Puerto Rico", "Quatar", "Republic of the Congo", "Romania", "Russia", "Rwanda", "Saint Helena", "Saint Kitts and Nevis",
		"Saint Lucia", "Saint Pierre and Miquelon", "Saint Vincent", "Samoa", "San Marino", "Sao Tome and Principe", "Saudi Arabia",
		"Scotland", "Senegal", "Serbia", "Seychelles", "Sierra Leone", "Singapore", "Slovakia", "Slovenia", "Solomon Islands", "Somalia",
		"South Africa", "South Georgia and the South Sandwitch Islands", "South Korea", "Spain", "Sri Lanka", "Sudan", "Suriname",
		"Swaziland", "Sweden", "Switzerland", "Syria", "Taiwan", "Tajikistan", "Tamil Nation", "Tanzania", "Thailand", "The Bahamas",
		"The Gambia", "Tibet", "Timor Leste", "Togo", "Tonga", "Trinidad and Tobago", "Tunisia", "Turkey", "Turkmenistan",
		"Turks and Caicos Islands", "Tuvalu", "Uganda", "Ukraine", "United Arab Emirates", "United Kingdom", "United States", "Uruguay",
		"Uzbekistan", "Vanuatu", "Venezuela", "Vietnam", "Virgin Islands", "Wales", "Wallis and Futuna", "Yemen", "Zambia", "Zimbabwe"
	};

    void Awake()
    {
        instance = this;
    }
	// Use this for initialization
	void Start () {		
		float zoom_rate=Screen.height/480.0f;

        if (GameManager.IsScene("Splash"))
        {
			GUIText loadingtext=GameObject.Find ("LoadingText").GetComponent<GUIText>();
			GUIText loadingtext_shadow=GameObject.Find ("LoadingTextShadow").GetComponent<GUIText>();
			loadingtext_shadow.fontSize=loadingtext.fontSize=(int)(20.0f*zoom_rate);
			loadingtext.pixelOffset=new Vector2(0,-100*zoom_rate);
			loadingtext_shadow.pixelOffset=new Vector2(2*zoom_rate,-102*zoom_rate);
		}

		LoadDataFromServer();

		GlobalInfo.tablecolor_id=PlayerPrefs.GetInt("table_color",1);
		GlobalInfo.tableFrame_id=PlayerPrefs.GetInt("table_frame",-1);
		GlobalInfo.tablePattern_id=PlayerPrefs.GetInt("table_pattern",-1);

		GlobalInfo.cue_id=PlayerPrefs.GetInt("cue",-1);
        GlobalInfo.myProfile = new Profile();
        GlobalInfo.opponentProfile = new Profile();
        
        UserData.Init();
        LoadBundles();
	}
    /*int loadBundleId = 0;*/
    public void LoadBundles()
    {
        /*loadBundleId++;*/
        if (GlobalInfo.tablecolor_id != 1)
        {
            TableItem color = (TableItem)GlobalInfo.tablecolor_List[GlobalInfo.tablecolor_id];
            GameManager.instance.LoadBundle("color" + color.fileName);            
        }
        if (GlobalInfo.tablePattern_id != -1)
        {
            TableItem pattern = (TableItem)GlobalInfo.tablepattern_List[GlobalInfo.tablePattern_id];
            GameManager.instance.LoadBundle(pattern.fileName);            
        }
        if (GlobalInfo.tableFrame_id != -1)
        {
            TableItem segment = (TableItem)GlobalInfo.tableseg_List[GlobalInfo.tableFrame_id];
            GameManager.instance.LoadBundle("frame" + segment.fileName);
        }
    }

	void LoadDataFromServer(){
		LoadTournamentBets();
		LoadPlayGameBets();
		
		LoadCueList();
		LoadAvatarList();
		LoadTableSegList ();
		LoadTableColorList();
		LoadTablePatternList();
		LoadCountryList();
	}

	void LoadTableSegList(){
		GlobalInfo.tableseg_List.Add (new TableItem(1,"19","19",1500,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tableseg_List.Add (new TableItem(2,"6","6",15,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tableseg_List.Add (new TableItem(3,"20","20",2000,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.tableseg_List.Add (new TableItem(4,"18","18",20,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tableseg_List.Add (new TableItem(5,"17","17",3500,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tableseg_List.Add (new TableItem(6,"9","9",35,PRICE_TYPE.CASH,PURCHASE_MODE.BOUGHT));
		GlobalInfo.tableseg_List.Add (new TableItem(7,"11","11",4000,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.tableseg_List.Add (new TableItem(8,"4","4",40,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tableseg_List.Add (new TableItem(9,"10","10",55,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tableseg_List.Add (new TableItem(10,"15","15",7500,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.tableseg_List.Add (new TableItem(11,"8","8",70,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tableseg_List.Add (new TableItem(12,"21","21",9000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tableseg_List.Add (new TableItem(13,"13","13",85,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tableseg_List.Add (new TableItem(14,"3","3",12000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tableseg_List.Add (new TableItem(15,"2","2",16000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tableseg_List.Add (new TableItem(16,"7","7",35000,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.tableseg_List.Add (new TableItem(17,"5","5",50000,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.tableseg_List.Add (new TableItem(18,"1","1",75000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tableseg_List.Add (new TableItem(19,"14","14",125000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tableseg_List.Add (new TableItem(20,"12","12",130000,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.tableseg_List.Add (new TableItem(21,"16","16",140000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
	}

	void LoadTableColorList(){
		GlobalInfo.tablecolor_List.Add (new TableItem(1,"1","1",50000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tablecolor_List.Add (new TableItem(2,"2","2",3000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tablecolor_List.Add (new TableItem(3,"3","3",45000,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.tablecolor_List.Add (new TableItem(4,"4","4",5500,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.tablecolor_List.Add (new TableItem(5,"5","5",15000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tablecolor_List.Add (new TableItem(6,"6","6",20000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tablecolor_List.Add (new TableItem(7,"7","7",7,PRICE_TYPE.CASH,PURCHASE_MODE.BOUGHT));
		GlobalInfo.tablecolor_List.Add (new TableItem(8,"8","8",30000,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.tablecolor_List.Add (new TableItem(9,"9","9",10,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tablecolor_List.Add (new TableItem(10,"10","10",1500,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tablecolor_List.Add (new TableItem(11,"11","11",11000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tablecolor_List.Add (new TableItem(12,"12","12",15,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tablecolor_List.Add (new TableItem(13,"13","13",1250,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.tablecolor_List.Add (new TableItem(14,"14","14",5,PRICE_TYPE.CASH,PURCHASE_MODE.BOUGHT));
		GlobalInfo.tablecolor_List.Add (new TableItem(15,"15","15",20,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
	}

	void LoadTablePatternList(){
		GlobalInfo.tablepattern_List.Add (new TableItem(1,"Jocker","jocker",10,PRICE_TYPE.CASH,PURCHASE_MODE.BOUGHT));
		GlobalInfo.tablepattern_List.Add (new TableItem(2,"Sun","sun",1500,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.tablepattern_List.Add (new TableItem(3,"Lion","lion",20,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tablepattern_List.Add (new TableItem(4,"Butterfly","butterfly",25,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tablepattern_List.Add (new TableItem(5,"Eagle","eagle",4500,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tablepattern_List.Add (new TableItem(6,"Skull","skull",35,PRICE_TYPE.CASH,PURCHASE_MODE.BOUGHT));
		GlobalInfo.tablepattern_List.Add (new TableItem(7,"Biker","biker",50,PRICE_TYPE.CASH,PURCHASE_MODE.BOUGHT));
		GlobalInfo.tablepattern_List.Add (new TableItem(8,"Dragon","dragon",15000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tablepattern_List.Add (new TableItem(9,"Blazon","blazon",20000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tablepattern_List.Add (new TableItem(10,"Panda","panda",25000,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.tablepattern_List.Add (new TableItem(11,"Swords","swords",35000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tablepattern_List.Add (new TableItem(12,"Flowersheart","flowersheart",50000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tablepattern_List.Add (new TableItem(13,"Horse","horse",65000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tablepattern_List.Add (new TableItem(14,"Indian","indian",80000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.tablepattern_List.Add (new TableItem(15,"Dragons","dragons",130000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
	}

	void LoadTournamentBets(){
		GlobalInfo.tBet_List.Add (new Bet(1,200,1000,"Call Pocket on 8 Ball",true, false));
		GlobalInfo.tBet_List.Add (new Bet(2,500,2500,"Call Pocket on 8 Ball",true, false));
		GlobalInfo.tBet_List.Add (new Bet(3,2000,10000,"Call Pocket on 8 Ball",true, false));
		GlobalInfo.tBet_List.Add (new Bet(4,5000,25000,"Call Pocket on All Shots",true, true));
		GlobalInfo.tBet_List.Add (new Bet(5,10000,50000,"Call Pocket on All Shots",true, true));
		GlobalInfo.tBet_List.Add (new Bet(6,50000,250000,"Call Pocket on All Shots",true, true));
		GlobalInfo.tBet_List.Add (new Bet(7,100000,500000,"No Guidelines, Call Pocket on 8 Ball",true, false));
		GlobalInfo.tBet_List.Add (new Bet(8,250000,1000000,"No Guidelines, Call Pocket on All Shots",true, true));
	}

	void LoadPlayGameBets(){
		GlobalInfo.pBet_List.Add (new Bet(1,50,100,"Standard",false, false));
		GlobalInfo.pBet_List.Add (new Bet(2,200,400,"Standard",false, false));
		GlobalInfo.pBet_List.Add (new Bet(3,500,1000,"Call Pocket on 8 Ball",true, false));
		GlobalInfo.pBet_List.Add (new Bet(4,2000,4000,"Call Pocket on 8 Ball",true, false));
		GlobalInfo.pBet_List.Add (new Bet(5,5000,10000,"Call Pocket on All Shots",true, true));
		GlobalInfo.pBet_List.Add (new Bet(6,10000,20000,"Call Pocket on All Shots",true, true));
		GlobalInfo.pBet_List.Add (new Bet(7,50000,100000,"No Guidelines, Call Pocket on 8 Ball",true, false));
		GlobalInfo.pBet_List.Add (new Bet(8,150000,300000,"No Guidelines, Call Pocket on All Shots",true, true));
	}

    public static int cueCn=0;
	void LoadCueList(){
        cueCn = 0;
		GlobalInfo.cue_List.Add (new TableItem(0,"23",950,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(1,"21",8,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(2,"NewYork",0,PRICE_TYPE.COIN,PURCHASE_MODE.TOURNAMENT));
		GlobalInfo.cue_List.Add (new TableItem(3,"26",1500,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.cue_List.Add (new TableItem(4,"27",12,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(5,"47",2500,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(6,"53",18,PRICE_TYPE.CASH,PURCHASE_MODE.BOUGHT));
		GlobalInfo.cue_List.Add (new TableItem(7,"Melbourne",0,PRICE_TYPE.COIN,PURCHASE_MODE.TOURNAMENT));
		GlobalInfo.cue_List.Add (new TableItem(8,"20",3500,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(9,"06",30,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(10,"52",4500,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(11,"14",45,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(12,"37",6000,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.cue_List.Add (new TableItem(13,"15",55,PRICE_TYPE.CASH,PURCHASE_MODE.USING));
		GlobalInfo.cue_List.Add (new TableItem(14,"01",8000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(15,"25",60,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(16,"03",9000,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.cue_List.Add (new TableItem(17,"43",75,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(18,"40",90,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(19,"Amsterdam",0,PRICE_TYPE.COIN,PURCHASE_MODE.TOURNAMENT));
		GlobalInfo.cue_List.Add (new TableItem(20,"50",19000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(21,"45",95,PRICE_TYPE.CASH,PURCHASE_MODE.BOUGHT));
		GlobalInfo.cue_List.Add (new TableItem(22,"49",21000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(23,"31",110,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(24,"48",25000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(25,"59",125,PRICE_TYPE.CASH,PURCHASE_MODE.BOUGHT));
        GlobalInfo.cue_List.Add(new TableItem(26, "Moscow", 0, PRICE_TYPE.COIN, PURCHASE_MODE.TOURNAMENT));
		GlobalInfo.cue_List.Add (new TableItem(27,"42",28000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(28,"56",135,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(29,"30",30000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(30,"57",150,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(31,"13",45000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(32,"55",165,PRICE_TYPE.CASH,PURCHASE_MODE.BOUGHT));
		GlobalInfo.cue_List.Add (new TableItem(33,"44",50000,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.cue_List.Add (new TableItem(34,"58",185,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(35,"Dubai",0,PRICE_TYPE.COIN,PURCHASE_MODE.TOURNAMENT));
		GlobalInfo.cue_List.Add (new TableItem(36,"19",550,PRICE_TYPE.CASH,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(37,"12",85000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(38,"64",90000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(39,"63",120000,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.cue_List.Add (new TableItem(40,"60",135000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(41,"RiodeJaneiro",0,PRICE_TYPE.COIN,PURCHASE_MODE.TOURNAMENT));
		GlobalInfo.cue_List.Add (new TableItem(42,"41",300000,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.cue_List.Add (new TableItem(43,"16",350000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(44,"18",420000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(45,"Shanghai",0,PRICE_TYPE.COIN,PURCHASE_MODE.TOURNAMENT));
		GlobalInfo.cue_List.Add (new TableItem(46,"17",1000000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.cue_List.Add (new TableItem(47,"Delhi",0,PRICE_TYPE.COIN,PURCHASE_MODE.TOURNAMENT));
		GlobalInfo.cue_List.Add (new TableItem(48,"62",3000000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
	}
	void LoadAvatarList(){

		GlobalInfo.avatar_List.Add (new TableItem(1,"1",0,PRICE_TYPE.COIN,PURCHASE_MODE.FREE));
		GlobalInfo.avatar_List.Add (new TableItem(2,"2",0,PRICE_TYPE.COIN,PURCHASE_MODE.FREE));
		GlobalInfo.avatar_List.Add (new TableItem(11,"11",15000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.avatar_List.Add (new TableItem(24,"24",18000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.avatar_List.Add (new TableItem(5,"5",0,PRICE_TYPE.COIN,PURCHASE_MODE.FREE));
		GlobalInfo.avatar_List.Add (new TableItem(29,"29",20500,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.avatar_List.Add (new TableItem(40,"40",22000,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.avatar_List.Add (new TableItem(16,"16",28500,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.avatar_List.Add (new TableItem(10,"10",0,PRICE_TYPE.COIN,PURCHASE_MODE.FREE));
		GlobalInfo.avatar_List.Add (new TableItem(18,"18",0,PRICE_TYPE.COIN,PURCHASE_MODE.FREE));
		GlobalInfo.avatar_List.Add (new TableItem(32,"32",30000,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.avatar_List.Add (new TableItem(22,"22",0,PRICE_TYPE.COIN,PURCHASE_MODE.FREE));
		GlobalInfo.avatar_List.Add (new TableItem(19,"19",33500,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.avatar_List.Add (new TableItem(6,"6",35000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.avatar_List.Add (new TableItem(37,"37",51000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.avatar_List.Add (new TableItem(3,"3",53500,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.avatar_List.Add (new TableItem(25,"25",0,PRICE_TYPE.COIN,PURCHASE_MODE.FREE));
		GlobalInfo.avatar_List.Add (new TableItem(33,"33",0,PRICE_TYPE.COIN,PURCHASE_MODE.FREE));
		GlobalInfo.avatar_List.Add (new TableItem(8,"8",55000,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.avatar_List.Add (new TableItem(13,"13",60500,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.avatar_List.Add (new TableItem(17,"17",62500,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.avatar_List.Add (new TableItem(41,"41",0,PRICE_TYPE.COIN,PURCHASE_MODE.FREE));
		GlobalInfo.avatar_List.Add (new TableItem(7,"7",65000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.avatar_List.Add (new TableItem(9,"9",67500,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.avatar_List.Add (new TableItem(36,"36",0,PRICE_TYPE.COIN,PURCHASE_MODE.FREE));
		GlobalInfo.avatar_List.Add (new TableItem(26,"26",69000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.avatar_List.Add (new TableItem(30,"30",88000,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.avatar_List.Add (new TableItem(39,"39",0,PRICE_TYPE.COIN,PURCHASE_MODE.FREE));
		GlobalInfo.avatar_List.Add (new TableItem(28,"28",91000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.avatar_List.Add (new TableItem(12,"12",93500,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.avatar_List.Add (new TableItem(38,"38",0,PRICE_TYPE.COIN,PURCHASE_MODE.FREE));
		GlobalInfo.avatar_List.Add (new TableItem(14,"14",95000,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.avatar_List.Add (new TableItem(21,"21",98000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.avatar_List.Add (new TableItem(35,"35",0,PRICE_TYPE.COIN,PURCHASE_MODE.FREE));
		GlobalInfo.avatar_List.Add (new TableItem(20,"20",0,PRICE_TYPE.COIN,PURCHASE_MODE.FREE));
		GlobalInfo.avatar_List.Add (new TableItem(34,"34",100000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.avatar_List.Add (new TableItem(31,"31",175000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
		GlobalInfo.avatar_List.Add (new TableItem(4,"4",178000,PRICE_TYPE.COIN,PURCHASE_MODE.BOUGHT));
		GlobalInfo.avatar_List.Add (new TableItem(27,"27",0,PRICE_TYPE.COIN,PURCHASE_MODE.FREE));
		GlobalInfo.avatar_List.Add (new TableItem(15,"15",180000,PRICE_TYPE.COIN,PURCHASE_MODE.SHOULDBUY));
	}
	
	void LoadCountryList(){

		for (int i=0;i<country_list.Length;i++)
			GlobalInfo.country_List.Add(country_list[i]);
	}

	// Update is called once per frame
	void Update () {
        if (GameManager.IsScene("Splash"))
        {
			IncreaseProgress(0.02f);
			if(progress>=1f)
			{
                GameManager.instance.LoadScene("Main");
   			}
		}
	}

	void IncreaseProgress(float increase_amount)
	{
		//Increase the Progress
		progress+=increase_amount;
        if (!GameManager.isMobileScene)
        {
            if (progress > GameManager.downloadPercent)
                progress = GameManager.downloadPercent;
        }

		float progress_amount=progress>1f?1f:progress;

		//Update Progress Text
		GUIText loadingtext=GameObject.Find ("LoadingText").GetComponent<GUIText>();
		GUIText loadingtext_shadow=GameObject.Find ("LoadingTextShadow").GetComponent<GUIText>();
		loadingtext.text=loadingtext_shadow.text="loading "+(int)(progress_amount*100)+"%";

		//Update Progress Bar
		GameObject progressbar=GameObject.Find("Progress");
		progressbar.transform.localScale=new Vector3(1.72f*progress_amount,progressbar.transform.localScale.y,progressbar.transform.localScale.z);
		progressbar.transform.localPosition=new Vector3(8.6f*(1-progress_amount),progressbar.transform.localPosition.y,progressbar.transform.localPosition.z);
	}	
}
