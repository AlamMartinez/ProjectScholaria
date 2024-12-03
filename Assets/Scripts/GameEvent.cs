using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent
{
    private string id;
    private string name;
    private string description;
    private List<Option> options;
    private bool available;

    struct Option
    {
        public Option(string optionText, string optionNext)
        {
            this.optionText = optionText;
            this.optionNext = optionNext;
        }
        public string optionText;
        public string optionNext;
    }

    public GameEvent(string id, string name, string description)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        available = false;

        options = new List<Option>();
    }
    public void AddOption(string optionText, string optionNext)
    {
        options.Add(new Option(optionText, optionNext));
    }
    /// <summary>
    /// Gets the number of options this GameEvent has.
    /// </summary>
    public int GetOptionCount()
    {
        return options.Count;
    }
    /// <summary>
    /// Given the option number, eturns the option text for this GameEvent's corresponding option
    /// </summary>
    public string GetOptionText(int i)
    {
        return options[i].optionText;
    }
    public string GetOptionNext(int i)
    {
        return options[i].optionNext;
    }
    public string GetID()
    {
        return id;
    }
    public bool IsAvailable()
    {
        return available;
    }
    public void SetAvailable(bool b)
    {
        available = b;
    }
    public override string ToString()
    {
        string output = name + "(" + id + ") \n" + description + "\n";
        for(int i = 0; i < options.Count; i++)
        {
            output += "1: " + options[i].optionText + " --> " + options[i].optionNext + "\n";
        }
        return output;
    }
}
