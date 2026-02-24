using UnityEngine;

namespace Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] PlayerMovement _playerMovement;
        [SerializeField] Animator _animator;
        [SerializeField, HideInInspector] float _jumpClipLength = 1f;
        [SerializeField, HideInInspector] float _slideClipLength = 1f;

        static readonly int JumpHash = Animator.StringToHash("Jump");
        static readonly int SlideHash = Animator.StringToHash("Slide");
        static readonly int DieHash = Animator.StringToHash("Die");
        static readonly int JumpSpeedHash = Animator.StringToHash("JumpSpeed");
        static readonly int SlideSpeedHash = Animator.StringToHash("SlideSpeed");

        private void OnValidate()
        {
            if (!_playerMovement) TryGetComponent(out _playerMovement);
            if (!_animator) TryGetComponent(out _animator);
            
#if UNITY_EDITOR
            if (_animator && _animator.runtimeAnimatorController is UnityEditor.Animations.AnimatorController ac)
            {
                foreach (var s in ac.layers[0].stateMachine.states)
                {
                    if (s.state.motion == null) continue;
                    if (s.state.name == "Jump") _jumpClipLength = s.state.motion.averageDuration;
                    else if (s.state.name == "Slide") _slideClipLength = s.state.motion.averageDuration;
                }
            }
#endif
        }

        private void Start()
        {
            _playerMovement.OnJump += Jump;
            _playerMovement.OnRoll += Slide;
            GameManager.OnDie += Die;
        }

        private void Jump(float duration)
        {
            _animator.SetFloat(JumpSpeedHash, _jumpClipLength / duration);
            _animator.SetTrigger(JumpHash);
        }

        private void Slide(float duration)
        {
            _animator.SetFloat(SlideSpeedHash, _slideClipLength / duration);
            _animator.SetTrigger(SlideHash);
        }

        private void Die() => _animator.SetTrigger(DieHash);
    }
}
