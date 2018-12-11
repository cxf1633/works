using UnityEngine;
using UnityEditor;

public class Editor_Settings
{

    #region Generic Get and Set methods

    /// <summary>
    /// Save the specified object in settings.
    /// </summary>

    static public void Set(string name, Object obj)
    {
        if (obj == null)
        {
            EditorPrefs.DeleteKey(name);
        }
        else
        {
            if (obj != null)
            {
                string path = AssetDatabase.GetAssetPath(obj);
                Debug.Log("path==>" + path);
                if (!string.IsNullOrEmpty(path))
                {
                    EditorPrefs.SetString(name, path);
                }
                else
                {
                    EditorPrefs.SetString(name, obj.GetInstanceID().ToString());
                }
            }
            else EditorPrefs.DeleteKey(name);
        }
    }
    /// <summary>
    /// Get a previously saved object from settings.
    /// </summary>

    static public T Get<T>(string name, T defaultValue) where T : Object
    {
        string path = EditorPrefs.GetString(name);
        if (string.IsNullOrEmpty(path)) return null;

        T retVal = NGUIEditorTools.LoadAsset<T>(path);

        if (retVal == null)
        {
            int id;
            if (int.TryParse(path, out id))
                return EditorUtility.InstanceIDToObject(id) as T;
        }
        return retVal;
    }

    /// <summary>
    /// Save the specified string value in settings.
    /// </summary>

    static public void SetString(string name, string val) { EditorPrefs.SetString(name, val); }

    /// <summary>
    /// Get the previously saved string value.
    /// </summary>

    static public string GetString(string name, string defaultValue) { return EditorPrefs.GetString(name, defaultValue); }

    /// <summary>
    /// Save the specified integer value in settings.
    /// </summary>

    static public void SetInt(string name, int val) { EditorPrefs.SetInt(name, val); }
    /// <summary>
    /// Get the previously saved integer value.
    /// </summary>

    static public int GetInt(string name, int defaultValue) { return EditorPrefs.GetInt(name, defaultValue); }
    #endregion


    #region setting properties
    static public UIAtlas atlas
    {
        get { return Get<UIAtlas>("NGUI Atlas", null); }
        set { Set("NGUI Atlas", value); }
    }
    static public string selectedSprite
    {
        get { return GetString("NGUI Sprite", null); }
        set { SetString("NGUI Sprite", value); }
    }
    static public int selectedSpriteId
    {
        get
        {
            return GetInt("NGUI selectedSpriteId", 0);
        }
        set
        {
            SetInt("NGUI selectedSpriteId", value);
        }
    }
    static public int inputBoxId
    {
        get
        {
            return GetInt("NGUI inputBoxId", 400001);
        }
        set
        {
            Debug.Log("set inputBoxId" + value);
            SetInt("NGUI inputBoxId", value);
        }
    }


    #endregion

}