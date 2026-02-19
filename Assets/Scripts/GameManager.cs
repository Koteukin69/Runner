using UnityEngine;
using System;
using DG.Tweening;
using Level;
using RunnerInput;
using UnityEngine.SceneManagement;
using Requests;

[DisallowMultipleComponent, DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static IInput Input => _instance?._input ?? throw MissingInstanceException;
    public static uint Lines => _instance?._lines ?? throw MissingInstanceException;
    public static uint ChunkSize => _instance?._chunkSize ?? throw MissingInstanceException;
    public static float LinesShift => _instance?._lineShift ?? throw MissingInstanceException;
    public static float LevelLength => _instance?._levelLength ?? throw MissingInstanceException;
    public static LevelManager LevelManager => _instance?._levelManager ?? throw MissingInstanceException;
    public static Action OnDie;
    public static Action OnFinish;

    [SerializeField, Min(1)] private uint _lines = 3;
    [SerializeField, Min(1)] private uint _chunkSize = 8;
    [SerializeField, Min(0)] private float _lineShift = 2f;
    [SerializeField] private uint _levelLength = 20;
    [SerializeField] private LevelManager _levelManager;
    
    [SerializeField] private Inputs _inputs = Inputs.Keyboard | Inputs.Swipes;
    [SerializeField] private LevelTemplates _levelTemplates;

    private IInput _input;
    private static GameManager _instance;

    private void Awake()
    {
        // Singleton
        if (_instance != null) throw new Exception("Only one GameManager instance is allowed.");
        _instance = this;
        
        InitializeInput();
        InitializeLevel();
        
        // TODO Move this implementation out of GameManager
        OnDie += () =>
        {
            Debug.Log("You Died!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        };
        OnFinish += () =>
        {
            Debug.Log("You Won!");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            
            new Post("https://localhost:3000/finnish", "{}", this)
                .AddResponseCallback(response => Debug.Log(response));
        };
    }

    private void InitializeLevel()
    {
        if (!_levelManager) throw new MissingFieldException(nameof(_levelManager));
        if (!_levelTemplates) throw new MissingFieldException(nameof(_levelTemplates));

        _levelManager.Initialize(new RandomLevelProvider(_levelTemplates));
    }

    private void InitializeInput()
    {
        _input = _inputs switch
        {
            Inputs.Everything => new InputManager(new IInput[] { new KeyboardInput(), new SwipesInput() }),
            Inputs.Keyboard => new KeyboardInput(),
            Inputs.Swipes => new SwipesInput(),
            _ => _input
        };
    }

    private void Update()
    {
        _input.Update?.Invoke();
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            OnDie = null;
            DOTween.KillAll();
            _instance = null;
        }
    }

    private static Exception MissingInstanceException => new ("GameManager is missing from the scene or game hasn't started yet.");
}

[Flags]
enum Inputs
{
    Keyboard = 1,
    Swipes = 2,
    Everything = ~0,
}
