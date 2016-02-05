using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Dialog2Script : MonoBehaviour {
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
    void Start()
    {
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
    void Update()
    {
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
    IEnumerator DialogTalk()
    {
        yield return new WaitForSeconds(waitTime);
		StartCoroutine(AnimateText("B.R.A.I.N: This is the Core of my systems!"));
        yield return new WaitForSeconds(waitTime);
		StartCoroutine(AnimateText("B.R.A.I.N: If we manage to upload the anti-virus we will beat em for good!"));
        yield return new WaitForSeconds(waitTime);
		StartCoroutine(AnimateText("B.R.A.I.N: They are starting to tear down my back up system! Upload the anti-virus!"));
        yield return new WaitForSeconds(waitTime);
        spawnEvent.allowedToSpawn = true;
        canTalk = true;
    }

    IEnumerator EndTalk()
    {
        yield return new WaitForSeconds(waitTime);
		StartCoroutine(AnimateText("B.R.A.I.N: You did it!"));
        yield return new WaitForSeconds(waitTime);
		StartCoroutine(AnimateText("B.R.A.I.N: You defeated them and saved my program!"));
        yield return new WaitForSeconds(waitTime);
        StartCoroutine(AnimateText("B.R.A.I.N: Lets celebrate! and have some cake, oh wait the cake is a lie!"));
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
