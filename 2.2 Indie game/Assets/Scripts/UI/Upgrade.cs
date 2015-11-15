using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Upgrade :MonoBehaviour {

	bool isDrawable = true;

	public Sprite upGradeImage;
	public string text;

	public int Cost;
	// Use this for initialization

	void Initialize (bool drawable){

	}

	public virtual void Apply (){

	}
}
