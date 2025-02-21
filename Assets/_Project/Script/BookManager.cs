using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class BookManager : MonoBehaviour
{
    public static BookManager instance;

    void Awake() { instance = this; }

    public Transform inspectTransform;
    public Transform beforeEditTransform;
    public Transform editTransform;

    [HideInInspector] public Book bookInspecting;
    [HideInInspector] public Book bookSelected;

    public bool movingInspected;
    private bool startMouseOnInspected;
    public Book[] books = new Book[96];

    private int caseTooMuch;
    [HideInInspector] public int nextBook;

    private void Start()
    {
        caseTooMuch = 0;
        nextBook = -1;

        LoadBooks(); // should load bookData in each book

        for (int i = 0; i < books.Length; i++)
        {
            if (books[i].shown)
            {
                books[i].ShowBook();
            }
        }
    }
    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // Molette vers le haut
        if (scroll != 0f)
        {
            if (bookInspecting != null)
            {
                Vector3 pos = bookInspecting.bookGameObject.transform.localPosition;
                float z = Mathf.Clamp(pos.z - scroll * 1f, -13.19f, -12.75f);
                pos.z = z;
                bookInspecting.bookGameObject.transform.localPosition = pos;
            }
        }

        if (Input.GetMouseButtonDown(0) && bookSelected == bookInspecting) startMouseOnInspected = true;
        if (Input.GetMouseButtonUp(0)) startMouseOnInspected = false;
        if (Input.GetMouseButton(0) && bookInspecting) // 0 = Click gauche
        {
            if (!startMouseOnInspected)
            {
                bookInspecting.StopAllCoroutines();
                bookInspecting.ResetPosition(bookInspecting.duration, bookInspecting.rangee);
                bookInspecting = null;
                return;
            }
            movingInspected = true;
            Vector3 rotation = new Vector3(0, -Input.GetAxis("Mouse X") * 5.0f, Input.GetAxis("Mouse Y") * 5.0f);
            bookInspecting.RotateBook(rotation);
        }
        else
        {
            movingInspected = false;
        }
    }

    // init Books from save
    private void LoadBooks()
    {

    }

    // init nextBook
    private void GetUnusedBook()
    {
        for (int i = 0; i < books.Length; i++)
        {
            if (!books[i].shown)
            {
                nextBook = i;
                return;
            }
        }
        nextBook = caseTooMuch;
        caseTooMuch++;
    }

    // Called every time we want to create another book
    public void StartGame()
    {
        GetUnusedBook();
        books[nextBook].SetPositionBeforeEditing();
        books[nextBook].rangee = false;
    }

    public void SetTitle(string title)
    {
        books[nextBook].bookName.text = title;
        books[nextBook].bookData.title = title;
        books[nextBook].UITextTitle.text = "\" " + title + " \"";
    }

    public void SetSyno(string syno)
    {
        books[nextBook].bookSyno.text = syno;
        books[nextBook].bookData.synopsis = syno;
        books[nextBook].UITextTitle.text = syno;
    }

    public void SetAuthor(string author)
    {
        // ADD THINGS HERE
        //books[nextBook].bookAutho.text = syno;
        books[nextBook].bookData.author = author;
    }

    public void AddToCouverture(SpriteData _spriteData)
    {
        books[nextBook].spritesCouverture.Add(_spriteData);
        if (_spriteData.level == 0)
        {
            books[nextBook].bookData.spriteSide = _spriteData.sprite;
            books[nextBook].meshRenderer.materials[0].mainTexture = _spriteData.sprite.texture;

            float color = _spriteData.sprite.texture.GetPixel(_spriteData.sprite.texture.width / 2, _spriteData.sprite.texture.height / 2).grayscale;
            books[nextBook].bookName.color = color < 0.5f ? Color.white : Color.black;
        }
        //books[nextBook].spritesCouverture.OrderBy(x => x.level).ToList();
        //books[nextBook].spriteMerger.Merge(books[nextBook].meshRenderer, books[nextBook].spritesCouverture, true);
    }

    public void AddToBack(SpriteData _spriteData)
    {
        books[nextBook].spritesBack.Add(_spriteData);
        //books[nextBook].spritesBack.OrderBy(x => x.level).ToList();
        //books[nextBook].spriteMerger.Merge(books[nextBook].meshRenderer, books[nextBook].spritesBack, false);
    }

    public void SetFontTitle(TMP_FontAsset font)
    {
        books[nextBook].bookName.font = font;
        books[nextBook].bookData.fontTitle = font;

    }

    public void SetFontSyno(TMP_FontAsset font)
    {
        books[nextBook].bookSyno.font = font;
        books[nextBook].bookData.fontSynopsis = font;
    }

    public void SetBackMaterial(bool holographic, bool golden)
    {
        books[nextBook].meshRenderer.materials[1].SetFloat("_IsHolographic", holographic ? 1 : 0);
        books[nextBook].meshRenderer.materials[1].SetFloat("_IsGolden", golden ? 1 : 0);
    }

    public void SetFrontMaterial(bool holographic, bool golden)
    {
        books[nextBook].meshRenderer.materials[2].SetFloat("_IsHolographic", holographic ? 1 : 0);
        books[nextBook].meshRenderer.materials[2].SetFloat("_IsGolden", golden ? 1 : 0);
    }

    public void SetFontAuthor(TMP_FontAsset font)
    {
        //books[nextBook].bookAutho.font = font;  // ADD THINGS HERE
        books[nextBook].bookData.fontAuthor = font;
    }

    public Book GameFinished()
    {
        Book current = books[nextBook];
        // set book position behind Cam, set display camera into lib one, set book in inspectionPlace.
        current.shown = true;
        current.ShowBook();
        //current.ResetPosition(current.duration * 3f);
        current.SetPositionEditing();
        //nextBook = -1;

        return current;
    }

    public void CreateBook(Book book)
    {
        books[nextBook] = book;
    }
}
