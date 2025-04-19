using UnityEngine;

public class TapSwipeDetection : MonoBehaviour
{
    InputManager im;

    //variables to control tapping and swiping
    float distThreshold = 0.8f;
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


    }

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
        endTime = Time.time;
        endPos = im.PrimaryPosition();
        DetectInput();
    }


    private void DetectInput()
    {
        float totalTime = endTime - startTime;
        if (totalTime > swipeTimeout)
        {
            Debug.Log("Not a tap or swipe");
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
        Debug.Log($"Tap at {im.PrimaryPosition()}");
    }

    private void CheckSwipe()
    {
        float distance = Vector2.Distance(startPos, endPos);
        if (distance < distThreshold) return;

        Vector2 dir = (endPos - startPos).normalized;

        float checkUp = Vector2.Dot(Vector2.down, dir);
        float checkLeft = Vector2.Dot(Vector2.left, dir);

        if (checkUp >= dirThreshold)
        {
            Debug.Log($"Swipe Up: {checkUp}");
            return;
        }

        if (checkUp <= -dirThreshold)
        {
            Debug.Log($"Swipe Down: {checkUp}");
            return;
        }

        if (checkLeft >= dirThreshold)
        {
            Debug.Log("Swipe right");
            return;
        }

        if (checkLeft <= -dirThreshold)
        {
            Debug.Log("Swipe leftd");
            return;
        }
    }
}
