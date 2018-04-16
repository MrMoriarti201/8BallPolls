using UnityEngine;
using System.Collections;

public class Users
{
	int _id;//
	string _username = "";//
	string _password = "";//
	string _name = "";//
	int _level;//
	int _ordernum;
	int _popnum;//
	int _coinnum;//
	int _goldnum;//
	int _foodnum;//
	int _intenum;
	int _solnum;//
	double _truceend;
	int _expneed;//
	int _explmax;
	int _tormax;
	int _slavemax;
	int _groudmax;
	int _groupmax;
	int _htiso;
	int _arenaiso;//
	int _gfiso;
	int _expliso;
	int _viplevel;//
	int _injurednum;
	int _genmax;
	int _logindays;
	double _lastlogin;
	int _status;//
	int _officerid;//
	int _exp;//
	int _ordermax;
	int _heroleft;
	int _groupleft;
	int _arenaleft;//
	int _digleft;
	int _explleft;
	int _stealleft;//
	int _torleft;
	int _newergift;
	int _vip_hotel_left;
	string _head = "";//
	int _camp;
	int _group_num;
	int _buy_coin_times;//
	int _buy_food_times;//
	int _buy_pop_times;//
	int _buy_sol_times;//
	int _buy_order_times;//
	int _buy_injured_times;//
	int _work_num;
	int _red_Heart;
	int _gene_num;
	int _total_pay;
	int _other_tax_left;
	int _is_get_daily;

	
	
	public int buy_injured_times {
		get{ return _buy_injured_times; }
		set{ _buy_injured_times = value; }
	}	
		
	public int buy_order_times {
		get{ return _buy_order_times; }
		set{ _buy_order_times = value; }
	}
		
	public int buy_sol_times {
		get{ return _buy_sol_times; }
		set{ _buy_sol_times = value; }
	}
	
	public int buy_pop_times {
		get{ return _buy_pop_times; }
		set{ _buy_pop_times = value; }
	}
	
	public int buy_food_times {
		get{ return _buy_food_times; }
		set{ _buy_food_times = value; }
	}
	
	public int buy_coin_times {
		get{ return _buy_coin_times; }
		set{ _buy_coin_times = value; }
	}
		
	public int red_Heart {
		get{ return _red_Heart; }
		set{ _red_Heart = value; }
	}
	
	public string head {
		get{ return _head; }
		set{ _head = value; }
	}
	
	public int camp {
		get{ return _camp; }
		set{ _camp = value; }
	}
	
	public int work_num {
		get{ return _work_num; }
		set{ _work_num = value; }
	}
	
	public int  total_pay {
		get{ return _total_pay; }
		set{ _total_pay = value; }
	}
	
	public int gene_num {
		get{ return _gene_num; }
		set{ _gene_num = value; }
	}
	
	public int order_max {
		get{ return _ordermax; }
		set{ _ordermax = value; }
	}
	
	public int hero_left {
		get{ return _heroleft; }
		set{ _heroleft = value; }
	}
	
	public int group_left {
		get{ return _groupleft; }
		set{ _groupleft = value; }
	}
	
	public int arena_left {
		get{ return _arenaleft; }
		set{ _arenaleft = value; }
	}
	public int dig_left {
		get{ return _digleft; }
		set{ _digleft = value; }
	}
	
	public int expl_left {
		get{ return _explleft; }
		set{ _explleft = value; }
	}
	
	public int steal_left {
		get{ return _stealleft; }
		set{ _stealleft = value; }
	}
	
	public int tor_left {
		get{ return _torleft; }
		set{ _torleft = value; }
	}
	
	public int newer_gift {
		get{ return _newergift; }
		set{ _newergift = value; }
	}
	
	public int officer_id {
		get{ return _officerid; }
		set{ _officerid = value; }
	}
	
	public int exp {
		get{ return _exp; }
		set{ _exp = value; }
	}
	
	public int injured_num {
		get{ return _injurednum; }
		set{ _injurednum = value; }
	}

	public int gen_max {
		get{ return _genmax; }
		set{ _genmax = value; }
	}

	public int login_days {
		get{ return _logindays; }
		set{ _logindays = value; }
	}

	public double last_login {
		get{ return _lastlogin; }
		set{ _lastlogin = value; }
	}

	public int status {
		get{ return _status; }
		set{ _status = value; }
	}
		
	public int id { 
		get { return _id; }
		set { _id = value; }
	}

	public string username { 
		get { return _username; }
		set { _username = value; }
	}

	public string password { 
		get { return _password; }
		set { _password = value; }
	}

	public string name { 
		get { return _name; }
		set { _name = value; }
	}

	public int level { 
		get { return _level; }
		set { _level = value; }
	}

	public int order_num { 
		get { return _ordernum; }
		set { _ordernum = value; }
	}

	public int pop_num { 
		get { return _popnum; }
		set { _popnum = value; }
	}

	public int coin_num { 
		get { return _coinnum; }
		set { _coinnum = value; }
	}

	public int gold_num { 
		get { return _goldnum; }
		set { _goldnum = value; }
	}

	public int food_num { 
		get { return _foodnum; }
		set { _foodnum = value; }
	}

	public int inte_num { 
		get { return _intenum; }
		set { _intenum = value; }
	}

	public int sol_num { 
		get { return _solnum; }
		set { _solnum = value; }
	}

	public double truce_end { 
		get { return _truceend; }
		set { _truceend = value; }
	}

	public int exp_need { 
		get { return _expneed; }
		set { _expneed = value; }
	}

	public int expl_max { 
		get { return _explmax; }
		set { _explmax = value; }
	}

	public int tor_max { 
		get { return _tormax; }
		set { _tormax = value; }
	}

	public int slave_max { 
		get { return _slavemax; }
		set { _slavemax = value; }
	}

	public int groud_max { 
		get { return _groudmax; }
		set { _groudmax = value; }
	}

	public int group_num { 
		get { return _groupmax; }
		set { _groupmax = value; }
	}

	public int ht_iso { 
		get { return _htiso; }
		set { _htiso = value; }
	}

	public int arena_iso { 
		get { return _arenaiso; }
		set { _arenaiso = value; }
	}

	public int gf_iso { 
		get { return _gfiso; }
		set { _gfiso = value; }
	}

	public int expl_iso { 
		get { return _expliso; }
		set { _expliso = value; }
	}

	public int vip_level { 
		get { return _viplevel; }
		set { _viplevel = value; }
	}
	public int vip_hotel_left { 
		get { return _vip_hotel_left; }
		set { _vip_hotel_left = value; }
	}
	
	public int other_tax_left { 
		get { return _other_tax_left; }
		set { _other_tax_left = value; }
	}

	public int is_get_daily { 
		get { return _is_get_daily; }
		set { _is_get_daily = value; }
	}

}
