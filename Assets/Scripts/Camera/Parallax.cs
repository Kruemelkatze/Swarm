using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private Transform[] _layers;
    private int _leftIndex;
    private int _rightIndex;
    private Vector3 _lastCameraPos;
    private Vector3 _offsetToCam;
    
    [Tooltip("Camera, on which the parallax movement is based.")]
    [SerializeField] private Transform cameraTransform;

    [Tooltip("Viewzone of the camera. Does not need to be changed if the default camera has not been changed.")]
    [SerializeField] private float viewZone = 10;

    [Tooltip("Background size for scrolling. Can usually be automatically retrieved if set to 0.")]
    [SerializeField] private float backgroundSize = 0;

    [Tooltip("Should the parallax effect occur on the y-axis?")]
    [SerializeField] private bool moveY = false;

    [Tooltip(
        "Enables background-scrolling. This script must be placed on parent object with 3 children, aligned next to each other.")]
    [SerializeField] private bool scroll = false;

    [Tooltip(
        "Parallax factor. Positive values results in slower parallax, negative values in faster. 1=fixed in world. 0=fixed with cam")]
    [SerializeField] private float parallaxFactor = 0;


    // Use this for initialization
    void Start()
    {
        cameraTransform = cameraTransform != null ? cameraTransform : Camera.main.transform;
        _lastCameraPos = cameraTransform.position;
        _offsetToCam = cameraTransform.position - transform.position;

        if (!scroll) 
            return;
        
        _layers = new Transform[transform.childCount];

        for (int i = 0; i < _layers.Length; i++)
        {
            _layers[i] = transform.GetChild(i);
        }

        _leftIndex = 0;
        _rightIndex = _layers.Length - 1;

        if (Math.Abs(backgroundSize) < 0.05f && _layers.Length > 1)
            backgroundSize = Mathf.Abs(_layers[1].position.x - _layers[0].position.x);
    }

    // Update is called once per frame
    void Update()
    {
        var currentCameraPos = cameraTransform.position;
        var deltaX = currentCameraPos.x - _lastCameraPos.x;
        var deltaY = currentCameraPos.y - _lastCameraPos.y;
        _lastCameraPos = currentCameraPos;

        if (Math.Abs(parallaxFactor) < 0.05f)
        {
            transform.position = cameraTransform.position - _offsetToCam;
        }
        else
        {
            transform.position +=
                new Vector3(deltaX * (1 - parallaxFactor), moveY ? deltaY * (1 - parallaxFactor) : deltaY);
        }

        if (scroll && cameraTransform.position.x < (_layers[_leftIndex].transform.position.x + viewZone))
        {
            ScrollLeft();
        }
        else if (scroll && cameraTransform.position.x > (_layers[_rightIndex].transform.position.x - viewZone))
        {
            ScrollRight();
        }
    }

    void ScrollLeft()
    {
        _layers[_rightIndex].position = new Vector3(_layers[_leftIndex].position.x - backgroundSize,
            _layers[_rightIndex].position.y, _layers[_rightIndex].position.z);

        _leftIndex = _rightIndex;
        _rightIndex--;

        if (_rightIndex < 0)
            _rightIndex = _layers.Length - 1;
    }

    void ScrollRight()
    {
        _layers[_leftIndex].position = new Vector3(_layers[_rightIndex].position.x + backgroundSize,
            _layers[_leftIndex].position.y, _layers[_leftIndex].position.z);

        _rightIndex = _leftIndex;
        _leftIndex++;

        if (_leftIndex == _layers.Length)
            _leftIndex = 0;
    }
}