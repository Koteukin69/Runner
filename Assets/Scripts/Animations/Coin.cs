using Animations;
using DG.Tweening;
using UnityEngine;

namespace Level
{
    public class Coin : MonoBehaviour, ICollidable
    {
        [SerializeField] private float _rotationSpeed = .5f;
        [SerializeField] private float _jumpHeight = 2f;
        [SerializeField] private float _animDuration = 0.5f;

        private Vector3 _initialScale;

        private void Awake() =>
            _initialScale = transform.localScale;

        private void Update() =>
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, _rotationSpeed * 360 * Time.time, 
                transform.eulerAngles.z);

        void ICollidable.OnCollide()
        {
            DOTween.Sequence()
                .Append(transform.DOMoveY(transform.position.y + _jumpHeight, _animDuration).SetEase(Ease.OutQuad))
                .Join(transform.DOScale(Vector3.zero, _animDuration).SetEase(Ease.InQuad))
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    transform.localScale = _initialScale;
                }).Play();
        }
    }
}