using UnityEngine;

public class BookManager : MonoBehaviour
{
    public Transform inspectTransform;
    
    [HideInInspector] public Book bookInspecting;
    [HideInInspector] public Book bookSelected;
    public bool movingInspected;
    
    private bool startMouseOnInspected;
    private void Update()
    {
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
            Vector3 rotation = new Vector3(0, -Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            bookInspecting.RotateBook(rotation);
        }
        else
        {
            movingInspected = false;
        }
    }

    private void OnMouseDown()
    {
        print("OnMouseDown");
    }
}
