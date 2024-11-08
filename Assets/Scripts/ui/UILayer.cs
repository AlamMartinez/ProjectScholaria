using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public struct UI_State
{
    int currentMode;

    public UI_State() {
        mode = 0;
    }
}

public class UILayer : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject pauseUI;
    //public TextMeshProUGUI infoDisplay;
    private UI_State uiState;
    private Grid grid;

    public UILayer(GameManager gameManager, Grid grid)
    {
        uiState = new UI_State();
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

    void Start()
    {
        this.uiState.currentMode = gameManager.GetMode();
    }

    void Update()
    {
        // handle input
        if(Input.GetKeyDown("h")) {
            pauseUI.SetActive(true);
            Debug.Log("Pause menu open");
        }

        //update ui info
        //infoDisplay.text = "Students: " + gameManager.GetGameState().numStudents().toString();
    }
}
