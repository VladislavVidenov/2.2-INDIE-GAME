using UnityEngine;
using System.Collections;

public class RecoilStabilizer : MonoBehaviour
{
    float speed = 2;
    void Update()
    {
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, Time.deltaTime * speed);
    }
}
	
