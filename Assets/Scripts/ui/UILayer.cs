using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.UIElements;
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
    public GameObject confirmationUI;
    public GameObject eventUI;
    public GameObject buttonPrefab;
    public Transform buttonsList;
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
        GameEvent env = eventMan.GetCurrentEvent();
        ShowEvent(ref env);
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
            "\n" + gameManager.GetGameState().selectionContext;
    }
}
