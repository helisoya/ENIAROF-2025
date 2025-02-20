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

    private Dictionary<string, CoverElement> elements;
    private Dictionary<string, int> titlePoolsWeights;
    private Dictionary<string, int> elementsWeights;

    void Start()
    {
        titlePoolsWeights = new Dictionary<string, int>();
        elementsWeights = new Dictionary<string, int>();
        elements = new Dictionary<string, CoverElement>();

        CoverElement[] elementsArray = Resources.LoadAll<CoverElement>("Elements/");
        foreach (CoverElement element in elementsArray)
        {
            elements.Add(element.ID, element);
        }

        BoostRandomTitles();

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
    /// Boost random titles (masculine / feminine)
    /// </summary>
    private void BoostRandomTitles()
    {
        bool boostFem = Random.Range(0, 2) == 0;

        TitlePool[] pools = Resources.LoadAll<TitlePool>("Titles/");
        foreach (TitlePool pool in pools)
        {
            if ((boostFem && pool.ID.Contains("_Fem_")) || (!boostFem && pool.ID.Contains("_Masc_")))
            {
                titlePoolsWeights.Add(pool.ID, 100);
            }
        }
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

            /*
            if ((question.isFirstQuestion && stepProgress == 0) ||
                (!question.isFirstQuestion && stepProgress != 0 && RequirementsFulfilled(question)))
            */

            if (RequirementsFulfilled(question))
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

                if (!elements.ContainsKey(param[0]))
                {
                    Debug.LogError("Missing Element : " + param[0]);
                    return;
                }

                CoverElement element = elements[param[0]];
                if (element.type == CoverElement.CoverElementType.SET)
                {
                    foreach (string elementInSet in element.linkedElements)
                    {
                        if (!elementsWeights.ContainsKey(elementInSet)) elementsWeights.Add(elementInSet, 0);
                        elementsWeights[elementInSet]++;
                    }
                }
                else
                {
                    if (!elementsWeights.ContainsKey(element.ID)) elementsWeights.Add(element.ID, 0);
                    elementsWeights[element.ID]++;
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
        Dictionary<SearchElementKey, SearchElementValue> search = new Dictionary<SearchElementKey, SearchElementValue>();
        CoverElement coverElement;
        SearchElementKey key;
        SearchElementValue value;
        int scoreForElment;

        foreach (string element in elementsWeights.Keys)
        {
            coverElement = elements[element];
            scoreForElment = elementsWeights[element];

            if (coverElement.placement != CoverElement.CoverElementPlacement.BACK)
            {
                key = new SearchElementKey
                {
                    placement = CoverElement.CoverElementPlacement.FRONT,
                    type = coverElement.type
                };

                if (search.ContainsKey(key))
                {
                    if (search[key].score < scoreForElment)
                    {
                        search[key].score = scoreForElment;
                        search[key].value = coverElement;
                    }
                }
                else
                {
                    value = new SearchElementValue
                    {
                        value = coverElement,
                        score = scoreForElment
                    };
                    search.Add(key, value);
                }
            }
            if (coverElement.placement != CoverElement.CoverElementPlacement.FRONT)
            {
                key = new SearchElementKey
                {
                    placement = CoverElement.CoverElementPlacement.BACK,
                    type = coverElement.type
                };

                if (search.ContainsKey(key))
                {
                    if (search[key].score < scoreForElment)
                    {
                        search[key].score = scoreForElment;
                        search[key].value = coverElement;
                    }
                }
                else
                {
                    value = new SearchElementValue
                    {
                        value = coverElement,
                        score = scoreForElment
                    };
                    search.Add(key, value);
                }
            }

        }

        foreach (SearchElementValue v in search.Values)
        {
            AddElement(v.value);
        }
    }

    private class SearchElementKey
    {
        public CoverElement.CoverElementPlacement placement;
        public CoverElement.CoverElementType type;

        public override bool Equals(object? obj) => obj is SearchElementKey other && this.Equals(other);

        public bool Equals(SearchElementKey p) => placement == p.placement && type == p.type;

        public override int GetHashCode() => (placement, type).GetHashCode();

        public static bool operator ==(SearchElementKey lhs, SearchElementKey rhs) => lhs.Equals(rhs);

        public static bool operator !=(SearchElementKey lhs, SearchElementKey rhs) => !(lhs == rhs);
    }
    private class SearchElementValue
    {
        public CoverElement value;
        public int score;
    }

    /// <summary>
    /// Adds an element to the current book
    /// </summary>
    /// <param name="element">The element to add</param>
    private void AddElement(CoverElement element)
    {

        if (element.type == CoverElement.CoverElementType.TYPOGRAPHY)
        {
            TMP_FontAsset font = Resources.Load<TMP_FontAsset>("Fonts/" + element.ID);
            BookManager.instance.SetFontAuthor(font);
            BookManager.instance.SetFontSyno(font);
            BookManager.instance.SetFontTitle(font);
        }
        else if (element.type == CoverElement.CoverElementType.MATERIAL)
        {
            bool golden = element.ID.Contains("Golden");
            bool holo = element.ID.Contains("Holo");
            // Change material
            if (element.placement != CoverElement.CoverElementPlacement.FRONT) BookManager.instance.SetBackMaterial(holo, golden);
            if (element.placement != CoverElement.CoverElementPlacement.BACK) BookManager.instance.SetBackMaterial(holo, golden);
        }
        else
        {
            Sprite sprite = Resources.Load<Sprite>("Sprites/Elements/" + element.ID);
            int layer = 0;
            if (element.type == CoverElement.CoverElementType.BACKGROUNDCOLOR) layer = 0;
            else if (element.type == CoverElement.CoverElementType.ENLUMINURE) layer = 3;
            else if (element.type == CoverElement.CoverElementType.SCENERY) layer = 1;
            else if (element.type == CoverElement.CoverElementType.SUBJECT) layer = 2;

            SpriteData data = new()
            {
                level = layer,
                sprite = sprite

            };

            if (!(element.placement == CoverElement.CoverElementPlacement.BACK)) BookManager.instance.AddToCouverture(data);
            if (!(element.placement == CoverElement.CoverElementPlacement.FRONT)) BookManager.instance.AddToBack(data);
        }
    }

}
