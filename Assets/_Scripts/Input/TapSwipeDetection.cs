using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TapSwipeDetection : MonoBehaviour
{
    InputManager im;

    //variables to control tapping and swiping
    float dirThreshold = 0.9f;

    float tapTimeout = 0.2f;
    float swipeTimeout = 0.5f;
    float startTime = 0;
    float endTime = 0;

    Vector2 startPos;
    Vector2 endPos;


    private void OnEnable()
    {
        im = Bootstrapper.Instance.InputManager;
        im.OnTouchBegin += TouchStarted;
        im.OnTouchEnd += TouchEnded;
    }

    private void OnDisable()
    {
        im.OnTouchBegin -= TouchStarted;
        im.OnTouchEnd -= TouchEnded;
    }


    private void TouchStarted()
    {
        startPos = im.PrimaryPosition();
        startTime = Time.time;
    }

    private void TouchEnded()
    {
        endPos = im.PrimaryPosition();
        endTime = Time.time;
        DetectInput();
    }


    private void DetectInput()
    {
        float totalTime = endTime - startTime;
        float distance = Vector2.Distance(startPos, endPos);

        if (totalTime > swipeTimeout || distance < 0.1f)
        {
            Debug.Log("Not a valid tap/swipe");
            return;
        }

        if (totalTime < tapTimeout)
        {
            Tap();
        }
        else
        {
            CheckSwipe();
        }
    }

    private void Tap()
    {
        Debug.Log($"Tap at {endPos}");
        RaycastCard(endPos);
    }

    private void CheckSwipe()
    {
        Vector2 direction = (endPos - startPos).normalized;

        float vertical = Vector2.Dot(Vector2.up, direction);
        float horizontal = Vector2.Dot(Vector2.right, direction);

        if(vertical >= dirThreshold)
        {
            Debug.Log("Swipe Up");
        }

        else if (vertical <= -dirThreshold)
        {
            Debug.Log("Swipe Down");
        }

        else if(horizontal >= dirThreshold)
        {
            Debug.Log("Swipe Right");
        }
        else if (horizontal <= -dirThreshold)
        {
            Debug.Log("Swipe Left");
        }

    }

    private void RaycastCard(Vector2 screenPosition)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData pd = new PointerEventData(EventSystem.current)
            {
                position = screenPosition
            };

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pd, raycastResults);

            foreach (RaycastResult result in raycastResults)
            {
                if (result.gameObject.CompareTag("Card"))
                {
                    Debug.Log($"Tapped/Swiped on card: {result.gameObject.name}");
                    //CardUI card = result.gameObject.GetComponent<CardUI>();
                    //card.TryPlay(); **need to write this logic, set up CardUI.cs
                    break;
                }
            }
        }

    }
}
