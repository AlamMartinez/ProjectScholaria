using UnityEngine;

public class TimeManager
{
    /*
     * The year is split into 52 weeks;
     * There are two semesters: fall semester, and spring semester
     * There are two breaks consisting of two weeks each in between the semesters, for a total of 4 weeks off
     * Each semester is 24 weeks, which is split into 3 sections
     * At the start of each section, students will try to visit all the destination in their schedule. They will have until the end of the section to reach it. Failing that, they lose happiness
     * At the start of each semester, students' schedules will be randomized.
     * An event can fire at any point during a semester, but cannot fire within two sections (16 weeks) of another event
     * The chance for an event to fire starts at 0.2%. After each week, if the event doesn't fire, it increases by another 0.2%. This repeats until the event fires, which on average will be after about 24 weeks, but can fire 8 weeks sooner or 76 weeks later (though unlikely).
     */
    private float time;
    private float timeModifier;
    private int speed;
    private float eventCooldown;
    private float eventChance;
    private bool isPaused;

    private GameManager gameManager;
    private GameEventManager gameEventManager;
    public TimeManager(GameManager gameManager, GameEventManager gameEventManager)
    {
        this.gameManager = gameManager;
        this.gameEventManager = gameEventManager;
        this.timeModifier = 1.0f;
    }
    /// <summary>
    /// Returns the time since the last frame multiplied by the current speed of the game. If the game is paused, this value will be zero.
    /// </summary>
    public void Update()
    {
        // Delta is a fraction of a week
        float delta = DeltaTime() * timeModifier;
        time += delta;
        // Reduce event cooldown, but not below zero
        if(eventCooldown > 0)
        {
            eventCooldown = Mathf.Max(0, eventCooldown - delta);
        }
        // Increase event chance
        eventChance += delta * 0.002f;
        // If cooldown has elapsed, roll the dice and fire an event if it succeeds
        if(eventCooldown == 0 && eventChance > Random.Range(0f,1f))
        {
            Debug.Log("Event Progress");
            gameEventManager.ProgressEvent();
        }
    }
    public float DeltaTime()
    {
        if(isPaused)
        {
            return 0;
        }
        return Time.deltaTime * speed;
    }
    public string GetTimeString()
    {
        return "Year " + (((int)time) / 52 + 1) + ", week " + (((int)time) % 52);
    }

    public float GetTimeMod() { return this.timeModifier; }
    public void SetTimeMod(float val) { this.timeModifier = val; }

    public int GetTimeVar()
    {
        return (int)time;
    }
}
