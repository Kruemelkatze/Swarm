using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseMenuManager : MonoBehaviour
{
    [Tooltip("List of panels/canvases that can be opened or closed")] [SerializeField]
    private RectTransform[] childMenuUiElements;

    [SerializeField] private RectTransform confirmCloseUiElement;

    [Tooltip("Image that is shown when any child menu is open to capture backdrop clicks.")] [SerializeField]
    private RectTransform backDropImage;

    private void Start()
    {
        foreach (var childMenuElement in childMenuUiElements)
        {
            var c = childMenuElement.GetComponent<Canvas>();
            if (c)
            {
                c.gameObject.SetActive(true);
                c.overrideSorting = true;
                c.sortingOrder = 5;
            }
        }
        DisableChildMenus();
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var anyWasOpen = false;
            foreach (var childMenuElement in childMenuUiElements)
            {
                anyWasOpen |= childMenuElement.gameObject.activeInHierarchy;
                childMenuElement.gameObject.SetActive(false);
            }

            if (!anyWasOpen && confirmCloseUiElement != null)
            {
                confirmCloseUiElement.gameObject.SetActive(true);
            }
        }
    }

    public void EnableUiElement(RectTransform uiElement)
    {
        uiElement.gameObject.SetActive(true);
    }

    public void DisableUiElement(RectTransform uiElement)
    {
        uiElement.gameObject.SetActive(false);
    }

    public void EnableChildMenu(RectTransform uiElement)
    {
        DisableChildMenus();
        EnableUiElement(uiElement);
        SetBackDropActive(true);
    }

    public void DisableChildMenus()
    {
        foreach (var childMenuElement in childMenuUiElements)
        {
            childMenuElement.gameObject.SetActive(false);
        }

        SetBackDropActive(false);
    }

    public void ToggleUiElementActive(RectTransform uiElement)
    {
        uiElement.gameObject.SetActive(!uiElement.gameObject.activeSelf);
    }

    private void SetBackDropActive(bool activeState)
    {
        if (backDropImage)
        {
            if (activeState)
            {
                EnableUiElement(backDropImage);
            }
            else
            {
                DisableUiElement(backDropImage);
            }
        }
    }
}