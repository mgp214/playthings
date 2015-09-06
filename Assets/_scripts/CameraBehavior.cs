using UnityEngine;
using System.Collections;

public class CameraBehavior : MonoBehaviour {

    public float cameraMoveSpeed;
    public float cameraRotateSpeed;

	void Update () {
        MoveUpdate();
	}

    private void MoveUpdate() {
        float rightTranslation = cameraMoveSpeed * Input.GetAxis("Camera Strafe");
        float forwardTranslation = cameraMoveSpeed * Input.GetAxis("Camera Forward");
        Vector3 translationVector = new Vector3(rightTranslation,0f,forwardTranslation);
        translationVector = gameObject.transform.TransformDirection(translationVector);
      //  translationVector = Vector3.ProjectOnPlane(translationVector,Vector3.up)
        gameObject.transform.Translate(Vector3.ProjectOnPlane(translationVector,Vector3.up), Space.World);
    }
}
