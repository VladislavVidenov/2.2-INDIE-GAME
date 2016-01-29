using UnityEngine;
using System.Collections;

public class UpSprintSpeed : Upgrade {


    public override void Apply() {
        print(string.Format("UPGRADE UNLOCKED: '{0}'", upgradeName));

    }
}
