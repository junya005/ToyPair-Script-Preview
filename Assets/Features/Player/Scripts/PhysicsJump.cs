using UnityEngine;
using System;

/// <summary>
/// ジャンプ機能を提供するコンポーネント
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CheckGround))]
public class PhysicsJump : MonoBehaviour
{
    [Header("コンポーネント参照")]
    [SerializeField]
    private Rigidbody _rb;

    [SerializeField]
    private CheckGround _checkGround;

    [Header("パラメーター")]
    [SerializeField]
    private float _jumpPower = 10.0f;

    public float JumpPower { get { return _jumpPower; } }

    /// <summary>ジャンプした際に発火するイベント</summary>
    /// <remarks>登録を行った場合、必ずOnDestroyメソッドで-=を使用して解除を行ってください</remarks>
    public event Action OnJumpEvent;

    public void Awake()
    {
        if (_rb == null)
            _rb = GetComponent<Rigidbody>();

        if (_rb == null)
            _checkGround = GetComponent<CheckGround>();
    }

    public void OnDestroy()
    {
        // 念のためイベントをクリア
        OnJumpEvent = null;
    }

    /// <summary>
    /// ジャンプを実行します
    /// </summary>
    public void Execute()
    {
        if (!IsExistsComponent()) { return; }

        if (!_checkGround.IsGrounded) { return; }

        // ジャンプ前に上方向の速度をリセット（木村が追加）
        Vector3 velocity = _rb.linearVelocity;
        velocity.y = 0f;
        _rb.linearVelocity = velocity;

        _rb.AddForce(Vector3.up * _jumpPower, ForceMode.Impulse);

        if (_checkGround.IsGrounded)
            OnJumpEvent?.Invoke();
    }

    /// <summary>
    /// 必要コンポーネントが存在するか確認する
    /// </summary>
    private bool IsExistsComponent()
    {
        return _rb != null && _checkGround != null;
    }
}

