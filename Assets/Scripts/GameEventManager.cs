using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Text;
using UnityEngine;

public class GameEventManager
{
    private GameManager gameManager;
    private ScoreSystem scoreSystem;
    private StudentManager studentManager;
    private Dictionary<string, GameEvent> gameEvents;
    private List<GameEvent> starterEvents;

    private GameEvent currentEvent;
    private UILayer uiLayer;
    public GameEventManager(GameManager gameManager, UILayer ui, ScoreSystem scoreSystem, StudentManager studentManager)
    {
        this.gameManager = gameManager;
        this.uiLayer = ui;
        this.scoreSystem = scoreSystem;
        this.studentManager = studentManager;
        gameEvents = new Dictionary<string, GameEvent>();
        starterEvents = new List<GameEvent>();
        // Load GameEvents from file
        string filePath = Path.Combine(Application.streamingAssetsPath, "gameEventSheet.txt");
        if (File.Exists(filePath))
        {
            using (StreamReader file = new StreamReader(filePath))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    string[] split = line.Split('\t');
                    // First three items are id, name, and description
                    Debug.Log(split.Length);
                    GameEvent gameEvent = new GameEvent(
                        split[0],
                        split[1],
                        split[2]
                    );
                    // Fourth item is whether the event can be a starter. Process later
                    // Remaining items are pairs of option texts and next events
                    for (int i = 4; i < split.Length; i += 2)
                    {
                        if (split[i].Length > 0)
                        {
                            string nextText = split[i];
                            string nextOption = split[i + 1];
                            nextOption.Replace(" ", "");
                            gameEvent.AddOption(nextText, nextOption);
                        }
                    }
                    // Map new event to its id
                    gameEvents.Add(gameEvent.GetID(), gameEvent);
                    // If event can be a starter, add it to a separate list
                    if (string.Equals((split[3]), "TRUE"))
                    {
                        starterEvents.Add(gameEvent);
                    }
                    Debug.Log("Loaded event: " + gameEvent);
                }
            }
        }
        else
        {
            Debug.LogError("Could not find gameEvents file.");
        }
    }

    public void ProgressEvent()
    {
        // If there is no current event, select a new starter event
        if (currentEvent == null)
        {
            currentEvent = GetRandomStarterEvent();
            // If all starter events have already been used, allow them to be reused.
            if (currentEvent == null)
            {
                foreach (GameEvent e in starterEvents)
                {
                    e.SetAvailable(true);
                }
                currentEvent = GetRandomStarterEvent();

            }

            PlayEvent(currentEvent.GetID());
        }
        // Tell UI manager to show the event window and update text
        uiLayer.ShowEvent(ref currentEvent);
    }

    public void ChooseOption(int o)
    {
        // Tell UI manager to hide event window
        // Mark event as unavailable to be used again (as starter)
        currentEvent.SetAvailable(false);
        // Get corresponding event
        string key = currentEvent.GetOptionNext(o);
        key.Replace(" ", "");
        if (string.Equals(key, "none"))
        {
            // If next event is "none", then the event chain has concluded
            currentEvent = null;
        }
        else
        {
            // Otherwise, set the next event in the chain as the current event.
            currentEvent = gameEvents[key];
        }

    }

    public GameEvent GetRandomStarterEvent()
    {
        int index = UnityEngine.Random.Range(0, starterEvents.Count);
        for (int i = 0; i < starterEvents.Count; i++)
        {
            int val = (index + i) % starterEvents.Count;
            if (starterEvents[val].IsAvailable())
            {
                starterEvents[val].GetID();
                return starterEvents[val];
            }
        }
        return null;
    }

    public GameEvent GetCurrentEvent() { return this.currentEvent; }

    public void PlayEvent(string id)
    {
        switch (id)
        {
            case "weather_heavy_rain":

                break;
            case "storm_no_class":

                break;
            case "storm_online_class":

                break;
            case "storm_in_person_class":

                break;
            case "construction_small_road":

                break;
            case "construction_medium_road":

                break;
            case "repavement_hire_workers":

                break;
            case "repavement_wait":

                break;
            case "construction_small_paths":

                break;
            case "construction_medium_paths":

                break;
            case "sport_home_game":

                break;
            case "student_no":
                //studentManager.DecreaseHappiness();
                break;
        }

        return;
    }
}