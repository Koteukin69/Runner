using UnityEngine;

public class InputManager
{
    public int MovedOn => (
        Input.GetKeyDown(KeyCode.LeftArrow) ? -1 :
        Input.GetKeyDown(KeyCode.RightArrow) ? 1 : 0
    );

    public bool Jumped => Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space);
    public bool Rolled => Input.GetKeyDown(KeyCode.DownArrow);
}
