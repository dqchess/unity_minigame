using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Vectrosity;
/*
参考游戏： 交叉线!
https://www.taptap.com/app/64361

线段交叉算法：
https://www.cnblogs.com/sparkleDai/p/7604895.html
https://blog.csdn.net/rickliuxiao/article/details/6259322

*/


public class LineInfo
{
    public List<Vector3> listPoint;
    public VectorLine line;

    //dot
    public int idxStart;
    public int idxEnd;

}

public class GameCrossLine : GameBase
{
    public UIGameDot uiGameDotPrefab;
    public GameObject objBg;
    public GameObject objFt;
    public const float RATIO_RECT = 0.9f;

    public List<object> listLine;
    public List<UIGameDot> listDot;
    float lineWidth = 20f;//屏幕像素
    Material matLine;
    int indexLine;



    int offsetDotRowY = 3;


    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        //11x17

        listDot = new List<UIGameDot>();
        listLine = new List<object>();
        DrawBgGrid();
        LayOut();
    }
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        //  LayOut();
    }

    void OnDestroy()
    {
        ClearLine();

    }

    public override void LayOut()
    {
        float x, y, z, w, h;
        Vector2 sizeWorld = Common.GetWorldSize(mainCam);
        Vector2 sizeCanvas = this.frame.size;
        float ratio = 1f;
        if (sizeCanvas.x <= 0)
        {
            return;
        }

    }
    public void UpdateGuankaLevel(int level)
    {
        CrossItemInfo info = (CrossItemInfo)GameLevelParse.main.GetGuankaItemInfo(level);
        InitLines();

        DrawDots();
        DrawLines();
        LayOut();
        // LayOut();
        // Invoke("LayOut", 0.6f);
    }


    public void ClearLine()
    {
        foreach (LineInfo info in listLine)
        {
            DestroyImmediate(info.line.GetObj());
        }
        listLine.Clear();
    }

    public void OnUIGameDotMove(UIGameDot ui)
    {
        DrawLines();
    }

    //背景格子点
    void DrawBgGrid()
    {
        //11x17
        Texture2D texDot = TextureCache.main.Load(GameRes.Image_GameDotBg);
        for (int i = 0; i < GameUtil.main.rowTotal; i++)
        {
            for (int j = 0; j < GameUtil.main.colTotal; j++)
            {
                UIGameDot ui = (UIGameDot)GameObject.Instantiate(uiGameDotPrefab);
                ui.isBg = true;
                ui.transform.SetParent(this.objBg.transform);
                ui.transform.localPosition = GameUtil.main.GetDotPostion(i, j);
                // UIViewController.ClonePrefabRectTransform(UIGameDotPrefab.gameObject, ui.gameObject);
            }
        }
    }
    void DrawDots()
    {
        CrossItemInfo infoGuanka = GameLevelParse.main.GetItemInfo();
        for (int i = 0; i < infoGuanka.listDot.Count; i++)
        {
            Vector2 pttmp = infoGuanka.listDot[i];
            Vector3 pt = GameUtil.main.GetDotPostion((int)pttmp.y + offsetDotRowY, (int)pttmp.x);
            UIGameDot ui = (UIGameDot)GameObject.Instantiate(uiGameDotPrefab);
            ui.rowOrigin = (int)pttmp.y + offsetDotRowY;
            ui.row = ui.rowOrigin;
            ui.colOrigin = (int)pttmp.x;
            ui.col = ui.colOrigin;
            ui.isBg = false;
            ui.transform.SetParent(this.objFt.transform);
            ui.transform.localPosition = pt;
            ui.callBackMove = OnUIGameDotMove;
            ui.index = i;
            listDot.Add(ui);
        }
    }

    void InitLines()
    {

        CrossItemInfo infoGuanka = GameLevelParse.main.GetItemInfo();
        for (int i = 0; i < infoGuanka.listLine.Count; i++)
        {
            Vector2 pttmp = infoGuanka.listLine[i];
            LineInfo info = CreateLine();
            int dotIndexStart = (int)pttmp.x;
            int dotIndexEnd = (int)pttmp.y;
            info.idxStart = dotIndexStart;
            info.idxEnd = dotIndexEnd;
        }

    }


    void DrawLines()
    {
        for (int i = 0; i < listLine.Count; i++)
        {
            LineInfo info = listLine[i] as LineInfo;

            //擦除上次的线
            info.listPoint.Clear();
            info.line.Draw();

            int dotIndexStart = info.idxStart;
            int dotIndexEnd = info.idxEnd;
            UIGameDot uiStart = listDot[dotIndexStart];
            int rowStart = uiStart.row;
            int colStart = uiStart.col;

            UIGameDot uiEnd = listDot[dotIndexEnd];
            int rowEnd = uiEnd.row;
            int colEnd = uiEnd.col;

            Vector3 ptstart = GameUtil.main.GetDotPostion(rowStart, colStart);
            info.listPoint.Add(ptstart);

            Vector3 ptend = GameUtil.main.GetDotPostion(rowEnd, colEnd);
            info.listPoint.Add(ptend);

            info.line.Draw();
        }

    }

    string GetLineName(int idx)
    {
        return "line" + idx.ToString();
    }
    LineInfo CreateLine()
    {
        LineInfo linfo = new LineInfo();
        float w_line = 20;
        if (Device.isLandscape)
        {
            lineWidth = w_line * Screen.width / 2048;
        }
        else
        {
            lineWidth = w_line * 2 * Screen.width / 1536;
        }

        linfo.listPoint = new List<Vector3>();
        VectorLine lineConnect = new VectorLine(GetLineName(indexLine), linfo.listPoint, lineWidth);
        lineConnect.lineType = LineType.Continuous;
        //圆滑填充画线
        lineConnect.joins = Joins.Fill;
        lineConnect.Draw3D();
        GameObject objLine = lineConnect.GetObj();
        //AppSceneBase.main.AddObjToMainWorld(objLine);
        objLine.transform.parent = this.transform;
        objLine.transform.localScale = new Vector3(1f, 1f, 1f);
        objLine.transform.localPosition = new Vector3(0f, 0f, 0f);
        lineConnect.material = matLine;
        lineConnect.color = Color.red;
        indexLine++;
        linfo.line = lineConnect;

        listLine.Add(linfo);
        return linfo;
    }

    LineInfo GetLineByName(string name)
    {
        foreach (LineInfo info in listLine)
        {
            if (info.line.GetObj().name == name)
            {
                return info;
            }
        }
        return null;
    }
}
