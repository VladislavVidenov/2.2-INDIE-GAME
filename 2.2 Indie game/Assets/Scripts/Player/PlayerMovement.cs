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
    [HideInInspector]
    public Rigidbody rigidBody;
    CapsuleCollider bodyCollider;
    GameObject mainCamera;
    GameObject weaponCamera;
    GameObject recoilEffectGO;
    [SerializeField]
    Animation playerAnimation;
    PlayerScript player; //used for stamina

    //Variables
    float speed = 10;
    public float walkSpeed;
    public float crouchSpeed;
    public float runSpeed;
    float inAirSpeed;

    //Jumping
    public float heightToJump;  //set to public to access it with an upgrade
    float GetJumpHeight { get { return Mathf.Sqrt(heightToJump * gravity); } }

    public float sprintCost = 50; //stamina usage per second sprint
    float maxVelocityClamp = 5f;
    [SerializeField]
    float gravity;
    bool isReloading = false;
    [HideInInspector]
    public bool isWepAiming = false;
    [HideInInspector]
    public bool releasedRun = true;
    bool isGrounded = false;
    bool canJump = true;
    [HideInInspector]
    public bool isCrouching = false;
    [HideInInspector]
    public bool isRunning = false;
    Vector3 standingCamHeight = new Vector3(0, 0.4f, 0);
    Vector3 crouchingCamHeight = new Vector3(0, -0.2f, 0);

    // Use this for initialization
    void Start()
    {
        recoilEffectGO = GameObject.Find("Root");
        rigidBody = GetComponent<Rigidbody>();
        bodyCollider = GetComponent<CapsuleCollider>();
        mainCamera = Camera.main.gameObject;
        weaponCamera = GameObject.FindGameObjectWithTag(Tags.weaponCamera);
        player = GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       // Debug.Log("SPEED ->" + rigidBody.velocity.magnitude);

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        input.Normalize();


        if (isGrounded)
        {
            Vector3 targetVel;
            targetVel = StopInertia() ? Vector3.zero : input;
            targetVel *= speed;
            targetVel = transform.TransformDirection(targetVel);


            Vector3 currentVel = rigidBody.velocity;
            Vector3 newVelocity = targetVel - currentVel;

            newVelocity.x = Mathf.Clamp(newVelocity.x, -maxVelocityClamp, maxVelocityClamp);
            newVelocity.y = -0.55f;
            newVelocity.z = Mathf.Clamp(newVelocity.z, -maxVelocityClamp, maxVelocityClamp);

            rigidBody.AddForce(newVelocity, ForceMode.VelocityChange);
        }
        else
        {
            //air control
            Vector3 targetVel = Vector3.zero;
            targetVel.x = (input.x * inAirSpeed);
            targetVel.z = (input.z * inAirSpeed);
            targetVel = transform.TransformDirection(targetVel);

            Vector3 currentVel = rigidBody.velocity;
            Vector3 newVelocity = targetVel - currentVel;

            newVelocity.x = Mathf.Clamp(newVelocity.x, -maxVelocityClamp, maxVelocityClamp);
            newVelocity.y = 0;
            newVelocity.z = Mathf.Clamp(newVelocity.z, -maxVelocityClamp, maxVelocityClamp);

            rigidBody.AddForce(newVelocity, ForceMode.VelocityChange);
        }

        //apply gravity 
        rigidBody.AddForce(new Vector3(0, (-gravity * rigidBody.mass), 0));

        isGrounded = false;
    }

    //dirty way :<
    bool StopInertia()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)
            || (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W)) || ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S)))
            || (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S)) || ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W))))
            return false;
        else
            return true;
    }

    void Update()
    {
      
        if (isGrounded && !isReloading)
        {

            if (isWalking()) {
                if (isCrouching || isWepAiming)
                    playerAnimation["Walk"].speed = 0.5f;
                else
                    playerAnimation["Walk"].speed = 1;

                playerAnimation.CrossFade("Walk");
            } else
                if (isRunning) {
                player.ChangeStamina(-sprintCost * Time.deltaTime);
                player.timeNotRun = 0;
                playerAnimation.CrossFade("Run");
            } else
                playerAnimation.CrossFade("IdleBreath");
        }


        if (Input.GetKeyDown(KeyCode.C))
        {
            if (stateID == 1)
                state = PlayerStates.Crouch;
            else if (stateID == 2 && CanStandUp())
                state = PlayerStates.Stand;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W) && releasedRun && !isWepAiming && player.HasStamina())
        {
            //Debug.Log("player.hasstamina: " + player.HasStamina());
            isRunning = true;
            releasedRun = false;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.W) || !player.HasStamina())
        {
            isRunning = false;
            releasedRun = true;
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

        Jump();
    }

    void Jump()
    {
        if (isGrounded)
        {
            if (stateID == 1 && canJump)
            {

                if (isWalking())
                {
                    inAirSpeed = speed;
                }
                else if (isRunning)
                {
                    inAirSpeed = speed / 1.5f;
                }
                else
                {
                    inAirSpeed = speed / 3f;
                }

                rigidBody.velocity = new Vector3(rigidBody.velocity.x, GetJumpHeight, rigidBody.velocity.z);

            }
        }
        canJump = false;
    }

    void PlayerStand()
    {
        isCrouching = false;
        bodyCollider.height = 2.0f;
        bodyCollider.center = Vector3.zero;

        speed = isWepAiming ? crouchSpeed : isRunning ? runSpeed : walkSpeed;
        
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
        if ((Input.GetKeyDown(KeyCode.Space)|| Input.GetKeyDown(KeyCode.LeftShift)) && CanStandUp())
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
    void SetAim(bool aiming)
    {
        isWepAiming = aiming;
    }
    void SetIsReloading(bool reload)
    {
        isReloading = reload;
    }
    public bool isWalking()
    {
        if (rigidBody.velocity.magnitude > (speed - 0.6f) && rigidBody.velocity.magnitude < (runSpeed - 0.7f))
            return true;
        else
            return false;
    }

    bool CanStandUp()
    {                       //standing position half height + small amount so is above collider.
        float rayDistance = 1.07f;
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.up * rayDistance, Color.red, 5f);
        if(Physics.Raycast(transform.position,transform.up,out hit, rayDistance))
        {
            return false;
        }else
        {
            return true;
        }

    }
}
