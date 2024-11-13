using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
/// <summary>
/// Observes game input and sends it to the GameManager
/// </summary>
public class InputManager : MonoBehaviour
{
    public GameManager gameManager;
    public CameraManager cameraManager;

    private void Start()
    {
        cameraManager = new CameraManager(GameObject.Find("Camera Focus").GetComponent<Transform>(), GameObject.Find("Camera Pivot").GetComponent<Transform>());
    }

    void Update()
    {
        //Update Camera Position
        cameraManager.TranslateCamera(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        cameraManager.ZoomCamera(Input.GetAxis("Zoom"));
        //Update Camera Rotaiton
        if (Input.GetMouseButton(1))
        {
            cameraManager.PanCamera(Input.GetAxis("MouseHorizontal"));
            cameraManager.TiltCamera(Input.GetAxis("MouseVertical"));
        }
        //Update Cursor Information
        Vector3Int mousePosition = Vector3Int.zero;
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            mousePosition = Vector3Int.RoundToInt(hit.point);
        }
        gameManager.UpdateCursor(mousePosition);
        //Pass whether mouse was clicked, unless mouse was clicking UI object.
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            gameManager.MouseClicked();
        }
        //Mode switch buttons
        if (Input.GetButtonDown("BuildMode"))
        {
            if(gameManager.GetMode() == GameManager.PLACEMENT && gameManager.GetPlacementManager().GetPlacementMode() == PlacementManager.BUILDING)
            {
                gameManager.SetMode(GameManager.NONE);
            }
            else 
            {
                gameManager.SetMode(GameManager.PLACEMENT);
                gameManager.GetPlacementManager().SetPlacementMode(PlacementManager.BUILDING);
            }
        }
        if (Input.GetButtonDown("PathingMode"))
        {
            if (gameManager.GetMode() == GameManager.PLACEMENT && gameManager.GetPlacementManager().GetPlacementMode() == PlacementManager.PATHING)
            {
                gameManager.SetMode(GameManager.NONE);
            }
            else
            {
                gameManager.SetMode(GameManager.PLACEMENT);
                gameManager.GetPlacementManager().SetPlacementMode(PlacementManager.PATHING);
            }
        }
        if (Input.GetButtonDown("RoadMode"))
        {
            if (gameManager.GetMode() == GameManager.PLACEMENT && gameManager.GetPlacementManager().GetPlacementMode() == PlacementManager.PATHING)
            {
                gameManager.SetMode(GameManager.NONE);
            }
            else
            {
                gameManager.SetMode(GameManager.PLACEMENT);
                gameManager.GetPlacementManager().SetPlacementMode(PlacementManager.ROAD);
            }
        }
        if (Input.GetButtonDown("BusStopMode"))
        {
            if (gameManager.GetMode() == GameManager.PLACEMENT && gameManager.GetPlacementManager().GetPlacementMode() == PlacementManager.PATHING)
            {
                gameManager.SetMode(GameManager.NONE);
            }
            else
            {
                gameManager.SetMode(GameManager.PLACEMENT);
                gameManager.GetPlacementManager().SetPlacementMode(PlacementManager.BUS_STOP);
            }
        }
        if (Input.GetButtonDown("spawnBus"))
        {
            gameManager.AddRandomBus();
        }
        if (Input.GetButtonDown("DemolishMode"))
        {
            if (gameManager.GetMode() == GameManager.DEMOLITION)
            {
                gameManager.SetMode(GameManager.NONE);
            }
            else
            {
                gameManager.SetMode(GameManager.DEMOLITION);
            }
        }
        if(Input.GetButtonDown("Cancel"))
        {
            gameManager.SetMode(GameManager.NONE);
            gameManager.ClearSelectedBuilding();
        }
        //Cycle buttons
        if(Input.GetButtonDown("CycleLeft"))
        {
            gameManager.Cycle(-1);
        }
        else if(Input.GetButtonDown("CycleRight"))
        {
            gameManager.Cycle(1);
        }
        //Submit button TODO: swap with official use
        if(Input.GetButtonDown("Submit"))
        {
            gameManager.AddRandomStudent();
        }
        //TODO: make better
        if(Input.GetKeyDown("r"))
        {
            SceneManager.LoadScene(1);
        }
    }
}
