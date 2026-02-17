using UnityEngine;
using System;
using Level;
using RunnerInput;

[DisallowMultipleComponent, DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static IInput Input => _instance?._input ?? throw MissingInstanceException;
    public static uint Lines => _instance?._lines ?? throw MissingInstanceException;
    public static uint ChunkSize => _instance?._chunkSize ?? throw MissingInstanceException;
    public static float LinesShift => _instance?._lineShift ?? throw MissingInstanceException;
    
    [SerializeField, Min(1)] private uint _lines = 3;
    [SerializeField, Min(1)] private uint _chunkSize = 8;
    [SerializeField, Min(0)] private float _lineShift = 2f;
    [SerializeField] private Inputs _inputs = Inputs.Keyboard | Inputs.Swipes;
    
    private IInput _input;
    private static GameManager _instance;

    private void Awake()
    {
        // Singleton
        if (_instance != null) throw new Exception("Only one GameManager instance is allowed.");
        _instance = this;
        
        InitializeInput();
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
        if (_instance == this) _instance = null;
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
