using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    PlayerControls input;
    Camera mainCamera;

    public event System.Action OnTouchBegin;
    public event System.Action OnTouchEnd;

    //Recent active touch ID for IsPointerOver checks
    public int LastFingerId { get; private set; } = -1;
    public Vector2 LastTouchPos { get; private set; }


    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void OnEnable()
    {
        input = new PlayerControls();

        input.Enable();

        input.Base.Touch.started += ctx =>
        {
            if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
            {
                var touch = Touchscreen.current.touches[0];
                LastFingerId = touch.touchId.ReadValue();
                LastTouchPos = touch.position.ReadValue();
            }

            OnTouchBegin?.Invoke();
        };

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
