using UnityEngine;

/// <summary>
/// Represent a step in the creation of the book
/// </summary>
[CreateAssetMenu(fileName = "Step", menuName = "ENIAROF/Step", order = 0)]
public class Step : ScriptableObject
{
    public string ID;
    public int stepsAmount;
    public Question[] pool;
}

