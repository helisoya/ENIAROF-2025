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
		AUTHOR
	}

	public enum CoverElementPlacement
	{
		FRONT,
		BACK
	}

	public string ID;
	public CoverElementType type;
	public CoverElementPlacement placement;
	public string linkedAnwser;
}
