using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents the quiz's GUI
/// </summary>
public class QuizGUI : MonoBehaviour
{
    [Header("Question")]
    [SerializeField] private TextMeshProUGUI questionLabelText;
    [SerializeField] private AnwserGUI[] anwsers;

    [Header("Progress")]
    [SerializeField] private Image progressFill;

    [Header("Transition")]
    [SerializeField] private Animator transitionAnimator;

    public static QuizGUI instance;

    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Sets the progress's fill amount on the GUI
    /// </summary>
    /// <param name="fillAmount">The fill amount of the GUI</param>
    public void SetProgressFill(float fillAmount)
    {
        progressFill.fillAmount = fillAmount;
    }


    /// <summary>
    /// Changes the question of the GUI
    /// </summary>
    /// <param name="question">The question to display</param>
    public void SetQuestion(Question question)
    {
        questionLabelText.text = question.label;

        for (int i = 0; i < question.anwsers.Length && i < anwsers.Length; i++)
        {
            anwsers[i].SetText(question.anwsers[i].label);
        }
    }

    /// <summary>
    /// Starts an animation of a button
    /// </summary>
    /// <param name="buttonIdx">The button's index</param>
    public void StartAnimationForButton(int buttonIdx)
    {
        for (int i = 0; i < anwsers.Length; i++)
        {
            if (i == buttonIdx) anwsers[i].SetAnimationTrigger("Interract");
            else anwsers[i].SetHidden(true);
        }
    }

    /// <summary>
    /// Sets if the buttons are hidden
    /// </summary>
    /// <param name="isHidden">Are the button hidden ?</param>
    public void SetButtonHidden(bool isHidden)
    {
        foreach (AnwserGUI anwser in anwsers)
        {
            anwser.SetHidden(isHidden);
        }
    }

    /// <summary>
    /// Sets the question's label
    /// </summary>
    /// <param name="label">The label to display</param>
    public void SetQuestionLabel(string label)
    {
        questionLabelText.text = label;
    }

    /// <summary>
    /// Starts the transition
    /// </summary>
    public void ShowTransition()
    {
        transitionAnimator.SetTrigger("Transition");
    }

}
