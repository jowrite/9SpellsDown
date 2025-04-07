using System;
using UnityEngine;

public class TapSwipeDetection : MonoBehaviour
{
    //variables to control tapping and swiping
    float distThreshold = 0.5f;
    float dirThreshold = 0.9f;

    float tapTimeout = 0.2f;
    float swipeTimeout = 0.5f;
    float startTime = 0;
    float endTime = 0;

    Vector2 startPos;
    Vector2 endPos; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputManager.Instance.OnTouchBegin += TouchStarted;
        InputManager.Instance.OnTouchEnd += TouchEnded;
    }

    private void TouchStarted()
    {
        startPos = InputManager.Instance.PrimaryPosition();
        startTime = Time.time;
    }

    private void TouchEnded()
    {
        endTime = Time.time;
        endPos = InputManager.Instance.PrimaryPosition();
        DetectInput();
    }

    private void DetectInput()
    {
        float totalTime = endTime - startTime;
        if (totalTime > swipeTimeout)
        {
            Debug.Log("not a tap or swipe");
            return;
        }

        if (totalTime < tapTimeout)
        {
            Tap();
            return;
        }

        CheckSwipe();
    }


    private void Tap()
    {
        Debug.Log($"Tap at {InputManager.Instance.PrimaryPosition()}");
    }

    private void CheckSwipe()
    {
        float distance = Vector2.Distance(startPos, endPos);
        if (distance < distThreshold) return;

        Vector2 direction = (endPos - startPos).normalized;

        float checkUp = Vector2.Dot(Vector2.up, direction);
        float checkLeft = Vector2.Dot(Vector2.left, direction);

        if (checkUp >= dirThreshold)
        {
            Debug.Log("Swiped up!");
            return;
        }
        if (checkUp <= -dirThreshold)
        {
            Debug.Log("Swiped down!");
            return;
        }
        if (checkLeft >= dirThreshold)
        {
            Debug.Log("Swiped left!");
            return;
        }
        if (checkLeft <= -dirThreshold)
        {
            Debug.Log("Swiped right!");
            return;
        }

    }


    // Update is called once per frame
    void Update()
    {
        
    }


}
