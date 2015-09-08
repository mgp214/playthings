using UnityEngine;
using System.Collections;


public class CursorBehavior : MonoBehaviour {

    public float maxCursorDistance,freeRotationSpeed;
    public LayerMask cursorLayerMask;
    private GameObject objAtCursor,manipulateObj,manipulateGhost;
    private RaycastHit cursorRayHit;
    private int manipulatedLayerBuffer,rotateIncrement;

    public GameObject rotateXTemplate, rotateYTemplate, rotateZTemplate,degreeMarkerTemplate,offsetMarkerTemplate;
    private GameObject rotateX, rotateY, rotateZ,degreeMarker,offsetMarker;

    public bool offsetMode { get; private set; }
    private Vector3 ghostOffset;
    public float offsetSmoothing, offsetSpeed;
    private float smoothedOffsetX, smoothedOffsetY, smoothedOffsetZ;
    private GameObject playerObj;

    private enum SnapAxis {off,x,y,z};
    private SnapAxis snapAxis;
    private GameObject snapAxisMasterObj;

    void Start() {
        playerObj = GameObject.Find("Camera");
        rotateIncrement = 0;
        offsetMode = false;
        smoothedOffsetX = 0f;
        smoothedOffsetY = 0f;
        smoothedOffsetZ = 0f;
        snapAxis = SnapAxis.off;
    }

