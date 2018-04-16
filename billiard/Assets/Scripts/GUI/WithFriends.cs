using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WithFriends : MonoBehaviour {
    public Text searchTxt;
    public InputField searchInput;
	[SerializeField]
	private GameObject FriendBar;

	private ArrayList friend_list;

    FriendBar[] friendBarList;
	// Use this for initialization
	void Start () {
        GameManager.status = 1;
        StartCoroutine(FriendOnlineStatus());
		LoadFilteredFriends();        
	}

	public void LoadFilteredFriends(){
		friend_list=new ArrayList();
		for(int i=0;i<GlobalInfo.friends.Length;i++)
		{
            friend_list.Add(GlobalInfo.friends[i]);
		}
        UpdateList();
	}

	private void UpdateList(){

		Transform gameobject=GameObject.Find ("FriendPanel").transform.Find("TrophiesPanel").Find("ScrollView").Find("Rect");

		foreach (Transform child in gameobject) {
			GameObject.Destroy(child.gameObject);
		}

		gameobject.GetComponent<RectTransform>().sizeDelta=new Vector2(100,5.5f*friend_list.Count);

		GameObject.Find ("FriendPanel").transform.Find("TrophiesPanel").Find ("Scrollbar").GetComponent<Scrollbar>().value=1;
		if(friend_list.Count<6)
			GameObject.Find ("FriendPanel").transform.Find("TrophiesPanel").Find ("Scrollbar").gameObject.SetActive(false);
		else
			GameObject.Find ("FriendPanel").transform.Find("TrophiesPanel").Find ("Scrollbar").gameObject.SetActive(true);

		int offset_index=friend_list.Count-1;
		if (friend_list.Count<6)
			offset_index=4;
        friendBarList=new FriendBar[friend_list.Count];
        for(int i=0;i<friend_list.Count;i++){
            GameObject friend_bar;
            if (GameManager.isMobileScene)
                friend_bar = GameObject.Instantiate(Resources.Load("Mobile/FriendBar")) as GameObject;
            else
                friend_bar = GameObject.Instantiate(FriendBar) as GameObject;
            
            friend_bar.transform.parent=gameobject;
			friend_bar.transform.localEulerAngles=new Vector3(0,0,0);
            if (GameManager.isMobileScene)
            {
                friend_bar.transform.localScale = new Vector3(0.09617468f, 0.05297692f, 0.95187474f);
                friend_bar.GetComponent<RectTransform>().localPosition = new Vector3(0, 5f * offset_index / 2 - i * 6f, 0);
            }
            else
            {
                friend_bar.transform.localScale = new Vector3(0.09617468f, 0.04297692f, 0.95187474f);
                friend_bar.GetComponent<RectTransform>().localPosition = new Vector3(0, 5.5f * offset_index / 2 - i * 5.5f, 0);
            }

            friendBarList[i] = friend_bar.GetComponent<FriendBar>();
            friendBarList[i].Init((Profile)friend_list[i]);            
		}
        
        gameobject.localPosition = new Vector3(0, -10 * (offset_index - 5) - 8.3f, 0);
		//Rect.
	}

    public void SearchBtn()
    {
        if (GameManager.isMobile)
            return;
        if (searchTxt.text!="" && searchTxt.text!=GlobalInfo.myProfile.Nickname && searchTxt.text!=GlobalInfo.myProfile.user_id.ToString())
            Net.instance.SendMsg(new UserManager.SearchFriend(this,searchTxt.text));
    }

    public void OnSearchReceive(Profile searchRes)
    {
        if (searchRes == null)
        {
            searchInput.text = " Opponent was not found";
            print(searchTxt.text);
            return;
        }
        friend_list = new ArrayList();        
        friend_list.Add(searchRes);
        int len = friend_list.Count;
        for (int i = 0; i < GlobalInfo.friends.Length; i++)
        {
            bool isFriend = false;
            for (int j = 0; j < len; j++)
            {
                Profile tem = (Profile)friend_list[j];
                if (tem.user_id == GlobalInfo.friends[i].user_id)
                    isFriend = true;
            }
            if (!isFriend)
                friend_list.Add(GlobalInfo.friends[i]);
        }
        UpdateList();
    }

    IEnumerator FriendOnlineStatus()
    {
        while (true)
        {
            Net.instance.SendMsg(new UserManager.GetFriendOnlineStatus(this));
            yield return new WaitForSeconds(5f);
        }
    }

    public void OnOnlineStatusReceive(UserManager.GetFriendOnlineStatus script)  //from Get FriendOnline
    {
        for (int i = 0; i < friendBarList.Length; i++)
        {
			if(script.statusList.Length > 0)
				friendBarList[i].UpdateStatus(script.statusList[i].status);
        }
    }
}