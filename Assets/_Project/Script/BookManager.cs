using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

[System.Serializable]
public class BookList
{
    public List<BookDataWrapper> books;

    public BookList(List<BookDataWrapper> books)
    {
        this.books = books;
    }
}

[System.Serializable]
public class BookDataWrapper
{
    public Book.BookData data;
    public bool shown;

    public BookDataWrapper(Book.BookData data, bool shown)
    {
        this.data = data;
        this.shown = shown;
    }
}

public class BookManager : MonoBehaviour
{
    public static BookManager instance;

    void Awake()
    {
        instance = this;
        savePath = Path.Combine(Application.persistentDataPath, "books.json");
        LoadBooks();
    }

    public Transform inspectTransform;
    public Transform beforeEditTransform;
    public Transform editTransform;

    [HideInInspector] public Book bookInspecting;
    [HideInInspector] public Book bookSelected;

    public bool movingInspected;
    private bool startMouseOnInspected;
    public Book[] books = new Book[96];
    private string savePath;

    private int caseTooMuch;
    [HideInInspector] public int nextBook;

    private void Start()
    {
        caseTooMuch = 0;
        nextBook = -1;
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

        if (Input.GetKey(KeyCode.P))
        {
            EraseSave();
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

    private void EraseSave()
    {
        if (File.Exists(savePath)) File.Delete(savePath);
    }

    public void SaveBooks()
    {
        List<BookDataWrapper> booksToSave = new List<BookDataWrapper>();

        foreach (var book in books)
        {
            if (book.shown)
            {
                booksToSave.Add(new BookDataWrapper(book.bookData, book.shown));
            }
        }
        
        BookList bookList = new BookList(booksToSave);
        string json = JsonUtility.ToJson(bookList, true);

        try
        {
            File.WriteAllText(savePath, json);
            Debug.Log("Books saved successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving books: " + e.Message);
        }
    }
    
    public void LoadBooks()
    {
        if (!File.Exists(savePath)) return;
        
        try
        {
            string json = File.ReadAllText(savePath);
            
            BookList loadedBooks = JsonUtility.FromJson<BookList>(json);
            
            for (int i = 0; i < loadedBooks.books.Count; i++)
            {
                var bookWrapper = loadedBooks.books[i];
                Book book = books[i];
                book.bookData = bookWrapper.data;
                book.shown = bookWrapper.shown;
                book.ShowBook();
                book.gameObject.SetActive(book.shown);
            }

            Debug.Log("Books loaded successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError("Error loading books: " + e.Message);
        }
        
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
        books[nextBook].bookAuthor.text = author;
        books[nextBook].UITextAuthor.text = author;
    }

    public void AddToCouverture(SpriteData _spriteData)
    {
        books[nextBook].bookData.spriteCouverture.Add(_spriteData);
        if (_spriteData.level == 0)
        {
            books[nextBook].bookData.spriteSide = _spriteData.sprite;
            //books[nextBook].meshRenderer.materials[3].mainTexture = _spriteData.GetSprite().texture;

            float color = _spriteData.GetSprite().texture.GetPixel(_spriteData.GetSprite().texture.width / 2, _spriteData.GetSprite().texture.height / 2).grayscale;
            books[nextBook].bookName.color = color < 0.5f ? Color.white : Color.black;
            books[nextBook].bookAuthor.color = color < 0.5f ? Color.white : Color.black;
        }
        //books[nextBook].spritesCouverture.OrderBy(x => x.level).ToList();
        //books[nextBook].spriteMerger.Merge(books[nextBook].meshRenderer, books[nextBook].spritesCouverture, true);
    }

    public void AddToBack(SpriteData _spriteData)
    {
        books[nextBook].bookData.spriteBack.Add(_spriteData);
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

        books[nextBook].meshRenderer.materials[3].SetFloat("_IsHolographic", holographic ? 1 : 0);
        books[nextBook].meshRenderer.materials[3].SetFloat("_IsGolden", golden ? 1 : 0);

        books[nextBook].meshRenderer.materials[1].SetFloat("_IsHolographic", holographic ? 1 : 0);
        books[nextBook].meshRenderer.materials[1].SetFloat("_IsGolden", golden ? 1 : 0);

        books[nextBook].bookData.holo = holographic ? "true" : "false";
        books[nextBook].bookData.golden= golden? "true" : "false";
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
        
        SaveBooks();

        return current;
    }
}
