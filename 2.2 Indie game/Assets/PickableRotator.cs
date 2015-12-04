using UnityEngine;
using System.Collections;

public class PickableRotator : MonoBehaviour {
	// Update is called once per frame
	void Update () {
        Vector3 rotation = new Vector3(35, 45, 0);
        transform.Rotate(rotation * Time.deltaTime);
	}
}
