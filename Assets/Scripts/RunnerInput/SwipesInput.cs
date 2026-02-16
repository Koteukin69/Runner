using UnityEngine;

namespace RunnerInput
{
    [DefaultExecutionOrder(-1)]
    public class SwipesInput : IInput
    {
        public override int MovedOn => 0;

        public override bool Jumped => false;
        public override bool Rolled => false;
    }
}
