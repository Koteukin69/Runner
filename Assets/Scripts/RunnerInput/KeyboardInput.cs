using UnityEngine;

namespace RunnerInput
{
    public class KeyboardInput : IInput
    {
        public override int MovedOn => (
            Input.GetKeyDown(KeyCode.LeftArrow) ? -1 :
            Input.GetKeyDown(KeyCode.RightArrow) ? 1 : 0
        );

        public override bool Jumped => Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space);
        public override bool Rolled => Input.GetKeyDown(KeyCode.DownArrow);
    }
}
