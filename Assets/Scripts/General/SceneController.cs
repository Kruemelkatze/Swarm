using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
///     Controller used for managing Scene Transistions.
///     Courtesy by Fire Totem Games, https://www.firetotemgames.com/
///     Adjusted a bit to reuse the Singleton<> Scheme
/// </summary>
public class SceneController : PersistentSingleton<SceneController>
{
    /* ======================================================================================================================== */
    /* VARIABLE DECLARATIONS                                                                                                    */
    /* ======================================================================================================================== */

    [Header("Fade Image")] [SerializeField]
    private Image fadeImage;

    [SerializeField] private Color fadeImageColor = Color.black;

    [Header("Fade Out")] [SerializeField] private float fadeOutTimeDefault = 0.3f;
    [SerializeField] private AnimationCurve fadeOutCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("In Between")] [SerializeField]
    private float waitTimeDefault = 0.1f;

    [Header("Fade In")] [SerializeField] private float fadeInTimeDefault = 0.3f;
    [SerializeField] private AnimationCurve fadeInCurve = AnimationCurve.Linear(1f, 1f, 0f, 0f);

    public static float fadeOutTime;
    public static float waitTime;
    public static float fadeInTime;

    /* ======================================================================================================================== */
    /* UNITY CALLBACKS                                                                                                          */
    /* ======================================================================================================================== */

    private void Start()
    {
        SetDefaultTimes();
        fadeImage.gameObject.SetActive(false);
    }

    /* ======================================================================================================================== */
    /* PRIVATE FUNCTIONS                                                                                                        */
    /* ======================================================================================================================== */

    private void SetDefaultTimes()
    {
        fadeOutTime = fadeOutTimeDefault;
        waitTime = waitTimeDefault;
        fadeInTime = fadeInTimeDefault;
    }

    /// <summary>
    /// Fade out from actual scene
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    private IEnumerator FadeOut(string sceneName)
    {
        fadeImage.gameObject.SetActive(true);

        float t = 0.0f;
        while (t < 1.0f)
        {
            t = UpdateFadeImageColor(t, fadeOutTime, fadeOutCurve);
            yield return 0;
        }

        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(sceneName);

        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// Fade in to new scene
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeIn()
    {
        float t = 0.0f;
        while (t < 1.0f)
        {
            t = UpdateFadeImageColor(t, fadeInTime, fadeInCurve);
            yield return 0;
        }

        SetDefaultTimes();
        fadeImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// Helper function to update fade image color
    /// Called in IEnumators FadeIn and FadeOut
    /// </summary>
    /// <param name="t"></param>
    /// <param name="fadeTime"></param>
    /// <param name="curve"></param>
    /// <returns></returns>
    private float UpdateFadeImageColor(float t, float fadeTime, AnimationCurve curve)
    {
        t += Time.deltaTime / fadeTime;
        Color color = fadeImage.color;
        color.a = curve.Evaluate(t);
        fadeImage.color = color;
        return t;
    }

    /* ======================================================================================================================== */
    /* PUBLIC FUNCTIONS                                                                                                         */
    /* ======================================================================================================================== */

    /// <summary>
    /// Fade out of current scene and fade in to new scene
    /// </summary>
    /// <param name="sceneName">Scene to load</param>
    [UsedImplicitly]
    public void LoadScene(string sceneName)
    {
        // todo: stop sound effect bus
        Time.timeScale = 1f;
        Instance.StartCoroutine(Instance.FadeOut(sceneName));
    }

    [UsedImplicitly]
    public void RestartScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    [UsedImplicitly]
    public void SetFadeOutTime(float newFadeOutTime)
    {
        fadeOutTime = newFadeOutTime;
    }

    [UsedImplicitly]
    public void SetWaitTime(float newWaitTime)
    {
        waitTime = newWaitTime;
    }

    [UsedImplicitly]
    public void SetFadeInTime(float newFadeInTime)
    {
        fadeInTime = newFadeInTime;
    }

    /// <summary>
    /// Exit Game Function
    /// </summary>
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit ();
#endif
    }

    /* ======================================================================================================================== */
    /* EVENT CALLERS                                                                                                            */
    /* ======================================================================================================================== */

    /* ======================================================================================================================== */
    /* EVENT LISTENERS                                                                                                          */
    /* ======================================================================================================================== */
}