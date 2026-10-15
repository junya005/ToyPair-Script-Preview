using System;
using UnityEngine;

/// <summary>
/// 床に接しているか調べるクラス
/// </summary>
/// <remarks>
/// RayCastを使用して判定を行っているため、汎用的に使用できる
/// </remarks>
public class CheckGround : MonoBehaviour
{
    [Header("パラメーター")]
    [SerializeField, Tooltip("床として判定するレイヤーを設定してください")]
    private LayerMask _targetLayer = new LayerMask();

    [SerializeField, Tooltip("床までの判定の長さを設定してください")]
    private float _rayDistance = 0.5f;

    /// <summary>床に触れているかのフラグ</summary>
    public bool IsGrounded { get; private set; } = false;

    private bool _isGroundedLastFixedFrame = false;

    /// <summary>床に設置した時に発火されるイベント</summary>
    public event Action OnGrounded = null;
    public event Action OnAirborne = null;

    // 物理演算と同期させるためFixedUpdateを使用しています
    private void FixedUpdate()
    {
        // 接触できる当たり判定のみ床とするため、QueryTriggerInteraction.Ignoreを設定
        IsGrounded = Physics.Raycast(
            this.transform.position,
            Vector3.down, _rayDistance,
            _targetLayer,
            QueryTriggerInteraction.Ignore
        );

        // 床に着地した時に処理を行えるようにイベントを発火
        if(IsGrounded && !_isGroundedLastFixedFrame)
            OnGrounded?.Invoke();

        // 床から離れた時に処理を行えるようにイベントを発火
        if(!IsGrounded && _isGroundedLastFixedFrame)
            OnAirborne?.Invoke();
        // 最後に、イベント発火の条件判定のため、現在の固定フレームの床設置フラグを保存
        _isGroundedLastFixedFrame = IsGrounded;
    }

    private void OnDestroy()
    {
        OnGrounded = null;
    }

    /// <summary>
    /// 床までの判定の長さを設定する
    /// オブジェクトの原点が足元でない場合、長めに設定することをお勧めします
    /// </summary>
    /// <param name="rayDistance">指定する長さ</param>
    public void SetRayDistance(float rayDistance)
    {
        _rayDistance = rayDistance;
    }
}
