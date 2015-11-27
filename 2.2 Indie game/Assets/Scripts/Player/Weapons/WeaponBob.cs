using UnityEngine;
using System.Collections;

public class WeaponBob : MonoBehaviour {
    [SerializeField, Range(0.01f,0.03f)]
    float delay;
    [SerializeField, Range(0.1f,0.5f)]
    float smoothSpeed;

    float minDelay;
    float maxDelay;
    private float delayX;
    private float delayY;
    private Vector3 localPos;
    private Vector3 vel = Vector3.zero; //ref for smoothdamp.
    // Use this for initialization
    void Start()
    {
        maxDelay = delay + 0.001f;
        minDelay = -maxDelay;
        localPos = transform.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {                       //negative input, because we add the delay to our position
        delayX = ClampDelay(-Input.GetAxis("Mouse X") * delay, minDelay, maxDelay);
                            //same.
        delayY = ClampDelay(-Input.GetAxis("Mouse Y") * delay, minDelay, maxDelay);

        Vector3 newPos = new Vector3(localPos.x + delayX, localPos.y + delayY, localPos.z);
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, newPos, ref vel, smoothSpeed);

    }
    float ClampDelay(float value,float min,float max)
    {
        if (value > max)
            value = max;
        if (value < min)
            value = min;
        return value;
    }
}
