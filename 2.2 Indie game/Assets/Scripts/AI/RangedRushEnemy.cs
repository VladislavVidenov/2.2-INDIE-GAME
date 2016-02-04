using UnityEngine;
using System.Collections;

public class RangedRushEnemy : EnemyScript {

   // public delegate void HitPlayer();
   // public static event HitPlayer OnHitPlayer;

    Vector3 fightingPosition;
    float attackTimer;
    float changeCoverTimer;
    float playerNotSeenTimer;
   
    [SerializeField]
    float attackRotationSpeed = 5f;

    CoverSpotScript[] coverSpots;

    CoverSpotScript coverSpot;
    CoverSpotScript previousSpot;


    float shortestLength = 0;

    bool crouching = false;
    bool doAction = false;

    bool inDanger = false;

    //try
    float eyeLightIntensity = 4f;
    Light eyeLight;

    void Awake() {
        eyeLight = GetComponentInChildren<Light>();
        eyeLight.intensity = 0f;
    }

    // Use this for initialization
    override public void Start() {
        base.Start(); 
        coverSpots = GameManager.Instance.coverSpots;
      
		Invoke ("StartFind", 0.1f);
    }
	void StartFind () {
		state = AIState.HuntEnemy;
	}

    void Update() {

        float velocity = agent.velocity.magnitude;
        if (velocity > 0.1f) {
            myAnimator.SetBool("Moving", true);
        }
        else {
            myAnimator.SetBool("Moving", false);
        }

        if (health < 30 && !inDanger) {
            inDanger = true;
            state = AIState.FindCover;
        }

        switch (state) {
            case AIState.HuntEnemy:
                HuntEnemy();
                break;
            case AIState.Shooting:
                Shooting();
                break;
            case AIState.FindCover:
                FindCover();
                break;
            case AIState.InCover:
                InCover();
                break;
            case AIState.MovingToCover:
                MovingToCover();
                break;
            //case AIState.Flank:
            //    Flank();
            //    break;
        }
    }

    void HuntEnemy() {
        agent.Resume();
        agent.SetDestination(player.position);
        if (DoISeePlayer(Color.red)) {
            Invoke("ChangeToShooting", Random.Range(.5f,1.5f));
        }
    }

    void Shooting() {
        agent.Stop();
        agent.updateRotation = false;
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(player.transform.position - this.transform.position), attackRotationSpeed);
        Shoot(3f,1f);

