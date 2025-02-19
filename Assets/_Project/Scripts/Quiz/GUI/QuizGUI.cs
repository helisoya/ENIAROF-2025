using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents the quiz's GUI
/// </summary>
public class QuizGUI : MonoBehaviour
{

    [Header("QCM")]
    [SerializeField] private GameObject qcmRoot;
    [SerializeField] private TextMeshProUGUI questionLabelText;
    [SerializeField] private AnwserGUI[] anwsers;
    [SerializeField] private Image progressFill;

    [Header("Menu")]
    [SerializeField] private GameObject menuRoot;

    [Header("End")]
    [SerializeField] private GameObject endRoot;
    [SerializeField] private GameObject endBook;

    [Header("Transition")]
    [SerializeField] private Animator transitionAnimator;

    private Coroutine routineTransition;
    public bool IsTransitioning { get { return routineTransition != null; } }

    public static QuizGUI instance;

    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Starts a transition to a menu
    /// </summary>
    /// <param name="menu">Menu Index</param>
    public void TransitionTo(int menu)
    {
        if (routineTransition != null) return;

        routineTransition = StartCoroutine(Routine_To(menu));
    }

    IEnumerator Routine_To(int menu)
    {
        ShowTransition();

        yield return new WaitForSeconds(0.5f);

        menuRoot.SetActive(menu == 0);
        qcmRoot.SetActive(menu == 1);
        endRoot.SetActive(menu == 2);
        endBook.SetActive(menu == 2);

        yield return new WaitForSeconds(0.5f);

        routineTransition = null;
    }

    /// <summary>
    /// Sets the progress's fill amount on the GUI
    /// </summary>
    /// <param name="fillAmount">The fill amount of the GUI</param>
    public void SetProgressFill(float fillAmount)
    {
        progressFill.fillAmount = fillAmount; //fillAmount entre 0 et 1
    }


    /// <summary>
    /// Changes the question of the GUI
    /// </summary>
    /// <param name="question">The question to display</param>
    /// <param name="anwsers">The awnser to display</param>
    public void SetQuestion(Question question, Anwser[] questionAnwsers)
    {
        questionLabelText.text = question.label;

        for (int i = 0; i < questionAnwsers.Length && i < anwsers.Length; i++)
        {
            anwsers[i].SetText(questionAnwsers[i].label);
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
        AudioManager.instance.PlayOneShot(FMODEvents.instance.Transition_SFX, this.transform.position);
    }

}
