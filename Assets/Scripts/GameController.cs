using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hellmade.Sound;
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
    
    private Audio _swarmLoopAudio;

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
        
        SetPause(false);
        StartCoroutine(StartGameDelayed());
    }

    private IEnumerator StartGameDelayed()
    {
        var spawner = Hub.Get<FishSpawner>();
        //spawner.Spawn(1);

        yield return new WaitForSeconds(startDelay);
        isStarted = true;

        
        yield return new WaitForSeconds(0.5f);
        spawner.Spawn(spawner.SpawnCount, true);
        AudioController.Instance.PlaySound("splash");
        
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
        isPaused = paused;

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
    }
}
#endif