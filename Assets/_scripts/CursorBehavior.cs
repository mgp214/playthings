using UnityEngine;
using System.Collections;

public class CursorBehavior : MonoBehaviour {

    public float maxCursorDistance;
    public LayerMask cursorLayerMask;

	void Update () {
	    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        
        if (Physics.Raycast(ray,out rayHit,maxCursorDistance,cursorLayerMask)) {
            //if the cursor ray hits a valid object    
            transform.position = rayHit.point;
        } else {
            //if the cursor ray doesn't hit anything
            transform.position = ray.origin + ray.direction*maxCursorDistance;
        }
	}
}
