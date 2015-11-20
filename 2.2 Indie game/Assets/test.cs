using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {
    public Transform trans;
	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody>().centerOfMass = trans.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
