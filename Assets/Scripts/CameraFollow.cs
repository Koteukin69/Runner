using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 _followOffset = new (0, 5, -10);
    [SerializeField] private Vector3 _lookAtOffset = new (0, 0, 0);
    [SerializeField] private Transform _followTransform;
    [SerializeField] private FreezePosition _freezePosition = FreezePosition.y;

    private void OnValidate()
    {
        _followTransform ??= FindFirstObjectByType<Player.PlayerMovement>().transform;

        transform.position = _followTransform.position + _followOffset;
        if (_lookAtOffset != Vector3.zero) transform.LookAt(-_lookAtOffset);
    }
    
    private void LateUpdate()
    {
        Vector3 position = _followTransform.position + _followOffset;

        transform.position = new Vector3(
            !_freezePosition.HasFlag(FreezePosition.x) ? position.x : transform.position.x, 
            !_freezePosition.HasFlag(FreezePosition.y) ? position.y : transform.position.y,
            !_freezePosition.HasFlag(FreezePosition.z) ? position.z : transform.position.z);
    }
}

[Flags]
enum FreezePosition
{
    x=1,
    y=2,
    z=4
}
