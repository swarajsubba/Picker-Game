using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum ThisScreen
{
    None,
    Intro,
    Game,
    Settings,
    Result,
    Outro
}

public class ScreenController : MonoBehaviour
{
    private ThisScreen _currentScreenId = ThisScreen.None;
    private ThisScreen _previousScreenId = ThisScreen.None;
    private Screen _currentScreen;
    private Screen _previousScreen;

    [SerializeField] private List<Screen> scenesList;
    [SerializeField] private GameObject backbutton;
    public static ScreenController Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (!Instance)
            Instance = this;
        else
            Destroy(this);
    }

    /// <summary>
    /// initialize with intro menu screen
    /// </summary>
    private void Start()
    {
        if (_currentScreenId.Equals(ThisScreen.None))
            _currentScreenId = ThisScreen.Intro;

        SwitchScreen(_currentScreenId.ToString());
    }

    /// <summary>
    /// switches screens from the linked list creating a basic FSM
    /// </summary>
    /// <param name="screenId"></param>
    public void SwitchScreen(string screenId)
    {
        var cScreen = _currentScreen;
        if (cScreen != null)
        {
            _previousScreen = cScreen;
            _previousScreenId = cScreen.screenId;
            _previousScreen.DisableScreen();
        }

        var screenIdTemp = Enum.Parse(typeof(ThisScreen), screenId);
        backbutton.SetActive(!screenIdTemp.Equals(ThisScreen.Intro));

        var screen = scenesList.Find(s => s.screenId.Equals(screenIdTemp));
        _currentScreen = screen;
        _currentScreen.screenId =
            _currentScreenId = screen.screenId;
        _currentScreen.prevScreenId = cScreen != null ? cScreen.screenId : ThisScreen.None;

        screen.EnableScreen();
    }

    /// <summary>
    /// return you to the previous opened screen
    /// </summary>
    public void GoBack()
    {
        if (_currentScreen.prevScreenId.Equals(ThisScreen.None)) return;
        backbutton.SetActive(!_currentScreen.prevScreenId.Equals(ThisScreen.Intro));

        _currentScreen.DisableScreen();
        _currentScreen.prevScreenId = ThisScreen.None;

        _previousScreen.EnableScreen();
        _currentScreen = _previousScreen;

        _currentScreenId = _currentScreen.screenId;
        _previousScreenId = _currentScreen.prevScreenId;

        _previousScreen = scenesList.Find(s => s.screenId.Equals(_previousScreenId));
    }

    //replace timer with timestamp for better timer implementation
    internal float TimeLeft;
    public TextMeshProUGUI timerText;
    public bool freePicksTimer;

    [ContextMenu("Run Timer")]
    public void RunTimer()
    {
        timerText.enabled = freePicksTimer;

        if (!freePicksTimer) return;

        float minutes = Mathf.FloorToInt(TimeLeft / 60);
        float seconds = Mathf.FloorToInt(TimeLeft % 60);

        TimeLeft -= Time.deltaTime;
        timerText.text = $"Free 2 picks in\n{minutes:00}m:{seconds:00}s";

        if (!(TimeLeft <= 0)) return;

        freePicksTimer = false;
        TimeLeft = 60;
        PlayerPrefs.SetInt("Picks", 2);

        GameController.UpdatePicksUI?.Invoke(0);
    }

    private void Update()
    {
        RunTimer();
    }
}
