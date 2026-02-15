using System;
using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField, Min(0)] private float _speed = 5f;
        
        [Header("Jumping")]
        [SerializeField, Min(0.001f)] private float _jumpTime = 1f;
        [SerializeField, Min(0f)] private float _jumpHeight = 2f;
        
        [Header("Rolling")]
        [SerializeField, Min(0.001f)] private float _rollTime = 1f;
        
        private int _position;
        private float _jumped = float.MinValue;
        private float _rolled = float.MinValue;

        // TODO Animations
        public Action<float> OnJump;
        public Action<float> OnRoll;

        private void Start()
        {
            _position = Mathf.FloorToInt((GameManager.Lines - 1f) / 2);
        }
        
        private void Update()
        {
            HandleInput();
            MoveUpdate();
        }

        private void MoveUpdate()
        {
            float jumpT = Mathf.Clamp01((Time.time - _jumped) / _jumpTime);
            Vector3 position = Vector3.right * (GameManager.LinesShift * (_position - (GameManager.Lines - 1f) / 2)) + 
                               Vector3.forward * (Time.time * _speed) + 
                               Vector3.up * (4f * jumpT * (1f - jumpT) * _jumpHeight) +
                               (IsRolling ? Vector3.down : Vector3.zero);
            
            transform.position = position;
        }

        private void HandleInput()
        {
            _position = Mathf.Clamp(_position + GameManager.Input.MovedOn, 0, GameManager.Lines - 1);
            
            if (IsJumping || IsRolling) return;
            if (GameManager.Input.Jumped) Jump();
            else if (GameManager.Input.Rolled) Roll();
        }

        private void Jump()
        {
            _jumped = Time.time;
            OnJump?.Invoke(_jumped);
        }

        private void Roll()
        {
            _rolled = Time.time;
            OnRoll?.Invoke(_rollTime);
        }

        public bool IsJumping => Time.time - _jumped < _jumpTime;
        public bool IsRolling => Time.time - _rolled < _jumpTime;
    }
}
