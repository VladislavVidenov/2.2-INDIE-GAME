﻿using UnityEngine;
using System.Collections;

public class TutJumpScript : MonoBehaviour {

    TutorialScript tutorialScript;
    // Use this for initialization
    void Start() {
        tutorialScript = GameObject.Find("TutorialManager").GetComponent<TutorialScript>();
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Tags.player)) {
            tutorialScript.hasJumped = true;
        }
    }
}
