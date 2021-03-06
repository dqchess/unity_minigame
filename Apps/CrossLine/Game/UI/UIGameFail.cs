﻿using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIGameFail : UIViewPop
{
    public const string KEYNAME_VIEWALERT_VIDEO_FAIL = "KEYNAME_VIEWALERT_VIDEO_FAIL";
    public Image imageBg;
    public Text textTitle;
    public Text textDetail;
    public Button btnRevive;//复活
    public Button btnRestart; //从第一关开始

    protected override void Awake()
    {
        base.Awake();
        textTitle.text = Language.main.GetString("STR_GameFail_TITLE");
        textDetail.text = Language.main.GetString("STR_GameFail_Detail");
        textDetail.color = new Color32(192, 90, 59, 255);

        Common.SetButtonText(btnRevive, Language.main.GetString("STR_GameFail_btnRevive"), 128);
        Common.SetButtonText(btnRestart, Language.main.GetString("STR_GameFail_btnRestart"), 128);
    }
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        LayOut();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public override void LayOut()
    {
        float x = 0, y = 0, w = 0, h = 0;
        float ratio = 0.8f;
        if (Device.isLandscape)
        {
            ratio = 0.7f;
        }

        RectTransform rctranRoot = this.GetComponent<RectTransform>();
        Vector2 sizeCanvas = AppSceneBase.main.sizeCanvas;
        {

            w = sizeCanvas.x * ratio;
            h = sizeCanvas.y * ratio;//rctran.rect.size.y * w / rctran.rect.size.x;
            rctranRoot.sizeDelta = new Vector2(w, h);

        }
    }

    public void OnClickBtnRevive()
    {
        if (!AppVersion.appCheckHasFinished)
        {
            Close();
            GameManager.main.GotoPlayAgain();
            return;
        }

        AdKitCommon.main.callbackAdVideoFinish = OnAdKitAdVideoFinish;
        AdKitCommon.main.ShowAdVideo();


    }
    public void OnClickBtnRestart()
    {
        Close();
        LevelManager.main.placeLevel = 0;
        LevelManager.main.gameLevel = 0;
        LevelManager.main.gameLevelFinish = -1;
        GameManager.main.GotoPlayAgain();
    }
    void ShowVideoFail()
    {
        string title = Language.main.GetString("STR_UIVIEWALERT_TITLE_VIDEO_FAIL");
        string msg = Language.main.GetString("STR_UIVIEWALERT_MSG_VIDEO_FAIL");
        string yes = Language.main.GetString(("STR_UIVIEWALERT_YES_VIDEO_FAIL"));
        string no = Language.main.GetString("STR_UIVIEWALERT_NO_VIDEO_FAIL");
        ViewAlertManager.main.ShowFull(title, msg, yes, no, true, KEYNAME_VIEWALERT_VIDEO_FAIL, OnUIViewAlertFinished);
    }
    void OnUIViewAlertFinished(UIViewAlert alert, bool isYes)
    {

        if (KEYNAME_VIEWALERT_VIDEO_FAIL == alert.keyName)
        {
            if (isYes)
            {
                OnClickBtnRevive();
            }
        }
    }

    public void OnAdKitAdVideoFinish(AdKitCommon.AdType type, AdKitCommon.AdStatus status, string str)
    {
        if (type == AdKitCommon.AdType.VIDEO)
        {
            if (status == AdKitCommon.AdStatus.SUCCESFULL)
            {
                Close();
                GameManager.main.GotoPlayAgain();
            }
            if (status == AdKitCommon.AdStatus.FAIL)
            {
                ShowVideoFail();
            }

        }
    }
}
