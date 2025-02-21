using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an element on the cover
/// </summary>
[CreateAssetMenu(fileName = "CoverElement", menuName = "ENIAROF/CoverElement")]
public class CoverElement : ScriptableObject
{

	public enum CoverElementType
	{
		BACKGROUNDCOLOR,
		ENLUMINURE,
		SCENERY,
		SUBJECT,
		TYPOGRAPHY,
		EDITOR,
		AUTHOR,
		MATERIAL,
		SET
	}

	public enum CoverElementPlacement
	{
		FRONT,
		BACK,
		BOTH
	}

	public string ID;
	public CoverElementType type;
	public CoverElementPlacement placement;
	public List<string> linkedElements;
}
