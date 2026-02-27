using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HandGuide : MonoBehaviour
{
    [SerializeField] private RectTransform _handRectTransform;
    [SerializeField] private Image _handImage;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Vector2 _offsetFactor = new (.5f, .5f);
    [SerializeField] private float _actionDuration = 1f;
    
    private Sequence _sequence;
    
    private static readonly Color ColorTransparent = new(1f, 1f, 1f, 0f);
    private static readonly Color ColorOpaque = new(1f, 1f, 1f, 1f);
    
    private void OnValidate()
    {
        if (!_handRectTransform) TryGetComponent(out _handRectTransform);
        if (!_handImage) TryGetComponent(out _handImage);
        if (!_canvas) _canvas = GetComponentInParent<Canvas>();
    }

    private void Awake()
    {
        if (!_handImage) throw new MissingFieldException(nameof(_handImage));
        _handImage.color = ColorTransparent;
    }
    
    private void Start()
    {
        if (!_handRectTransform) throw new MissingFieldException(nameof(_handRectTransform));
        if (!_canvas) throw new MissingFieldException(nameof(_canvas));

        Vector2 positionOffset = 0.5f * ((RectTransform)_canvas.transform).rect.size * _offsetFactor;
        
        _sequence = DOTween.Sequence()
            .When(positionOffset.y != 0, s => s
                .AppendCallback(() => _handRectTransform.anchoredPosition = Vector2.down * positionOffset.y)
                .Append(_handRectTransform.DOAnchorPosY(positionOffset.y, _actionDuration))
                .Join(CreateFadePingPong(_handImage, _actionDuration)))
            .When(positionOffset.x != 0, s => s
                .AppendCallback(() => _handRectTransform.anchoredPosition = Vector2.left * positionOffset.x)
                .Append(_handRectTransform.DOAnchorPosX(positionOffset.x, _actionDuration))
                .Join(CreateFadePingPong(_handImage, _actionDuration))
            ).Play();
    }
    
    private static Sequence CreateFadePingPong(Image image, float duration) => DOTween.Sequence()
        .Append(image.DOColor(ColorOpaque, duration / 2))
        .Append(image.DOColor(ColorTransparent, duration / 2));

    private void OnDestroy() => _sequence?.Kill();
}

public static class SequenceExtensions
{
    public static Sequence When(this Sequence s, bool condition, Action<Sequence> apply)
    {
        if (condition) apply(s);
        return s;
    }
}
