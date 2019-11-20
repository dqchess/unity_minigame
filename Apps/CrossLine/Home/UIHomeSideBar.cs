﻿using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Moonma.IAP;
using Moonma.Share;
using UnityEngine;
using UnityEngine.UI;

public class UIHomeSideBar : UIView
{
    public const string STR_KEYNAME_VIEWALERT_NOAD = "STR_KEYNAME_VIEWALERT_NOAD";
    public const string STR_KEYNAME_VIEWALERT_UPDATE_VERSION = "STR_KEYNAME_VIEWALERT_UPDATE_VERSION";
    public const string STR_KEYNAME_VIEWALERT_EXIT_APP = "STR_KEYNAME_VIEWALERT_EXIT_APP";

    public Button btnMusic;
    public Button btnSound;
    public Button btnSetting;
    public Button btnMore;
    public Button btnShare;
    public Button btnNoAd;
    public void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
        LayOut();

    }



    public override void LayOut()
    {

    }

    public void UpdateBtnMusic()
    {
        bool ret = Common.GetBool(AppString.STR_KEY_BACKGROUND_MUSIC);
        if (btnMusic != null)
        {
            TextureUtil.UpdateButtonTexture(btnMusic, ret ? AppRes.IMAGE_BtnMusicOn : AppRes.IMAGE_BtnMusicOff, false);
        }
    }
    public void UpdateBtnSound()
    {
        bool ret = Common.GetBool(AppString.KEY_ENABLE_PLAYSOUND);
        if (btnSound != null)
        {
            TextureUtil.UpdateButtonTexture(btnSound, ret ? AppRes.IMAGE_BtnSoundOn : AppRes.IMAGE_BtnSoundOff, false);
        }
    }

    public void OnClickBtnMusic()
    {
        bool ret = Common.GetBool(AppString.STR_KEY_BACKGROUND_MUSIC);
        bool value = !ret;
        Common.SetBool(AppString.STR_KEY_BACKGROUND_MUSIC, value);
        if (value)
        {
            MusicBgPlay.main.PlayMusicBg();
        }
        else
        {
            MusicBgPlay.main.Stop();
        }
        UpdateBtnMusic();
    }

    public void OnClickBtnSound()
    {
        bool ret = Common.GetBool(AppString.KEY_ENABLE_PLAYSOUND);
        bool value = !ret;
        Common.SetBool(AppString.KEY_ENABLE_PLAYSOUND, value);

        UpdateBtnSound();
    }

    public void OnClickBtnMore()
    {
        MoreViewController.main.Show(null, null);
    }
    public void OnClickBtnSetting()
    {

        // TextureUtil.UpdateImageTexture(imageBg, AppRes.IMAGE_SETTING_BG, true);//IMAGE_SETTING_BG 导致PlayBtnSound声音播放不完整 延迟执行
        // Invoke("DoClickBtnSetting", 0.1f);
        DoClickBtnSetting();
    }

    public void DoClickBtnSetting()
    {
        SettingViewController.main.Show(null, null);
    }

    public void OnClickBtnShare()
    {
        ShareViewController.main.callBackClick = OnUIShareDidClick;
        ShareViewController.main.Show(null, null);
    }

    public void OnClickBtnAdVideo()
    {
        AdKitCommon.main.ShowAdVideo();
    }

    public void OnClickBtnNoAd()
    {
        if (Config.main.APP_FOR_KIDS)
        {
            ParentGateViewController.main.Show(null, null);
            ParentGateViewController.main.ui.callbackClose = OnUIParentGateDidCloseNoAd;
        }
        else
        {
            DoBtnNoAdAlert();
        }
    }


    public void DoBtnNoAdAlert()
    {
        string title = Language.main.GetString("STR_UIVIEWALERT_TITLE_NOAD");
        string msg = Language.main.GetString("STR_UIVIEWALERT_MSG_NOAD");
        string yes = Language.main.GetString("STR_UIVIEWALERT_YES_NOAD");
        string no = Language.main.GetString("STR_UIVIEWALERT_NO_NOAD");

        ViewAlertManager.main.ShowFull(title, msg, yes, no, true, STR_KEYNAME_VIEWALERT_NOAD, OnUIViewAlertFinished);
    }
    public void DoBtnNoADIAP()
    {
        IAP.main.SetObjectInfo(this.gameObject.name, "IAPCallBack");
        IAP.main.StartBuy(IAP.productIdNoAD, false);
    }

    public void OnUIShareDidClick(ItemInfo item)
    {
        string title = Language.main.GetString("UIMAIN_SHARE_TITLE");
        string detail = Language.main.GetString("UIMAIN_SHARE_DETAIL");
        string url = Config.main.shareAppUrl;
        Share.main.ShareWeb(item.source, title, detail, url);
    }

    void OnUIViewAlertFinished(UIViewAlert alert, bool isYes)
    {
        if (STR_KEYNAME_VIEWALERT_NOAD == alert.keyName)
        {
            if (isYes)
            {
                DoBtnNoADIAP();

            }
        }

        if (STR_KEYNAME_VIEWALERT_EXIT_APP == alert.keyName)
        {
            Debug.Log("OnUIViewAlertFinished 1");
            if (isYes)
            {
                Application.Quit();
            }

        }
        Debug.Log("OnUIViewAlertFinished 2");
        if (STR_KEYNAME_VIEWALERT_UPDATE_VERSION == alert.keyName)
        {
            Debug.Log("OnUIViewAlertFinished 3");
            if (isYes)
            {

                if (AppVersion.main.appNeedUpdate)
                {
                    string url = AppVersion.main.strUrlAppstore;
                    if (!Common.BlankString(url))
                    {
                        Application.OpenURL(url);
                    }
                }

            }
        }
    }

    public void OnUIParentGateDidCloseNoAd(UIParentGate ui, bool isLongPress)
    {
        if (isLongPress)
        {
            DoBtnNoAdAlert();
        }
    }
}
