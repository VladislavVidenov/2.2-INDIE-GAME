using UnityEngine;
using System.Collections;

public class UpJumpHeight : Upgrade {
    PlayerMovement playerMov;
    public float jumpHeightMultiplier = 3.0f; //set in inspector!

    public override void Apply() {
        print(string.Format("UPGRADE UNLOCKED: '{0}'", upgradeName));
        playerMov = FindObjectOfType<PlayerMovement>();
        playerMov.heightToJump = (playerMov.heightToJump * jumpHeightMultiplier);
    }
}
