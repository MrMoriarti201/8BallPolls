using UnityEngine;
using System.Collections;

public enum GameState {
	NoLogin,
	InLogin,
	FinishLogin,
	Loading,
	InGame,
	Error,
}

public class GamePlayer {
	
	private static GamePlayer _gameplayer;
	public static GamePlayer Instance{
		get{
			if( _gameplayer == null ) {
				_gameplayer = new GamePlayer();
			}
			return _gameplayer;
		}
	}
	
	public GameState gameState;
	public Users users;
	public Data_Farm farm;
	public Data_Mine Game;
	public Data_Barrack barrack;
	public Data_House house;
	public Data_Hospital hospital;
	public Data_Hotel hotel;
	public Data_Arrow_Tower arrowtower;
	public Data_Wall wall;
	public Data_Government government;
	public Data_Drill_Ground ground;
	public Data_Strategy  strategy;

	public Data_Item[] itemList;

	private GamePlayer() {
		gameState = GameState.NoLogin;
		users = new Users();
		farm = new Data_Farm();
		Game = new Data_Mine();
		barrack = new Data_Barrack();
		house = new Data_House();
		hospital= new Data_Hospital();
		hotel = new Data_Hotel();
		arrowtower = new Data_Arrow_Tower();
		wall = new Data_Wall();
		ground = new Data_Drill_Ground();
		government = new Data_Government();
		strategy=new Data_Strategy();

//		users.username = PlayerPrefs.GetString( "username", "" );
//		users.password = PlayerPrefs.GetString( "password", "" );
		users.username = "test";//SystemInfo.deviceUniqueIdentifier;//"333";// "coalaTest017";
		users.password = "test";//SystemInfo.deviceUniqueIdentifier;//"333";//"abc123456";
	}	
}

