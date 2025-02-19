using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an anwser for a question
/// </summary>
[System.Serializable]
public class Anwser
{
    public string ID;
    public string label;
    public List<string> actions;
}
