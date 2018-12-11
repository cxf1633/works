using UnityEngine;
using System.Collections;

public class LeftJoystick1 : MonoBehaviour{

	public GameObject thumbParent;
	public Transform thumb;
    public Transform thumbBg;
	public Transform thumbCircle;
	public float circleRadius = 64;
    public float circleNoMoveRadius = 30;
    public bool switchFree;
    public GameObject moveIconObj;
    public GameObject noMoveObj;
    public int joystickType;
    bool isDraged;
    bool isEnabled;

	// API
	public System.Action<Vector2> onMove;
	public System.Action<Vector2> onStart;
	public System.Action<Vector2> onStop;

	[System.NonSerialized]
	public bool isPressed;

	[System.NonSerialized]
	public Vector2 normalizedDirection;

    // PRIVATE
    Vector3 thumbLerpTarget;
    Vector2 touchStartPos;
    Vector3 thumbParentPos;
    Vector3 circleLocalPos;

    Vector3 tempVector = Vector3.zero; 

	void Start () {
        Debug.Log("Start==>>>>" + joystickType);
        circleLocalPos = thumbCircle.localPosition;
        thumbParentPos = thumbParent.transform.localPosition;
        switchFree = SystemSettings.instance.LockLeftJoystickState;
        SystemSettings.instance.ChangeLeftJoystick.AddListener(UpdateJoyStickType);
    }

    void OnDestroy() {
        SystemSettings.instance.ChangeLeftJoystick.RemoveListener(UpdateJoyStickType);
    }

    void UpdateJoyStickType(bool _value) {
        Debug.Log("UpdateJoyStickType==>>>>");
        switchFree = _value;
    }

    public void ReleaseThumb() {
        FadeOut();
    }

	// CALLBACK
	void OnPress (bool pressed) {
        Debug.Log("OnPress==>>>>" + joystickType);
        isPressed = pressed;
		if (pressed) {
            isDraged = false;
			touchStartPos = UICamera.currentTouch.pos;
            FadeIn();
		}
        else {
            FadeOut();
        }

        if (pressed && !isDraged)
            OnPressMove();
	}

	void OnDragStart () {
        Debug.Log("OnDragStart==>>>>");
        isDraged = true;
        leftJoystickStart(normalizedDirection);
  //      if (onStart != null) {
		//	onStart(normalizedDirection);
		//}
	}

	void OnDragEnd () {
        Debug.Log("OnDragEnd==>>>>");
        isDraged = false;
		FadeOut();
	}

