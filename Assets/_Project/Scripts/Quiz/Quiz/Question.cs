using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;


/// <summary>
/// Represents a question
/// </summary>
[CreateAssetMenu(fileName = "Question", menuName = "ENIAROF/Question", order = 1)]
public class Question : ScriptableObject
{
    public string ID;
    public bool isFirstQuestion;
    public string label;
    public List<Requirement> requirements;
    public List<Anwser> anwsers;

    /// <summary>
    /// Represents a requirement for a question
    /// </summary>
    [System.Serializable]
    public class Requirement
    {
        public enum RequirementType
        {
            HAVE,
            DONT
        }
        public RequirementType type;
        public string linkedAnwser;
    }
}
