using UnityEngine;
using System.Collections;

public class DegreeMarkerDisplay : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = transform.parent.position + (Camera.main.transform.position - transform.parent.position).normalized * (transform.parent.gameObject.GetComponent<Renderer>().bounds.ClosestPoint(Camera.main.transform.position) - transform.parent.position).magnitude;
	}
}
