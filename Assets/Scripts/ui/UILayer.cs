using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

public struct UI_State
{
    int currentMode;
    public UI_State(int mode) {
        this.currentMode =  mode;
    }
}

public class UILayer : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject pauseUI;
    public GameObject confirmationUI;
    public TextMeshProUGUI infoDisplay;
    private UI_State uiState;


    public void OnSavePress()
    {
        gameManager.SaveGame();
    }

    public void OnLoadPress()
    {
        gameManager.LoadGame();
    }

    public void OnResetPress()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnUnpausePress() {
        pauseUI.SetActive(false);
        Debug.Log("Pause menu close");
    }

    public void OnExitPress() {
        SceneManager.LoadScene(0);
    }

    public void OnAddStudentPress() {
        gameManager.AddRandomStudent();
    }

    public void OnCycleMode() {
        gameManager.Cycle(1);
    }

    public void OnConfirmYes() {
        confirmationUI.SetActive(false);
    }

    public void OnConfirmCancel() {
        confirmationUI.SetActive(false);
        pauseUI.SetActive(false);
    }

    void Start()
    {
        //this.uiState.currentMode = gameManager.GetMode();
    }

    void Update()
    {
        // handle input
        if(Input.GetKeyDown("h")) {
            pauseUI.SetActive(true);
            Debug.Log("Pause menu open");
        }

        var modeText = "";
        switch(gameManager.GetMode())
        {
            case 0:
                modeText = "Overview";
                break;
            case 1:
                switch(gameManager.GetPlacementManager().GetPlacementMode())
                {
                    case 1:
                        modeText = "Placing Buildings";
                        break;
                    case 2:
                        modeText = "Placing Pathways";
                        break;
                }
                break;
            case 2:
                modeText = "Demolition";
                break;
        }

        //update ui info
        infoDisplay.text = "Students: " + gameManager.GetGameState().numStudents +
            "\nCurrent Mode: " + modeText +
            "\nSelected Building: " + gameManager.GetGameState().selectionContext;
    }
}
