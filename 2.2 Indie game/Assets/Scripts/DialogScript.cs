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
		StartCoroutine( AnimateText("B.R.A.I.N: This is the first firewall, its been corrupted, we need to upload the anti-virus!"));
        yield return new WaitForSeconds(waitTime);
		StartCoroutine(AnimateText("B.R.A.I.N: Only if we get this first fire wall back online can we continue!"));
        yield return new WaitForSeconds(waitTime);
		StartCoroutine(AnimateText("B.R.A.I.N: To start uploading interact with the desk!"));
        yield return new WaitForSeconds(waitTime);
        spawnEvent.allowedToSpawn = true;
        canTalk = true;
    }

    IEnumerator EndTalk()
    {
        yield return new WaitForSeconds(waitTime);
		StartCoroutine(AnimateText("B.R.A.I.N: Great job!"));
        yield return new WaitForSeconds(waitTime);
		StartCoroutine(AnimateText("B.R.A.I.N: Thanks for doing this"));
        yield return new WaitForSeconds(waitTime);
		StartCoroutine(AnimateText("B.R.A.I.N: Now we can proceed to the next area! Use the desk again to continue!"));
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
