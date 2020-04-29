using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BaseMenuManager : MonoBehaviour
{
    [SerializeField] private RectTransform initiallyOpenChildMenu;

    [Tooltip("List of panels/canvases that can be opened or closed")] [SerializeField]
    private RectTransform[] childMenuUiElements;

    [SerializeField] private RectTransform confirmCloseUiElement;

    [Tooltip("Image that is shown when any child menu is open to capture backdrop clicks.")] [SerializeField]
    private RectTransform backDropImage;

    [Tooltip("Enabled when any menu is opened. Disabled when closed.")] [SerializeField]
    private RectTransform[] showWithAnyOpenMenu;

    [Tooltip("Disabled when any menu is opened. Enabled otherwise.")] [SerializeField]
    private RectTransform[] showWithoutAnyOpenMenu;

    [Tooltip("Textfields with Creators or Tasks in each line. Lines can be randomized.")] [SerializeField]
    private TextMeshProUGUI[] creditFields;

    [SerializeField] private bool randomizeCreditFields = true;

    [SerializeField] private string clickSoundName = "click";

    [SerializeField] private bool closeChildMenusOnEsc = true;


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

        if (initiallyOpenChildMenu)
        {
            EnableChildMenu(initiallyOpenChildMenu);
        }
        else
        {
            DisableChildMenus();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (closeChildMenusOnEsc)
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
            else
            {
                confirmCloseUiElement.gameObject.SetActive(!confirmCloseUiElement.gameObject.activeInHierarchy);
            }
        }
    }

    public void PlayClickSound()
    {
        AudioController.Instance.PlaySound(clickSoundName ?? "click");
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
        OnEnableChildMenu();
    }

    public void OnEnableChildMenu()
    {
        SetBackDropActive(true);

        foreach (var e in showWithAnyOpenMenu)
        {
            EnableUiElement(e);
        }

        foreach (var e in showWithoutAnyOpenMenu)
        {
            DisableUiElement(e);
        }
    }

    public void OnDisableChildMenu()
    {
        SetBackDropActive(false);

        foreach (var e in showWithAnyOpenMenu)
        {
            DisableUiElement(e);
        }

        foreach (var e in showWithoutAnyOpenMenu)
        {
            EnableUiElement(e);
        }
    }

    public void DisableChildMenus()
    {
        foreach (var childMenuElement in childMenuUiElements)
        {
            childMenuElement.gameObject.SetActive(false);
        }

        OnDisableChildMenu();
    }

    public void RandomizeCreditsLines()
    {
        if (!randomizeCreditFields || creditFields.Length == 0)
        {
            return;
        }

        var lines = creditFields
            .Select(field => field.text.Split('\n')).ToList();

        if (lines.Any(l => l.Length != lines[0].Length))
        {
            Debug.LogWarning("creditFields do not have the same number of lines");
            return;
        }

        var c = 0;
        var indices = lines[0].Select(l => c++).OrderBy(x => Random.value).ToList();

        for (int i = 0; i < creditFields.Length; i++)
        {
            var field = creditFields[i];
            var fieldLines = lines[i];
            var randFieldLines = new string[fieldLines.Length];
            for (int l = 0; l < fieldLines.Length; l++)
            {
                randFieldLines[indices[l]] = fieldLines[l];
            }

            field.text = string.Join("\n", randFieldLines);
        }
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

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(BaseMenuManager))]
    public class BaseMenuManagerCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var bmm = target as BaseMenuManager;

            if (GUILayout.Button("Randomize Credits"))
            {
                bmm.RandomizeCreditsLines();
            }
        }
    }
#endif
}