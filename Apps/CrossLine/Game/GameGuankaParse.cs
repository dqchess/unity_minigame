﻿using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class PoemContentInfo
{
    public string content;
    public string pinyin;
    public bool skip;
}
public class WordItemInfo : ItemInfo
{
    public List<object> listFormulation;//公式
    public string[] listLetter;
    public string[] listAnswer;//答案 LO|SOL 
    public string[] listError;

    public string author;
    public string year;
    public string style;
    public string album;
    public string intro;
    public string translation;
    public string appreciation;
    public string pinyin;
    public string head;
    public string end;
    public string tips;

    public List<PoemContentInfo> listPoemContent;
}
public class GameGuankaParse : GuankaParseBase
{
    public string strWord3500;
    Language languageGame;
    static private GameGuankaParse _main = null;
    public static GameGuankaParse main
    {
        get
        {
            if (_main == null)
            {
                _main = new GameGuankaParse();
                _main.UpdateLanguage();
            }
            return _main;
        }
    }


    public override ItemInfo GetGuankaItemInfo(int idx)
    {
        if (listGuanka == null)
        {
            return null;
        }
        if (idx >= listGuanka.Count)
        {
            return null;
        }
        ItemInfo info = listGuanka[idx] as ItemInfo;
        return info;
    }

    public WordItemInfo GetItemInfo()
    {
        int idx = LevelManager.main.gameLevel;
        return GetGuankaItemInfo(idx) as WordItemInfo;
    }


    public void UpdateLanguage()
    {
        ItemInfo info = LevelManager.main.GetPlaceItemInfo(LevelManager.main.placeLevel);
        string strlan = Common.GAME_RES_DIR + "/language/" + info.language + ".csv";
        languageGame = new Language();
        languageGame.Init(strlan);
        languageGame.SetLanguage(SystemLanguage.Chinese);
    }

    public string GetGuankaAnswer(WordItemInfo info)
    {
        //真正的答案
        string str = languageGame.GetString(info.id);
        //歇后语languageWord
        if ((!Common.BlankString(info.head)) && (!Common.BlankString(info.end)))
        {
            return info.end;
        }
        return str;
    }
    public override int GetGuankaTotal()
    {
        ParseGuanka();
        if (listGuanka != null)
        {
            return listGuanka.Count;
        }
        return 0;
    }

    public override void CleanGuankaList()
    {
        if (listGuanka != null)
        {
            listGuanka.Clear();
        }

    }

    int GetRandomOtherLevelIndex(int level)
    {
        int total = listGuanka.Count;
        int size = total - 1;
        int[] idxTmp = new int[size];
        int idx = 0;
        for (int i = 0; i < total; i++)
        {
            if (i != level)
            {
                idxTmp[idx++] = i;
            }
        }

        int rdm = Random.Range(0, size);
        if (rdm >= size)
        {
            rdm = size - 1;
        }
        idx = idxTmp[rdm];
        return idx;
    }
    public override int ParseGuanka()
    {
        if (Common.appKeyName == GameRes.GAME_WORDCONNECT)
        {
            return ParseGuankaWordConnect();
        }
        if (Common.appKeyName == GameRes.GAME_IDIOM)
        {
            return ParseGuankaIdiom();
        }
        return 0;
    }

