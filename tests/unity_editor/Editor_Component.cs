using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//自定义Tset脚本
[CustomEditor(typeof(Test_Component))]

public class MyEditor : Editor
{
    //在这里方法中就可以绘制面板。
    public override void OnInspectorGUI()
    {
        //得到Test对象
        Test_Component test = (Test_Component)target;
        //绘制一个窗口
        test.mRectValue = EditorGUILayout.RectField("窗口坐标",
                test.mRectValue);
        //绘制一个贴图槽
        test.texture = EditorGUILayout.ObjectField("增加一个贴图", test.texture, typeof(Texture), true) as Texture;

    }
}
