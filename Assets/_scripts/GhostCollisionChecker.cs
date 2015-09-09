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
            mat.color = new Color(1f, 0f, 0f, 0.5f);
        }
    }
    void OnTriggerStay(Collider collider) {
        if (!placementBlocked) {
            placementBlocked = true;
            foreach (Material mat in GetComponent<Renderer>().materials) {
                mat.color = new Color(1f,0f,0f,0.5f);
            }
        }
    }
    void OnTriggerExit(Collider colliders) {
        placementBlocked = false;
        foreach (Material mat in GetComponent<Renderer>().materials) {
            mat.color = new Color(1f, 1f, 1f, 0.5f);
        }
    }
}