        if (DoISeePlayer(Color.white) == false) {
            state = AIState.HuntEnemy;
        }
    }

    void ChangeToShooting() {
        state = AIState.Shooting;
    }

    //void Flank() {
    //    agent.Resume();
    //    agent.SetDestination(player.transform.position);
       
    //    RaycastHit hit;
    //    Vector3 direction = playerHeadPos.position - enemyHeadPos.position;
    //    if (Physics.Raycast(agent.transform.position - (agent.transform.up/2), direction, out hit, 100f)) {
    //        if (hit.collider.CompareTag(Tags.player)) {
    //            state = AIState.Shooting;
    //        }
    //    }
    //}

    bool DoISeePlayer(Color color) {
        if (color == null) color = Color.red;
        RaycastHit hit;
        Vector3 direction = (playerHeadPos.position - enemyHeadPos.position).normalized;
        if (Physics.Raycast(enemyHeadPos.position, direction, out hit, 100f, layer)) {
    //        Debug.DrawRay(enemyHeadPos.position, direction * Vector3.Distance(enemyHeadPos.position, hit.point), color);

            if (hit.collider.CompareTag(Tags.player)) return true;
            else return false;
        }
        else {
            return false;
        }
    
    }

    void FindCover() {

        for (int i = 0; i < coverSpots.Length; i++) {

            if (coverSpots[i].CheckCoverSpot(player.transform.position) && coverSpots[i] != previousSpot) {

                float length = Vector3.Distance(player.transform.position, coverSpots[i].gameObject.transform.position);

                if (shortestLength == 0) {
                    shortestLength = length;
                    coverSpot = coverSpots[i];
                }
                else if (length < shortestLength) {
                    shortestLength = length;
                    coverSpot = coverSpots[i];
                }
            }
        }

        if (coverSpot != null) {

            agent.SetDestination(coverSpot.gameObject.transform.position);
            state = AIState.MovingToCover;
            coverSpot.isTaken = true;

            shortestLength = 0;
        }

    }
    
    void MovingToCover() {
	
		agent.Resume ();
        Shoot(0.1f,5f);
		agent.updateRotation = false;
		this.transform.rotation = Quaternion.RotateTowards (this.transform.rotation, Quaternion.LookRotation (player.transform.position - this.transform.position), attackRotationSpeed);
		
        if (agent.remainingDistance <= agent.stoppingDistance) {
			state = AIState.InCover;
			return;
		} else if (!coverSpot.checkIfSafe (player.transform.position)) {
			state = AIState.FindCover;
		}
	}

    void InCover() {
        agent.updateRotation = false;

        // changeCoverTimer += Time.deltaTime;

        // if (crouching) {
        if (DoISeePlayer(Color.yellow)) {

            state = AIState.Shooting;

        }
        //      if (!doAction) StartCoroutine(StopCrouching(true));
        // }
        //  else if (!crouching) {
        //      if (DoISeePlayer(Color.blue)) {
        //          playerNotSeenTimer = 0;
        //          Shoot(3f,1);
        //     } else if (!inDanger) {
        //         playerNotSeenTimer += Time.deltaTime;
        //         if (playerNotSeenTimer > 10) {
        //             state = AIState.Flank;
        //         }
        //     }

        //     if (!doAction) StartCoroutine(StartCrouching());
        // }
        agent.Stop();
    }
    



    //IEnumerator StartCrouching() {
    //    doAction = true;
    //    yield return new WaitForSeconds((float)Random.Range(2, 5));
    //    //play anim
    //    crouching = true;
    //    print("starting to crouch");
    //    doAction = false;
    //}

    //IEnumerator StopCrouching(bool wait) {
    //    doAction = true;
    //    if (wait) {
    //        yield return new WaitForSeconds((float)Random.Range(2, 5));
    //    }
    //    //play anim
    //    crouching = false;
    //    print("stopped to crouch");
    //    doAction = false;
    //}

    void Shoot(float AttackInterval, float Accuracy) {

        if (Time.time - attackTimer > AttackInterval) {
            if (DoISeePlayer(Color.black)) {

                ChargeBullet(Accuracy);
               // ShootRaycast(Accuracy);
                attackTimer = Time.time;
            }
        }
    }

    void ChargeBullet(float Accuracy) {
        myAnimator.SetTrigger("Charge");
        StartCoroutine("IShootRaycast", Accuracy);
    }



    IEnumerator IShootRaycast(float Accuracy) {
        yield return new WaitForSeconds(2);
        ShootRaycast(Accuracy);
    }

    void ShootRaycast(float Accuracy) {
        audioSource.volume = 1f;
        audioSource.PlayOneShot(enemySounds[1]);
        
        RaycastHit hit;

        Vector3 direction = playerHeadPos.position - enemyHeadPos.position;
        direction += new Vector3(Random.Range(-Accuracy, Accuracy), Random.Range(-Accuracy, Accuracy), Random.Range(-Accuracy, Accuracy));
        myAnimator.SetTrigger("Shoot");

        if (Physics.Raycast(enemyHeadPos.position , direction.normalized, out hit, 100f)) {
            Debug.DrawRay(enemyHeadPos.position, direction * Vector3.Distance(enemyHeadPos.position, hit.point), Color.green);
            if (hit.collider.CompareTag(Tags.player)) {
                eyeLight.intensity = 4f;
                Invoke("DisableEyeLight", 0.5f);
                PlayerScript ps = hit.collider.gameObject.GetComponent<PlayerScript>();
                //if (OnHitPlayer != null) { OnHitPlayer(); }
                ps.PlayGotHitSound();
                ps.TakeDamage(10);
                ps.hitter = this.gameObject;
                ps.IndicatorAlpha = 1;
            
            }
            else {
            }
        }
    }

    // SpawnBullet();

    //public GameObject bulletPrefab;
    //void SpawnBullet() {
    //    GameObject temp = Instantiate(bulletPrefab, this.transform.position, Quaternion.identity) as GameObject;
    //    Rigidbody rb = temp.GetComponent<Rigidbody>();
    //    temp.transform.LookAt(player.position);
    //    rb.AddForce(temp.transform.forward * 0.0007f);

    //    Destroy(temp, 1f);
    //}


    void DisableEyeLight() {
        eyeLight.intensity = 0f;
    }
}
