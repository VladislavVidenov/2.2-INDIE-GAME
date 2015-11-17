using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
 
    


    //Components
    Rigidbody rigidBody;

    //Variables
    [SerializeField]
    float walkSpeed;
    [SerializeField]
    float crouchSpeed;

    bool grounded = false;
    bool canJump = true;
    float speed = 10;
    float maxVelocityClamp = 10f;

    Vector3 input;
    Vector3 targetVel;

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        input = new Vector3(Input.GetAxis("Horizontal"),0,Input.GetAxis("Vertical"));
        input *= (Mathf.Abs(input.x) == 1 && Mathf.Abs(input.z) == 1) ? 0.707f : 1.0f;

        if (grounded)
        {

            targetVel = input;
            targetVel = transform.TransformDirection(targetVel);
            targetVel *= speed;

            Vector3 currVel = rigidBody.velocity;
            Vector3 newVel = targetVel - currVel;

            newVel.x = Mathf.Clamp(newVel.x, -maxVelocityClamp, maxVelocityClamp);
            newVel.y = 0;
            newVel.z = Mathf.Clamp(newVel.z, -maxVelocityClamp, maxVelocityClamp);

            rigidBody.AddForce(newVel, ForceMode.VelocityChange);

        }



        grounded = false;
    }



    void OnCollisionStay(Collision collision)
    {
        foreach(ContactPoint contact in collision.contacts)
        {
            
            if (Vector3.Angle(contact.normal, Vector3.up) < 45)
            {
                grounded = true;
            }
        }
    }

}
