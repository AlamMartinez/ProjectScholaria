using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    public GameEventManager eventManager;
    public GameObject pauseUI;
    public GameObject buildingUI;
    public GameObject confirmationUI;
    public GameObject eventUI;
    public GameObject helpUI;
    public GameObject buttonPrefab;
    public Transform buttonsList;
    public TextMeshProUGUI buildingName;
    public TextMeshProUGUI buildingDisplay;
    public TextMeshProUGUI statsDisplay;
    public TextMeshProUGUI infoDisplay;
    public List<GameObject> eventButtons;
    private UI_State uiState;

    void Start()
    {

    }

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
        gameManager.ResetGame();
    }

    public void ShowPauseMenu() {
        pauseUI.SetActive(true);
    }

    public void OnUnpausePress() {
        pauseUI.SetActive(false);
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


    public void OnEventClose() {
        eventUI.SetActive(false);
    }

    public void ShowEvent(ref GameEvent env) {
        eventUI.SetActive(false);
        Debug.Log("Example event started: " + env.ToString());
        var fields = eventUI.GetComponentsInChildren<TextMeshProUGUI>();
        if(fields.Length > 1) {
            Debug.Log("description");
            fields[0].text = env.GetName();
            fields[1].text = env.GetDesc();
        }

        for (int i = 0;  i < env.GetOptionCount();  i++)
        {
            var button = Instantiate(buttonPrefab);
            button.transform.SetParent(buttonsList);
            button.GetComponentInChildren<TextMeshProUGUI>().text = env.GetOptionText(i);
            var value = i;
            button.GetComponent<Button>().onClick.AddListener(() => OnOptionPress(value));
            eventButtons.Add(button);
        }
        eventUI.SetActive(true);
    }

    public void OnOptionPress(int val) {
        var eventMan = gameManager.GetEventManager();
        eventUI.SetActive(false);
        eventMan.ChooseOption(val);
        ClearOptions();
    }


    public void ClearOptions() {
        //var buttons = gameManager.GetGameObjects().FindAll(gm => gm.tag == "eventname");
        //var buttons = GameObject.FindGameObjectsWithTag("eventname");
        if(eventButtons.Count == 0) {
            Debug.Log("Problem");
        }
        foreach (var button in eventButtons)
        {
            Destroy(button);
        }
        eventButtons.Clear();
    }


    public void OnEventTest() {
        var eventMan = gameManager.GetEventManager();
        eventMan.ProgressEvent();
    }

    public void OnBuildingUIShow(ref Building building) {
        buildingName.text = building.GetName();
        buildingDisplay.text = "Type: " + building.GetType() +
            "Vists: " + building.GetVisits() +
            "Capacity: " + building.GetCapacity();
        buildingUI.SetActive(true);
    }

    public void OnBuildingUIClose() {
        buildingUI.SetActive(false);
    }

    public void OnNormalSpeedPress() {
        gameManager.GetTimeManager().SetTimeMod(1.0f);
    }

    public void OnFastForwardPress() {
        var tm = gameManager.GetTimeManager();
        var multi = tm.GetTimeMod();
        multi *= 2.0f;
        tm.SetTimeMod(multi);
    }

    public void OnHelpPress() {
        pauseUI.SetActive(false);
        helpUI.SetActive(true);
    }

    public void OnHelpClose() {
        helpUI.SetActive(false);
    }


    void Update()
    {
        var gameState = gameManager.GetGameState();

        // handle input
        if(Input.GetKeyDown("h")) {
            ShowPauseMenu();
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

        if(gameState.selectionContext == "") {
            buildingUI.SetActive(false);
        }

        //update ui stats
        statsDisplay.text = "Score: " + gameState.Score +
            "\nExperience: " + gameState.Exp +
            "\nLevel: " + gameState.Level +
            "\nHappiness: " + gameState.Happiness;

        //update ui info
        infoDisplay.text = "Students: " + gameState.numStudents +
            "\nCurrent Mode: " + modeText +
            "\nCurrent Time: " + gameManager.GetTimeManager().GetTimeString() +
            "\n" + gameState.selectionContext;
    }
}
