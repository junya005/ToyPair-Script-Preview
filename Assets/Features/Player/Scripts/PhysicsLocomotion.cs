using System;
using UnityEngine;

/// <summary>
/// 移動機能を提供するコンポーネント
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PhysicsLocomotion : MonoBehaviour
{
    [Header("コンポーネント参照")]
    [SerializeField]
    private Rigidbody _rb;

    [Header("パラメーター")]
    [SerializeField, Tooltip("移動の加速度")]
    private float _acceleration = 1000.0f;

    [SerializeField, Tooltip("旋回性能")]
    private float _swingPerformance = 6.0f;

    public float CurrentAcceleration { get; private set; }

    public float Acceleration { get { return _acceleration; } }

    private bool _useUndersteer;

    private void Awake()
    {
        if (_rb == null)
            _rb = GetComponent<Rigidbody>();

        CurrentAcceleration = _acceleration;
    }

    /// <summary>
    /// FixedUpdateで実行想定の移動メソッド
    /// </summary>
    /// <param name="inputValue">入力値</param>
    public void MoveOnFixedUpdate(Vector2 inputValue)
    {
        if (_rb == null) { return; }

        Vector3 moveDirection = inputValue;

        if (_useUndersteer)
        {
            var k = Mathf.Clamp(_swingPerformance * _swingPerformance / _rb.linearVelocity.sqrMagnitude, 0.0f, 1.0f);
            moveDirection = Vector3.Lerp(_rb.linearVelocity.normalized, inputValue, k);
        }

        Vector3 force = new Vector3(
                    moveDirection.x * CurrentAcceleration,
                    0,
                    moveDirection.y * CurrentAcceleration
                );

        if ((_rb.linearVelocity.x < 0 && force.x > 0 ||
            _rb.linearVelocity.x > 0 && force.x < 0) &&
            force.z == 0)
        {
            _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0);
            _rb.angularVelocity = Vector3.zero;
        }

        if ((_rb.linearVelocity.z < 0 && force.z > 0 ||
            _rb.linearVelocity.z > 0 && force.z < 0) &&
            force.x == 0)
        {
            _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0);
            _rb.angularVelocity = Vector3.zero;
        }

        _rb.AddForce(force * Time.deltaTime);
        //_rb.AddForce(force, ForceMode.Acceleration);
    }

    /// <summary>
    /// 移動を完全に停止する
    /// </summary>
    public void Stop()
    {
        if (_rb == null) { return; }

        while (_rb.linearVelocity != Vector3.zero || _rb.angularVelocity != Vector3.zero)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        _rb.isKinematic = true;
    }

    /// <summary>
    /// 移動を再開する
    /// </summary>
    public void RestartMove()
    {
        if (_rb == null) { return; }

        _rb.isKinematic = false;

        while (_rb.linearVelocity != Vector3.zero || _rb.angularVelocity != Vector3.zero)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
    }

    /// <summary>
    /// グリップを適用する
    /// </summary>
    /// <param name="multiply">倍率</param>
    public void ApplyGrip(float multiply)
    {
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x * multiply,
                                        _rb.linearVelocity.y * 1.0f,
                                        _rb.linearVelocity.z * multiply);
        _rb.angularVelocity *= multiply;
    }

    /// <summary>
    /// 現在の加速度を設定する
    /// </summary>
    /// <param name="value">設定値</param>
    public void SetCurrentAcceleration(float value)
    {
        CurrentAcceleration = value;
    }

    /// <summary>
    /// 加速度を係数に応じて設定する
    /// </summary>
    public void SetCurrentAccelerationByMultiply(float multiply)
    {
        CurrentAcceleration = _acceleration * multiply;
    }

    /// <summary>
    /// 現在の加速度をベース値にリセットする
    /// </summary>
    public void ResetCurrentAcceleration()
    {
        CurrentAcceleration = _acceleration;
    }

    /// <summary>
    /// アンダーステアを適用するかどうかを設定します
    /// </summary>
    public void UseUndersteer(bool use)
    {
        _useUndersteer = use;
    }
}