    void OnDrag(Vector2 delta) {
        Debug.Log("OnDrag==>>>>");
        if (!isEnabled) return;
        var touchPos = UICamera.currentTouch.pos;

        var thumbPos = ScreenToWorld(touchPos);
        moveIconObj.gameObject.SetActive(true);
        switch (switchFree) {
            case true:
                thumbLerpTarget = thumb.parent.InverseTransformPoint(thumbPos);
                if (thumbLerpTarget.sqrMagnitude > circleRadius * circleRadius) {
                    thumbLerpTarget = thumbLerpTarget.normalized * circleRadius;
                }
                thumb.localPosition = thumbLerpTarget;
                normalizedDirection = thumbLerpTarget / circleRadius;
                var noMovedir = thumb.localPosition - thumbBg.localPosition;
                if (noMovedir.sqrMagnitude <= circleNoMoveRadius * circleNoMoveRadius) {
                    //if (onStop != null) {
                    //    onStop(normalizedDirection);
                    //}
                    leftJoystickStop(normalizedDirection);
                    moveIconObj.gameObject.SetActive(false);
                    return;
                }

                var direction = thumb.localPosition - thumbCircle.localPosition;
                float angle = Vector3.Angle(direction, Vector3.up);
                direction = Vector3.Normalize(direction);
                float dot = Vector3.Dot(direction, Vector3.left);
                if (dot < 0)
                    angle = 360 - angle;
                tempVector.x = tempVector.y = 0f;
                tempVector.z = angle;
                moveIconObj.transform.localEulerAngles = tempVector;
                moveIconObj.transform.localPosition = thumbCircle.localPosition + direction * 153;
                break;
            case false:
                thumb.position = thumbPos;
                var dir = thumb.localPosition - thumbBg.localPosition;
                Vector3 normalizedDir;
                if (dir.magnitude > circleRadius) {
                    normalizedDir = dir.normalized;
                    var constrained = thumb.localPosition - normalizedDir * circleRadius;
                    thumbBg.localPosition = constrained;
                }
                else {
                    normalizedDir = dir.normalized;
                }
                thumbLerpTarget = thumbBg.InverseTransformPoint(thumbPos);
                normalizedDirection = thumbLerpTarget / circleRadius;
                var noMovedir1 = thumb.localPosition - thumbBg.localPosition;
                if (noMovedir1.sqrMagnitude <= circleNoMoveRadius * circleNoMoveRadius) {
                    //if (onStop != null) {
                    //    onStop(normalizedDirection);
                    //}
                    leftJoystickStop(normalizedDirection);
                    moveIconObj.gameObject.SetActive(false);
                    return;
                }

                var direction1 = thumb.localPosition - thumbBg.localPosition;
                float angle1 = Vector3.Angle(direction1, Vector3.up);
                direction1 = Vector3.Normalize(direction1);
                float dot1 = Vector3.Dot(direction1, Vector3.left);
                if (dot1 < 0)
                    angle1 = 360 - angle1;
                tempVector.x = tempVector.y = 0f;
                tempVector.z = angle1;
                moveIconObj.transform.localEulerAngles = tempVector;
                moveIconObj.transform.localPosition = thumbCircle.localPosition + direction1 * 153 + thumbBg.localPosition;
                break;
        }

        leftJoystickMove(normalizedDirection);
        //if (onMove != null) {
        //    onMove(normalizedDirection);
        //}
    }

    void OnPressMove() {
        if (!isEnabled) return;
        if (joystickType == (int)TouchArea.NoMove)
            return;
        var touchPos = UICamera.currentTouch.pos;
        var thumbPos = ScreenToWorld(touchPos);

        var dir = thumb.parent.localPosition - noMoveObj.transform.localPosition;
        float angle = Vector3.Angle(dir, Vector3.right);
        dir = Vector3.Normalize(dir);
        float dot = Vector3.Cross(dir, Vector3.right).z;
        if (dot < 0)
            angle = 360 - angle;
     
        var height = noMoveObj.transform.GetComponent<UIWidget>().height / 2;
        var width = noMoveObj.transform.GetComponent<UIWidget>().width / 2;
        float length = 0;
        if (angle >= 0 && angle < 45) {
            length = width / Mathf.Cos(angle* Mathf.Deg2Rad);
        }
        else if (angle >= 45 && angle < 90) {
            length = height / Mathf.Cos((90 - angle)* Mathf.Deg2Rad);
        }
        else if (angle >= 90 && angle < 135) {
            length = height / Mathf.Cos((angle - 90)* Mathf.Deg2Rad);
        }
        else if (angle >= 135 && angle < 180) {
            length = width / Mathf.Cos((180 - angle)* Mathf.Deg2Rad);
        }
        else if (angle >= 180 && angle < 225) {
            length = width / Mathf.Cos((angle - 180)* Mathf.Deg2Rad);
        }
        else if (angle >= 225 && angle < 270) {
            length = height / Mathf.Cos((270 - angle)* Mathf.Deg2Rad);
        }
        else if (angle >= 270 && angle < 315) {
            length = height / Mathf.Cos((angle - 270)* Mathf.Deg2Rad);
        }
        else if (angle >= 315 && angle <= 360) {
            length = width / Mathf.Cos((360 - angle)* Mathf.Deg2Rad);
        }
        if (joystickType == (int)TouchArea.NoMove)
            return;
        //thumb.parent.localPosition = dir * circleRadius;
        thumb.parent.localPosition = dir * length + noMoveObj.transform.localPosition;
        Vector3 thumbTarget =  thumbBg.parent.parent.InverseTransformPoint(thumbPos);
        
        if ((thumbTarget - thumbBg.parent.localPosition).sqrMagnitude > circleRadius * circleRadius) {
            thumb.localPosition = (thumbTarget - thumbBg.parent.localPosition).normalized * circleRadius;
        }
        else {
            thumb.localPosition = (thumbTarget - thumbBg.parent.localPosition).normalized * (thumbTarget - thumbBg.parent.localPosition).magnitude;
        }

        thumbLerpTarget = thumbBg.InverseTransformPoint(thumbPos);
        normalizedDirection = thumbLerpTarget / circleRadius;

        var dir1 = thumb.parent.localPosition - noMoveObj.transform.localPosition;
        float angle1 = Vector3.Angle(dir1, Vector3.up);
        dir1 = Vector3.Normalize(dir1);
        float dot1 = Vector3.Dot(dir1, Vector3.left);
        moveIconObj.gameObject.SetActive(true);
        if (dot1 < 0)
            angle1 = 360 - angle1;
        tempVector.x = tempVector.y = 0f;
        tempVector.z = angle1;
        moveIconObj.transform.localEulerAngles = tempVector;
        moveIconObj.transform.localPosition = thumbCircle.localPosition + dir1 * 153 + thumbBg.localPosition;
        //if (onMove != null) {
        //    onMove(normalizedDirection);
        //}
        leftJoystickMove(normalizedDirection);
    }

