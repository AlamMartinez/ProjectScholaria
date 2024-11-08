using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

//public struct UI_State
//{
//    int currentMode;
//
//    public UI_State() {
//        mode = 0;
//    }
//}

public class UILayer : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject pauseUI;
    public TextMeshProUGUI infoDisplay;
    //private UI_State uiState;
    private Grid grid;


    public UILayer(GameManager gameManager, Grid grid)
    {
        //uiState = new UI_State();
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
                modeText = "Mode: Overview";
                break;
            case 1:
                switch(gameManager.GetPlacementManager().GetPlacementMode())
                {
                    case 1:
                        modeText = "Mode: Placing Buildings";
                        break;
                    case 2:
                        modeText = "Mode: Placing Pathways";
                        break;
                }
                break;
            case 2:
                modeText = "Mode: Demolition";
                break;
        }

        //update ui info
        infoDisplay.text = "Students: " + gameManager.GetGameState().numStudents +
            "\nCurrent Mode: " + modeText;
    }
}
