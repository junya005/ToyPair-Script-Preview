using UnityEngine;

/// <summary>
/// 丸影のレンダラーを追従させるコンポーネント
/// </summary>
/// <remarks>
/// このコンポーネントは位置だけでなく、影の大きさも自動計算
/// 計算自体は割合のため、元々の影のスケールを変更することで影の大きさを調整できる
/// </remarks>
public class ShadowRendererMover : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField, Tooltip("プレイヤーのTransform")]
    private Transform _playerTransform = null;

    [Header("Parameters")]
    [SerializeField, Tooltip("影が落ちる床オブジェクトのレイヤー")]
    private LayerMask _enableLayer = new LayerMask();

    [SerializeField, Tooltip("影が最小になる床との距離")]
    private float _maxHeight = 10f;

    [SerializeField, Tooltip("影の床からのオフセット")]
    private float _offsetY = 0.05f;

    private Vector3 _initLocalScale = Vector3.zero;

    void Awake()
    {
        // 影の大きさを計算するためにこのオブジェクトのスケール初期値を代入
        _initLocalScale = this.transform.localScale;
    }

    // 移動処理の完了直後に影を追従させるため、LateUpdateを使用
    void LateUpdate()
    {
        if(_playerTransform == null) return;

        float y = 0;

        Ray ray = new(_playerTransform.position, Vector3.down);

        // 透明な当たり判定に影が落ちないように、QueryTriggerInteraction.Ignoreを指定
        if(Physics.Raycast(
            ray,out RaycastHit hit,
            float.PositiveInfinity,
            _enableLayer,
            QueryTriggerInteraction.Ignore
        ))
        {
            y = hit.point.y;
        }

        // 影の大きさを算出するため、プレイヤーと地面の距離を求める
        float distanceY = _playerTransform.position.y - y;

        // _maxHeightを最小の大きさとして距離から大きさの割合(0~1)を計算
        float clampedY = Mathf.Clamp01(distanceY / _maxHeight);
        float sizeOffsetY = Mathf.Lerp(1f, 0f, clampedY);

        // 影の座標とスケールを適用
        this.transform.position = new Vector3(_playerTransform.position.x, y + 0.01f, _playerTransform.position.z);
        this.transform.localScale = _initLocalScale * sizeOffsetY;
    }
}
