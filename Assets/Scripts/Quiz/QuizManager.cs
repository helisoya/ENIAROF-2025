using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the quiz
/// </summary>
public class QuizManager : MonoBehaviour
{
    private Step[] steps;
    private int currentStep;
    private List<string> questionsDone;
    private List<string> anwsersSelected;
    private Question currentQuestion;
    private int stepProgress;

    private int globalProgress;
    private int maxProgress;

    void Start()
    {
        steps = Resources.LoadAll<Step>("Steps/");
        currentStep = -1;
        questionsDone = new List<string>();
        anwsersSelected = new List<string>();

        QuizGUI.instance.SetProgressFill(0f);

        foreach (Step step in steps) maxProgress += step.stepsAmount;

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
            // Display

            QuizGUI.instance.SetQuestion(currentQuestion);
        }
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
        anwsersSelected.Add(currentQuestion.anwsers[idxAnwser].ID);

        foreach (string action in currentQuestion.anwsers[idxAnwser].actions)
        {
            print(action);
        }
        currentQuestion = null;

        StartCoroutine(Routine_SelectionAnimation(idxAnwser));
    }

    IEnumerator Routine_SelectionAnimation(int idxAnwser)
    {
        QuizGUI.instance.StartAnimationForButton(idxAnwser);

        yield return new WaitForSeconds(1f);

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
            foreach (string anwser in pool.linkedAnwsers)
            {
                if (anwsersSelected.Contains(anwser)) currentMax++;
            }

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
    }

}
