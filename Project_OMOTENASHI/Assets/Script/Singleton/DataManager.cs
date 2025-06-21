using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : SingletonMonoBehaviour<DataManager>
{
    private int NowGameNumber;
    
    [SerializeField, Header("ゲームデータ")] private List<GameData> GameDatas;

    private void Start()
    {
        NowGameNumber = 0;
    }

    public void NextGameScene()
    {
        if (NowGameNumber < GameDatas.Count)
        {
        }
        else
        {
        }
    }

    /*----------* Get・Set *----------*/
    internal GameData GetNowGameData()
    {
        NowGameNumber += 1;
        return GameDatas[NowGameNumber - 1];
    }

    internal GameData GetGameData(int number)
    {
        return GameDatas[number];
    }
    internal void SetOneGameData(int number, GameData oneGameData)
    {
        GameDatas[number] = oneGameData;
    }
    internal int GetNowGameNumber()
    {
        return NowGameNumber;
    }
}
