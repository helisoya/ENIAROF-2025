using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Utils class for the editor (THIS DOES NOT SHIP IN BUILD)
/// </summary>
public class Utils : MonoBehaviour
{
    [MenuItem("ENIAROF/Generate Titles")]
    public static void GenerateTitlePools()
    {
        List<string> lines = FileManager.ReadTextAsset(Resources.Load<TextAsset>("CSV/poolTitle"));
        string[] splitedLine;
        string[] splitedTitles;

        for (int i = 1; i < lines.Count; i++)
        {
            splitedLine = lines[i].Split("\t");

            TitlePool titlePool = ScriptableObject.CreateInstance<TitlePool>();
            titlePool.ID = splitedLine[0];
            titlePool.place = splitedLine[1] == "Début" ? TitlePool.TitlePlace.START : TitlePool.TitlePlace.END;
            splitedTitles = splitedLine[3].Split(",");
            titlePool.poolContent = new string[splitedTitles.Length];
            for (int j = 0; j < splitedTitles.Length; j++)
            {
                titlePool.poolContent[j] = splitedTitles[j];
                if (titlePool.poolContent[j].StartsWith(" "))
                {
                    titlePool.poolContent[j] = titlePool.poolContent[j].Substring(1);
                }
            }


            AssetDatabase.CreateAsset(titlePool, "Assets/Resources/Titles/" + titlePool.ID.Replace(" ", "") + ".asset");
            AssetDatabase.SaveAssets();
        }
    }

    private static Dictionary<string, Anwser> GenerateAnwsers()
    {
        Dictionary<string, Anwser> anwers = new Dictionary<string, Anwser>();

        List<string> linesAnwsers = FileManager.ReadTextAsset(Resources.Load<TextAsset>("CSV/anwsers"));
        string[] splitedLine;
        string[] splitedInner;

        for (int i = 1; i < linesAnwsers.Count; i++)
        {
            splitedLine = linesAnwsers[i].Split("\t");

            Anwser anwser = new Anwser();
            anwser.ID = splitedLine[0];
            anwser.label = splitedLine[4];
            anwser.actions = new List<string>();

            if (!string.IsNullOrEmpty(splitedLine[6]) && !string.IsNullOrWhiteSpace(splitedLine[6]))
            {
                splitedInner = splitedLine[6].Split(", ");
                for (int j = 0; j < splitedInner.Length; j++)
                {
                    anwser.actions.Add("AddPoolScore(" + splitedInner[j] + ";1)");
                }
            }
            if (!string.IsNullOrEmpty(splitedLine[7]) && !string.IsNullOrWhiteSpace(splitedLine[7]))
            {
                splitedInner = splitedLine[7].Split(", ");
                for (int j = 0; j < splitedInner.Length; j++)
                {
                    anwser.actions.Add("AddElement(" + splitedInner[j] + ";1)");
                }
            }
            anwers.Add(anwser.ID, anwser);
        }

        return anwers;
    }

    [MenuItem("ENIAROF/Generate Questions")]
    public static void GenerateQuestions()
    {
        Dictionary<string, Anwser> anwers = GenerateAnwsers();

        Step step;

        List<string> linesQuestions = FileManager.ReadTextAsset(Resources.Load<TextAsset>("CSV/questions"));
        string[] splitedLine;
        string[] splitedInner;

        for (int i = 1; i < linesQuestions.Count; i++)
        {
            splitedLine = linesQuestions[i].Split("\t");

            Question question = ScriptableObject.CreateInstance<Question>();
            question.ID = splitedLine[0];

            step = Resources.Load<Step>("Steps/" + splitedLine[1]);
            if (step != null)
            {
                if (step.pool == null) step.pool = new List<Question>();
                step.pool.Add(question);
                EditorUtility.SetDirty(step);
            }

            question.label = splitedLine[2];
            splitedInner = splitedLine[3].Split(", ");
            question.anwsers = new List<Anwser>();
            for (int j = 0; j < splitedInner.Length; j++)
            {
                if (anwers.ContainsKey(splitedInner[j]))
                {
                    question.anwsers.Add(anwers[splitedInner[j]]);
                }
                else
                {
                    print("-" + splitedInner[j] + "-");
                }
            }


            question.requirements = new List<Question.Requirement>();
            if (!string.IsNullOrEmpty(splitedLine[4]) && !string.IsNullOrWhiteSpace(splitedLine[4]))
            {
                splitedInner = splitedLine[4].Split(", ");
                for (int j = 0; j < splitedInner.Length; j++)
                {
                    question.requirements.Add(new Question.Requirement
                    {
                        type = Question.Requirement.RequirementType.HAVE,
                        linkedAnwser = splitedInner[j]
                    });
                }
            }

            if (!string.IsNullOrEmpty(splitedLine[5]) && !string.IsNullOrWhiteSpace(splitedLine[5]))
            {
                splitedInner = splitedLine[5].Split(", ");
                for (int j = 0; j < splitedInner.Length; j++)
                {
                    question.requirements.Add(new Question.Requirement
                    {
                        type = Question.Requirement.RequirementType.HAVE,
                        linkedAnwser = splitedInner[j]
                    });
                }
            }

            AssetDatabase.CreateAsset(question, "Assets/_Project/Resources/Questions/" + question.ID.Replace(" ", "") + ".asset");
            AssetDatabase.SaveAssets();
        }
    }
}
