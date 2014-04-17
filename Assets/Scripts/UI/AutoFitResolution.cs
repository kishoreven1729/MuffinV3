using UnityEngine;
using System.Collections;

public class AutoFitResolution : MonoBehaviour {

    public enum Type
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Center,
        Top
    }

    public Type type;

    private float screenWidth;
    private float screenHeight;


	// Use this for initialization
	void Start () {
	
        SetPosition();
	}

    void SetPosition()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        print("screenWidth:" + screenWidth);

        float top = 0.5f * screenHeight;
        float bottom = -0.5f * screenHeight;
        float left = -0.5f * screenWidth;
        float right = 0.5f * screenWidth;

        Vector3 pos = new Vector3();
        switch(type)
        {
            case Type.TopLeft:
                pos = new Vector3(left, top, 0f);
                break;
            case Type.TopRight:
                pos = new Vector3(right, top, 0f);
                break;
            case Type.BottomLeft:
                pos = new Vector3(left, bottom, 0f);
                break;
            case Type.BottomRight:
                pos = new Vector3(right, bottom, 0f);
                break;
            case Type.Center:
                pos = Vector3.zero;
                break;
            case Type.Top:
                pos = new Vector3(0f, top, 0f);
                break;
        }

        transform.localPosition = pos;
    }

}
