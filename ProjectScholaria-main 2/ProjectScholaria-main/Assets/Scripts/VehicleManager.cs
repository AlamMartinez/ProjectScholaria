using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class VehicleManager
{
    private Grid grid;
    private GameManager gameManager;
    private List<Vehicles> vehicles;
    private int vehicleIndex;
    private IEnumerator corocutine;

    public VehicleManager(GameManager gameManager, Grid grid)
    {
        this.gameManager = gameManager;
        this.grid = grid;
        vehicles = new List<Vehicles>();
        vehicleIndex = 0;

    }

    public void Update()
    {
        //Debug.Log("updating vehicles...");
        foreach (Vehicles vehicle in vehicles)
        {
            Debug.Log(vehicle == null);
            Debug.Log("updating vehicle " + vehicle.GetID());
            if (!vehicle.HasPath())
            {

                Debug.Log("vehicle does not have a path");

                if (gameManager.GetPlacementManager().GetBusStopCount() < 2)
                {
                    Debug.Log("Not enough bus stops in play");
                    vehicle.Remove();
                    vehicles.Remove(vehicle);
                    continue;
                }
                else
                {
                    Debug.Log("Finding new road for vehicle");
                    vehicle.CalculateRoad(grid, gameManager.GetPlacementManager().GetBusStops());
                }
            }

            if (vehicle.HasPath())
            {
                Debug.Log("Bus: " + vehicle.GetID() + " has path and is now updating");
                vehicle.Update();
            }
        }
    }

    public GameObject CreateRandomBus(GameObject busPrefab)
    {
        Debug.Log("creating new bus...");
        if (gameManager.GetPlacementManager().GetBusStopCount() >= 2)
        {
            Vehicles bus = new Vehicles(vehicleIndex++, 'b');
            //Cell spawnPosition = gameManager.GetPlacementManager().GetRandomBusStop();
            Cell spawnPosition = gameManager.GetPlacementManager().GetBusStops()[0];
            Debug.Log("New bus is now at: " + spawnPosition.GetX() + ", " + spawnPosition.GetY());
            bus.SetPosition(spawnPosition);
            bus.CalculateRoad(grid, gameManager.GetPlacementManager().GetBusStops());
            vehicles.Add(bus);

            Debug.Log("making gameObject for bus");
            GameObject busVehicle = GameObject.Instantiate(busPrefab);
            busVehicle.name = "busStop" + spawnPosition.GetX() + "," + spawnPosition.GetY();
            busVehicle.transform.position = new Vector3(spawnPosition.GetX(), 0, spawnPosition.GetY());
            bus.SetGameObject(busVehicle);
            return busVehicle;
        }
        else
        {
            Debug.Log("Insufficient bus stops to add Bus");
            return null;
        }
    }

}