    public int ParseGuankaIdiom()
    {
        int count = 0;

        if ((listGuanka != null) && (listGuanka.Count != 0))
        {
            return listGuanka.Count;
        }
        UpdateLanguage();
        listGuanka = new List<object>();
        int idx = LevelManager.main.placeLevel;

        ItemInfo infoPlace = LevelManager.main.GetPlaceItemInfo(LevelManager.main.placeLevel);
        string filepath = Common.GAME_RES_DIR + "/guanka/guanka_list_place" + idx + ".json";
        if (!FileUtil.FileIsExistAsset(filepath))
        {
            filepath = Common.GAME_RES_DIR + "/guanka/item_" + infoPlace.id + ".json";
        }

        //
        //FILE_PATH
        string json = FileUtil.ReadStringAsset(filepath);

        JsonData root = JsonMapper.ToObject(json);
        string strPlace = infoPlace.id;
        JsonData items = root["items"];

        for (int i = 0; i < items.Count; i++)
        {
            JsonData item = items[i];
            WordItemInfo info = new WordItemInfo();
            info.id = JsonUtil.JsonGetString(item, "id", "");
            //string str = "aa";// = languageGame.GetString(info.id);
            //Debug.Log(i + ":ParseGame:" + str);
            info.pic = Common.GAME_RES_DIR + "/image/" + strPlace + "/" + info.id + ".png";
            info.icon = Common.GAME_RES_DIR + "/image_thumb/" + strPlace + "/" + info.id + ".png";
            if (!FileUtil.FileIsExistAsset(info.icon))
            {
                info.icon = info.pic;
            }
            string key = "xiehouyu";
            if (JsonUtil.ContainsKey(item, key))
            {
                JsonData xiehouyu = item[key];
                for (int j = 0; j < xiehouyu.Count; j++)
                {
                    JsonData item_xhy = xiehouyu[j];
                    if (j == 0)
                    {
                        info.head = (string)item_xhy["content"];
                    }
                    if (j == 1)
                    {
                        info.end = (string)item_xhy["content"];
                    }
                }

            }

            key = "head";
            if (JsonUtil.ContainsKey(item, key))
            {
                //Riddle
                info.head = (string)item["head"];
                info.end = (string)item["end"];
                info.tips = (string)item["tips"];
                info.type = (string)item["type"];
            }



            listGuanka.Add(info);
        }

        count = listGuanka.Count;

        for (int i = 0; i < listGuanka.Count; i++)
        {
            WordItemInfo info = listGuanka[i] as WordItemInfo;
            string word0 = GetGuankaAnswer(info);
            int idx1 = GetRandomOtherLevelIndex(i);
            string word1 = GetGuankaAnswer(GetGuankaItemInfo(idx1) as WordItemInfo);
           // word1 = word1.Substring(0, word1.Length / 2);
            string word = word0 + word1;
            info.listLetter = new string[word.Length];
            for (int k = 0; k < word.Length; k++)
            {
                info.listLetter[k] = word.Substring(k, 1);
            }

            info.listAnswer = new string[2];
            info.listAnswer[0] = word0;
            info.listAnswer[1] = word1;
        }

        //word3500
        filepath = Common.GAME_DATA_DIR + "/words_3500.json";
        json = FileUtil.ReadStringAsset(filepath);
        root = JsonMapper.ToObject(json);
        strWord3500 = (string)root["words"];
        Debug.Log(strWord3500);

        Debug.Log("ParseGame::count=" + count);
        return count;
    }

    public int ParseGuankaWordConnect()
    {
        int count = 0;

        if ((listGuanka != null) && (listGuanka.Count != 0))
        {
            return listGuanka.Count;
        }

        listGuanka = new List<object>();
        int idx = LevelManager.main.placeLevel;
        string fileName = Common.GAME_RES_DIR + "/guanka/Chapter_" + (idx + 1) + ".txt";
        //FILE_PATH
        string json = FileUtil.ReadStringAsset(fileName);//((TextAsset)Resources.Load(fileName, typeof(TextAsset))).text;
                                                         // Debug.Log("json::"+json);
        JsonData root = JsonMapper.ToObject(json);
        JsonData items = root["levels"];


        /*
             "n": "1",
            "o": 0,
            "l": "L|O|S",
            "r": "LO|SOL",
            "e": ""
         */
        char[] charSplit = { '|' };

        for (int i = 0; i < items.Count; i++)
        {
            JsonData item = items[i];
            WordItemInfo info = new WordItemInfo();
            string str = (string)item["l"];

            info.listLetter = str.Split(charSplit);

            str = (string)item["r"];
            info.listAnswer = str.Split(charSplit);

            str = (string)item["e"];
            info.listError = str.Split(charSplit);

            listGuanka.Add(info);
        }

        count = listGuanka.Count;

        Debug.Log("ParseGame::count=" + count);
        return count;
    }

}