    Vector3 ScreenToWorld (Vector2 touchPos) {
		return UICamera.currentCamera.ScreenToWorldPoint(touchPos);
	}

	void FadeIn () {
        isEnabled = true;
        var touchPos = UICamera.currentTouch.pos;
        var thumbPos = ScreenToWorld(touchPos);
        thumbParent.transform.position = thumbPos;
        thumb.gameObject.SetActive(true);
        thumbBg.gameObject.SetActive(true);
        thumbCircle.gameObject.SetActive(false);
	}

	void FadeOut () {
        isPressed = false;
        isDraged = false;
        isEnabled = false;
        normalizedDirection = Vector2.zero;
		if (onStop != null) {
			onStop(normalizedDirection);
		}
        leftJoystickStop(normalizedDirection);
        thumb.localPosition = Vector3.zero;
        thumbBg.localPosition = Vector3.zero;
        moveIconObj.gameObject.SetActive(false);
        thumb.gameObject.SetActive(false);
        thumbBg.gameObject.SetActive(false);
        thumbCircle.gameObject.SetActive(true);
        thumbCircle.localPosition = circleLocalPos;
        thumbParent.transform.localPosition = thumbParentPos;
	}
    void leftJoystickStart(Vector2 dir) {
        Debug.Log("============leftJoystickStart1============" + dir);
        Vector3 dir3 = dir.toVec3();
        Debug.Log("============leftJoystickStart2============" + dir3);
    }
    void leftJoystickMove(Vector2 dir) {
        Vector3 dir3 = dir.toVec3();
        Debug.Log("============leftJoystickMove1============" + dir3);
        Vector3 wdir3 = transformToWorldDir(dir.toVec3());
        Debug.Log("============leftJoystickMove2============" + wdir3);
    }
    void leftJoystickStop(Vector2 dir) {
        Vector3 dir3 = dir.toVec3();
        Debug.Log("============leftJoystickStop1============" + dir3);
        Vector3 wdir3 = transformToWorldDir(dir.toVec3());
        Debug.Log("============leftJoystickStop2============" + wdir3);
    }
    Vector3 transformToWorldDir(Vector3 localDir) {
        var cameraDir = GameCamera.Instance.CameraForward.NewY(0f);
        return Quaternion.LookRotation(cameraDir) * localDir;
    }
}
