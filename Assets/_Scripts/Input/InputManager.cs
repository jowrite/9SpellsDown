using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls input;
    Camera mainCamera;

    public event System.Action OnTouchBegin;
    public event System.Action OnTouchEnd;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void OnEnable()
    {
        input = new PlayerControls();

        input.Enable();
        input.Base.Touch.started += ctx => OnTouchBegin?.Invoke();
        input.Base.Touch.canceled += ctx => OnTouchEnd?.Invoke();
    }

    public void OnDisable()
    {
        input.Base.Touch.started -= ctx => OnTouchBegin?.Invoke();
        input.Base.Touch.canceled -= ctx => OnTouchEnd?.Invoke();
        input.Disable();
    }

    public Vector2 PrimaryPosition()
    {
        Vector2 touchPos = input.Base.PrimaryPosition.ReadValue<Vector2>();

        if (!mainCamera) mainCamera = Camera.main;

        return mainCamera.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, mainCamera.nearClipPlane));
    }
}
