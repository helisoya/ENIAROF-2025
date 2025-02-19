using System.Collections.Generic;
using UnityEngine;

public class SpriteMerger : MonoBehaviour
{
    [SerializeField] private Sprite[] spritesToMerge;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private int width = 2048;
    [SerializeField] private int height = 2048;

    void Start()
    {
        Merge();        
    }
    
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
        
        meshRenderer.materials[1].mainTexture = finalSprite.texture;
        meshRenderer.materials[0].mainTexture = finalSprite.texture;
        //finalSpriteRenderer.sprite = finalSprite;
    }
    
    public void Merge(MeshRenderer _meshRenderer, List<SpriteData> spriteList, bool couverture)
    {
        Resources.UnloadUnusedAssets();
        var newTexture = new Texture2D(width, height);
        
        // Background transparent
        for(int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                newTexture.SetPixel(x, y, new Color(1, 1, 1, 0));
        
        for (int i = 0; i < spriteList.Count; i++)
            for(int x = 0; x < spriteList[i].sprite.texture.width; x++)
                for (int y = 0; y < spriteList[i].sprite.texture.height; y++)
                {
                    var color = spriteList[i].sprite.texture.GetPixel(x, y).a == 0 ? 
                        newTexture.GetPixel(x, y) : 
                        spriteList[i].sprite.texture.GetPixel(x, y);
                    newTexture.SetPixel(x, y, color);
                }
        newTexture.Apply();
        var finalSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), new Vector2(0.5f, 0.5f));
        finalSprite.name = "New Sprite";
        
        if (couverture) _meshRenderer.materials[0].mainTexture = finalSprite.texture;
        else _meshRenderer.materials[1].mainTexture = finalSprite.texture;
    }
}
