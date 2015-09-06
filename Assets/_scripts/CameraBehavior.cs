using UnityEngine;
using System.Collections;

public class CameraBehavior : MonoBehaviour {

    public float cameraMoveSpeed, minHeight, maxHeight, cameraSmoothing;
    private float zoomDistanceToLerp, smoothedCameraForward, smoothedCameraStrafe,smoothedCameraVertical;
    private Vector3 zoomPreviousPosition;


    Vector2 _mouseAbsolute;
    Vector2 _smoothMouse;

    public Vector2 clampInDegrees = new Vector2(360, 180);
    public bool lockCursor;
    public Vector2 sensitivity = new Vector2(2, 2);
    public Vector2 smoothing = new Vector2(3, 3);
    public Vector2 targetDirection;
    public Vector2 targetCharacterDirection;



    void Start() {
        // Set target direction to the camera's initial orientation.
        targetDirection = transform.localRotation.eulerAngles;
        smoothedCameraForward = 0f;
        smoothedCameraStrafe = 0f;
        smoothedCameraVertical = 0f;
    }
	
	void Update () {
        MoveUpdate();
        LookUpdate();
	}

    private void MoveUpdate() {
        //calculate smoothed movement
        smoothedCameraForward = Mathf.Lerp(smoothedCameraForward, Input.GetAxisRaw("Camera Forward"), cameraSmoothing);
        smoothedCameraStrafe = Mathf.Lerp(smoothedCameraStrafe, Input.GetAxisRaw("Camera Strafe"), cameraSmoothing);
        smoothedCameraVertical = Mathf.Lerp(smoothedCameraVertical, Input.GetAxisRaw("Camera Vertical"), cameraSmoothing);
        //project x and z movement onto world plane, then apply
        transform.Translate(Vector3.ProjectOnPlane(transform.TransformDirection(new Vector3(smoothedCameraStrafe, 0f, 0f)), Vector3.up).normalized * Mathf.Abs(smoothedCameraStrafe), Space.World);
        transform.Translate(Vector3.ProjectOnPlane(transform.TransformDirection(new Vector3(0f, 0f, smoothedCameraForward)), Vector3.up).normalized * Mathf.Abs(smoothedCameraForward), Space.World);
       // Vector3 translationVector = /*transform.TransformDirection(*/new Vector3(smoothedCameraStrafe, 0f, smoothedCameraForward);//);
       // transform.Translate(translationVector * cameraMoveSpeed, Space.World);
        //apply  y translation, clamped to min/max
        transform.Translate(new Vector3(0f, smoothedCameraVertical * cameraMoveSpeed),Space.World);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y,minHeight,maxHeight), transform.position.z);
    }
    private void LookUpdate()
	{
		// Ensure the cursor is always locked when set
		if (lockCursor) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		// Allow the script to clamp based on a desired target value.
		var targetOrientation = Quaternion.Euler(targetDirection);
		
		// Get raw mouse input for a cleaner reading on more sensitive mice.
		var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
		
		// Scale input against the sensitivity setting and multiply that against the smoothing value.
		mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));
		
		// Interpolate mouse movement over time to apply smoothing delta.
		_smoothMouse.x = Mathf.Lerp(_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
		_smoothMouse.y = Mathf.Lerp(_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);
		
		// Find the absolute mouse movement value from point zero.
		_mouseAbsolute += _smoothMouse;
		
		// Clamp and apply the local x value first, so as not to be affected by world transforms.
		if (clampInDegrees.x < 360)
			_mouseAbsolute.x = Mathf.Clamp(_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);
		
		var xRotation = Quaternion.AngleAxis(-_mouseAbsolute.y, targetOrientation * Vector3.right);
		transform.localRotation = xRotation;
		
		// Then clamp and apply the global y value.
		if (clampInDegrees.y < 360)
			_mouseAbsolute.y = Mathf.Clamp(_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);
		
		transform.localRotation *= targetOrientation;
		
		var yRotation = Quaternion.AngleAxis(_mouseAbsolute.x, transform.InverseTransformDirection(Vector3.up));
		transform.localRotation *= yRotation;
	}
}
