using UnityEngine;
using System.Collections;

public class UpJumpHeight : Upgrade {


    public override void Apply() {
        print(string.Format("UPGRADE UNLOCKED: '{0}'", upgradeName));

    }
}
