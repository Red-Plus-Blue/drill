﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerComponent : MonoBehaviour
{
    protected const int LEVEL_SCENE = 0;
    public static GameManagerComponent Instance { get; protected set; }
    public bool IsFirstLoad { get; protected set; } = true;
    public bool IsTutorial { get; protected set;}
    public int Level { get; protected set; }
    public bool Defeated { get; protected set; }

    protected bool _exiting;

    private void Awake()
    {
        if(Instance)
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += (scene, mode) => _exiting = false; Defeated = false;
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    private void Start()
    {
        IsFirstLoad = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            StartGame(false);
        }
    }

    public void StartGame(bool fromTitleScreen)
    {
        Level = 0;
        FindObjectOfType<PlayerControllerComponent>().ResetDefaults();
        ExitLevel(fromTitleScreen);
    }

    public void ExitLevel(bool fromTitleScreen)
    {
        IsTutorial = fromTitleScreen;
        if (_exiting) { return; }
        Level += 1;
        _exiting = true;
        FindObjectOfType<PlayerControllerComponent>().StoreResults();
        ExitComponent.TouchingExitCount = 0;
        SceneManager.LoadScene(LEVEL_SCENE);
    }

    public void Lose(string reason)
    {
        Defeated = true;
        var player = FindObjectOfType<PlayerControllerComponent>(true);
        player.InputLocked = true;
        var details = $"{reason}\nYou lost on level: {Level}\nFinal Score: $ {player.Money.Current}\nPress 'r' to restart";
        FindObjectOfType<UIComponent>().ShowDefeatScreen(details);
    }
}
