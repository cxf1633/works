using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Editor_BoxDropItem : EditorWindow
{
    //输入文字的内容
    static private string text = "400001";

    public delegate void Callback(string sprite);
    //选择贴图的对象
    //private Texture texture;
    //public UITexture itemImage;
    ////static GameObject atlasObj = null;
    //static UIAtlas atlasObj = null;

    ////搜索关键字
    //static string partialSprite = null;

    static BetterList<ItemVo> itemList = null;

    SerializedObject mObject;
    SerializedProperty mProperty;

    UISprite mSprite;
    Vector2 mPos = Vector2.zero;
    Callback mCallback;
    float mClickTime = 0f;

    [MenuItem("CxfTools/BoxDropItem")]
    static void AddWindow()
    {
        Editor_BoxDropItem window = (Editor_BoxDropItem)EditorWindow.GetWindow(typeof(Editor_BoxDropItem), false, "BoxDropItem");
        window.Show();
    }

    void OnGUI()
    {
        //NGUIEditorTools.SetLabelWidth(100f);
        GUILayout.Space(3f);
        NGUIEditorTools.DrawHeader("Input", true);
        NGUIEditorTools.BeginContents(false);
        //GUILayout.BeginHorizontal();
        //{
        //    ComponentSelector.Draw<UIAtlas>("Atlas", Editor_Settings.atlas, OnSelectAtlas, true);//, GUILayout.MinWidth(80f));

        //    EditorGUI.BeginDisabledGroup(Editor_Settings.atlas == null);
        //    if (GUILayout.Button("New", GUILayout.Width(40f)))
        //        Editor_Settings.atlas = null;
        //    EditorGUI.EndDisabledGroup();
        //}
        //GUILayout.EndHorizontal();
        //输入框控件
        text = EditorGUILayout.TextField("输入box_id:", text, GUILayout.Width(500));
        if (GUILayout.Button("查看", GUILayout.Width(100)))
        {
            if (text == null)
            {
                Debug.Log("text is null, please input!");
                return;
            }
            Debug.Log("box_id="+ text);
            //Debug.Log(BoxCfgMgr.instance.ItemTable[400002].Name);
            int boxId = 0;
            if (System.Int32.TryParse(text, out boxId))
            {
                if (!BoxCfgMgr.instance.Has(boxId))
                {
                    Debug.Log("没有这个boxid：" + boxId);
                    return;
                }
                Debug.Log(BoxCfgMgr.instance.ItemTable[boxId].Name);
                Editor_Settings.inputBoxId = boxId;
                itemList = new BetterList<ItemVo>();
                //BoxDropCfgMgr.instance.ItemTable
                bool isShow = false;
                foreach (var boxDrop in BoxDropCfgMgr.instance.ItemTable.Values)
                {
                    if (boxDrop.BoxId == boxId && boxDrop.ItemId > 0)
                    {
                        //Debug.Log("ItemId===>>" + boxDrop.ItemId);
                        ItemVo item = ItemCfgMgr.instance.ItemTable[boxDrop.ItemId];
                        //Debug.Log("ItemName===>>" + item.Name);
                        itemList.Add(item);
                        isShow = true;
                    }
                }
                if (isShow)
                {
                    Editor_BoxItemPanel.ShowSelected();
                    //ShowBoxItem();
                }
            }
            else
            {
                Debug.Log("box id 必须是整数box id：" + text);
            }
        }
        #region 注释的代码
        //选择贴图
        //texture = EditorGUILayout.ObjectField("宝箱物品信息", texture, typeof(Texture), true) as Texture;

        //atlasObj = Asset.Load<GameObject>("Art/Atlas/PartsUIAtlas/PartsUIAtlas") as GameObject;

        //图集
        //atlasObj = Asset.Load<UIAtlas>("Assets/Art/Atlas/PartsUIAtlas/PartsUIAtlas");
        //atlasObj = EditorGUI.ObjectField(new Rect(3, 150, position.width - 30, 20), "Atlas Prefab:", atlasObj, typeof(UIAtlas), false) as UIAtlas;
        //atlasObj = EditorGUI.ObjectField(new Rect(3, 150, position.width - 30, 20), "Find Dependency", atlasObj, typeof(GameObject), false) as GameObject;

        //if (atlasObj)
        //{
        //    Object[] roots = new Object[] { atlasObj };
        //    if (GUI.Button(new Rect(3, 175, position.width - 6, 20), "Check Dependencies"))
        //    {
        //        //Selection.objects = EditorUtility.CollectDependencies(roots);

        //        // DrowBoxItem(atlasObj);
        //        //Editor_BoxItemPanel.ShowSelected(itemImageList);

        //    }
        //    //NGUIEditorTools.DrawSeparator();
        //    //string before = "Item";
        //    //string after = EditorGUILayout.TextField("", before, "SearchTextField");
        //    //if (before != after) partialSprite = after;
        //}
        //else
        //    EditorGUI.LabelField(new Rect(3, 175, position.width - 6, 20), "Missing:", "Select an object first");
        #endregion
    }

    /// <summary>
    /// Add the sprite (context menu selection)
    /// </summary>

    void AddSprite(object obj)
    {
        Debug.Log("添加物品=============>>>");
    }

    /// <summary>
    /// Delete the sprite (context menu selection)
    /// </summary>

    void DeleteSprite(object obj)
    {
        Debug.Log("删除物品=============>>>");
    }

    static public BetterList<ItemVo> getItemList()
    {
        return itemList;
    }

    /// <summary>
    /// Show the sprite selection wizard.
    /// </summary>

    static public void ShowSelected()
    {
        if (Editor_Settings.atlas != null)
        {
            Show(delegate (string sel) { NGUIEditorTools.SelectSprite(sel); });
        }
    }

    /// <summary>
    /// Show the selection wizard.
    /// </summary>

    static public void Show(Callback callback)
    {
        //if (instance != null)
        //{
        //    instance.Close();
        //    instance = null;
        //}

        //Editor_DropItem_tool comp = ScriptableWizard.DisplayWizard<Editor_DropItem_tool>("Box Drop Item");
        //comp.mSprite = null;
        //comp.mCallback = callback;
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
    void OnDestroy()
    {
        //Debug.Log("当窗口关闭时调用");
    }
}
