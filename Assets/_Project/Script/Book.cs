using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public struct SpriteData
{
    public Sprite sprite;
    public int level;
}

public class Book : MonoBehaviour
{
    struct BookData
    {
        public string title;
        public string author;
        public string synopsis;
        public Sprite spriteCouverture;
        public Sprite spriteBack;
    }
    
    [SerializeField] private GameObject bookGameObject;
    [SerializeField] private BookManager bookManager;
    [SerializeField] private TextMeshPro bookName;
    [SerializeField] private TextMeshPro bookSyno;
    [SerializeField] private bool shown;
    
    private BookData bookData;
    private Outline outline;
    private Animator animator;
    
    private bool inspected;
    private float duration = 0.75f;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private MeshRenderer meshRenderer;
    private bool isMoving;
    
    private List<SpriteData> spritesCouverture;
    private List<SpriteData> spritesBack;
    private TMP_FontAsset fontTitle;
    private TMP_FontAsset fontSyno;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get Component
        outline = GetComponent<Outline>();
        animator = GetComponent<Animator>();
        meshRenderer = GetComponent<MeshRenderer>();
        // Init Outline and initialTransform
        outline.enabled = false;
        startPosition = transform.position;
        startRotation = transform.rotation;
        meshRenderer.enabled = shown;
        bookName.enabled = shown;
        bookSyno.enabled = shown;
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
            if (!(animator.GetCurrentAnimatorStateInfo(0).IsName("MouseExit") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)) 
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
            StartCoroutine(Wait(0.35f));
        }
    }

    private IEnumerator Wait(float delay)
    {
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
            StartCoroutine(MoveObject(startPosition, bookManager.inspectTransform.position, startRotation, bookManager.inspectTransform.rotation));
        }
    }

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

    public void SetTitle(string title)
    {
        bookName.text = title;
        bookData.title = title;
    }

    public void SetSyno(string syno)
    {
        bookSyno.text = syno;
        bookData.synopsis = syno;
    }

    public void SetAuthor(string author)
    {
        // ADD THINGS HERE
        bookData.author = author;
    }

    public void AddToCouverture(SpriteData _spriteData)
    {
        spritesCouverture.Add(_spriteData);
        spritesCouverture.OrderBy(x => x.level).ToList();
    }
    
    public void AddToBack(SpriteData _spriteData)
    {
        spritesBack.Add(_spriteData);
        spritesBack.OrderBy(x => x.level).ToList();
    }

    public void SetFontTitle(TMP_FontAsset font)
    {
        fontTitle = font;
        bookName.font = fontTitle;
    }

    public void SetFontSyno(TMP_FontAsset font)
    {
        fontSyno = font;
        bookSyno.font = fontSyno;
    }
    
    public void CreateBook(TMP_FontAsset font)
    {
        
    }

    private void SortListSprites(List<SpriteData> spriteList)
    {
        spriteList.OrderBy(x => x.level).ToList();
    }
}
