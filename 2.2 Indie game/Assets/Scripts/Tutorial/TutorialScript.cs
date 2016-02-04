using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour {
    //TASK
    public enum Task {None, Start, Walk, Jump, Sprint, Crouch, AimDownSight, Shoot, Reload, KillEnemy, BuyUpgrade, Finished
    }
    Task task;

    //HUD
    [SerializeField]
    GameObject tutorialImage;
    GameObject screen;
    Text text;
    [SerializeField]
    float waitTime;
   
   //player
    GameObject player;


    //killenemy
    [SerializeField]
    GameObject meleeEnemy;
    [SerializeField]
    GameObject spawnPos;
    GameObject go;

    //Booleans

    bool playerShot = false;
    bool enterShop = false;

    bool playerReload = false;
    [HideInInspector]
    public bool hasWalked = false;
    [HideInInspector]
    public bool hasCrouched = false;
    [HideInInspector]
    public bool hasJumped = false;
    [HideInInspector]
    public bool finishedTutorial = false;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag(Tags.player);
        text = tutorialImage.GetComponentInChildren<Text>();
        screen = GameObject.Find("screen");
        tutorialImage.gameObject.SetActive(false);
        StartTutorial();
	}
	
	// Update is called once per frame
	void Update () {
        switch (task) {

            case Task.Finished:
                finishedTutorial = true;
                tutorialImage.gameObject.SetActive(true);
                text.text = "";
                screen.transform.Translate(screen.transform.up);
                if (tutorialImage != null) {
                    Destroy(tutorialImage, 5f);
                }
                //Destroy(this.gameObject,5f);

               
                break;
            case Task.Walk:
                tutorialImage.gameObject.SetActive(true);
			text.text = "B.R.A.I.N: Use WASD to move!";
                if (hasWalked) {
				StartCoroutine(NextTask("B.R.A.I.N: Lets hurry up!", waitTime, Task.Sprint));
                }
                break;
            case Task.Sprint:
                tutorialImage.gameObject.SetActive(true);
			text.text = "B.R.A.I.N: Press Shift to SPRINT!";
                if (player.GetComponent<PlayerMovement>().isRunning) {
				StartCoroutine(NextTask("B.R.A.I.N: You have to crouch to continue", waitTime, Task.Crouch));
                }
                break;
            case Task.Crouch:
                tutorialImage.gameObject.SetActive(true);
			text.text = "B.R.A.I.N: Press C to crouch!";
                if (hasCrouched) {
				StartCoroutine(NextTask("B.R.A.I.N: There is a gap, Jump over it!", waitTime, Task.Jump));
                }
                break;

            case Task.Jump:
                tutorialImage.gameObject.SetActive(true);
			text.text = "B.R.A.I.N: Press Space To Jump!";
                if (hasJumped) {
				StartCoroutine(NextTask("B.R.A.I.N: Lets try aiming your weapon!", waitTime, Task.AimDownSight));
                }
                break;

            case Task.AimDownSight:
                tutorialImage.gameObject.SetActive(true);
			text.text = "B.R.A.I.N: Use RMB to aim!";
                if (player.GetComponent<PlayerMovement>().isWepAiming) {
				StartCoroutine(NextTask("B.R.A.I.N: Lets try shooting!", waitTime, Task.Shoot));
                }
                break;
            case Task.Shoot:
                tutorialImage.gameObject.SetActive(true);
			text.text = "B.R.A.I.N: Use LMB to shoot!";

                if (playerShot) {
				StartCoroutine(NextTask("B.R.A.I.N: You need to reload your gun!", waitTime, Task.Reload));
                }
                break;

            case Task.Reload:
                tutorialImage.gameObject.SetActive(true);
                text.text = "B.R.A.I.N: Use R to reload!";

                if (playerReload) {
                    StartCoroutine(NextTask("B.R.A.I.N: I'll make a dummy virus for you to shoot at!", waitTime, Task.KillEnemy));
                }
                break;

            case Task.KillEnemy:
                tutorialImage.gameObject.SetActive(true);
			text.text = "B.R.A.I.N: Shoot the virus untill it is destroyed!";

                if (go == null) {
				StartCoroutine(NextTask("B.R.A.I.N: Next I will show you how you can use bits to become stronger!", waitTime, Task.BuyUpgrade));
                }
                break;

            case Task.BuyUpgrade:
                tutorialImage.gameObject.SetActive(true);
			text.text = "B.R.A.I.N: Interact with the vendor machine and buy an upgrade!!!";
                break;
        }
	
	}

    void ExitTutorial() {
        tutorialImage.gameObject.SetActive(false);
    }

    void StartTutorial() {
        tutorialImage.gameObject.SetActive(true);
        StartCoroutine(NextTask("B.R.A.I.N: Thanks for helping me, first let me show you how to walk", waitTime, Task.Walk));
    }

    void SetShot() {
        if (task == Task.Shoot) playerShot = true;
    }

    void SetReload() {
        if (task == Task.Reload) playerReload = true;
    }

    void BuyUpgrade() {
        if (task == Task.BuyUpgrade) StartCoroutine(NextTask("Your now ready to go, head over to the desk to proceed", waitTime, Task.Finished));
        print("boughtUpgrade");
    }
    void OnEnable() {
        WeaponScript.OnPistolShoot += SetShot;
        WeaponScript.OnPistolReload += SetReload;
        VendingMachine.OnBuysUpgrade += BuyUpgrade;

    }
    void OnDisable() {
        WeaponScript.OnPistolShoot -= SetShot;
        WeaponScript.OnPistolReload -= SetReload;
        VendingMachine.OnBuysUpgrade -= BuyUpgrade;
    }

    IEnumerator NextTask(string TextToShow,float WaitTime, Task pTask) {
        task = Task.None;
        //text.text = TextToShow;
        StartCoroutine(AnimateText(TextToShow));
        yield return new WaitForSeconds(WaitTime);
        if (pTask == Task.KillEnemy) {
            go = GameObject.Instantiate(meleeEnemy, spawnPos.transform.position,spawnPos.transform.rotation) as GameObject;
        }
        task = pTask;
    }


    IEnumerator AnimateText(string strComplete) {
        int i = 0;
        text.text = "";
        while (i < strComplete.Length) {
            text.text += strComplete[i++];
            yield return new WaitForSeconds(0.01F);
        }
    }
}
