using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    PlayerControls input;
    Camera mainCamera;

    public event System.Action OnTouchBegin;
    public event System.Action OnTouchEnd;

    protected override void Awake()
    {
        base.Awake();
        input = new PlayerControls();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        input.Enable();
        input.Base.Touch.started += ctx => OnTouchBegin?.Invoke();
        input.Base.Touch.canceled += ctx => OnTouchEnd?.Invoke();
    }

    private void OnDisable()
    {
        input.Disable();
        input.Base.Touch.started -= ctx => OnTouchBegin?.Invoke();
        input.Base.Touch.canceled -= ctx => OnTouchEnd?.Invoke();
    }

    public Vector2 PrimaryPosition()
    {
        Vector2 touchPos = input.Base.Touch.ReadValue<Vector2>();
        return mainCamera.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, mainCamera.nearClipPlane));
    }

}
