using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    enum PlayerStates
    {
        Stand = 1, Crouch = 2
    }
    PlayerStates state = PlayerStates.Stand;
    int stateID { get { return state.GetHashCode(); } }

    //Components
    Rigidbody rigidBody;
    CapsuleCollider bodyCollider;
    GameObject mainCamera;
    GameObject weaponCamera;
    GameObject recoilEffectGO;


    //Variables
    [SerializeField]
    float walkSpeed;
    [SerializeField]
    float crouchSpeed;
    [SerializeField]
    float runSpeed;
    [SerializeField]
    float inAirSpeed;
    [SerializeField]
    float heightToJump;
    float GetJumpHeight { get { return Mathf.Sqrt(heightToJump * gravity); } }
    float speed = 10;
    float maxVelocityClamp = 10f;
    [SerializeField]
    float gravity;

    bool isGrounded = false;
    bool canJump = true;
    [HideInInspector]
    public bool isCrouching = false;
    Vector3 standingCamHeight = new Vector3(0, 0.4f, 0);
    Vector3 crouchingCamHeight = new Vector3(0, -0.2f, 0);

    Vector3 input;
    Vector3 targetVel;

    // Use this for initialization
    void Start()
    {
        recoilEffectGO = GameObject.Find("Root");
        rigidBody = GetComponent<Rigidbody>();
        bodyCollider = GetComponent<CapsuleCollider>();
        mainCamera = Camera.main.gameObject;
        weaponCamera = GameObject.FindGameObjectWithTag(Tags.weaponCamera);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        input *= (Mathf.Abs(input.x) == 1 && Mathf.Abs(input.z) == 1) ? 0.707f : 1.0f; //normalize input.

        if (isGrounded)
        {

            targetVel = input;
            targetVel = transform.TransformDirection(targetVel);
            targetVel *= speed;

            Vector3 currentVel = rigidBody.velocity;
            Vector3 newVel = targetVel - currentVel;

            newVel.x = Mathf.Clamp(newVel.x, -maxVelocityClamp, maxVelocityClamp);
            newVel.y = 0;
            newVel.z = Mathf.Clamp(newVel.z, -maxVelocityClamp, maxVelocityClamp);

            rigidBody.AddForce(newVel, ForceMode.VelocityChange);
            if (stateID == 1 && canJump)
            {
                rigidBody.velocity = new Vector3(currentVel.x, GetJumpHeight, currentVel.z);
            }
        }
        else
        {
            //if we are not grounded => apply in air speed.
            targetVel = input;
            targetVel = transform.TransformDirection(targetVel * inAirSpeed);
            rigidBody.AddForce(targetVel, ForceMode.VelocityChange);
        }

        //apply gravity
        rigidBody.AddForce(new Vector3(0, (-gravity * rigidBody.mass), 0));

        isGrounded = false;
        canJump = false;
    }

    void Update()
    {

        if (isGrounded) { /* play animations */ } else { /* play idle*/}


        if (Input.GetKeyDown(KeyCode.C))
        {
            if (stateID == 1)
                state = PlayerStates.Crouch;
            else if (stateID == 2)
                state = PlayerStates.Stand;
        }

        switch (state)
        {
            case PlayerStates.Stand:
                PlayerStand();
                break;
            case PlayerStates.Crouch:
                PlayerCrouch();
                break;
        }
        weaponCamera.transform.localPosition = mainCamera.transform.localPosition; //sync wep cam with main cam.
        //Debug.Log("State ->  " + state.ToString());
        //Debug.Log("Speed ->  " + speed);
        //Debug.Log("Grounded -> " + isGrounded);
    }


    void PlayerStand()
    {
        isCrouching = false;
        bodyCollider.height = 2.0f;
        bodyCollider.center = Vector3.zero;
        speed = (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W)) ? runSpeed : walkSpeed;
        if (mainCamera.transform.localPosition.y < standingCamHeight.y)
        {
            mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, standingCamHeight, Time.deltaTime * 5);
        }
        if (Input.GetKeyDown(KeyCode.Space))
            canJump = true;
    }

    void PlayerCrouch()
    {
        isCrouching = true;
        bodyCollider.height = 1.5f;
        bodyCollider.center = new Vector3(0, -0.25f, 0);
        speed = crouchSpeed;
        if (mainCamera.transform.localPosition.y > crouchingCamHeight.y)
        {
            mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, crouchingCamHeight, Time.deltaTime * 5);
        }
        if (Input.GetKeyDown(KeyCode.Space))
            state = PlayerStates.Stand;
    }

    void OnCollisionStay(Collision collision)
    {
        foreach(ContactPoint contact in collision.contacts)
        {
            
            if (Vector3.Angle(contact.normal, Vector3.up) < 45)
            {
                isGrounded = true;
            }
        }
    }

    void crouchEffect(float amount)
    {
        recoilEffectGO.transform.localRotation = Quaternion.Euler(recoilEffectGO.transform.localRotation.eulerAngles - new Vector3(amount, 0, 0));
    }

}
