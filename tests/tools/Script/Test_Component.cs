using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Component : MonoBehaviour {

    //   public Rect mRectValue;
    //   public Texture texture;
    //   // Use this for initialization
    //   void Start () {

    //}

    //// Update is called once per frame
    //void Update () {

    //}

    [HideInInspector] [SerializeField] Rect pRectValue;
    public Rect mRectValue
    {
        get
        {
            return pRectValue;
        }
        set
        {
            pRectValue = value;
        }
    }

    [HideInInspector] [SerializeField] Texture pTexture;
    public Texture texture
    {
        get
        {
            return pTexture;
        }
        set
        {
            pTexture = value;
        }
    }
}