    void Update() {
        if (!offsetMode) {
            MoveUpdate();
        }
        SnapAxisUpdate();
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
            //create the rotation guides
            rotateX = Instantiate(rotateXTemplate, manipulateGhost.transform.position, manipulateGhost.transform.rotation * Quaternion.Euler(90f, 0f, 0f)) as GameObject;
            rotateY = Instantiate(rotateYTemplate, manipulateGhost.transform.position, manipulateGhost.transform.rotation * Quaternion.Euler(0f, 90f, 0f)) as GameObject;
            rotateZ = Instantiate(rotateZTemplate, manipulateGhost.transform.position, manipulateGhost.transform.rotation * Quaternion.Euler(0f, 0f, 0f)) as GameObject;
            //create the degree snap marker
            degreeMarker = Instantiate(degreeMarkerTemplate, manipulateGhost.transform.position, manipulateGhost.transform.rotation) as GameObject;
            //create the offset marker
            offsetMarker = Instantiate(offsetMarkerTemplate, manipulateGhost.transform.position, manipulateGhost.transform.rotation) as GameObject;
            //update the degree snap marker based on current value of rotateIncrement
            switch (rotateIncrement) {
                case 0:
                    degreeMarker.GetComponent<TextMesh>().text = "free";
                    break;
                case 15:
                    degreeMarker.GetComponent<TextMesh>().text = "15\u00B0";
                    break;
                case 45:
                    degreeMarker.GetComponent<TextMesh>().text = "45\u00B0";
                    break;
                case 90:
                    degreeMarker.GetComponent<TextMesh>().text = "90\u00B0";
                    break;
            }
            rotateX.transform.SetParent(manipulateGhost.transform,true);
            rotateY.transform.SetParent(manipulateGhost.transform,true);
            rotateZ.transform.SetParent(manipulateGhost.transform, true);
            degreeMarker.transform.SetParent(manipulateGhost.transform, true);
            offsetMarker.transform.SetParent(manipulateGhost.transform, true);
            //determine scaling for arrow mesh based on block bounds for the red guide
            rotateX.transform.GetChild(0).localScale = new Vector3(manipulateGhost.GetComponent<MeshFilter>().mesh.bounds.extents.x + 1.0f, manipulateGhost.GetComponent<MeshFilter>().mesh.bounds.extents.z + 1.0f, 1f);
            //determine letter placement based on block bounds for the red guide
            rotateX.transform.GetChild(1).position += rotateX.transform.GetChild(0).TransformDirection(new Vector3(-manipulateGhost.GetComponent<MeshFilter>().mesh.bounds.extents.y - 1.0f, 1f, 0f));
            rotateX.transform.GetChild(2).position += rotateX.transform.GetChild(0).TransformDirection(new Vector3(manipulateGhost.GetComponent<MeshFilter>().mesh.bounds.extents.y + 1.0f, 1f, 0f));
            //determine scaling for arrow mesh based on block bounds for the green guide
            rotateY.transform.GetChild(0).localScale = new Vector3(manipulateGhost.GetComponent<MeshFilter>().mesh.bounds.extents.z + 1.0f, manipulateGhost.GetComponent<MeshFilter>().mesh.bounds.extents.y + 1.0f, 1f);
            //determine letter placement based on block bounds for the green guide
            rotateY.transform.GetChild(1).position += rotateY.transform.GetChild(0).TransformDirection(new Vector3(-manipulateGhost.GetComponent<MeshFilter>().mesh.bounds.extents.z - 1.0f, 1f, 0f));
            rotateY.transform.GetChild(2).position += rotateY.transform.GetChild(0).TransformDirection(new Vector3(manipulateGhost.GetComponent<MeshFilter>().mesh.bounds.extents.z + 1.0f, 1f, 0f));
            //determine scaling for arrow mesh based on block bounds for the blue guide
            rotateZ.transform.GetChild(0).localScale = new Vector3(manipulateGhost.GetComponent<MeshFilter>().mesh.bounds.extents.y + 1.0f, manipulateGhost.GetComponent<MeshFilter>().mesh.bounds.extents.x + 1.0f, 1f);
            //determine letter placement based on block bounds for the blue guide
            rotateZ.transform.GetChild(1).position += rotateZ.transform.GetChild(0).TransformDirection(new Vector3(-manipulateGhost.GetComponent<MeshFilter>().mesh.bounds.extents.x - 1.0f, 1f, 0f));
            rotateZ.transform.GetChild(2).position += rotateZ.transform.GetChild(0).TransformDirection(new Vector3(manipulateGhost.GetComponent<MeshFilter>().mesh.bounds.extents.x + 1.0f, 1f, 0f));

        }
        //if manipulate key is released, place the manip obj at the ghost's position, zero it's velocities, and destroy the ghost
        if (Input.GetButtonUp("Manipulate") && manipulateGhost) {
            offsetMode = false;
            snapAxis = SnapAxis.off;
            ghostOffset = Vector3.zero;
            Destroy(rotateX);
            Destroy(rotateY);
            Destroy(rotateZ);
            Destroy(degreeMarker);
            Destroy(offsetMarker);
            //check that the ghost isn't obstructed
            if (!manipulateGhost.GetComponent<GhostCollisionChecker>().placementBlocked) {
                //stop all motion on the block (speedy thing goes in, still thing comes out)
                manipulateObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
                manipulateObj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                //check for blocks near the teleported block
                Collider[] colliderArray = Physics.OverlapSphere(manipulateObj.transform.position,0.5f + Mathf.Max(new float[] {manipulateObj.GetComponent<Renderer>().bounds.extents.x,
                                                                                                                         manipulateObj.GetComponent<Renderer>().bounds.extents.y,
                                                                                                                         manipulateObj.GetComponent<Renderer>().bounds.extents.z}));
                foreach (Collider collider in colliderArray) {
                    if (collider.GetComponent<Rigidbody>()) {
                        collider.gameObject.GetComponent<Rigidbody>().WakeUp();
                    }
                }

                //copy position info from ghost to actual block
                manipulateObj.transform.position = manipulateGhost.transform.position;
                manipulateObj.transform.rotation = manipulateGhost.transform.rotation;
            }
            //restore the original layer
            manipulateObj.layer = manipulatedLayerBuffer;
            Destroy(manipulateGhost);
        }
        //if the rotate button is down, check for rotation
        if (manipulateGhost) {
            if (Input.GetButton("Select / Rotate")) {
                rotateX.SetActive(true);
                rotateY.SetActive(true);
                rotateZ.SetActive(true);
                degreeMarker.SetActive(true);
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
            } else {
                rotateX.SetActive(false);
                rotateY.SetActive(false);
                rotateZ.SetActive(false);
                degreeMarker.SetActive(false);
            }
        } 
        //cycle through rotation increments if the rotatation increment button is pressed
        if (Input.GetButtonDown("Angle Snap Toggle") && Input.GetButton("Select / Rotate")) {
            switch (rotateIncrement) {
                case 0:
                    rotateIncrement = 15;
                    degreeMarker.GetComponent<TextMesh>().text = "15\u00B0";
                    break;
                case 15:
                    degreeMarker.GetComponent<TextMesh>().text = "45\u00B0";
                    rotateIncrement = 45;
                    break;
                case 45:
                    rotateIncrement = 90;
                    degreeMarker.GetComponent<TextMesh>().text = "90\u00B0";
                    break;
                case 90:
                    rotateIncrement = 0;
                    degreeMarker.GetComponent<TextMesh>().text = "free";
                    break;
            }
        }
        if (Input.GetButtonDown("Offset Toggle") && !Input.GetButton("Select / Rotate")) {
            //if we are in translate mode, block offset move
            if (offsetMode) {
                offsetMode = false;
            } else {
                offsetMode = true;
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

            //if the snap axis is off, offset normally
            if (snapAxis == SnapAxis.off) {
                ghostOffset += Vector3.ProjectOnPlane(playerObj.transform.TransformDirection(new Vector3(smoothedOffsetX, 0f, 0f)), Vector3.up).normalized * Mathf.Abs(smoothedOffsetX) * offsetSpeed;
                ghostOffset += new Vector3(0f, smoothedOffsetY * offsetSpeed, 0f);
                ghostOffset += Vector3.ProjectOnPlane(playerObj.transform.TransformDirection(new Vector3(0f, 0f, smoothedOffsetZ)), Vector3.up).normalized * Mathf.Abs(smoothedOffsetZ) * offsetSpeed;
            } else if (snapAxis == SnapAxis.x) {
                //if snapped to x axis, snap x offset
                ghostOffset += Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(playerObj.transform.TransformDirection(new Vector3(smoothedOffsetX, 0f, 0f)), Vector3.up).normalized * Mathf.Abs(smoothedOffsetX) * offsetSpeed, snapAxisMasterObj.transform.right);
                ghostOffset += Vector3.ProjectOnPlane(new Vector3(0f, smoothedOffsetY * offsetSpeed, 0f), snapAxisMasterObj.transform.right);
                ghostOffset += Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(playerObj.transform.TransformDirection(new Vector3(0f, 0f, smoothedOffsetZ)), Vector3.up).normalized * Mathf.Abs(smoothedOffsetZ) * offsetSpeed, snapAxisMasterObj.transform.right);
            } else if (snapAxis == SnapAxis.y) {
                //if snapped to y axis, snap y offset
                ghostOffset += Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(playerObj.transform.TransformDirection(new Vector3(smoothedOffsetX, 0f, 0f)), Vector3.up).normalized * Mathf.Abs(smoothedOffsetX) * offsetSpeed, snapAxisMasterObj.transform.up);
                ghostOffset += Vector3.ProjectOnPlane(new Vector3(0f, smoothedOffsetY * offsetSpeed, 0f), snapAxisMasterObj.transform.up);
                ghostOffset += Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(playerObj.transform.TransformDirection(new Vector3(0f, 0f, smoothedOffsetZ)), Vector3.up).normalized * Mathf.Abs(smoothedOffsetZ) * offsetSpeed, snapAxisMasterObj.transform.up);
            } else if (snapAxis == SnapAxis.z) {
                //if snapped to z axis, snap z offset
                ghostOffset += Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(playerObj.transform.TransformDirection(new Vector3(smoothedOffsetX, 0f, 0f)), Vector3.up).normalized * Mathf.Abs(smoothedOffsetX) * offsetSpeed, snapAxisMasterObj.transform.forward);
                ghostOffset += Vector3.ProjectOnPlane(new Vector3(0f, smoothedOffsetY * offsetSpeed, 0f), snapAxisMasterObj.transform.forward);
                ghostOffset += Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(playerObj.transform.TransformDirection(new Vector3(0f, 0f, smoothedOffsetZ)), Vector3.up).normalized * Mathf.Abs(smoothedOffsetZ) * offsetSpeed, snapAxisMasterObj.transform.forward);
            }
        }
        if (manipulateGhost) {
            manipulateGhost.transform.position = transform.position + new Vector3(0f,manipulateGhost.GetComponent<Renderer>().bounds.extents.y+0.05f,0f) + ghostOffset;
        }
        if (offsetMarker) {
            if (offsetMode) {
                offsetMarker.SetActive(true);
            } else {
                offsetMarker.SetActive(false);
            }
        }
    }
    private void SnapAxisUpdate() {
        //if Axis Snap is pressed on a moveable object while in regular manipulate mode while snapAxis is off
        if (snapAxis == SnapAxis.off && objAtCursor && objAtCursor.GetComponent<Movable>() && Input.GetButtonDown("Axis Snap") && manipulateGhost && !offsetMode && !Input.GetButton("Select / Rotate")) {
            snapAxis = SnapAxis.x;
            //turn off offset mode if it's on
            offsetMode = false;
            snapAxisMasterObj = objAtCursor;
            manipulateGhost.transform.rotation = snapAxisMasterObj.transform.rotation;
            return;
        }
        //if the snapAxis is on and the snap axis key is pressed,
        if (snapAxis != SnapAxis.off && Input.GetButtonDown("Axis Snap")) {
            //cycle through the snap axes, and copy the rotation of the target object to the ghost UNLESS we are turning snap axis off
            switch (snapAxis) {
                case SnapAxis.x:
                    snapAxis = SnapAxis.z;
                //    snapAxisMasterObj = objAtCursor;
                    manipulateGhost.transform.rotation = snapAxisMasterObj.transform.rotation;
                    break;
                case SnapAxis.z:
                    snapAxis = SnapAxis.y;
                //    snapAxisMasterObj = objAtCursor;
                    manipulateGhost.transform.rotation = snapAxisMasterObj.transform.rotation;
                    break;
                case SnapAxis.y:
                    snapAxisMasterObj = null;
                    snapAxis = SnapAxis.off;
                    break;
            }
        }
        //if snap axis is on, snap the cursor(and thus the ghost) to the axis
        switch (snapAxis) {
            case SnapAxis.off:
                break;
            case SnapAxis.x:
                transform.position = Vector3.Project(snapAxisMasterObj.transform.position, snapAxisMasterObj.transform.right) + Vector3.ProjectOnPlane(transform.position, snapAxisMasterObj.transform.right);// + snapAxisMasterObj.transform.position;
                break;
            case SnapAxis.y:
                transform.position = Vector3.Project(snapAxisMasterObj.transform.position, snapAxisMasterObj.transform.up) + Vector3.ProjectOnPlane(transform.position, snapAxisMasterObj.transform.up);// + snapAxisMasterObj.transform.position;
                break;
            case SnapAxis.z:
                transform.position = Vector3.Project(snapAxisMasterObj.transform.position, snapAxisMasterObj.transform.forward) + Vector3.ProjectOnPlane(transform.position, snapAxisMasterObj.transform.forward);// + snapAxisMasterObj.transform.position;
                break;
        }
    }
}
