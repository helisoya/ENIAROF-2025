using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles the quiz
/// </summary>
public class QuizManager : MonoBehaviour
{
    [SerializeField] private Step[] steps;
    private int currentStep;
    private List<string> questionsDone;
    private List<string> anwsersSelected;
    private Question currentQuestion;
    private Anwser[] currentAnwsers;
    private int stepProgress;

    private int globalProgress;
    private int maxProgress;

    private Dictionary<string, int> titlePoolsWeights;

    void Start()
    {
        titlePoolsWeights = new Dictionary<string, int>();

        currentStep = -1;
        questionsDone = new List<string>();
        anwsersSelected = new List<string>();

        currentAnwsers = new Anwser[2];

        QuizGUI.instance.SetProgressFill(0f);

        foreach (Step step in steps) maxProgress += step.stepsAmount;

        BookManager.instance.StartGame();

        NextStep();
    }


    /// <summary>
    /// Starts the next step
    /// </summary>
    private void NextStep()
    {
        currentStep++;
        stepProgress = 0;

        if (currentStep == steps.Length)
        {
            QuizGUI.instance.SetButtonHidden(true);
            QuizGUI.instance.SetQuestionLabel("");

            GenerateTitle();
            GenerateCoverElements();

            BookManager.instance.GameFinished();
        }
        else
        {
            print("New Step : " + currentStep);
            PoolNewQuestion();
        }

    }

    /// <summary>
    /// Pools a new question
    /// </summary>
    private void PoolNewQuestion()
    {
        Step step = steps[currentStep];

        if (stepProgress == step.stepsAmount) { NextStep(); return; }

        List<Question> subPool = new List<Question>();
        foreach (Question question in step.pool)
        {
            if (questionsDone.Contains(question.ID)) continue;

            if ((question.isFirstQuestion && stepProgress == 0) ||
                (!question.isFirstQuestion && stepProgress != 0 && RequirementsFulfilled(question)))
            {
                subPool.Add(question);
            }
        }

        if (subPool.Count == 0)
        {
            globalProgress += step.stepsAmount - stepProgress;
            QuizGUI.instance.SetProgressFill((float)globalProgress / maxProgress);

            NextStep();
        }
        else
        {
            currentQuestion = subPool[Random.Range(0, subPool.Count)];
            SelectRandomQuestions();
            // Display

            QuizGUI.instance.SetQuestion(currentQuestion, currentAnwsers);
        }
    }

    /// <summary>
    /// Selects random questions from the current question
    /// </summary>
    private void SelectRandomQuestions()
    {
        if (currentQuestion.anwsers.Count < 2)
        {
            Debug.LogError("Not enough anwsers for question : " + currentQuestion.ID);
            return;
        }

        List<int> pool = new List<int>();
        for (int i = 0; i < currentQuestion.anwsers.Count; i++)
        {
            pool.Add(i);
        }


        int selected = pool[Random.Range(0, pool.Count)];
        currentAnwsers[0] = currentQuestion.anwsers[selected];
        pool.Remove(selected);

        selected = pool[Random.Range(0, pool.Count)];
        currentAnwsers[1] = currentQuestion.anwsers[selected];
    }

    /// <summary>
    /// Checks if the requirements are fulfilled for a question 
    /// </summary>
    /// <param name="question">The question</param>
    /// <returns>True if the requirements are fulfilled</returns>
    private bool RequirementsFulfilled(Question question)
    {
        foreach (Question.Requirement requirement in question.requirements)
        {
            if (requirement.type == Question.Requirement.RequirementType.DONT && anwsersSelected.Contains(requirement.linkedAnwser)) return false;
            if (requirement.type == Question.Requirement.RequirementType.HAVE && !anwsersSelected.Contains(requirement.linkedAnwser)) return false;
        }

        return true;
    }

    /// <summary>
    /// Selects an anwser
    /// </summary>
    /// <param name="idxAnwser">The anwser's index</param>
    public void SelectAnwser(int idxAnwser)
    {
        stepProgress++;
        globalProgress++;
        QuizGUI.instance.SetProgressFill((float)globalProgress / maxProgress);

        questionsDone.Add(currentQuestion.ID);
        anwsersSelected.Add(currentAnwsers[idxAnwser].ID);

        foreach (string action in currentAnwsers[idxAnwser].actions)
        {
            ProcessAction(action);
        }
        currentQuestion = null;

        StartCoroutine(Routine_SelectionAnimation(idxAnwser));
    }

