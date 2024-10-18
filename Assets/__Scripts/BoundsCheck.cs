using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsCheck : MonoBehaviour
{
    [System.Flags]
    public enum eScreenLocs{
        onScreen = 0,
        offRight = 1,
        offLeft = 2,
        offUp = 4,
        offDown = 8
    }
    public enum eType {center, inset, outset};

    [Header("Inscribed")]
    public eType boundsType = eType.center;
    public float radius = 1f;
    public bool keepOnScreen = true;

    [Header("Dynamic")]
    public eScreenLocs screenLocs = eScreenLocs.onScreen;
    public float camWidth;
    public float camHeight;

    void Awake(){
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
    }

    void LateUpdate(){
        Vector3 pos = transform.position;
        screenLocs = eScreenLocs.onScreen;

        if(pos.x > camWidth){
            pos.x = camWidth;
            screenLocs |= eScreenLocs.offRight;
        }
        if(pos.x < -camWidth){
            pos.x = -camWidth;
            screenLocs |= eScreenLocs.offLeft;
        }

        if(pos.y > camHeight){
            pos.y = camHeight;
            screenLocs |= eScreenLocs.offUp;
        }
        if(pos.y < -camHeight){
            pos.y = -camHeight;
            screenLocs |= eScreenLocs.offDown;
        }

        if(keepOnScreen && !isOnScreen){
            transform.position = pos;
            screenLocs = eScreenLocs.onScreen;
        }
    }

    public bool isOnScreen{
        get{return(screenLocs == eScreenLocs.onScreen);}
    }
    public bool LocIs(eScreenLocs checkLoc){
        if(checkLoc == eScreenLocs.onScreen) return isOnScreen;
        return ((screenLocs & checkLoc) == checkLoc);
    }
    
}
