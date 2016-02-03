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

	// Use this for initialization
	void Start () {
        spawner.spawn = true;
		Normal = new Color(0f / 255f, 0f / 255f, 0f / 255f, 0 / 255f);
		Alarm = new Color(255f / 255f, 0f / 255f, 0f / 255f, 255 / 255f);

	}
	
	// Update is called once per frame
	void Update () {
        time  = Mathf.PingPong(Time.time, 1);
        if (spawner.spawn) {
            dirLight.color = Color.Lerp(Normal, Alarm, time);
           // Debug.Log(Color.Lerp(Normal, Alarm, time));
        }
        else {
            dirLight.color = Normal;
        }
	
	}
}
