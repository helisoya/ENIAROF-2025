using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;


[System.Serializable] public struct SpriteData
{
    [SerializeField] public Sprite sprite;
    [SerializeField] public int level;
}

public class Book : MonoBehaviour
{
    [System.Serializable]
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
    
    [HideInInspector] public GameObject bookGameObject;
    [SerializeField] private BookManager bookManager;
    [HideInInspector] public TextMeshPro bookName;
    [HideInInspector] public TextMeshPro bookSyno;
    public bool shown;
    [HideInInspector] public BookData bookData;
    public MeshRenderer meshRenderer;
    [SerializeField] public List<SpriteData> spritesCouverture;
    [SerializeField] public List<SpriteData> spritesBack;
    [HideInInspector] public SpriteMerger spriteMerger;
    [SerializeField] private Volume postProcess;
    [SerializeField] private RawImage darkImage;
    private DepthOfField depthOfField;
    public TextMeshProUGUI UITextTitle;
    public TextMeshProUGUI UITextSyn;
    
    private Outline outline;
    private Animator animator;
    private bool inspected;
    private float duration = 0.75f;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool isMoving;

    private bool audioPlayed = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get Component
        outline = GetComponent<Outline>();
        animator = GetComponent<Animator>();
        if (postProcess.profile.TryGet<DepthOfField>(out var _depthOfField))
            depthOfField = _depthOfField;
        
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
        if (!shown ) return;
        bookManager.bookSelected = this;
        if (bookManager.bookInspecting != null) return;
        
        if (!inspected && !bookManager.movingInspected)
        {
            outline.enabled = true;
            if (!movingBack)
                if (!(animator.GetCurrentAnimatorStateInfo(0).IsName("MouseExit") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) && !movingBack)
                {
                    animator.Play("MouseOver");
                    if (!audioPlayed) // Prevent sound from playing repeatedly while hovering
                    {
                        AudioManager.instance.PlayOneShot(FMODEvents.instance.BookHoverIn_SFX, this.transform.position);
                        audioPlayed = true; // Set flag to true to prevent replaying
                    }
                }


        }
    }

    private void OnMouseExit()
    {
        if (!shown) return;
        if (bookManager.bookSelected == this) bookManager.bookSelected = null;
        if (bookManager.bookInspecting != null) return;
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
        AudioManager.instance.PlayOneShot(FMODEvents.instance.BookHoverOut_SFX, this.transform.position);
        audioPlayed = false; // Reset flag
    }

    private void OnMouseDown()
    {
        if (!shown) return;
        if (!inspected && bookManager.bookInspecting == null)
        {
            // reset position of the last book inspected if there is one
            if (bookManager.bookInspecting)
            {
                if (bookManager.bookInspecting.isMoving) bookManager.bookInspecting.StopAllCoroutines();
                bookManager.bookInspecting.ResetPosition();
                AudioManager.instance.PlayOneShot(FMODEvents.instance.BookStored_SFX, this.transform.position);
            }
            UITextTitle.text = "\" " + bookData.title + " \"";
            UITextSyn.text = bookData.synopsis;
            animator.Play("MouseExit");
            AudioManager.instance.PlayOneShot(FMODEvents.instance.BookPick_SFX, this.transform.position);
            inspected = true;
            outline.enabled = false;
            
            bookManager.bookInspecting = this;
            StopAllCoroutines();
            StartCoroutine(MoveObject(bookGameObject.transform.position, bookManager.inspectTransform.position, bookGameObject.transform.rotation, bookManager.inspectTransform.rotation, false));
        }
    }

    // Put Book in Lib place
    public void ResetPosition()
    {
        StartCoroutine(MoveObject(bookGameObject.transform.position, startPosition, bookGameObject.transform.rotation,  startRotation, true));
        inspected = false;
        
        AudioManager.instance.PlayOneShot(FMODEvents.instance.BookStored_SFX, this.transform.position);
    }
    
    IEnumerator MoveObject(Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot, bool reset)
    {
        float time = 0;
        isMoving = true;
        float aStart = darkImage.color.a;
        float aEnd = reset ? 0.0f : 200/255f;
        float dofStart = darkImage.color.a;
        float dofEnd = reset ? 11.2f : 8.67f;
        float descStart = darkImage.color.a;
        float descEnd = reset ? 0.0f : 1.0f;
        
        while (time < duration)
        {
            float t = time / duration;
            float easedT = t * t * (3 - 2 * t);

            bookGameObject.transform.position = Vector3.Lerp(startPos, endPos, easedT);
            bookGameObject.transform.rotation = Quaternion.Slerp(startRot, endRot, easedT);
            
            float a = Mathf.Lerp(aStart, aEnd, easedT);
            Color color = darkImage.color;
            color.a = a;
            darkImage.color = color;

            depthOfField.gaussianStart.value = Mathf.Lerp(dofStart, dofEnd, easedT);
            
            Color colorDesc = UITextTitle.color;
            colorDesc.a = Mathf.Lerp(descStart, descEnd, easedT);
            UITextTitle.color = colorDesc;
            UITextSyn.color = colorDesc;
            
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
        {
            _meshRenderer.materials[1].mainTexture = finalSprite.texture;
            bookData.spriteCouverture = finalSprite;
        }
        else
        {
            _meshRenderer.materials[2].mainTexture = finalSprite.texture;
            bookData.spriteBack = finalSprite;
        }
    }

    public void ShowBook()
    {
        spritesCouverture.Sort((a, b) => a.level.CompareTo(b.level));
        spritesBack.Sort((a, b) => a.level.CompareTo(b.level));
        Merge(meshRenderer, spritesCouverture, true);
        Merge(meshRenderer, spritesBack, false);
        
        FileManager.SaveJSON(FileManager.savPath+"/book.json",bookData);
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
