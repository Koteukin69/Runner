using System;

namespace RunnerInput
{
    public abstract class IInput
    {
        public abstract int MovedOn { get; }
        public abstract bool Jumped { get; }
        public abstract bool Rolled { get; }

        public Action Update;
    }
}
