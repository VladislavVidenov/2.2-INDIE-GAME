using UnityEngine;
using System.Collections;

public class SpawnEventScript : MonoBehaviour {
    [SerializeField]
    SpawningScript spawner;
    Color Normal;
    Color Alarm;
    float time =0;
    [SerializeField]
    Light dirLight;

    [HideInInspector]
    public bool hasSpawned = false;
    public bool allowedToSpawn = false;

    AudioSource audioSource;

	// Use this for initialization
	void Start () {
		Normal = new Color(0f / 255f, 0f / 255f, 0f / 255f, 0 / 255f);
		Alarm = new Color(255f / 255f, 0f / 255f, 0f / 255f, 255 / 255f);
        audioSource = GetComponent<AudioSource>();

	}
	
	// Update is called once per frame
    void Update() {
        time = Mathf.PingPong(Time.time, 1);
        if (spawner.spawn) {
            dirLight.color = Color.Lerp(Normal, Alarm, time);
            // Debug.Log(Color.Lerp(Normal, Alarm, time));
        }
        else {
            dirLight.color = Normal;
        }


        if (spawner.spawn) {
            if (!audioSource.isPlaying) {
                audioSource.Play();
            }
        }
        else {
            if (audioSource.isPlaying) {
                audioSource.Stop();
                GameObject.Find("SoundManager").GetComponent<AudioManagerScript>().FadeOutIn(0);
            }
        }


    }

    public void Spawn()
    {
        if (!hasSpawned && allowedToSpawn)
        {
            spawner.spawn = true;
            hasSpawned = true;
            GameObject.Find("SoundManager").GetComponent<AudioManagerScript>().FadeOutIn(1);
        }
    }
}
