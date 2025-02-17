using UnityEngine;

public class SpriteMerger : MonoBehaviour
{
    [SerializeField] private Sprite[] spritesToMerge;
    [SerializeField] private SpriteRenderer finalSpriteRenderer;
    [SerializeField] private int width = 500;
    [SerializeField] private int height = 500;

    private void Merge()
    {
        Resources.UnloadUnusedAssets();
        var newTexture = new Texture2D(width, height);
        
        // Background transparent
        for(int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                newTexture.SetPixel(x, y, new Color(1, 1, 1, 0));
        
        //
        for (int i = 0; i < spritesToMerge.Length; i++)
            for(int x = 0; x < spritesToMerge[i].texture.width; x++)
                for (int y = 0; y < spritesToMerge[i].texture.height; y++)
                {
                    var color = spritesToMerge[i].texture.GetPixel(x, y).a == 0 ? 
                        newTexture.GetPixel(x, y) : 
                        spritesToMerge[i].texture.GetPixel(x, y);
                    newTexture.SetPixel(x, y, color);
                }
        newTexture.Apply();
        var finalSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), new Vector2(0.5f, 0.5f));
        finalSprite.name = "New Sprite";
        finalSpriteRenderer.sprite = finalSprite;
    }
}
