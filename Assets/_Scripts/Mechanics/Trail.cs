using System;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class Trail : MonoBehaviour
{
    public InputManager im;

    TrailRenderer tr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        tr = GetComponent<TrailRenderer>();

        tr.enabled = false;
    }

    private void OnEnable()
    {
        im = Bootstrapper.Instance.InputManager;
        im.OnTouchBegin += EnableTrail;
        im.OnTouchEnd += DisableTrail;

    }

    private void OnDisable()
    {
        im.OnTouchBegin -= EnableTrail;
        im.OnTouchEnd -= DisableTrail;
    }

    void EnableTrail()
    {
        transform.position = im.PrimaryPosition();
        tr.enabled = true;
    }

    private void DisableTrail() => tr.enabled = false;

    void Update()
    {
        if (tr.enabled) transform.position = im.PrimaryPosition();
    }
}
