using UnityEngine;
using System.Collections;

public class GameManagerScript : MonoBehaviour {

    public enum GameState{
        InMenu,InGame,InBuyScreen,InPauseMenu
    }

    GameState currentState;

    VendingMachine vendingMachine;
    CameraMouseControl[] cmc;


	void Start () {
        currentState = GameState.InGame;
        vendingMachine = GameObject.Find("VendingMachine").GetComponent<VendingMachine>();
	}

    void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            ChangeState(GameState.InBuyScreen);
        }
    }

    public void ChangeState(GameState newGameState) {

        DisablePreviousState(currentState);

        switch (newGameState) {

            case GameState.InMenu:
                break;

            case GameState.InGame:
				Time.timeScale = 1;
				SetSensitivity(2);
                break;

            case GameState.InBuyScreen:
                Time.timeScale = 0;
                vendingMachine.ActivateStation();
                SetSensitivity(0);
                break;

            case GameState.InPauseMenu:
                break;


        }
    }

    void DisablePreviousState(GameState previousState) {
        switch (previousState) {

            case GameState.InMenu:
                break;

            case GameState.InGame:
                break;

            case GameState.InBuyScreen:
                break;

            case GameState.InPauseMenu:
                break;
        }
    }

    void SetSensitivity(int sensitivity) {
        cmc = FindObjectsOfType(typeof(CameraMouseControl)) as CameraMouseControl[];
        for (int i = 0; i < cmc.Length; i++) {
            cmc[i].camSensitivity = sensitivity;
        }
    }
}
