using UnityEngine;
using System.Collections;

public class DeathAnimation : MonoBehaviour {


    public void Die()
    {
        this.transform.parent = null;
        GetComponentInChildren<CameraMouseControl>().enabled = false;
        GetComponentInChildren<PlayerActionScript>().enabled = false;
        gameObject.AddComponent<Rigidbody>();
        gameObject.AddComponent<SphereCollider>();
        GetComponent<SphereCollider>().radius = 0.3f;
        GetComponent<Rigidbody>().AddTorque(transform.right + transform.up);

    }
}
