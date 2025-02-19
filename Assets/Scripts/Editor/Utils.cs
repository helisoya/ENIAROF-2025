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
}
