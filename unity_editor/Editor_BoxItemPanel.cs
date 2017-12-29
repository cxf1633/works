using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Editor component used to display a list of sprites.
/// </summary>

public class Editor_BoxItemPanel : ScriptableWizard
{
    static public Editor_BoxItemPanel instance;

    void OnEnable() { instance = this; }
    void OnDisable() { instance = null; }

    public delegate void Callback(string sprite);

    SerializedObject mObject;
    SerializedProperty mProperty;

    UISprite mSprite;
    Vector2 mPos = Vector2.zero;
    Callback mCallback;
    float mClickTime = 0f;
    static string atlasPath = "Assets/Art/Atlas/PartsUIAtlas/PartsUIAtlas.prefab";

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

    /// <summary>
    /// Draw the custom wizard.
    /// </summary>

    void OnGUI()
    {
        //ShowBoxItem();
        ShowAtlas();
    }

    void ShowAtlas()
    {
        NGUIEditorTools.SetLabelWidth(80f);

        if (NGUISettings.atlas == null)
        {
            GUILayout.Label("No Atlas selected.", "LODLevelNotifyText");
        }
        else
        {
            //UIAtlas atlas = NGUISettings.atlas;
            UIAtlas atlas = LoadAsset<UIAtlas>(atlasPath);

            bool close = false;
            GUILayout.Label(atlas.name + " Sprites", "LODLevelNotifyText");
            NGUIEditorTools.DrawSeparator();

            GUILayout.BeginHorizontal();
            GUILayout.Space(84f);

            string before = NGUISettings.partialSprite;
            string after = EditorGUILayout.TextField("", before, "SearchTextField");
            if (before != after) NGUISettings.partialSprite = after;

            if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
            {
                NGUISettings.partialSprite = "";
                GUIUtility.keyboardControl = 0;
            }
            GUILayout.Space(84f);
            GUILayout.EndHorizontal();

            Texture2D tex = atlas.texture as Texture2D;

            if (tex == null)
            {
                GUILayout.Label("The atlas doesn't have a texture to work with");
                return;
            }

            BetterList<string> sprites = atlas.GetListOfSprites(NGUISettings.partialSprite);

            float size = 80f;
            float padded = size + 10f;
            int columns = Mathf.FloorToInt(Screen.width / padded);
            if (columns < 1) columns = 1;

            int offset = 0;
            Rect rect = new Rect(10f, 0, size, size);

            GUILayout.Space(10f);
            mPos = GUILayout.BeginScrollView(mPos);
            int rows = 1;

            while (offset < sprites.size)
            {
                GUILayout.BeginHorizontal();
                {
                    int col = 0;
                    rect.x = 10f;

                    for (; offset < sprites.size; ++offset)
                    {
                        UISpriteData sprite = atlas.GetSprite(sprites[offset]);
                        if (sprite == null) continue;

                        //// Button comes first
                        //if (GUI.Button(rect, ""))
                        //{
                        //    if (Event.current.button == 0)
                        //    {
                        //        float delta = Time.realtimeSinceStartup - mClickTime;
                        //        mClickTime = Time.realtimeSinceStartup;

                        //        if (NGUISettings.selectedSprite != sprite.name)
                        //        {
                        //            if (mSprite != null)
                        //            {
                        //                NGUIEditorTools.RegisterUndo("Atlas Selection", mSprite);
                        //                mSprite.MakePixelPerfect();
                        //                EditorUtility.SetDirty(mSprite.gameObject);
                        //            }

                        //            NGUISettings.selectedSprite = sprite.name;
                        //            NGUIEditorTools.RepaintSprites();
                        //            if (mCallback != null) mCallback(sprite.name);
                        //        }
                        //        else if (delta < 0.5f) close = true;
                        //    }
                        //    else
                        //    {
                        //        NGUIContextMenu.AddItem("Edit", false, EditSprite, sprite);
                        //        NGUIContextMenu.AddItem("Delete", false, DeleteSprite, sprite);
                        //        NGUIContextMenu.Show();
                        //    }
                        //}

                        if (Event.current.type == EventType.Repaint)
                        {
                            // On top of the button we have a checkboard grid
                            NGUIEditorTools.DrawTiledTexture(rect, NGUIEditorTools.backdropTexture);
                            //Rect uv = default(Rect);
                            //if (sprite.rotated)
                            //{
                            //    uv = new Rect(sprite.x, sprite.y, sprite.height, sprite.width);
                            //}
                            //else
                            //{
                            //    uv = new Rect(sprite.x, sprite.y, sprite.width, sprite.height);
                            //}

                            //uv = NGUIMath.ConvertToTexCoords(uv, tex.width, tex.height);

                            //// Calculate the texture's scale that's needed to display the sprite in the clipped area
                            //float scaleX = rect.width / uv.width;
                            //float scaleY = rect.height / uv.height;

                            //// Stretch the sprite so that it will appear proper
                            //float aspect = (scaleY / scaleX) / ((float)tex.height / tex.width);
                            //Rect clipRect = rect;

                            //if (aspect != 1f)
                            //{
                            //    if (aspect < 1f)
                            //    {
                            //        // The sprite is taller than it is wider
                            //        float padding = size * (1f - aspect) * 0.5f;
                            //        clipRect.xMin += padding;
                            //        clipRect.xMax -= padding;
                            //    }
                            //    else
                            //    {
                            //        // The sprite is wider than it is taller
                            //        float padding = size * (1f - 1f / aspect) * 0.5f;
                            //        clipRect.yMin += padding;
                            //        clipRect.yMax -= padding;
                            //    }
                            //}

                            //GUI.DrawTextureWithTexCoords(clipRect, tex, uv);

                            // Draw the selection
                            //if (NGUISettings.selectedSprite == sprite.name)
                            //{
                            //    NGUIEditorTools.DrawOutline(rect, new Color(0.4f, 1f, 0f, 1f));
                            //}
                        }

                        //GUI.backgroundColor = new Color(1f, 1f, 1f, 0.5f);
                        //GUI.contentColor = new Color(1f, 1f, 1f, 0.7f);
                        //GUI.Label(new Rect(rect.x, rect.y + rect.height, rect.width, 32f), sprite.name, "ProgressBarBack");
                        //GUI.contentColor = Color.white;
                        //GUI.backgroundColor = Color.white;

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

            if (close) Close();
        }
    }

    void ShowBoxItem()
    {
        NGUIEditorTools.SetLabelWidth(80f);

        if (Editor_Settings.atlas == null)
        {
            GUILayout.Label("No Atlas selected.", "LODLevelNotifyText");
        }
        else
        {
            //UIAtlas atlas = Editor_Settings.atlas;
            UIAtlas atlas = LoadAsset<UIAtlas>(atlasPath);

            bool close = false;
            GUILayout.Label("Box Id =" + Editor_Settings.inputBoxId + " Item:", "LODLevelNotifyText");
            //GUILayout.Label("Box Id =" + " Item:", "LODLevelNotifyText");
            NGUIEditorTools.DrawSeparator();

            //GUILayout.BeginHorizontal();
            //GUILayout.Space(84f);

            //string before = Editor_Settings.partialSprite;
            //string after = EditorGUILayout.TextField("", before, "SearchTextField");
            //if (before != after) Editor_Settings.partialSprite = after;

            //if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
            //{
            //    Editor_Settings.partialSprite = "";
            //    GUIUtility.keyboardControl = 0;
            //}
            //GUILayout.Space(84f);
            //GUILayout.EndHorizontal();

            Texture2D tex = atlas.texture as Texture2D;

            if (tex == null)
            {
                GUILayout.Label("The atlas doesn't have a texture to work with");
                return;
            }

            //BetterList<string> sprites = atlas.GetListOfSprites(Editor_Settings.partialSprite);

            BetterList<ItemVo> sprites = Editor_BoxDropItem.getItemList();

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
                            if (Event.current.button == 0)
                            {
                                //float delta = Time.realtimeSinceStartup - mClickTime;
                                //mClickTime = Time.realtimeSinceStartup;

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
                                    //Editor_Settings.selectedSprite = sprite.name;
                                    NGUIEditorTools.RepaintSprites();
                                    //选中
                                    if (mCallback != null) mCallback(sprite.name);
                                }
                                //双击关闭
                                //else if (delta < 0.5f) close = true;
                            }
                            else
                            {
                                //if (Editor_Settings.selectedSpriteId != spriteItem.Id)
                                //{
                                //    Editor_Settings.selectedSpriteId = spriteItem.Id;
                                //    this.Repaint();
                                //}
                                //NGUIContextMenu.AddItem("Edit", false, EditSprite, sprite);
                                NGUIContextMenu.AddItem("Add", false, AddSprite, sprite);
                                NGUIContextMenu.AddItem("Delete", false, DeleteSprite, spriteItem);
                                NGUIContextMenu.Show();
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

            if (close) Close();
        }
    }

    /// <summary>
    /// Edit the sprite (context menu selection)
    /// </summary>

    void EditSprite(object obj)
    {
        if (this == null) return;
        UISpriteData sd = obj as UISpriteData;
        NGUIEditorTools.SelectSprite(sd.name);
        Close();
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
        if (this == null) return;
        Debug.Log("删除物品=============>>>");

        //ItemVo sd = obj as ItemVo;

        //BetterList<ItemVo> itemList = Editor_DropItem_tool.getItemList();
        //itemList.Remove(sd);

    }

    /// <summary>
    /// Property-based selection result.
    /// </summary>

    void OnSpriteSelection(string sp)
    {
        Debug.Log("选中的图标是：" + sp);
        //if (mObject != null && mProperty != null)
        //{
        //    mObject.Update();
        //    mProperty.stringValue = sp;
        //    mObject.ApplyModifiedProperties();
        //}
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
    /// Show the sprite selection wizard.
    /// </summary>

    static public void Show(SerializedObject ob, SerializedProperty pro, UIAtlas atlas)
    {
        if (instance != null)
        {
            instance.Close();
            instance = null;
        }

        if (ob != null && pro != null && atlas != null)
        {
            Editor_BoxItemPanel comp = ScriptableWizard.DisplayWizard<Editor_BoxItemPanel>("Select a Sprite");
            Editor_Settings.atlas = atlas;
            Editor_Settings.selectedSprite = pro.hasMultipleDifferentValues ? null : pro.stringValue;
            comp.mSprite = null;
            comp.mObject = ob;
            comp.mProperty = pro;
            comp.mCallback = comp.OnSpriteSelection;
        }
    }

    /// <summary>
    /// Show the selection wizard.
    /// </summary>

    static public void Show(Callback callback)
    {
        if (instance != null)
        {
            instance.Close();
            instance = null;
        }

        Editor_BoxItemPanel comp = ScriptableWizard.DisplayWizard<Editor_BoxItemPanel>("Box Drop Item");
        comp.mSprite = null;
        comp.mCallback = callback;
    }
}
