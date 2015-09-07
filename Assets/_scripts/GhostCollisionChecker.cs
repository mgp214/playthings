using UnityEngine;
using System.Collections;

public class GhostCollisionChecker : MonoBehaviour {
    public bool placementBlocked;

    void Start() {
        placementBlocked = false;
    }
    void OnTriggerEnter(Collider collider) {
        placementBlocked = true;
        foreach (Material mat in GetComponent<Renderer>().materials) {
            mat.color = Color.red;
        }
    }
    void OnTriggerStay(Collider collider) {
        if (!placementBlocked) {
            placementBlocked = true;
            foreach (Material mat in GetComponent<Renderer>().materials) {
                mat.color = Color.red;
            }
        }
    }
    void OnTriggerExit(Collider colliders) {
        placementBlocked = false;
        foreach (Material mat in GetComponent<Renderer>().materials) {
            mat.color = Color.white;
        }
    }
}
