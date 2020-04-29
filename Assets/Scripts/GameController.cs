using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hellmade.Sound;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] [Range(0, 5)] private float startDelay = 2;

    [Header("Game States")] [SerializeField]
    private bool isPaused;

    [SerializeField] private bool isStarted;
    
    [Header("UI")] [SerializeField] private GameObject gameUi;
    [SerializeField] private GameObject pauseUi;

    [SerializeField] private GameObject finishedUi;
    [SerializeField] private TextMeshProUGUI fishText;

    private Audio _swarmLoopAudio;
    private CameraMovement movement;

    [SerializeField] private float waitAfterWin = 2;

    public bool isFinished = false;
    private bool hasTriggeredFinished = false;
    private void Awake()
    {
        Hub.Register(this);
    }

    private void Start()
    {
        if (!AudioController.Instance.IsMusicPlaying)
        {
            AudioController.Instance.PlayDefaultMusic();
        }

        AudioController.Instance.StopAllSounds();
        AudioController.Instance.PlaySound("ambience");
        
        StartCoroutine(StartGameDelayed());
        movement = Hub.Get<CameraMovement>();

        isFinished = false;
        hasTriggeredFinished = false;
        SetPause(false, false);
    }

    private IEnumerator StartGameDelayed()
    {
        var dc = Hub.Get<DepthController>();
        dc.SetVisibilityOfUIFields(false);
        var spawner = Hub.Get<FishSpawner>();
        //spawner.Spawn(1);

        yield return new WaitForSeconds(startDelay);
        isStarted = true;

        
        yield return new WaitForSeconds(0.5f);
        spawner.Spawn(spawner.SpawnCount, true);
        AudioController.Instance.PlaySound("splash");
        
        dc.SetVisibilityOfUIFields(true);

        var audioId = AudioController.Instance.PlaySound("swarmloop");
        _swarmLoopAudio = EazySoundManager.GetSoundAudio(audioId);
    }
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPause(!isPaused);
        }

        if (isPaused)
        {
            return;
        }

        if (!hasTriggeredFinished && movement.GetDepthPercentage() >= 1)
        {
            Finished();
        }
    }

    public void Finished()
    {
        hasTriggeredFinished = true;
        StartCoroutine(LevelFinished());
    }

    private IEnumerator LevelFinished()
    {
        if (!isFinished)
        {
            var spawner = Hub.Get<FishSpawner>();
            spawner.SpawnEnabled = false;
            AudioController.Instance.PlaySound("won");

            yield return new WaitForSeconds(waitAfterWin);

            isFinished = true;
            isStarted = false;
            
            var fishCount = spawner.NumberOfFish;

            if (gameUi != null)
            {
                gameUi.SetActive(false);
            }

            if (finishedUi != null)
            {
                finishedUi.SetActive(true);
            }

            if (fishText)
            {
                fishText.text = fishCount.ToString();
            }
        }
    }

    //  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ PUBLIC  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public void PauseGame() => SetPause(true);
    public void ContinueGame() => SetPause(false);

    public bool IsActive() => !isPaused && isStarted;

    public void SetMovementAudio(float magnitude)
    {
        if (_swarmLoopAudio == null)
        {
            return;
        }

        magnitude = magnitude * 0.4f + 0.3f;
        
        _swarmLoopAudio.SetVolume(magnitude, 0.15f);
    }
    
    //  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ PRIVATE  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    private void SetPause(bool paused)
    {
        SetPause(paused, true);
    }
    
    private void SetPause(bool paused, bool playSound)
    {
        isPaused = paused;

        if (playSound)
        {
            AudioController.Instance.PlaySound("click");
        }

        if (gameUi != null)
        {
            gameUi.SetActive(!paused);
        }

        if (pauseUi != null)
        {
            pauseUi.SetActive(paused);
        }

        // Stopping time depends on your game! Turn-based games maybe don't need this
        Time.timeScale = paused ? 0 : 1;

        // Whatever else there is to do...
        // Deactivate other UI, etc.
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameController))]
public class GameControlTestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var gct = target as GameController;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("*click*"))
        {
            AudioController.Instance.PlaySound("click");
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Restart"))
        {
            SceneController.Instance.RestartScene();
        }
        if (GUILayout.Button("Finished"))
        {
            gct.Finished();
        }
    }
}
#endif