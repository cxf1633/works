using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Data.Sql;
using System.Data.SqlClient;

public class Editor_DropItem_tool : ScriptableWizard
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

    static BetterList<string> itemImageList = null;

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
        //创建窗口
        //Rect wr = new Rect(0, 0, 700, 700);
        //Editor_DropItem_tool window = (Editor_DropItem_tool)EditorWindow.GetWindowWithRect(typeof(Editor_DropItem_tool), wr, true, "BoxDropItem");
        Editor_DropItem_tool window = (Editor_DropItem_tool)EditorWindow.GetWindow(typeof(Editor_DropItem_tool), false, "DropItem");
        window.Show();
    }

    void OnSelectAtlas(Object obj)
    {
        if (Editor_Settings.atlas != obj)
        {
            Editor_Settings.atlas = obj as UIAtlas;
            Repaint();
        }
    }

    //绘制窗口时调用
    void OnGUI()
    {
        //NGUIEditorTools.SetLabelWidth(100f);
        GUILayout.Space(3f);
        NGUIEditorTools.DrawHeader("Input", true);
        NGUIEditorTools.BeginContents(false);
        GUILayout.BeginHorizontal();
        {
            ComponentSelector.Draw<UIAtlas>("Atlas", Editor_Settings.atlas, OnSelectAtlas, true);//, GUILayout.MinWidth(80f));

            EditorGUI.BeginDisabledGroup(Editor_Settings.atlas == null);
            if (GUILayout.Button("New", GUILayout.Width(40f)))
                Editor_Settings.atlas = null;
            EditorGUI.EndDisabledGroup();
        }
        GUILayout.EndHorizontal();
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
                //itemImageList = new BetterList<string>();
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
                        //itemImageList.Add("Item_" + item.ImgId);
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
    void ShowBoxItem()
    {
        Debug.Log("ShowBoxItem");
        NGUIEditorTools.SetLabelWidth(80f);

        if (Editor_Settings.atlas == null)
        {
            GUILayout.Label("No Atlas selected.", "LODLevelNotifyText");
        }
        else
        {
            UIAtlas atlas = Editor_Settings.atlas;
            //bool close = false;
            GUILayout.Label("Box Id =" + Editor_Settings.inputBoxId + " Item:", "LODLevelNotifyText");
            //GUILayout.Label("Box Id =" + " Item:", "LODLevelNotifyText");
            NGUIEditorTools.DrawSeparator();

            GUILayout.BeginHorizontal();
            GUILayout.Space(84f);

            //string before = Editor_Settings.partialSprite;
            //string after = EditorGUILayout.TextField("", before, "SearchTextField");
            //if (before != after) Editor_Settings.partialSprite = after;

            //if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
            //{
            //    Editor_Settings.partialSprite = "";
            //    GUIUtility.keyboardControl = 0;
            //}
            GUILayout.Space(84f);
            GUILayout.EndHorizontal();

            Texture2D tex = atlas.texture as Texture2D;

            if (tex == null)
            {
                GUILayout.Label("The atlas doesn't have a texture to work with");
                return;
            }

            //BetterList<string> sprites = atlas.GetListOfSprites(Editor_Settings.partialSprite);

            //BetterList<string> sprites = Editor_DropItem_tool.getImageList();
            BetterList<ItemVo> sprites = Editor_DropItem_tool.getItemList();

            if (sprites == null)
            {
                return;
            }
            float size = 80f;
            float padded = size + 10f;
            int columns = Mathf.FloorToInt(Screen.width / padded);
            if (columns < 1) columns = 1;

            int offset = 0;
            Rect rect = new Rect(10f, 0, size, size);

            GUILayout.Space(10f);
            mPos = GUILayout.BeginScrollView(mPos);
            int rows = 1;
            Debug.Log("Event.current.type001==>" + Event.current.type);
            while (offset < sprites.size)
            {
                GUILayout.BeginHorizontal();
                {
                    int col = 0;
                    rect.x = 10f;

                    for (; offset < sprites.size; ++offset)
                    {
                        ItemVo spriteItem = sprites[offset];
                        string spriteName = "Item_" + spriteItem.ImgId;
                        UISpriteData sprite = atlas.GetSprite(spriteName);
                        if (sprite == null) continue;

                        // Button comes first
                        if (GUI.Button(rect, ""))
                        {
                            Debug.Log("Event.current.type1==>" + Event.current.type);
                            if (Event.current.button == 0)
                            {
                                float delta = Time.realtimeSinceStartup - mClickTime;
                                mClickTime = Time.realtimeSinceStartup;

                                if (Editor_Settings.selectedSpriteId != spriteItem.Id)
                                {
                                    //if (mSprite != null)
                                    //{
                                    //    NGUIEditorTools.RegisterUndo("Atlas Selection", mSprite);
                                    //    mSprite.MakePixelPerfect();
                                    //    EditorUtility.SetDirty(mSprite.gameObject);
                                    //}
                                    //Debug.Log("old==>>" + Editor_Settings.selectedSpriteId);
                                    //Debug.Log("new==>>" + spriteItem.Id);
                                    Editor_Settings.selectedSpriteId = spriteItem.Id;
                                    Editor_Settings.selectedSprite = sprite.name;
                                    NGUIEditorTools.RepaintSprites();
                                    //选中
                                    if (mCallback != null) mCallback(sprite.name);
                                }
                                //双击关闭
                                //else if (delta < 0.5f) close = true;
                            }
                            else
                            {
                                //NGUIContextMenu.AddItem("Edit", false, EditSprite, sprite);
                                NGUIContextMenu.AddItem("Add", false, AddSprite, sprite);
                                NGUIContextMenu.AddItem("Delete", false, DeleteSprite, sprite);
                                NGUIContextMenu.Show();
                            }
                        }
                        
                        //if (Event.current.type == EventType.Repaint)
                        {
                            Debug.Log("Event.current.type222==>" + Event.current.type);
                            // On top of the button we have a checkboard grid
                            NGUIEditorTools.DrawTiledTexture(rect, NGUIEditorTools.backdropTexture);
                            Rect uv = default(Rect);
                            if (sprite.rotated)
                            {
                                uv = new Rect(sprite.x, sprite.y, sprite.height, sprite.width);
                            }
                            else
                            {
                                uv = new Rect(sprite.x, sprite.y, sprite.width, sprite.height);
                            }

                            uv = NGUIMath.ConvertToTexCoords(uv, tex.width, tex.height);

                            // Calculate the texture's scale that's needed to display the sprite in the clipped area
                            float scaleX = rect.width / uv.width;
                            float scaleY = rect.height / uv.height;

                            // Stretch the sprite so that it will appear proper
                            float aspect = (scaleY / scaleX) / ((float)tex.height / tex.width);
                            Rect clipRect = rect;

                            if (aspect != 1f)
                            {
                                if (aspect < 1f)
                                {
                                    // The sprite is taller than it is wider
                                    float padding = size * (1f - aspect) * 0.5f;
                                    clipRect.xMin += padding;
                                    clipRect.xMax -= padding;
                                }
                                else
                                {
                                    // The sprite is wider than it is taller
                                    float padding = size * (1f - 1f / aspect) * 0.5f;
                                    clipRect.yMin += padding;
                                    clipRect.yMax -= padding;
                                }
                            }

                            GUI.DrawTextureWithTexCoords(clipRect, tex, uv);

                            // Draw the selection
                            //if (Editor_Settings.selectedSprite == sprite.name)
                            if (Editor_Settings.selectedSpriteId == spriteItem.Id)
                            {
                                NGUIEditorTools.DrawOutline(rect, new Color(0.4f, 1f, 0f, 1f));
                            }
                        }

                        GUI.backgroundColor = new Color(1f, 1f, 1f, 0.5f);
                        GUI.contentColor = new Color(1f, 1f, 1f, 0.7f);
                        //创建文字描述
                        //GUI.Label(new Rect(rect.x, rect.y + rect.height, rect.width, 32f), sprite.name, "ProgressBarBack");
                        //创建文字描述
                        GUI.Label(new Rect(rect.x, rect.y + rect.height, rect.width, 32f), sprites[offset].Name, "ProgressBarBack");
                        GUI.contentColor = Color.white;
                        GUI.backgroundColor = Color.white;

                        if (++col >= columns)
                        {
                            ++offset;
                            break;
                        }
                        rect.x += padded;
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(padded);
                rect.y += padded + 26;
                ++rows;
            }
            GUILayout.Space(rows * 26);
            GUILayout.EndScrollView();

            //if (close) Close();
        }
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

        Editor_DropItem_tool comp = ScriptableWizard.DisplayWizard<Editor_DropItem_tool>("Box Drop Item");
        comp.mSprite = null;
        comp.mCallback = callback;
    }

    static public BetterList<ItemVo> getItemList()
    {
        return itemList;
    }

    static public BetterList<string> getImageList()
    {
        return itemImageList;
    }

    static public void DrawSeparator()
    {
        GUILayout.Space(12f);

        if (Event.current.type == EventType.Repaint)
        {
            Texture2D tex = EditorGUIUtility.whiteTexture;
            Rect rect = GUILayoutUtility.GetLastRect();
            GUI.color = new Color(0f, 0f, 0f, 0.25f);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 4f), tex);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 1f), tex);
            GUI.DrawTexture(new Rect(0f, rect.yMin + 9f, Screen.width, 1f), tex);
            GUI.color = Color.white;
        }
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
