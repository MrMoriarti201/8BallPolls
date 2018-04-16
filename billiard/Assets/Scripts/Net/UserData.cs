using UnityEngine;
using System.Collections;

public class User
{
    public int level;
    public int rank;
    public int point;
    public int maxPoint;
    public int winCount;
    public int playCnt;
    public int tWin;
    public int tCnt;
    public int[] cupList;
    public int[] trophy;

}

public class UserData {
    public static User[] user;

    static UserData()
    {
        user = new User[10];
    }
    static public void Init()
    {   
        for (int i = 0; i < 10; i++)
        {
            user[i] = new User();
        }
        user[0].level = 14;
        user[0].rank = 3;
        user[0].point = 800;
        user[0].maxPoint =2900 ;
        user[0].winCount = 95;
        user[0].playCnt = 180;
        user[0].tWin = 2;
        user[0].tCnt = 5;
        user[0].cupList=new int[]{0,1};
        user[0].trophy=new int[]{1, 2, 6, 7};

        user[1].level = 24;
        user[1].rank = 4;
        user[1].point = 3200;
        user[1].maxPoint = 4900;
        user[1].winCount = 144;
        user[1].playCnt = 313;
        user[1].tWin = 1;
        user[1].tCnt = 8;
        user[1].cupList = new int[] { 0 };
        user[1].trophy = new int[] { 1, 2, 3};

        user[2].level = 30;
        user[2].rank = 5;
        user[2].point = 4000;
        user[2].maxPoint = 6100;
        user[2].winCount = 229;
        user[2].playCnt = 394;
        user[2].tWin = 1;
        user[2].tCnt = 3;
        user[2].cupList = new int[] { 2 };
        user[2].trophy = new int[] { 1, 2, 3, 6 };

        user[3].level = 38;
        user[3].rank = 6;
        user[3].point = 2500;
        user[3].maxPoint = 7700;
        user[3].winCount = 195;
        user[3].playCnt = 500;
        user[3].tWin = 0;
        user[3].tCnt = 9;
        user[3].cupList = new int[] { };
        user[3].trophy = new int[] { 1, 2, 3, 11 };

        user[4].level = 51;
        user[4].rank = 7;
        user[4].point = 6800;
        user[4].maxPoint = 10300;
        user[4].winCount = 378;
        user[4].playCnt = 674;
        user[4].tWin = 3;
        user[4].tCnt = 8;
        user[4].cupList = new int[] {2,3,4 };
        user[4].trophy = new int[] { 1, 2, 3,6, 11,16 };

        user[5].level = 62;
        user[5].rank = 8;
        user[5].point = 5200;
        user[5].maxPoint = 12500;
        user[5].winCount = 390;
        user[5].playCnt = 820;
        user[5].tWin = 2;
        user[5].tCnt = 7;
        user[5].cupList = new int[] { 0,3};
        user[5].trophy = new int[] { 1, 2, 3, 6, 7, 11, 16 };

        user[6].level = 70;
        user[6].rank = 9;
        user[6].point = 9800;
        user[6].maxPoint = 14100;
        user[6].winCount = 551;
        user[6].playCnt = 927;
        user[6].tWin = 2;
        user[6].tCnt = 5;
        user[6].cupList = new int[] { 3, 5, 9 };
        user[6].trophy = new int[] { 1, 2, 3, 4, 6, 7, 11 };

        user[7].level = 86;
        user[7].rank = 10;
        user[7].point = 5900;
        user[7].maxPoint = 17300;
        user[7].winCount = 653;
        user[7].playCnt = 1140;
        user[7].tWin = 6;
        user[7].tCnt = 15;
        user[7].cupList = new int[] { 0, 1, 2, 3, 4, 5 };
        user[7].trophy = new int[] { 1, 2, 3, 4, 6, 11, 18 };

        user[8].level = 92;
        user[8].rank = 11;
        user[8].point = 12100;
        user[8].maxPoint = 18500;
        user[8].winCount = 790;
        user[8].playCnt = 1220;
        user[8].tWin = 8;
        user[8].tCnt = 21;
        user[8].cupList = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        user[8].trophy = new int[] { 1, 2, 3, 4, 6, 11, 16, 18 };

        user[9].level = 101;
        user[9].rank = 11;
        user[9].point = 15900;
        user[9].maxPoint = 20300;
        user[9].winCount = 717;
        user[9].playCnt = 1340;
        user[9].tWin = 12;
        user[9].tCnt = 19;
        user[9].cupList = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        user[9].trophy = new int[] { 1, 2, 3, 4, 6, 11, 16, 18 };
    }
}
