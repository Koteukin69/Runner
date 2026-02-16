using Player;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 _followOffset = new (0, 5, -10);
    [SerializeField] private Vector3 _lookAtOffset = new (0, 0, 0);
    [SerializeField] private Transform _followTransform;

    private void OnValidate()
    {
        _followTransform ??= FindFirstObjectByType<PlayerMovement>().transform;
    }

    private void Start()
    {
        if (_lookAtOffset != Vector3.zero) transform.LookAt(-_lookAtOffset);
    }
    
    private void LateUpdate()
    {
        transform.position = _followTransform.position + _followOffset;
    }
}
