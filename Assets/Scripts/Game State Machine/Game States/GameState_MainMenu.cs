using UnityEngine;
// Sam Robichaud 
// NSCC Truro 2024
// This work is licensed under CC BY-NC-SA 4.0 (https://creativecommons.org/licenses/by-nc-sa/4.0/)

public class GameState_MainMenu : IGameState
{
    #region Static instance
    private static GameState_MainMenu _instance;

    public static GameState_MainMenu instance
    {
        get
        {
            if (_instance == null) { _instance = new GameState_MainMenu(); }
            return _instance;
        }
    }
    #endregion


    public void EnterState(GameStateManager gameStateManager)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        // May want to consider locking the cursor to the center of the screen

        UIManager.instance.UIMainMenu();
        CameraManager.instance.UseMainMenuCamera();     
    }

    public void FixedUpdateState(GameStateManager gameStateManager) { }
    public void UpdateState(GameStateManager gameStateManager) { }
    public void LateUpdateState(GameStateManager gameStateManager) { }
    public void ExitState(GameStateManager gameStateManager) { }


}
