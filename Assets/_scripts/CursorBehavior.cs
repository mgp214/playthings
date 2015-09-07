using UnityEngine;
using System.Collections;


public class CursorBehavior : MonoBehaviour {

    public float maxCursorDistance;
    public LayerMask cursorLayerMask;
    private GameObject objAtCursor,manipulateObj,manipulateGhost;
    private RaycastHit cursorRayHit;
    private int manipulatedLayerBuffer;

    void Start() {

    }

    void Update() {
        MoveUpdate();
        ManipulateUpdate();
    }

	private void MoveUpdate () {
	    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray,out cursorRayHit,maxCursorDistance,cursorLayerMask)) {
            //if the cursor ray hits a valid object    
            transform.position = cursorRayHit.point;
            objAtCursor = cursorRayHit.collider.gameObject;
        } else {
            //if the cursor ray doesn't hit anything
            transform.position = ray.origin + ray.direction*maxCursorDistance;
            objAtCursor = null;
        }
	}

    private void ManipulateUpdate() {
        //if manipulate key is pressed while the cursor is on a moveable object, begin manipulating that object
        if (Input.GetButtonDown("Manipulate") && objAtCursor && objAtCursor.GetComponent<Movable>()) {
            manipulateObj = objAtCursor;
            //also, store the object's layer to a buffer and change to a special layer the cursor won't collide with
            manipulatedLayerBuffer = manipulateObj.layer;
            manipulateObj.layer = LayerMask.NameToLayer("Manipulate");
            manipulateGhost = Instantiate(objAtCursor.GetComponent<Movable>().ghostTemplate) as GameObject;
            //copy manipulated objects rotation to it's ghost
            manipulateGhost.transform.rotation = manipulateObj.transform.rotation;
        }
        //if manipulate key is released, place the manip obj at the ghost's position, zero it's velocities, and destroy the ghost
        if (Input.GetButtonUp("Manipulate") && manipulateGhost) {
            manipulateObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
            manipulateObj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            manipulateObj.transform.position = manipulateGhost.transform.position;
            manipulateObj.transform.rotation = manipulateGhost.transform.rotation;
            //restore the original layer
            manipulateObj.layer = manipulatedLayerBuffer;
            Destroy(manipulateGhost);
        }
        if (manipulateGhost) {
            manipulateGhost.transform.position = transform.position + new Vector3(0f,manipulateGhost.GetComponent<Renderer>().bounds.extents.y+0.01f,0f);
        }
    }
}
