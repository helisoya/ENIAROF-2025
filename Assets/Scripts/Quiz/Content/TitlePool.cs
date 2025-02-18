using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a title pool
/// </summary>
[CreateAssetMenu(fileName = "TitlePool", menuName = "ENIAROF/TitlePool")]
public class TitlePool : ScriptableObject
{
	public string ID;
	public enum TitlePlace
	{
		START,
		END
	}

	public TitlePlace place;
	public string[] poolContent;
	public string[] linkedAnwsers;
}
