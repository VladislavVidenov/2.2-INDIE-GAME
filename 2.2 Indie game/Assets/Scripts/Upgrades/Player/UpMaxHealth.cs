using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpMaxHealth : Upgrade {
    

	public override void Apply ()
	{
        Debug.Log ("MaxHealthUpgrade");
        PlayerScript player = FindObjectOfType<PlayerScript>();
        player.IncreasePlayerStats(20, 20, 0, 0);
        Debug.Log("Max Health Got Upgraded!");
    }
}
