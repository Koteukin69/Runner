using System;
using DG.Tweening;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField, Min(0)] private float _speed = 5f;
        [SerializeField, Min(0.001f)] private float _moveTime = 0.3f;
        
        [Header("Jumping")]
        [SerializeField, Min(0.001f)] private float _jumpTime = 1f;
        [SerializeField, Min(0f)] private float _jumpHeight = 2f;

        [Header("Rolling")]
        [SerializeField, Min(0.001f)] private float _rollTime = 1f;
        private int _position;
        private float _jumped = float.MinValue;
        private float _rolled = float.MinValue;
        private float _startTime;
        
        private float _laneX;
        private Tween _moveTween;
        
        private Action _queued;

        // TODO Animations
        public Action<float> OnJump;
        public Action<float> OnRoll;

        private void Awake()
        {
            _startTime = Time.time;
            _position = Mathf.FloorToInt((GameManager.Lines - 1f) / 2);
            _laneX = GameManager.LinesShift * (_position - (GameManager.Lines - 1f) / 2);
        }
        
        private void Update()
        {
            HandleInput();
            MoveUpdate();
        }

        private void MoveUpdate()
        {
            float jumpT = Mathf.Clamp01((Time.time - _jumped) / _jumpTime);
            Vector3 position = Vector3.right * _laneX + 
                               Vector3.forward * ((Time.time - _startTime) * _speed) +
                               Vector3.up * (4f * jumpT * (1f - jumpT) * _jumpHeight) +
                               (IsRolling ? Vector3.down : Vector3.zero);
            
            transform.position = position;
        }

        private void OnDestroy() => 
            _moveTween?.Kill();
        
        private void HandleInput()
        {
            int newPos = Mathf.Clamp(_position + GameManager.Input.MovedOn, 0, (int) GameManager.Lines - 1);
            if (newPos != _position)
            {
                _position = newPos;
                float targetX = GameManager.LinesShift * (_position - (GameManager.Lines - 1f) / 2);
                _moveTween?.Kill();
                _moveTween = DOTween
                    .To(() => _laneX, x => _laneX = x, targetX, _moveTime)
                    .Play();
            }
            
            if (IsJumping || IsRolling)
            {
                if (_queued != null) return;
                
                if (GameManager.Input.Jumped) _queued = Jump;
                else if (GameManager.Input.Rolled) _queued = Roll;
                return;
            }
            
            if (_queued != null)
            {
                _queued.Invoke();
                
                _queued = null;
                return;
            }
            
            if (GameManager.Input.Jumped) Jump();
            else if (GameManager.Input.Rolled) Roll();
        }

        private void Jump()
        {
            _jumped = Time.time;
            OnJump?.Invoke(_jumpTime);
        }

        private void Roll()
        {
            _rolled = Time.time;
            OnRoll?.Invoke(_rollTime);
        }

        public int Lane => _position;
        public bool IsJumping => Time.time - _jumped < _jumpTime;
        public bool IsRolling => Time.time - _rolled < _rollTime;
    }
}
