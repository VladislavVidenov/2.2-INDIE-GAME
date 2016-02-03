using UnityEngine;
using System.Collections;

public class MeleeRobotChargeLight : MonoBehaviour {
	Light alarmLight;

	public float lerpSpeed = 2f;            
	public float maxIntensity = 2f;        
	public float minIntensity = 0.5f;  
	public float targetIntensity;
	public float switchTreshold = 0.2f;  

	bool alarmingLightOn = false;
	// Use this for initialization
	void Start () {
		alarmLight = GetComponent<Light> ();
	}

	void Update()
	{
		if (alarmingLightOn) {
			alarmLight.intensity = Mathf.Lerp (alarmLight.intensity, targetIntensity, lerpSpeed * Time.deltaTime);
			BlinkLight ();
		}
	}

	void BlinkLight(){
		if(Mathf.Abs(targetIntensity - alarmLight.intensity) < switchTreshold)
		{
			if(targetIntensity == maxIntensity)
				targetIntensity = minIntensity;
			else
				targetIntensity = maxIntensity;
		}
	}


	void StartAlarmLight()
	{
		alarmingLightOn = true;
	}
	void StopAlarmLight()
	{
		alarmingLightOn = false;
		alarmLight.intensity = 0;
	}


	void OnEnable(){
		MeleeEnemyScript.OnRobotCharging += StartAlarmLight;
		MeleeEnemyScript.OnRobotStopCharging += StopAlarmLight;
		
	}
	void OnDisable(){
		MeleeEnemyScript.OnRobotCharging -= StartAlarmLight;
		MeleeEnemyScript.OnRobotStopCharging -= StopAlarmLight;
	}
}
