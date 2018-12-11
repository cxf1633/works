using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Linq;


public class Editor_ShowBoxItem : EditorWindow {
    
    //宝箱id
    static private string boxIdStr = "";//400001

    //static BetterList<ItemVo> itemList = null;

    ////应该把掉落包做为主要数据
    //static BetterList<BoxDropVo> boxDropList = null;

    //整个宝箱的数据
    List<BoxDropVo> boxDropList = null;
    //整个txt的数据
    Dictionary<int, List<BoxDropVo>> originSource = null;

    static float itemSize = 80f;
    Rect itemRect = new Rect(10f, 0, itemSize, itemSize);
    Vector2 mPos = Vector2.zero;
    static string atlasPath = "Assets/Art/Atlas/PartsUIAtlas/PartsUIAtlas.prefab";
    UIAtlas atlas = null;
    int selectedSpriteId = 0;
    ItemVo selectItem = null;
    BoxDropVo selectBoxDrop = null;
    
    //box_drop表数据
    static private string itme_id = "";//272205
    static private string random_type = "3";
    static private string type = "102";
    static private string item_param = "0";
    static private string parameter = "1";
    static private string min = "1";
    static private string max = "1";
    static private string day_limit = "0";


    [MenuItem("CxfTools/ShowBoxItem")]
    static void AddWindow()
    {
        //创建窗口
        Editor_ShowBoxItem window = (Editor_ShowBoxItem)EditorWindow.GetWindow(typeof(Editor_ShowBoxItem), false, "ShowBoxItem");
        window.Show();
    }
    void OnGUI()
    {
        #region 正式代码
        NGUIEditorTools.DrawHeader("Input", true);
        
        //NGUIEditorTools.BeginContents(false);
        //开始垂直布局
        GUILayout.BeginVertical();
        GUILayout.Space(10f);
        //开始水平布局
        GUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 80f;
        boxIdStr = EditorGUILayout.TextField("输入宝箱id:", boxIdStr, GUILayout.MaxWidth(400));


        GUILayout.Space(10f);
        if (GUILayout.Button("查看", GUILayout.MaxWidth(100)))
        {
            int boxId = 0;
            if (!System.Int32.TryParse(boxIdStr, out boxId))
            {
                EditorUtility.DisplayDialog("提示:", "宝箱id输入错误:id=" + boxIdStr, "OK");
                return;
            }
            Debug.Log("boxId=" + boxId);
            //
            boxDropList = BoxDropCfgMgr.instance.GetBoxData(boxId);
            originSource = BoxDropCfgMgr.instance.BoxDataList;
            if (boxDropList == null)
                EditorUtility.DisplayDialog("提示:", "不存在这个宝箱id:" + boxId, "OK");
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10f);

        NGUIEditorTools.DrawHeader("Modify", true);
        GUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 120f;
        itme_id = EditorGUILayout.TextField("输入 itme_id:", itme_id, GUILayout.MaxWidth(400));
        random_type = EditorGUILayout.TextField("输入 random_type:", random_type, GUILayout.MaxWidth(400));
        type = EditorGUILayout.TextField("输入 type:", type, GUILayout.MaxWidth(400));
        item_param = EditorGUILayout.TextField("输入 item_param:", item_param, GUILayout.MaxWidth(400));

        GUILayout.EndHorizontal();
        GUILayout.Space(10f);
        GUILayout.BeginHorizontal();
        parameter = EditorGUILayout.TextField("输入 parameter:", parameter, GUILayout.MaxWidth(400));
        min = EditorGUILayout.TextField("输入 min:", min, GUILayout.MaxWidth(400));
        max = EditorGUILayout.TextField("输入 max:", max, GUILayout.MaxWidth(400));
        day_limit = EditorGUILayout.TextField("输入 day_limit:", day_limit, GUILayout.MaxWidth(400));
        GUILayout.EndHorizontal();
        GUILayout.Space(10f);
        if (GUILayout.Button("添加", GUILayout.Height(30)))
        {
            if (boxDropList != null)
            {
                int itmeId = 0;
                if (System.Int32.TryParse(itme_id, out itmeId))
                {
                    Debug.Log("添加物品:" + itmeId);
                    BoxDropVo addItem = new BoxDropVo();
                    int id = boxDropList[boxDropList.Count - 1].Id + 1;
                    addItem.Id = id;
                    addItem.BoxId = System.Int32.Parse(boxIdStr);
                    addItem.RandomType = System.Int32.Parse(random_type);
                    addItem.Type = System.Int32.Parse(type);
                    addItem.ItemId = itmeId;
                    addItem.ItemParam = System.Int32.Parse(item_param);
                    addItem.Parameter = System.Int32.Parse(parameter);
                    addItem.Min = System.Int32.Parse(min);
                    addItem.Max = System.Int32.Parse(max);
                    addItem.DayLimit = System.Int32.Parse(day_limit);
                    //加入显示
                    boxDropList.Add(addItem);
                    //加入数据
                    //List<BoxDropVo> list = originSource[System.Int32.Parse(boxIdStr)];
                    //list.Add(addItem);
                }
                else
                {
                    EditorUtility.DisplayDialog("提示:", "输入一个物品id", "OK");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("提示:", "先点击查看", "OK");
                boxDropList = null;
            }
        }
        //GUILayout.Space(10f);
       
        //GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        if (boxDropList != null)
        {
            NGUIEditorTools.DrawHeader("Views", true);
            ShowBoxItem();
            if (GUILayout.Button("删除", GUILayout.Height(30)))
            {
                //Debug.Log("删除按钮");
                if (selectBoxDrop != null)
                {
                    boxDropList.Remove(selectBoxDrop);
                    originSource[selectBoxDrop.BoxId].Remove(selectBoxDrop);
                }
                else
                {
                    EditorUtility.DisplayDialog("提示:", "请选择一个物品", "OK");
                }
            }
            if (GUILayout.Button("导出", GUILayout.Height(50)))
            {
                ExportModify();
            }
        }
        #endregion

        #region 测试代码
        //if (GUILayout.Button("查看", GUILayout.Width(100)))
        //{
        //    Debug.Log("点击查看按钮");
        //    atlas = LoadAsset<UIAtlas>(atlasPath);
        //}

        //ShowBoxItem();

        //mScroll = GUILayout.BeginScrollView(mScroll);

        //testGUILayout1();

        //testGUILayout2();
        #endregion
    }

    /// <summary>
    /// 导出修改
    /// </summary>
    void ExportModify()
    {
        //Debug.Log("导出按钮");

        StringBuilder csv = new StringBuilder();
        //写入表名
        string tb_name = "id,box_id,random_type,type,item_id,item_param,parameter,min,max,day_limit";
        csv.Append(tb_name + System.Environment.NewLine);
        foreach (List<BoxDropVo> list in originSource.Values)
        {
            foreach (var item in list)
            {
                var valueList = new List<object>();
                foreach (var field in item.GetType().GetProperties())
                {
                    valueList.Add(field.GetValue(item, null));
                }
                var temp = valueList.Select((i) => "\"" + i + "\"");
                csv.Append(temp.Aggregate((i, j) => i + "," + j) + System.Environment.NewLine);
            }
        }
        string path = Application.dataPath + "/RefResources/DatabaseCN/box_drop.txt";
        //Debug.Log("path3=>" + path);
        File.WriteAllText(path, csv.ToString());
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 
    /// </summary>
    void ShowBoxItem()
    {
        //if (sprites == null&& sprites.size <= 0) return;

        UIAtlas atlas = LoadAsset<UIAtlas>(atlasPath);

        //NGUIEditorTools.DrawSeparator();
        Texture2D tex = atlas.texture as Texture2D;
        if (tex == null)
        {
            GUILayout.Label("The atlas doesn't have a texture to work with");
            return;
        }

        //BetterList<string> sprites = atlas.GetListOfSprites(NGUISettings.partialSprite);

        //BetterList<BoxDropVo> sprites = boxDropList;
        List<BoxDropVo> sprites = boxDropList;
        float size = 80f;
        float padded = size + 10f;
        int columns = Mathf.FloorToInt(Screen.width / padded);
        if (columns < 1) columns = 1;

        int offset = 0;
        Rect rect = new Rect(10f, 0, size, size);

        GUILayout.Space(10f);
        mPos = GUILayout.BeginScrollView(mPos);
        int rows = 1;

        while (offset < sprites.Count)
        {
            GUILayout.BeginHorizontal();
            {
                int col = 0;
                rect.x = 10f;

                for (; offset < sprites.Count; ++offset)
                {
                    //掉落包
                    BoxDropVo boxDrop = sprites[offset];
                    //获得物品
                    ItemVo spriteItem = ItemCfgMgr.instance.ItemTable[boxDrop.ItemId];
                    string spriteName = "Item_" + spriteItem.ImgId;
                    UISpriteData sprite = atlas.GetSprite(spriteName);
                    if (sprite == null) continue;

                    if (GUI.Button(rect, ""))
                    {
                        //Debug.Log("Event.current.button=>" + Event.current.button);
                        //0左键，1右键，2中键
                        if (Event.current.button == 0)
                        {
                            //Debug.Log("鼠标左键");
                            //selectedSpriteId = spriteItem.Id;
                            selectBoxDrop = boxDrop;
                        }
                    }

                    if (Event.current.type == EventType.Repaint)
                    {
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

                        if (selectBoxDrop == boxDrop)
                        {
                            NGUIEditorTools.DrawOutline(rect, new Color(0.4f, 1f, 0f, 1f));
                        }
                    }

                    GUI.backgroundColor = new Color(1f, 1f, 1f, 0.5f);
                    GUI.contentColor = new Color(1f, 1f, 1f, 0.7f);
                    //创建文字描述
                    //GUI.Label(new Rect(rect.x, rect.y + rect.height, rect.width, 32f), sprite.name, "ProgressBarBack");
                    //创建文字描述
                    GUI.Label(new Rect(rect.x, rect.y + rect.height, rect.width, 32f), spriteItem.Name, "ProgressBarBack");
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
    }

    /// <summary>
    /// Load the asset at the specified path.
    /// </summary>
    static public Object LoadAsset(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        return AssetDatabase.LoadMainAssetAtPath(path);
    }

    /// <summary>
    /// Convenience function to load an asset of specified type, given the full path to it.
    /// </summary>
    static public T LoadAsset<T>(string path) where T : Object
    {
        Object obj = LoadAsset(path);
        if (obj == null) return null;

        T val = obj as T;
        if (val != null) return val;

        if (typeof(T).IsSubclassOf(typeof(Component)))
        {
            if (obj.GetType() == typeof(GameObject))
            {
                GameObject go = obj as GameObject;
                return go.GetComponent(typeof(T)) as T;
            }
        }
        return null;
    }

    void testGUILayout1()
    {
        //开始水平线性布局
        GUILayout.BeginHorizontal();
        GUILayout.Box("开始水平布局");
        GUILayout.Button("按钮");
        GUILayout.Label("文本");
        GUILayout.TextField("输入框");
        //结束水平线性布局
        GUILayout.EndHorizontal();

        //开始垂直线性布局
        GUILayout.BeginVertical();
        GUILayout.Box("开始垂直布局");
        GUILayout.Button("按钮");
        GUILayout.Label("文本");
        GUILayout.TextField("输入框");

        //objCube.renderer.material.mainTexture=texture2D动态加贴图这么加的
        //结束垂直线性布局
        GUILayout.EndVertical();


    }

    void testGUILayout2()
    {
        GUILayout.BeginArea(new Rect(0, 10, 300, 100));//"开始一个显示的区域"
        GUILayout.BeginHorizontal("开始最外层横向布局");

        GUILayout.BeginVertical("嵌套第一个纵向布局");
        GUILayout.Box("Box11");
        GUILayout.Space(10);//两个Box中间偏移10 像素
        GUILayout.Box("Box12");
        GUILayout.EndVertical();//"结束嵌套的纵向布局"

        GUILayout.Space(20);//两个纵向布局间隔20像素

        GUILayout.BeginVertical("嵌套第二个纵向布局");
        GUILayout.Box("Box21");
        GUILayout.Space(10);//两个Box中间偏移10 像素
        GUILayout.Box("Box22");
        GUILayout.EndVertical();//"结束嵌套的纵向布局"

        GUILayout.EndHorizontal();//结束最外层横向布局
        GUILayout.EndArea();
    }
}
