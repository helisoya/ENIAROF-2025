using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Represents an anwser on the GUI
/// </summary>
public class AnwserGUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private GameObject buttonRoot;
    [SerializeField] private Animator animator;

    /// <summary>
    /// Sets the button's text
    /// </summary>
    /// <param name="txt">The new text to display</param>
    public void SetText(string txt)
    {

        labelText.text = txt;
    }

    /// <summary>
    /// Sets an animation trigger
    /// </summary>
    /// <param name="triggerName">The trigger's name</param>
    public void SetAnimationTrigger(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    /// <summary>
    /// Sets if the button is hidden or not
    /// </summary>
    /// <param name="isHidden">Is the button hidden ?</param>
    public void SetHidden(bool isHidden)
    {
        buttonRoot.SetActive(!isHidden);
    }

}
