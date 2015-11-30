using UnityEngine;
using System.Collections;

public class CoverSpotScript : MonoBehaviour {


	public Transform player;

	public bool isSafe = false;
	public bool isTaken = false;

	public LayerMask layer;

	public bool CheckCoverSpot(Vector3 target){
		if (!checkIftaken() && checkIfSafe(target)) {
			return true;
		} else {
			return false;
		}
	}
	void Update(){
		if (Input.GetKeyDown (KeyCode.H)) {

			Debug.Log(checkIftaken()) ;
		}
	}

	public bool checkIfSafe (Vector3 target) {
		RaycastHit hit;
		Vector3 direction = target - this.transform.position;

		if (Physics.Raycast (this.transform.position, direction, out hit)) {
			if (hit.collider.CompareTag (Tags.player)) {
				return false;
			} else {
				return true;
			}
		} else {
			return true;
		}
	}

	private bool checkIftaken()
	{
        return isTaken;
        //if (Physics.CheckSphere(transform.position, 0.1f, layer))
        //{
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
		
	}

}
