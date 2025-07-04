using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureController : MonoBehaviour
{
    public delegate void GestureEventHandler();
    public event GestureEventHandler rightFlick;
    public event GestureEventHandler leftFlick;
    public event GestureEventHandler upFlick;
    public event GestureEventHandler downFlick;

    private const float posDiff = 50f;
    private Vector2 startPos = Vector2.zero;
    private Vector2 currentPos = Vector2.zero;
    private bool isPrepared = false;
    private RectTransform targetRectTransform;

    void Start()
    {
        targetRectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            if (IsPointerOverRectTransform(startPos))
            {
                isPrepared = true;
                Debug.Log("isPrepared");
            }
        }

        if (Input.GetMouseButton(0) && isPrepared)
        {
            currentPos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0) && isPrepared)
        {
            if (Mathf.Abs(currentPos.x - startPos.x) > Mathf.Abs(currentPos.y - startPos.y))
            {
                if (currentPos.x - startPos.x >= posDiff)
                {
                    RightFlick();
                }
                else if (currentPos.x - startPos.x <= -posDiff)
                {
                    LeftFlick();
                }
            }
            else
            {
                if (currentPos.y - startPos.y >= posDiff)
                {
                    UpFlick();
                }
                else if (currentPos.y - startPos.y <= -posDiff)
                {
                    DownFlick();
                }
            }
            isPrepared = false;
        }
    }

    private bool IsPointerOverRectTransform(Vector2 screenPosition)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(targetRectTransform, screenPosition);
    }

    private void RightFlick()
    {
        
        if (rightFlick != null) { rightFlick(); }
    }

    private void LeftFlick()
    {
        if (leftFlick != null) { leftFlick(); }
    }

    private void UpFlick()
    {
        if (upFlick != null) { upFlick(); }
    }

    private void DownFlick()
    {
        if (downFlick != null) { downFlick(); }
    }
}
