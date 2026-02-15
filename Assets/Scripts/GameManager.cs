using UnityEngine;
using System;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    public static InputManager Input => _instance?._input ?? throw MissingInstanceException;
    public static int Lines => _instance?._lines ?? throw MissingInstanceException;
    public static float LinesShift => _instance?._lineShift ?? throw MissingInstanceException;
    
    private InputManager _input;
    [SerializeField, Min(1)] private int _lines = 3;
    [SerializeField, Min(0)] private float _lineShift = 2f;
    
    private static GameManager _instance;

    private void Awake()
    {
        // Singleton
        if (_instance == null) _instance = this;
        else throw new Exception("Only one GameManager instance is allowed.");

        _input = new ();
    }

    private static Exception MissingInstanceException => new ("GameManager is missing from the scene or game hasn't started yet.");
}