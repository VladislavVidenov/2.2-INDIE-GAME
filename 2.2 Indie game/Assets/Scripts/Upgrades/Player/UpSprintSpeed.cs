using UnityEngine;
using System.Collections;

public class UpSprintSpeed : Upgrade {
    PlayerMovement playerMov;
    public float speedMultiplier = 1.25f; //set in inspector!

    public override void Apply() {
        print(string.Format("UPGRADE UNLOCKED: '{0}'", upgradeName));
        playerMov = FindObjectOfType<PlayerMovement>();
        playerMov.runSpeed = (playerMov.runSpeed * speedMultiplier);
        print("Run speed now equals " + playerMov.runSpeed);
    }
}
