using UnityEngine;
using System.Collections;

public class SceneChangeManager : MonoBehaviour {

    public enum GameState{
        InMenu,InGame,InBuyScreen,InPauseMenu
    }

    GameState currentState;

    VendingMachine vendingMachine;
    PauseMenuScript pauseMenu;

    CameraMouseControl[] cmc;

    
	void Start () {

        
        currentState = GameState.InGame;
        vendingMachine = GameObject.Find("VendingMachine").GetComponent<VendingMachine>();
        pauseMenu = GameObject.Find("PauseMenuManager").GetComponent<PauseMenuScript>();
        
	}

    void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            ChangeState(GameState.InBuyScreen);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            switch(currentState){
            
                case GameState.InBuyScreen:
                    ChangeState(GameState.InGame);
                    break;

                case GameState.InGame:
                    ChangeState(GameState.InPauseMenu);
                    break;

                case GameState.InPauseMenu:
                    ChangeState(GameState.InGame);
                    break;
            }
            
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
                Time.timeScale = 0;
                pauseMenu.ActivatePauseMenu();
                SetSensitivity(0);

                break;
        }
        currentState = newGameState;

    }

    void DisablePreviousState(GameState previousState) {
        switch (previousState) {

            case GameState.InMenu:
                break;

            case GameState.InGame:
                break;

            case GameState.InBuyScreen:
                vendingMachine.DeActivateStation();
                break;

            case GameState.InPauseMenu:
                pauseMenu.DeActivatePauseMenu();
                break;
        }
    }

    void SetSensitivity(int sensitivity) {
        cmc = FindObjectsOfType(typeof(CameraMouseControl)) as CameraMouseControl[];
        for (int i = 0; i < cmc.Length; i++) {
            cmc[i].camSensitivity = sensitivity;
        }
        cmc = null;
    }

    public void ChangeToInGame() {
        ChangeState(SceneChangeManager.GameState.InGame);
    } 
}
