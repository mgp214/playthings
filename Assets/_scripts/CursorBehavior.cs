using UnityEngine;
using System.Collections;


public class CursorBehavior : MonoBehaviour {

    public float maxCursorDistance,freeRotationSpeed;
    public LayerMask cursorLayerMask;
    private GameObject objAtCursor,manipulateObj,manipulateGhost;
    private RaycastHit cursorRayHit;
    private int manipulatedLayerBuffer;
    private int rotateIncrement;
    void Start() {
        rotateIncrement = 0;
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
        //if the rotate button is down, check for rotation
        if (Input.GetButton("Select / Rotate") && manipulateGhost) {
            if (rotateIncrement == 0) {
                manipulateGhost.transform.Rotate(new Vector3(Input.GetAxis("Camera Strafe"), Input.GetAxis("Camera Forward"), Input.GetAxis("Camera Vertical"))*freeRotationSpeed*Time.deltaTime);
            } else {
                if (Input.GetButtonDown("Rotate Pitch Forward")) {
                    manipulateGhost.transform.Rotate(new Vector3(0f, rotateIncrement, 0f));
                }
                if (Input.GetButtonDown("Rotate Pitch Backward")) {
                    manipulateGhost.transform.Rotate(new Vector3(0f, -rotateIncrement, 0f));
                }
                if (Input.GetButtonDown("Rotate Roll Left")) {
                    manipulateGhost.transform.Rotate(new Vector3(-rotateIncrement, 0f, 0f));
                }
                if (Input.GetButtonDown("Rotate Roll Right")) {
                    manipulateGhost.transform.Rotate(new Vector3(rotateIncrement, 0f, 0f));
                }
                if (Input.GetButtonDown("Rotate Yaw Left")) {
                    manipulateGhost.transform.Rotate(new Vector3(0f, 0f, -rotateIncrement));
                }
                if (Input.GetButtonDown("Rotate Yaw Right")) {
                    manipulateGhost.transform.Rotate(new Vector3(0f, 0f, rotateIncrement));
                }
            }
        }
        //cycle through rotation increments if the rotatation increment button is pressed
        if (Input.GetButtonDown("Manipulate Toggle")) {
            switch (rotateIncrement) {
                case 0:
                    rotateIncrement = 15;
                    break;
                case 15:
                    rotateIncrement = 45;
                    break;
                case 45:
                    rotateIncrement = 0;
                    break;
            }
        }
        if (manipulateGhost) {
            manipulateGhost.transform.position = transform.position + new Vector3(0f,manipulateGhost.GetComponent<Renderer>().bounds.extents.y+0.01f,0f);
        }
    }
}
