using UnityEngine;
using System.Collections;

public class MeleeEnemyScript : EnemyScript {


	public delegate void RobotCharging();
	public delegate void RobotStopCharging();
	public static event RobotCharging OnRobotCharging;
	public static event RobotStopCharging OnRobotStopCharging;


	[Header("MeleeStats")]
    [SerializeField]
    float chargeSpeed;
    [SerializeField]
    float chargeAcceleration;
	[SerializeField]
	float chargeRange;

	public float attackRate;
	[HideInInspector]
	public float nextAttackTime;

	//Charge
	bool charged = false;
	float robotPlayerDelta;
	//Light
	Light eyeLight;

   

	override public void Start () {
		base.Start ();
		eyeLight = GetComponentInChildren<Light> ();


		state = AIState.Charge;
		if(OnRobotCharging != null) 
			OnRobotCharging();
	}
	
	// Update is called once per frame
	void Update () {
		SetMovingAnimation ();
		
	  switch (state) {
		case AIState.Attacking:
			Attacking ();
			break;

		case AIState.Running:
			robotPlayerDelta = Vector3.Distance (agent.transform.position, player.transform.position);
			if(robotPlayerDelta < agent.stoppingDistance + 3.0f) 
				this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(player.transform.position - this.transform.position), 5);

			agent.SetDestination (player.position);
			break;

		case AIState.Charge:
            if (!audioSource.isPlaying) {
                audioSource.volume = 1;
                audioSource.PlayOneShot(enemySounds[2]);
            }
			agent.SetDestination (player.position);
			robotPlayerDelta = Vector3.Distance (agent.transform.position, player.transform.position);
			if (robotPlayerDelta < chargeRange) {
				Charge();
			}
			break;
		}
	
	}
	void SetMovingAnimation()
	{
		float velocity = agent.velocity.magnitude;
		if (velocity > 0.1f) 
			myAnimator.SetBool("Moving",true);
		 else 
			myAnimator.SetBool("Moving",false);

	}
	void Attacking () {

        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(player.transform.position - this.transform.position), 5);
		if (CanFire ())
			MeleeAttack ();
	}

	void MeleeAttack () {
		int random = Random.Range (0,2);
		if(random == 1)
			myAnimator.SetTrigger ("Punch");
		else
			myAnimator.SetTrigger("Spin");

		eyeLight.intensity = 4f;
		Invoke ("DisableEyeLight", 0.8f);
		Invoke ("Hit", 0.3f);
	}
	void Hit(){
		Debug.Log ("NEW HIT Hit");
		player.GetComponent<PlayerScript> ().TakeDamage (damage);
	}
	void DisableEyeLight() {
		eyeLight.intensity = 0f;
	}

	void Charge () {
		//For smooth animation, not setting the trigger dozen of times.
		if (!charged) {
			myAnimator.SetTrigger ("Charge");
			charged = true;
		}
		agent.acceleration = chargeAcceleration;
		agent.speed = chargeSpeed;
		//Stop charging when near player
		if (robotPlayerDelta < agent.stoppingDistance + 2f) {
			if(OnRobotStopCharging != null) OnRobotStopCharging();
			charged = false;
			agent.acceleration = defaultAcceleration;
			agent.speed = defaultSpeed;
		}
		
	}

	void OnTriggerEnter (Collider other) {
		if (other.CompareTag (Tags.player)) {
			state = AIState.Attacking;
		}
	}
	
	void OnTriggerExit (Collider other) {
		if (other.CompareTag (Tags.player)) {
			state = AIState.Running;
		}
	}

     bool CanFire()
	{
		if (Time.time - attackRate > nextAttackTime)
			nextAttackTime = Time.time - Time.deltaTime;
		
		while (nextAttackTime < Time.time)
		{
			nextAttackTime = Time.time + attackRate;
			return true;
		}
		return false;
	}


}
