using UnityEngine;
using System.Collections;

public class TextUprighter : MonoBehaviour {

	// Use this for initialization
	void Start () {
     //   transform.localScale = new Vector3(1 / transform.parent.localScale.x, 1 / transform.parent.localScale.y, 1 / transform.parent.localScale.z);
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = Quaternion.LookRotation((Camera.main.transform.position - transform.position).normalized*-1f, Camera.main.transform.up);
	}
}
