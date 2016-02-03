using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour {
    public enum Task {None, STart, Walk, Jump, Sprint, Crouch, AimDownSight, Shoot, EnterShop, KillEnemy, BuyUpgrade, Finished
    }
    [SerializeField]
    GameObject tutorialImage;
    GameObject screen;
    Text text;
    bool playerShot = false;
    bool enterShop = false;
    Task task;
    GameObject player;

    [SerializeField]
    GameObject meleeEnemy;
    [SerializeField]
    GameObject spawnPos;

    GameObject go;
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
                tutorialImage.gameObject.SetActive(true);
                text.text = "";
                screen.transform.Translate(screen.transform.up);
                Destroy(tutorialImage, 5f);
                Destroy(this.gameObject,5f);
               // Invoke("ExitTutorial", 5f);

               
                break;
            case Task.Walk:
                tutorialImage.gameObject.SetActive(true);
                text.text = "WASD To walk!";
                if (player.GetComponent<PlayerMovement>().isWalking()) {
                    StartCoroutine(NextTask("Now let us move on to sprinting", 3f, Task.Sprint));
                }
                break;
            case Task.Sprint:
                tutorialImage.gameObject.SetActive(true);
                text.text = "Shift To SPRINT!";
                if (player.GetComponent<PlayerMovement>().isRunning) {
                    StartCoroutine(NextTask("And now crouching", 3f, Task.Crouch));
                }
                break;
            case Task.Crouch:
                tutorialImage.gameObject.SetActive(true);
                text.text = "Press C to crouch!";
                if (player.GetComponent<PlayerMovement>().isCrouching) {
                    StartCoroutine(NextTask("let me show how you weapon works, right mouse click to  aim", 3f, Task.AimDownSight));
                }
                break;
            case Task.AimDownSight:
                tutorialImage.gameObject.SetActive(true);
                text.text = "left Mouse to Aim Down Sight!";
                if (player.GetComponent<PlayerMovement>().isWepAiming) {
                    StartCoroutine(NextTask("And left Mouse to shoot", 3f, Task.Shoot));
                }
                break;
            case Task.Shoot:
                tutorialImage.gameObject.SetActive(true);
                text.text = "Right Mouse to Shoot!";

                if (playerShot) {
                    StartCoroutine(NextTask("Lets kill an enemy, be casrefull", 3f, Task.KillEnemy));
                }
                break;

            case Task.KillEnemy:
                tutorialImage.gameObject.SetActive(true);
                text.text = "Kill the Enemy!";

                if (go == null) {
                    StartCoroutine(NextTask("lets go to the shop", 3f, Task.EnterShop));
                }
                break;
            case Task.EnterShop:
                tutorialImage.gameObject.SetActive(true);
                text.text = "Enter the Shop!";
                break;

            case Task.BuyUpgrade:
                tutorialImage.gameObject.SetActive(true);
                text.text = "Buy An Upgrade!!";
                break;
        }
	
	}

    void ExitTutorial() {
        tutorialImage.gameObject.SetActive(false);
    }

    void StartTutorial() {
        tutorialImage.gameObject.SetActive(true);
        StartCoroutine(NextTask("Thanks for helping me, first let me show you how to walk",3f,Task.Walk));
    }

    void SetShot() {
        if (task == Task.Shoot) playerShot = true;
    }
    void SetShop() {
        if (task == Task.EnterShop) StartCoroutine(NextTask("Now buy an upgrade", 3f, Task.BuyUpgrade));
        print("entered shop");
    }

    void BuyUpgrade() {
        if (task == Task.BuyUpgrade) StartCoroutine(NextTask("Your now ready to go", 3f, Task.Finished));
        print("boughtUpgrade");
    }
    void OnEnable() {
        WeaponScript.OnPistolShoot += SetShot;
        VendingMachine.OnEnterShop += SetShop;
        VendingMachine.OnBuysUpgrade += BuyUpgrade;

    }
    void OnDisable() {
        WeaponScript.OnPistolShoot -= SetShot;
        VendingMachine.OnEnterShop -= SetShop;
        VendingMachine.OnBuysUpgrade -= BuyUpgrade;
    }

    IEnumerator NextTask(string TextToShow,float WaitTime, Task pTask) {
        task = Task.None;
        text.text = TextToShow;
        yield return new WaitForSeconds(WaitTime);
        if (pTask == Task.KillEnemy) {
            go = GameObject.Instantiate(meleeEnemy, spawnPos.transform.position, Quaternion.identity) as GameObject;
        }
        task = pTask;
    }
}
