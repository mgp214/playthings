using UnityEngine;
using System.Collections;

public class CameraBehavior : MonoBehaviour {

    public float cameraMoveSpeed;
    public float cameraRotateSpeed;

    private Transform pivotPointTransform;

    void Start() {
        pivotPointTransform = gameObject.transform.GetChild(0);
    }
	void Update () {
        MoveUpdate();
	}

    private void MoveUpdate() {
        //apply translation
        float rightTranslation = cameraMoveSpeed * Input.GetAxis("Camera Strafe");
        float forwardTranslation = cameraMoveSpeed * Input.GetAxis("Camera Forward");
        Vector3 translationVector = new Vector3(rightTranslation,0f,forwardTranslation);
        translationVector = gameObject.transform.TransformDirection(translationVector);
        gameObject.transform.Translate(Vector3.ProjectOnPlane(translationVector,Vector3.up), Space.World);
        //apply rotation
        gameObject.transform.RotateAround(pivotPointTransform.position, Vector3.up, cameraRotateSpeed/(pivotPointTransform.position-gameObject.transform.position).magnitude*Input.GetAxis("Camera Rotation"));
    }
}
