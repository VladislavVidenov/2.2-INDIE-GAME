﻿using UnityEngine;
using System.Collections;

public class CoverSpotScript : MonoBehaviour {

	public Transform player;

	public bool isTaken = false;

	public LayerMask layer;

	public bool CheckCoverSpot(Vector3 target){
		if (!checkIftaken() && checkIfSafe(target)) {
			return true;
		} else {
			return false;
		}
	}

	public bool checkIfSafe (Vector3 target) {
		RaycastHit hit;
		Vector3 direction = target - this.transform.position;

		if (Physics.Raycast (this.transform.position, direction, out hit,100f, layer)) {
			if (hit.collider.CompareTag (Tags.player)) {
				return false;
			} else {
				return true;
			}
		} else {
			return true;
		}
	}

    private bool checkIftaken() {
        return isTaken;
    }

}
