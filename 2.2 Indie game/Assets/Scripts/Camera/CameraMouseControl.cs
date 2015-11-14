using UnityEngine;
using System.Collections;

public class CameraMouseControl : MonoBehaviour {


    public enum RotateAxis { X , Y };
    public RotateAxis axis;



    //Components
    Camera cam;
    Quaternion rotation;

    //Variables
    float rotationX;
    float rotationY;

    //Maybe let the user select his sensitivity in the controls ? :P
    [SerializeField, Range(0,5)]
    float camSensitivity;

    // Min and max rotation, so we cant do a 360 rotation with Y axis  :)
    [SerializeField]
    float minRotationX;
    [SerializeField]
    float maxRotationX;
    [SerializeField]
    float minRotationY;
    [SerializeField]
    float maxRotationY;


	void Start () {
        cam = Camera.main;
        rotation = transform.localRotation;
	}
	

	void Update() {
        //Applying rotation with X only on rootPlayerGameObject.

        
        if(axis == RotateAxis.X)
        {
            rotationX += (Input.GetAxis("Mouse X") * camSensitivity / 60 * cam.fieldOfView);

            rotationX = FixAngle(rotationX, minRotationX, maxRotationX);
     
            Quaternion xRotation = Quaternion.AngleAxis(rotationX, Vector3.up);


            transform.localRotation = rotation * xRotation;
        }
        //Applying rotation with Y only on mainCamera. If we rotate the rootPlayerGameObject with Y .. well...
        // collider gets rotated sooo if we look up => colliders is laying on ground, not standing ;P.
        else if (axis == RotateAxis.Y)
        {
            rotationY += (Input.GetAxis("Mouse Y") * camSensitivity / 60 * cam.fieldOfView);
            rotationY = FixAngle(rotationY, minRotationY, maxRotationY);
            Quaternion yRotation = Quaternion.AngleAxis(rotationY, Vector3.left);
            transform.localRotation = rotation * yRotation;
        }
    

	}

    //a reminder of game physics. Tank barrel? :P
    static float FixAngle(float angle, float minAngle, float maxAngle)
    {
        if (angle < -360f)
            angle += 360f;
        if (angle > 360f)
            angle -= 360f;
        return Mathf.Clamp(angle, minAngle, maxAngle);
    }
}
