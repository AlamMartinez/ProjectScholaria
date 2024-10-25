using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class UILayer : MonoBehaviour
{
    private GameManager gameManager;
    private Grid grid;

    public UILayer(GameManager gameManager, Grid grid)
    {
        this.gameManager = gameManager;
        this.grid = grid;
    }

    private void Start()
    {

    }

    void Update()
    {

    }
}
