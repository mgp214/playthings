using UnityEngine;
using System.Collections;


public class CursorBehavior : MonoBehaviour {

    public float maxCursorDistance,freeRotationSpeed;
    public LayerMask cursorLayerMask;
    private GameObject objAtCursor,manipulateObj,manipulateGhost;
    private RaycastHit cursorRayHit;
    private int manipulatedLayerBuffer,rotateIncrement;
    
    public bool offsetMode { get; private set; }
    private Vector3 ghostOffset;
    public float offsetSmoothing, offsetSpeed;
    private float smoothedOffsetX, smoothedOffsetY, smoothedOffsetZ;
    private GameObject playerObj;

    void Start() {
        playerObj = GameObject.Find("Camera");
        rotateIncrement = 0;
        offsetMode = false;
        smoothedOffsetX = 0f;
        smoothedOffsetY = 0f;
        smoothedOffsetZ = 0f;
    }

    void Update() {
        if (!offsetMode) {
            MoveUpdate();
        }
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
            offsetMode = false;
            ghostOffset = Vector3.zero;
            //check that the ghost isn't obstructed
            if (!manipulateGhost.GetComponent<GhostCollisionChecker>().placementBlocked) {
                //stop all motion on the block (speedy thing goes in, still thing comes out)
                manipulateObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
                manipulateObj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                //copy position info from ghost to actual block
                manipulateObj.transform.position = manipulateGhost.transform.position;
                manipulateObj.transform.rotation = manipulateGhost.transform.rotation;
            }
            //restore the original layer
            manipulateObj.layer = manipulatedLayerBuffer;
            Destroy(manipulateGhost);
        }
        //if the rotate button is down, check for rotation
        if (Input.GetButton("Select / Rotate") && manipulateGhost) {
            if (rotateIncrement == 0) {
                manipulateGhost.transform.Rotate(new Vector3(Input.GetAxis("Rotate Roll"), Input.GetAxis("Rotate Pitch"), Input.GetAxis("Rotate Yaw")) * freeRotationSpeed * Time.deltaTime);
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
            //if we are in rotate mode, toggle angle snap
            if (Input.GetButton("Select / Rotate")) {
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
            } else {
                //if we are in translate mode, block offset move
                if (offsetMode) {
                    offsetMode = false;
                } else {
                    offsetMode = true;
                    ghostOffset = Vector3.zero;
                }
            }
        }
        //if manipulating an object in rotation mode, automatically turn off offsetMode
        if (Input.GetButton("Select / Rotate") && offsetMode && manipulateObj) {
            offsetMode = false;
        }
        if (offsetMode && manipulateGhost) {
            smoothedOffsetX = Mathf.Lerp(smoothedOffsetX, Input.GetAxisRaw("Camera Strafe"), offsetSmoothing);
            smoothedOffsetY = Mathf.Lerp(smoothedOffsetY, Input.GetAxisRaw("Camera Vertical"), offsetSmoothing);
            smoothedOffsetZ = Mathf.Lerp(smoothedOffsetZ, Input.GetAxisRaw("Camera Forward"), offsetSmoothing);

            ghostOffset += Vector3.ProjectOnPlane(playerObj.transform.TransformDirection(new Vector3(smoothedOffsetX, 0f, 0f)), Vector3.up).normalized * Mathf.Abs(smoothedOffsetX) * offsetSpeed;
            ghostOffset += new Vector3(0f, smoothedOffsetY * offsetSpeed, 0f);
            ghostOffset += Vector3.ProjectOnPlane(playerObj.transform.TransformDirection(new Vector3(0f, 0f, smoothedOffsetZ)), Vector3.up).normalized * Mathf.Abs(smoothedOffsetZ) * offsetSpeed;
        }
        if (manipulateGhost) {
            manipulateGhost.transform.position = transform.position + new Vector3(0f,manipulateGhost.GetComponent<Renderer>().bounds.extents.y+0.01f,0f) + ghostOffset;
        }
    }
}
