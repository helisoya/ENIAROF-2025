using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;



[System.Serializable] public struct SpriteData
{
    [SerializeField] public Sprite sprite;
    [SerializeField] public int level;
}

public class Book : MonoBehaviour
{
    public struct BookData
    {
        public string title;
        public string author;
        public string synopsis;
        public Sprite spriteCouverture;
        public Sprite spriteBack;
        public TMP_FontAsset   fontTitle;
        public TMP_FontAsset   fontAuthor;
        public TMP_FontAsset   fontSynopsis;
    }
    
    private bool movingBack = false;
    private bool movingForward = false;
    
    [SerializeField] private GameObject bookGameObject;
    [SerializeField] private BookManager bookManager;
    [HideInInspector] public TextMeshPro bookName;
    [HideInInspector] public TextMeshPro bookSyno;
    public bool shown;
    [HideInInspector] public BookData bookData;
    public MeshRenderer meshRenderer;
    [SerializeField] public List<SpriteData> spritesCouverture;
    [SerializeField] public List<SpriteData> spritesBack;
    [HideInInspector] public SpriteMerger spriteMerger;
    
    private Outline outline;
    private Animator animator;
    private bool inspected;
    private float duration = 0.75f;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool isMoving;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get Component
        outline = GetComponent<Outline>();
        animator = GetComponent<Animator>();
        
        // Init Outline and initialTransform
        outline.enabled = false;
        startPosition = transform.position;
        startRotation = transform.rotation;
        bookGameObject.SetActive(shown);
        
    }

    // Rotate Book (inspect)
    public void RotateBook(Vector3 rotation)
    {
        bookGameObject.transform.eulerAngles += rotation;
    }
    
    private void OnMouseOver()
    {
        if (!shown) return;
        bookManager.bookSelected = this;
        
        if (!inspected && !bookManager.movingInspected)
        {
            outline.enabled = true;
            if (!movingBack)
                if (!(animator.GetCurrentAnimatorStateInfo(0).IsName("MouseExit") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) && !movingBack) 
                    animator.Play("MouseOver");
        }
    }

    private void OnMouseExit()
    {
        if (!shown) return;
        if (bookManager.bookSelected == this) bookManager.bookSelected = null;
        if (!inspected && !bookManager.movingInspected)
        {
            outline.enabled = false;
            if (movingForward) StartCoroutine(Wait(0.35f));
            else animator.Play("MouseExit");
        }
    }

    private IEnumerator Wait(float delay)
    {
        movingForward = true;
        yield return new WaitForSeconds(delay);
        animator.Play("MouseExit");
    }

    private void OnMouseDown()
    {
        if (!shown) return;
        if (!inspected)
        {
            // reset position of the last book inspected if there is one
            if (bookManager.bookInspecting)
            {
                if (bookManager.bookInspecting.isMoving) bookManager.bookInspecting.StopAllCoroutines();
                bookManager.bookInspecting.ResetPosition();
            }
            animator.Play("MouseExit");
            inspected = true;
            outline.enabled = false;
            
            bookManager.bookInspecting = this;
            StopAllCoroutines();
            StartCoroutine(MoveObject(bookGameObject.transform.position, bookManager.inspectTransform.position, bookGameObject.transform.rotation, bookManager.inspectTransform.rotation));
        }
    }

    // Put Book in Lib place
    public void ResetPosition()
    {
        StartCoroutine(MoveObject(bookGameObject.transform.position, startPosition, bookGameObject.transform.rotation,  startRotation));
        inspected = false;
    }
    
    IEnumerator MoveObject(Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot)
    {
        float time = 0;
        isMoving = true;
        while (time < duration)
        {
            float t = time / duration;
            float easedT = t * t * (3 - 2 * t);

            bookGameObject.transform.position = Vector3.Lerp(startPos, endPos, easedT);
            bookGameObject.transform.rotation = Quaternion.Slerp(startRot, endRot, easedT);

            time += Time.deltaTime;
            yield return null;
        }

        isMoving = false;
        transform.position = endPos;
        transform.rotation = endRot;
    }

    private void Merge(MeshRenderer _meshRenderer, List<SpriteData> spriteList, bool couverture)
    {
        int textureSize = 2048;
        Texture2D newTexture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        
        Color[] clearPixels = new Color[textureSize * textureSize];
        for (int i = 0; i < clearPixels.Length; i++)
            clearPixels[i] = new Color(1, 1, 1, 0);
    
        newTexture.SetPixels(clearPixels);
        
        foreach (var spriteData in spriteList)
        {
            Texture2D spriteTexture = spriteData.sprite.texture;
            Color[] spritePixels = spriteTexture.GetPixels();
        
            int startX = Mathf.RoundToInt(spriteData.sprite.rect.x);
            int startY = Mathf.RoundToInt(spriteData.sprite.rect.y);
            int width = Mathf.RoundToInt(spriteData.sprite.rect.width);
            int height = Mathf.RoundToInt(spriteData.sprite.rect.height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color spriteColor = spritePixels[y * width + x];
                    if (spriteColor.a > 0)
                    {
                        int pixelIndex = (startY + y) * textureSize + (startX + x);
                        clearPixels[pixelIndex] = spriteColor;
                    }
                }
            }
        }
        
        newTexture.SetPixels(clearPixels);
        newTexture.Apply();
        
        var finalSprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), new Vector2(0.5f, 0.5f));
        finalSprite.name = "New Sprite";
        
        if (couverture) 
            _meshRenderer.materials[0].mainTexture = finalSprite.texture;
        else 
            _meshRenderer.materials[1].mainTexture = finalSprite.texture;
    }

    public void ShowBook()
    {
        spritesCouverture.Sort((a, b) => a.level.CompareTo(b.level));
        spritesBack.Sort((a, b) => a.level.CompareTo(b.level));
        Merge(meshRenderer, spritesCouverture, true);
        Merge(meshRenderer, spritesBack, false);
    }


    public void SetOnAnimStartBook()
    {
        movingForward = true;
        movingBack = false;
    }
    
    public void SetOffAnimStartBook()
    {
        movingForward = false;
        movingBack = true;
    }
}
