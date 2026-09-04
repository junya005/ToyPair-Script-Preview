using UnityEngine;

/// <summary>
/// ターゲットにカメラを追従させるコンポーネント
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [SerializeField, Tooltip("追従するターゲットを設定してください")]
    private GameObject _targetObj;

    [SerializeField, Tooltip("追従ターゲットからどのぐらい座標を離すか設定してください")]
    private Vector3 _offset;

    void Update()
    {
        if (_targetObj == null) { return; }

        transform.position = _targetObj.transform.position + _offset;
    }

    /// <summary>
    /// オフセットを設定する
    /// </summary>
    /// <param name="value">設定する値</param>
    public void SetOffset(Vector3 value)
    {
        _offset = value;
    }
}
