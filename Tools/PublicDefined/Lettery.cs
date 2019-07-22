using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicDefined
{
    public enum LetteryType
    {
        FootBall=1,
        BasketBall=2,
    }
    public enum FollowType
    {
        Follow=1,//关注
        UnFollow=2//取消关注
    }
    public enum ZqGameType
    {
        NotLatBallWithSigler=5,//不让球单关
        LetBallWithSigler=6,//让球单关
        NotLatball=0,//不让球
        Letball=1,//让球
        DoubleResult=2,//半全场
        Score=3,//比分
        NumberofGoalsScored=4//进球数
    }
    public enum LqGageType
    {
        VictoryOrFail=0,//胜负
        Letball=1,//让分
        SizeSoure=2,//大小分
        VictoryOrFailDiff_Main=3,//胜负差(客胜)
        VictoryOrFailDiff_Visable= 4////胜负差(主胜)
    }
    public enum GameType
    {
        Singler=1,//单关
        Two=2,//2串1
        Three=3,//3串1
        Fore=4,//4串1
        Five=5,//5串1
        Six=6,//6串1
        Seven=7,//7串1
        Eight=8,//8串1
    }
}