    /// <summary>
    /// Process an action
    /// </summary>
    /// <param name="action">The action's string</param>
    private void ProcessAction(string action)
    {
        print(action);

        string[] command = action.Split(new char[] { '(', ')' });
        string[] param = command[1].Split(';');

        switch (command[0])
        {
            case "AddPoolScore":
                if (!titlePoolsWeights.ContainsKey(param[0])) titlePoolsWeights.Add(param[0], 0);
                titlePoolsWeights[param[0]] += int.Parse(param[1]);
                break;
            case "AddElement":

                if (param[0].StartsWith("TYP_"))
                {
                    TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts/" + param[0]);
                    BookManager.instance.SetFontAuthor(font);
                    BookManager.instance.SetFontSyno(font);
                    BookManager.instance.SetFontTitle(font);
                }
                else if (param[0].StartsWith("MAT_"))
                {
                    // Change material
                }
                else
                {
                    Sprite sprite = Resources.Load<Sprite>("Elements/" + param[0]);
                    int layer = 0;
                    if (param[0].StartsWith("BG_")) layer = 0;
                    else if (param[0].StartsWith("ENL_")) layer = 3;
                    else if (param[0].StartsWith("DEC_")) layer = 1;
                    else if (param[0].StartsWith("SUJ_")) layer = 2;

                    SpriteData data = new()
                    {
                        level = layer,
                        sprite = sprite

                    };

                    BookManager.instance.AddToCouverture(data);
                }

                break;
        }
    }

    IEnumerator Routine_SelectionAnimation(int idxAnwser)
    {
        QuizGUI.instance.StartAnimationForButton(idxAnwser);

        yield return new WaitForSeconds(1f);

        QuizGUI.instance.ShowTransition();

        yield return new WaitForSeconds(0.5f);

        QuizGUI.instance.SetButtonHidden(false);
        QuizGUI.instance.SetQuestionLabel("");

        PoolNewQuestion();
    }

    void OnAnwserOne(InputValue input)
    {
        if (input.isPressed && currentQuestion != null)
        {
            SelectAnwser(0);
        }
    }

    void OnAnwserTwo(InputValue input)
    {
        if (input.isPressed && currentQuestion != null)
        {
            SelectAnwser(1);
        }
    }

    /// <summary>
    /// Generates the book's title
    /// </summary>
    public void GenerateTitle()
    {
        TitlePool[] pools = Resources.LoadAll<TitlePool>("Titles/");

        string selectedStart = "";
        string selectedEnd = "";
        int startMax = -1;
        int endMax = -1;
        int currentMax;

        foreach (TitlePool pool in pools)
        {
            currentMax = 0;

            if (titlePoolsWeights.ContainsKey(pool.ID)) currentMax = titlePoolsWeights[pool.ID];

            if (pool.place == TitlePool.TitlePlace.START && currentMax > startMax)
            {
                startMax = currentMax;
                selectedStart = pool.poolContent[Random.Range(0, pool.poolContent.Length)];
            }
            else if (pool.place == TitlePool.TitlePlace.END && currentMax > endMax)
            {
                endMax = currentMax;
                selectedEnd = pool.poolContent[Random.Range(0, pool.poolContent.Length)];
            }
        }

        print("Selected title : " + selectedStart + " " + selectedEnd);

        BookManager.instance.SetTitle(selectedStart + " " + selectedEnd);
    }

    /// <summary>
    /// Generates the cover elements
    /// </summary>
    public void GenerateCoverElements()
    {
        CoverElement[] coverElements = Resources.LoadAll<CoverElement>("CoverElements/");

        foreach (CoverElement coverElement in coverElements)
        {
            if (anwsersSelected.Contains(coverElement.linkedAnwser))
            {
                print("Adding " + coverElement.ID + " (" + coverElement.type + ", " + coverElement.placement + ")");
            }
        }
    }

}
