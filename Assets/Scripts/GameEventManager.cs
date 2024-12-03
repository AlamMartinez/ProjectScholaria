using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GameEventManager
{
    private GameManager gameManager;

    private Dictionary<string,GameEvent> gameEvents;
    private List<GameEvent> starterEvents;

    private GameEvent currentEvent;
    public GameEventManager(GameManager gameManager)
    {
        this.gameManager = gameManager;
        gameEvents = new Dictionary<string, GameEvent>();
        starterEvents = new List<GameEvent>();
        // Load GameEvents from file
        string filePath = Path.Combine(Application.streamingAssetsPath, "gameEvents.csv");
        if(File.Exists(filePath))
        {
            using (StreamReader file = new StreamReader(filePath))
            {
                string line;
                while((line = file.ReadLine()) != null)
                {
                    string[] split = line.Split(';');
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
                        gameEvent.AddOption(split[i], split[i + 1]);
                    }
                    // Map new event to its id
                    gameEvents.Add(gameEvent.GetID(), gameEvent);
                    // If event can be a starter, add it to a separate list
                    if (bool.Parse(split[3]))
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
            if(currentEvent == null)
            {
                foreach(GameEvent e in starterEvents)
                {
                    e.SetAvailable(true);
                }
                currentEvent = GetRandomStarterEvent();
            }
        }
        // Tell UI manager to show the event window and update text

    }

    public void ChooseOption(int o)
    {
        // Tell UI manager to hide event window
        // Mark event as unavailable to be used again (as starter)
        currentEvent.SetAvailable(false);
        // Get corresponding event
        string key = currentEvent.GetOptionNext(o);
        if (key == "none")
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
        int index = Random.Range(0, starterEvents.Count);
        for (int i = 0; i < starterEvents.Count; i++)
        {
            int val = (index + i) % starterEvents.Count;
            if (starterEvents[val].IsAvailable())
            {
                return starterEvents[val];
            }
        }
        return null;
    }
}
