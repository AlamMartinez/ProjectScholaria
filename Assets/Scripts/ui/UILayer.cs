using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

struct UI_State {
    public int currentMode;

}

public class UILayer : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject pauseUI;
    private Grid grid;
    private UI_State uiState;

    public UILayer(GameManager gameManager, Grid grid)
    {
        this.gameManager = gameManager;
        this.grid = grid;
        this.uiState = new UI_State();
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
    }
}
