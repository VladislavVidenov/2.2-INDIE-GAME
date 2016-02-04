using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class DialogScript : MonoBehaviour {


    //HUD
    [SerializeField]
    GameObject dialogGameObject;
    GameObject screen;
    Text text;
    [SerializeField]
    float waitTime;

    bool canTalk = false;

    [SerializeField]
    SpawnEventScript spawnEvent;

    // Use this for initialization
    void Start () {
        text = dialogGameObject.GetComponentInChildren<Text>();
        screen = GameObject.Find("screen");
        dialogGameObject.gameObject.SetActive(true);
        StartDialog();

    }

    void StartDialog()
    {
        canTalk = false;
        StartCoroutine(DialogTalk());
        
    }
	
	// Update is called once per frame
	void Update () {
        if (canTalk)
        {
            text.text = "Head to the desk to upload the files";
            
            if (spawnEvent.hasSpawned && !spawnEvent.completed)
            {
                text.text = "";
                dialogGameObject.SetActive(false);
            }
            else if (spawnEvent.hasSpawned && spawnEvent.completed)
            {
                dialogGameObject.SetActive(true);
                StartCoroutine(EndTalk());
                canTalk = false;
            }
        }
	
	}
    IEnumerator DialogTalk ()
    {
        yield return new WaitForSeconds(waitTime);
        StartCoroutine( AnimateText("Welcome to the 2nd core"));
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(AnimateText("In order to get to the first core we have to upload the anti-virus here first"));
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(AnimateText("When you are ready, head to the desk to upload the files"));
        yield return new WaitForSeconds(waitTime);
        spawnEvent.allowedToSpawn = true;
        canTalk = true;
    }

    IEnumerator EndTalk()
    {
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(AnimateText("GJ"));
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(AnimateText("Thanks for doing this"));
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(AnimateText("Now we can go to 1st core, head back to the desk"));
        yield return new WaitForSeconds(waitTime);
        spawnEvent.gameObject.tag = "LevelSwitcher";


    }

    IEnumerator AnimateText(string strComplete)
    {
        int i = 0;
        text.text = "";
        while (i < strComplete.Length)
        {
            text.text += strComplete[i++];
            yield return new WaitForSeconds(0.01F);
        }
    }
}
