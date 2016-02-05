using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeInout : MonoBehaviour {

    public float fadeSpeed = 1.5f;

    //[HideInInspector]
    public bool fade = false;
    bool startedScene = false;

    Image texture;

    void Awake() {
        texture = GetComponent<Image>();

    }

    void Start() {
        Invoke("SetFadeIn", 2f);
    }

    void Update() {
        if (fade) {
            EndScene();
        }
        else if (!fade && texture.enabled && startedScene) {
            StartScene();
        }
    }

    void SetFadeIn() {
        startedScene = true;
    }

    void FadeToClear() {
        texture.color = Color.Lerp(texture.color, Color.clear, .5f * Time.deltaTime);
    }

    void FadeToBlack() {
        texture.color = Color.Lerp(texture.color, Color.black, fadeSpeed * Time.deltaTime);
    }

    public void StartScene() {
        FadeToClear();

        if (texture.color.a <= 0.05f) {
            texture.color = Color.clear;
            texture.enabled = false;
        }
    }

    public void EndScene() {
        texture.enabled = true;
        FadeToBlack();

        if (texture.color.a >= 0.995f) {
            texture.color = Color.black;
        }
    }

    void LoadLevel() {
        Application.LoadLevel(0);
    }
}
