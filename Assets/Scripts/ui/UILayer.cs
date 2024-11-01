using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

struct UI_State {
    int currentMode;

}

public class UILayer : MonoBehaviour
{
    private GameManager gameManager;
    private Grid grid;
    private UI_State uiState;

    public UILayer(GameManager gameManager, Grid grid)
    {
        this.gameManager = gameManager;
        this.grid = grid;
        this.uiState = new UI_State();
    }

    void Start()
    {
        this.uiState.currentMode = this.gameManager.mode;
    }

    void Update()
    {

    }
}
