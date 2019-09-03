
using System.Collections;
using System.Collections.Generic;
using Moonma.Share;
using UnityEngine;
using UnityEngine.UI;
public class UIWordList : UIView
{
    public ScrollRect scrollRect;
    public GameObject objScrollContent;

    public UICellWord uiCellWordPrefab;

    public List<UICellWord> listItem;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        listItem = new List<UICellWord>();
        LoadPrefab();
        LayOut();
    }

    void LoadPrefab()
    {

    }
    public override void LayOut()
    {

    }

    public void UpdateItem()
    {
        float x, y, w, h;
        WordItemInfo info = GameGuankaParse.main.GetItemInfo();
        int len = info.listAnswer.Length;
        float w_item = 128f;
        float h_item = 128f;
        for (int i = 0; i < len; i++)
        {
            UICellWord item = GameObject.Instantiate(uiCellWordPrefab);
            RectTransform rctran = this.GetComponent<RectTransform>();
            string word = info.listAnswer[i];
            w = w_item * word.Length;
            h = h_item;
            rctran.sizeDelta = new Vector2(w, h);
            item.index = i;
            item.transform.SetParent(objScrollContent.transform);
            item.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            item.UpdateItem();
            listItem.Add(item);
        }
    }


    public void OnClickGold()
    {

    }


}