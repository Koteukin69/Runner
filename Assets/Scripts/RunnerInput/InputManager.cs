using System.Linq;

namespace RunnerInput
{
    public class InputManager : IInput
    {
        private IInput[] _inputs;

        public InputManager(IInput[] inputs)
        {
            foreach (var input in inputs) Update += input.Update;
            _inputs = inputs;
        }

        public override int MovedOn => _inputs.Sum(i => i.MovedOn);
        public override bool Jumped => _inputs.Any(i => i.Jumped);
        public override bool Rolled => _inputs.Any(i => i.Rolled);
    }
}
