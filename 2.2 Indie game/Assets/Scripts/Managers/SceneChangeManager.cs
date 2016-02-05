using UnityEngine;
using System.Collections;

public enum GameState {
    InMenu, InGame, InBuyScreen, InPauseMenu, PlayerDied
}

public class SceneChangeManager : MonoBehaviour {

    GameState currentState;
    CameraMouseControl[] cmc;

    void Awake() {
        DontDestroyOnLoad(GameManager.Instance); //DIRTY FIX SO WE CAN START ANY SCENE.
    }

    void Start() {
        currentState = GameManager.Instance.CurrentState;
    }

    void Update() {
        if (currentState == GameState.InGame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (currentState == GameState.InMenu)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            switch(currentState){
                case GameState.InBuyScreen:
                    SetState(GameState.InGame);
                    break;
                case GameState.InGame:
                    SetState(GameState.InPauseMenu);
                    break;
                case GameState.InPauseMenu:
                    SetState(GameState.InGame);
                    break;
                default:
                    print(string.Format("According to the game, our current state is '{0}'. Perhaps we should check the buildsettings if this was unexpected.", currentState));
                    break;
            }
        }
    }

    public void SetState(GameState newGameState) {
        DisablePreviousState(currentState);
        switch (newGameState) {
            case GameState.InMenu:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case GameState.InGame:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
				Time.timeScale = 1;
				SetSensitivity(2);
                break;
            case GameState.InBuyScreen:
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
                GameManager.Instance.vendingMachine.ActivateStation();
                SetSensitivity(0);
                Cursor.visible = true;
                break;
            case GameState.InPauseMenu:
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
                GameManager.Instance.pauseMenu.ActivatePauseMenu();
                SetSensitivity(0);
                Cursor.visible = true;
                break;
            case GameState.PlayerDied:
                Cursor.lockState = CursorLockMode.Locked;
                SetSensitivity(0);
                Cursor.visible = true;
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
                GameManager.Instance.vendingMachine.DeActivateStation();
                break;
            case GameState.InPauseMenu:
                GameManager.Instance.pauseMenu.DeactivatePauseMenu();
                break;
            case GameState.PlayerDied:
                break;
        }
    }

    public void SetSensitivity(int sensitivity) {
        cmc = FindObjectsOfType(typeof(CameraMouseControl)) as CameraMouseControl[];
        for (int i = 0; i < cmc.Length; i++) {
            cmc[i].camSensitivity = sensitivity;
        }
        cmc = null;
    }

    public void SwitchToLevel(int index) {
        GameManager.Instance.CurrentState = GameState.InGame;
        SetState(GameState.InGame);
        Application.LoadLevel(index);
    }

    public void SwitchToMainMenu() {
        GameManager.Instance.CurrentState = GameState.InMenu;
        GameManager.Instance.ResetGameManager();
        Application.LoadLevel("MenuScene");
    }

    public void CloseApllication() {
        Application.Quit();
    }

    public void ChangeToInGame() {
        SetState(GameState.InGame);
    } 
}
