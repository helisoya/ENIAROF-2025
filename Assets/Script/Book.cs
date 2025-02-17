using System.Collections;
using UnityEngine;

public class Book : MonoBehaviour
{
    [SerializeField] private GameObject bookGameObject;
    [SerializeField] private BookManager bookManager;
    
    private Outline outline;
    private Animator animator;
    private Vector3 initialPosition;
    
    private bool inspected = false;
    private float duration = 0.75f;
    private Vector3 startPosition;
    private Quaternion startRotation;
    
    private bool isMoving = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get Component
        outline = GetComponent<Outline>();
        animator = GetComponent<Animator>();
        
        // Init Outline and initialTransform
        outline.enabled = false;
        initialPosition = bookGameObject.transform.localPosition;
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    // Rotate Book (inspect)
    public void RotateBook(Vector3 rotation)
    {
        bookGameObject.transform.eulerAngles += rotation;
    }
    
    private void OnMouseOver()
    {
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
    
}
