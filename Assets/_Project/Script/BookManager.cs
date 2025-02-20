using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class BookManager : MonoBehaviour
{
    public static BookManager instance;

    void Awake() { instance = this; }

    public Transform inspectTransform;

    [HideInInspector] public Book bookInspecting;
    [HideInInspector] public Book bookSelected;

    public bool movingInspected;
    private bool startMouseOnInspected;
    [SerializeField] private Book[] books = new Book[96];

    private int caseTooMuch;
    private int nextBook;

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
            else if (nextBook == -1) nextBook = i; //First index with book unused
        }

        // case we don't have place for another book
        if (nextBook == -1)
        {
            nextBook = caseTooMuch;
            caseTooMuch++;
        }
    }
    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // Molette vers le haut
        if (scroll > 0f)
        {
            if (bookInspecting != null)
            {
                Vector3 pos = bookInspecting.bookGameObject.transform.localPosition;
                float z = Mathf.Clamp(pos.z - scroll * 10f, 5f, 10f);
                pos.z = z;
                bookInspecting.bookGameObject.transform.localPosition = pos;
            }
        }
        // Molette vers bas
        else if (scroll < 0f)
        {
            if (bookInspecting != null)
            {
                Vector3 pos = bookInspecting.bookGameObject.transform.localPosition;
                float z = Mathf.Clamp(pos.z - scroll * 10f, 5f, 10f);
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
                bookInspecting.ResetPosition();
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
        // case we restart a game (nextBook = -1 when game is finished)
        if (nextBook == -1)
            GetUnusedBook();

        // Display questions view

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
        books[nextBook].meshRenderer.materials[2].SetFloat("_IsHolographic", holographic ? 1 : 0);
        books[nextBook].meshRenderer.materials[2].SetFloat("_IsGolden", golden ? 1 : 0);
    }

    public void SetFrontMaterial(bool holographic, bool golden)
    {
        books[nextBook].meshRenderer.materials[1].SetFloat("_IsHolographic", holographic ? 1 : 0);
        books[nextBook].meshRenderer.materials[1].SetFloat("_IsGolden", golden ? 1 : 0);
    }

    public void SetFontAuthor(TMP_FontAsset font)
    {
        //books[nextBook].bookAutho.font = font;  // ADD THINGS HERE
        books[nextBook].bookData.fontAuthor = font;
    }

    public void GameFinished()
    {
        // set book position behind Cam, set display camera into lib one, set book in inspectionPlace.

        books[nextBook].shown = true;
        books[nextBook].ShowBook();
    }

    public void CreateBook(Book book)
    {
        books[nextBook] = book;
    }
}
