using UnityEngine;

namespace RunnerInput
{
    public class SwipesInput : IInput
    {
        private const float MinDistance = 50f;
        private const float MaxTime = 0.3f;
        private const float DirectionThreshold = 0.7f;

        private Vector2 _startPos;
        private float _startTime;
        private int _movedOn;
        private bool _jumped;
        private bool _rolled;

        public SwipesInput() => Update = ProcessInput;

        public override int MovedOn => Consume(ref _movedOn);
        public override bool Jumped => Consume(ref _jumped);
        public override bool Rolled => Consume(ref _rolled);

        private void ProcessInput()
        {
            _movedOn = 0;
            _jumped = false;
            _rolled = false;

            if (Input.GetMouseButtonDown(0)) Begin(Input.mousePosition);
            else if (Input.GetMouseButtonUp(0)) TryRecognize(Input.mousePosition);
        }

        private void Begin(Vector2 position)
        {
            _startPos = position;
            _startTime = Time.time;
        }

        private void TryRecognize(Vector2 endPos)
        {
            if (Time.time - _startTime > MaxTime) return;

            Vector2 delta = endPos - _startPos;
            if (delta.magnitude < MinDistance) return;

            Vector2 dir = delta.normalized;

            if (dir.y > DirectionThreshold)       _jumped = true;
            else if (dir.y < -DirectionThreshold) _rolled = true;
            else if (dir.x > DirectionThreshold)  _movedOn = 1;
            else if (dir.x < -DirectionThreshold) _movedOn = -1;
        }

        private static bool Consume(ref bool value)
        {
            bool v = value; value = false; return v;
        }

        private static int Consume(ref int value)
        {
            int v = value; value = 0; return v;
        }
    }
}
