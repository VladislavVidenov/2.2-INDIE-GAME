using UnityEngine;
using System.Collections;

public class MenuCameraControl : MonoBehaviour {

    public Transform currentMount;
    public float cameraLerpSpeed = 0.1f;

    void Update() {
        transform.position = Vector3.Lerp(transform.position, currentMount.position, cameraLerpSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, currentMount.rotation, cameraLerpSpeed);
    }

    public void SetMount(Transform newMount) {
        currentMount = newMount;
    }

}
