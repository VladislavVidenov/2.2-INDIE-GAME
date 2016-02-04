using UnityEngine;
using System.Collections;

public class AudioManagerScript : MonoBehaviour {

    [SerializeField]
    AudioClip[] audioClips;

    AudioSource audioSource;
    [SerializeField]
    float[] volumes;
   // float bassVolume = 0.27f;
   // float demo2Volume = 0.4f;

    bool fade = false;
    int nextToPlayIndex = 0;
	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClips[0];
        audioSource.Play();
	}
	
	// Update is called once per frame
	void Update () {
        if (fade) {
            audioSource.volume -= 0.07f * Time.deltaTime;
            if (audioSource.volume <= 0.01f) {
                fade = false;
                audioSource.clip = audioClips[nextToPlayIndex];
            }
        }
        else {
            if (audioSource.volume <= volumes[nextToPlayIndex]) {
                
                audioSource.volume += 0.07f * Time.deltaTime;
                if (!audioSource.isPlaying) {
                    audioSource.Play();
                }
            }
        }
	
	}

    public void FadeOutIn(int indexIn) {
        nextToPlayIndex = indexIn;
        fade = true;
        //audioSource.volume -= 0.01f * Time.deltaTime;
    }
}
