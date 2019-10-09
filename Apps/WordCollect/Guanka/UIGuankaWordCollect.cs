﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuankaWordCollect : UICellItemBase
{

    public Text textTitle;
    public RawImage imageBg;
    public Image imageSel;
    public Image imageIconLock;

    public UIGuankaItemPoem uiGuankaItemPoemPrefab;

    UIGuankaItemPoem uiGuankaItem;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        {
            GameObject obj = PrefabCache.main.Load("AppCommon/Prefab/Guanka/UIGuankaItemPoem");
            uiGuankaItemPoemPrefab = obj.GetComponent<UIGuankaItemPoem>();
        }
    }

    public override int GetCellHeight()
    {
        int h = 0;
        if (Common.appKeyName == GameRes.GAME_POEM)
        {
            h = 512;
        }
        else
        {
            h = 192;
        }
        return h;
    }
    public override void UpdateItem(List<object> list)
    {
        if (Common.appKeyName == GameRes.GAME_POEM)
        {
            UpdateItemPoem(list);
        }
        else
        {
            UpdateItemDefault(list);
        }
    }
    public void UpdateItemPoem(List<object> list)
    {
        textTitle.gameObject.SetActive(false);
        imageBg.gameObject.SetActive(false);
        imageSel.gameObject.SetActive(false);
        imageIconLock.gameObject.SetActive(false);

        uiGuankaItem = GameObject.Instantiate(uiGuankaItemPoemPrefab);
        uiGuankaItem.transform.SetParent(objContent.transform);
        uiGuankaItem.index = index;
        uiGuankaItem.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        UIViewController.ClonePrefabRectTransform(uiGuankaItemPoemPrefab.gameObject, uiGuankaItem.gameObject);
        WordItemInfo info = list[index] as WordItemInfo;
        uiGuankaItem.UpdateItem(info);

    }
    public void UpdateItemDefault(List<object> list)
    {
        textTitle.text = (index + 1).ToString();
        textTitle.fontSize = (int)(height * 0.5f);
        imageSel.gameObject.SetActive(false);
        textTitle.gameObject.SetActive(true);
        int idx_play = LevelManager.main.gameLevelFinish + 1;
        if (index > idx_play)
        {
            // if (!Application.isEditor)
            {
                textTitle.gameObject.SetActive(false);
                TextureUtil.UpdateRawImageTexture(imageBg, AppRes.IMAGE_GUANKA_CELL_ITEM_BG_LOCK, true);
            }

        }
        else if (index == idx_play)
        {
            textTitle.gameObject.SetActive(false);
            TextureUtil.UpdateRawImageTexture(imageBg, AppRes.IMAGE_GUANKA_CELL_ITEM_BG_PLAY, true);
        }
        else
        {

            TextureUtil.UpdateRawImageTexture(imageBg, AppRes.IMAGE_GUANKA_CELL_ITEM_BG_UNLOCK, true);
        }
        LayOut();
    }
    public override bool IsLock()
    {
        if (index > (LevelManager.main.gameLevelFinish + 1))
        {
            return true;
        }
        return false;//imageBgLock.gameObject.activeSelf;
    }

    public override void LayOut()
    {
        RectTransform rctran = imageBg.GetComponent<RectTransform>();
        float ratio = 0.9f;
        if (Common.appType == AppType.SHAPECOLOR)
        {
            ratio = 0.9f;
        }
        float scale = Common.GetBestFitScale(rctran.rect.width, rctran.rect.height, width, height) * ratio;
        imageBg.transform.localScale = new Vector3(scale, scale, 1.0f);
        if (uiGuankaItem != null)
        {
            uiGuankaItem.LayOut();
        }
    }
}
