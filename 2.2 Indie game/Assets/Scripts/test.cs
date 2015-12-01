using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {
    public LayerMask layer;
	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log(Check());
        }
	}


    bool Check()
    {
        if (Physics.CheckSphere(transform.position, 1, layer))
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
