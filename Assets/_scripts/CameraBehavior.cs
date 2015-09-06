using UnityEngine;
using System.Collections;

public class CameraBehavior : MonoBehaviour {

    public float cameraMoveSpeed, cameraRotateSpeed,cameraZoomSpeed, minHeight, maxHeight, minDegreesFromForward, maxDegreesFromForward,zoomFraction,zoomDistance;
    private float zoomDistanceToLerp;
    private Vector3 zoomPreviousPosition;

    private Transform pivotPointTransform;

    void Start() {
        pivotPointTransform = transform.GetChild(0);
        zoomFraction = 0.5f;
        zoomDistanceToLerp = 0f;
    }
	void Update () {
        MoveUpdate();
        ZoomUpdate();
	}

    private void MoveUpdate() {
        //apply translation
        float rightTranslation = cameraMoveSpeed * Input.GetAxis("Camera Strafe");
        float forwardTranslation = cameraMoveSpeed * Input.GetAxis("Camera Forward");
        Vector3 translationVector = new Vector3(rightTranslation,0f,forwardTranslation);
        translationVector = transform.TransformDirection(translationVector);
        transform.Translate(Vector3.ProjectOnPlane(translationVector,Vector3.up).normalized, Space.World);
        //apply rotation
        transform.RotateAround(pivotPointTransform.position, Vector3.up, cameraRotateSpeed/(pivotPointTransform.position-transform.position).magnitude*Input.GetAxis("Camera Rotation"));
    }

    private void ZoomUpdate() {
        float previousZoomFraction = zoomFraction;
        zoomFraction -= Input.GetAxis("Camera Zoom") * cameraZoomSpeed;
        zoomFraction = Mathf.Clamp01(zoomFraction);
        float zoomDelta = zoomFraction - previousZoomFraction;
        zoomDistanceToLerp -= zoomDelta * zoomDistance;
        if ((transform.position - new Vector3(transform.position.x, Mathf.Lerp(minHeight, maxHeight, zoomFraction), transform.position.z)).magnitude > 0.0001f) {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, Mathf.Lerp(minHeight, maxHeight, zoomFraction), transform.position.z), 0.1f);
        }
        if (Quaternion.Angle(transform.rotation, Quaternion.Slerp(Quaternion.Euler(minDegreesFromForward, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z),
                                                                 Quaternion.Euler(maxDegreesFromForward, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z),
                                                                 zoomFraction)) > 0.0001f) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Slerp(Quaternion.Euler(minDegreesFromForward, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z),
                                                  Quaternion.Euler(maxDegreesFromForward, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z),
                                                  zoomFraction), 0.1f);
        }
        zoomPreviousPosition = transform.position;
        if (Mathf.Abs(zoomDistanceToLerp) > 0.0001f) {
            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized * zoomDistanceToLerp, 0.1f);
            if (zoomDistanceToLerp > 0) {
                zoomDistanceToLerp -= (transform.position - zoomPreviousPosition).magnitude;
            } else {
                zoomDistanceToLerp += (transform.position - zoomPreviousPosition).magnitude;
            }
        }

    }
}
