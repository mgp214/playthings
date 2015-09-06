using UnityEngine;
using System.Collections;

public class CameraPivotPointBehavior : MonoBehaviour {

    private Transform parentTransform;
    public float pivotPointDistance;
    public LayerMask pivotLayerMask;
	// Use this for initialization
	void Start () {
        parentTransform = transform.parent;
	}
	
	// Update is called once per frame
	void Update () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, pivotPointDistance, pivotLayerMask)) {
            transform.position = rayHit.point;
        } else {
            //fixed pivot distance
            Vector3 parentOffset = Vector3.ProjectOnPlane(parentTransform.forward, Vector3.up).normalized * pivotPointDistance;
            transform.position = parentTransform.position + parentOffset;
            transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        }
	}
}
