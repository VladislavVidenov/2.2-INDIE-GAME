using UnityEngine;
using System.Collections;

public class RangedEnemyScript : EnemyScript {
    
    Vector3 fightingPosition;
    float attackTimer;
    float changeCoverTimer;
    [SerializeField] float attackTime = 2f;
    [SerializeField] float attackRotationSpeed = 5f;

    CoverSpotScript[] coverSpots;

	CoverSpotScript coverSpot;
    CoverSpotScript previousSpot;


    float shortestLength  = 0;

    bool crouching = false;
    bool doAction = false;


    void Start() {
		base.Start ();
        state = AIState.FindCover;
        coverSpots = GameManager.Instance.coverSpots;
        
    }

    void Update() {

        Debug.Log(state);
        switch (state) {
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
        }
    }


    void Shooting() {
        agent.Stop();
        agent.updateRotation = false;
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(player.transform.position - this.transform.position), attackRotationSpeed);
        Shoot();

        if (Vector3.Distance(agent.transform.position, player.transform.position) > 15) {
            state = AIState.FindCover;
        }
    }

    void FindCover() {
       // coverSpot = null;

        for (int i = 0; i < coverSpots.Length; i++) {
            if (coverSpots[i].CheckCoverSpot(player.transform.position) && coverSpots[i] != previousSpot) {
                float length = CalculatePathLenght(coverSpots[i].transform.position);
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
            coverSpot.isTaken = true;
            state = AIState.MovingToCover;
            
            shortestLength = 0;
        }
        else {
            state = AIState.Shooting;
        }

    }

    float CalculatePathLenght(Vector3 targetPos) {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(targetPos, path);
        Vector3[] waypoints = new Vector3[path.corners.Length + 2];
        waypoints[0] = agent.transform.position;
        waypoints[waypoints.Length - 1] = targetPos;

        for (int i = 0; i < path.corners.Length; i++) {
            waypoints[i + 1] = path.corners[i];
        }

        float pathLength = 0;

        for (int i = 0; i < waypoints.Length - 1; i++) {
            pathLength += Vector3.Distance(waypoints[i], waypoints[i + 1]);
        }

        return pathLength;
    }

    void MovingToCover() {
        agent.Resume();
        agent.updateRotation = false;
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(player.transform.position - this.transform.position), attackRotationSpeed);
        if (agent.remainingDistance <= agent.stoppingDistance) {
            state = AIState.InCover;
           
            return;
        }
        else if (!coverSpot.checkIfSafe(player.transform.position)) {
            state = AIState.FindCover;
        }

      
    }

    void InCover() {
        changeCoverTimer += Time.deltaTime;
        agent.updateRotation = false;
        if (!crouching) {
            this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(player.transform.position - this.transform.position), attackRotationSpeed);
            Shoot();
            if (!doAction) StartCoroutine(StartCrouching());      
        }
        else if (crouching) {
            if (!doAction) StartCoroutine(StopCrouching(true));
        }

        //RaycastHit hit;
        //Vector3 direction = agent.transform.position - player.transform.position;
        //Debug.DrawRay(player.transform.position - player.transform.up, direction * 100f, Color.cyan);
        //if (Physics.Raycast(player.transform.position - player.transform.up, direction, out hit, 100f)) {
        //    if (hit.collider.CompareTag(Tags.enemy)) {
        //        state = AIState.Shooting;

        //    }
        //} 

        agent.Stop();
    }



    IEnumerator StartCrouching() {
        doAction = true;
        yield return new WaitForSeconds((float)Random.Range(2, 5));
        //play anim
        crouching = true;
        print("starting to crouch");
        doAction = false;
    }

    IEnumerator StopCrouching(bool wait) {
        doAction = true;
        if (wait) {
            yield return new WaitForSeconds((float)Random.Range(2, 5));
        }
        //play anim
        crouching = false;
        print("stopped to crouch");
        doAction = false;
    }

    void Shoot() {
        Vector3 direction = player.transform.position - this.transform.position;
        direction += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        if (Time.time - attackTimer > attackTime) {
            ShootRaycast(direction);
            attackTimer = Time.time;
        }
    }

    void ShootRaycast(Vector3 direction) {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + transform.up / 3, direction.normalized, out hit, 100f)) {
            Debug.DrawRay(transform.position + transform.up / 3, direction, Color.red);
            if (hit.collider.CompareTag(Tags.player)) {
                Debug.Log("i shot u");
            }
            else {
                Debug.Log("i missed u");
            }
        }
    }

}
