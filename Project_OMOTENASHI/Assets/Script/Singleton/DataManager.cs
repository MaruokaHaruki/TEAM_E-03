using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : SingletonMonoBehaviour<DataManager>
{
    private int NowGameNumber;
    
    [SerializeField, Header("�Q�[���f�[�^")] private List<GameData> GameDatas;

    private void Start()
    {
        NowGameNumber = 0;
    }

    public void NextGameScene()
    {
        NowGameNumber += 1;
        if (NowGameNumber < GameDatas.Count)
        {
        }
        else
        {
        }
    }

    /*----------* Get_Set *----------*/
    internal GameData GetNowGameData()
    {
        return GameDatas[NowGameNumber];
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
