using System;
using UnityEngine;

/// <summary>
/// 速度を制限するクラス
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class VelocityLimiter : MonoBehaviour
{
    // 速度の倍率定数
    private const float NORMAL_SPEED_MULTIPLIER = 1.0f;
    private const float BRAKE_SPEED_MULTIPLIER = 0.3f;
    private const float DUSH_SPEED_MULTIPLIER = 1.3f;

    // 外部コンポーネント
    [Header("コンポーネント参照")]
    [SerializeField]
    private Rigidbody _rb;

    // パラメーター
    [Header("パラメーター")]
    [SerializeField, Tooltip("速さ制限の基礎値")]
    private float _baseLimitValue = 6.0f;

    private SpeedLimit _velocityLimit;

    // 外部からの力を制御するために追加
    private Vector3 _additionalVelocity = Vector3.zero;

    private bool _isStopLimitter = false;

    public float BaseMaxSpeed { get { return _velocityLimit.BaseValue; } }
    public float CurrentMaxSpeed { get { return _velocityLimit.CurrentValue; } }

    public void Awake()
    {
        if (_rb == null)
            _rb = GetComponent<Rigidbody>();

        _velocityLimit = new SpeedLimit(_baseLimitValue);
    }

    /// <summary>
    /// FixedUpdate内で実行想定のVelocity制限メソッド
    /// </summary>
    public void LimitOnFixedUpdate()
    {
        if (_rb == null) { return; }

        if (_isStopLimitter)
        {
            _isStopLimitter = false;
            return;
        }

        Vector3 clampedVelocity = Vector3.ClampMagnitude(_rb.linearVelocity, _velocityLimit.CurrentValue);
        float limitX = clampedVelocity.x, limitZ = clampedVelocity.z;

        if (Mathf.Abs(clampedVelocity.x) > _velocityLimit.CurrentValue)
            limitX = (clampedVelocity.x > 0) ? _velocityLimit.CurrentValue : -_velocityLimit.CurrentValue;

        if (Mathf.Abs(clampedVelocity.z) > _velocityLimit.CurrentValue)
            limitZ = (clampedVelocity.z > 0) ? _velocityLimit.CurrentValue : -_velocityLimit.CurrentValue;

        if (_rb.isKinematic != true)
        {
            Vector3 newVelocity = new Vector3(limitX, _rb.linearVelocity.y, limitZ) + _additionalVelocity;
            _rb.linearVelocity = newVelocity;
        }
    }

    /// <summary>
    /// 加速する
    /// </summary>
    /// <param name="targetSpeed">ターゲットの速度</param>
    /// <param name="direction">加速する方向</param>
    public void Accelerate(float targetSpeed, Vector2 direction)
    {
        if (_rb == null) { return; }

        _velocityLimit.SetCurrentValueByMultiply(targetSpeed);

        Vector3 force = new Vector3(
            direction.x * _velocityLimit.CurrentValue,
            0,
            direction.y * _velocityLimit.CurrentValue
        );

        if (direction != Vector2.zero)
            _rb.angularVelocity = Vector3.zero;
        _rb.linearVelocity = force;
        //_rb.AddForce(force, ForceMode.Acceleration);
    }

    /// <summary>
    /// 現在の速さ制限を基礎値にリセットする
    /// </summary>
    public void ResetCurrentMaxSpeedToBaseMaxSpeed()
    {
        _velocityLimit.ResetCurrentValueToBaseValue();
    }

    /// <summary>
    /// 現在の速さ制限を設定する
    /// </summary>
    /// <remarks>
    /// 基礎値を変えたくない場合(一時的なスピードアップ等)に使用してください
    /// </remarks>
    /// <param name="currentMaxSpeed">設定値</param>
    public void SetCurrentMaxSpeed(float currentMaxSpeed)
    {
        _velocityLimit.SetCurrentValue(currentMaxSpeed);
    }

    /// <summary>
    /// 最高速度を設定する
    /// </summary>
    /// <param name="targetSpeed">設定する値</param>
    public void SetCurrentMaxSpeedByMultiply(float targetSpeed)
    {
        _velocityLimit.SetCurrentValueByMultiply(targetSpeed);
    }

    public void StopLimiterInThisFrame()
    {
        _isStopLimitter = true;
    }

    /// <summary>
    /// 追加の速度に力を加えます
    /// </summary>
    /// <remarks>
    /// RigidbodyのAddForceをシミュレートしています
    /// 外部から力を加える場合に使用
    /// </remarks>
    /// <param name="force">加える力</param>
    /// <param name="forceMode">力のモード</param>
    public void AddForceToAdditionalVelocity(Vector3 force, ForceMode forceMode)
    {
        switch(forceMode)
        {
            case ForceMode.Force:
                _additionalVelocity += (force / _rb.mass) * Time.fixedDeltaTime;
                break;
            case ForceMode.Impulse:
                _additionalVelocity = force / _rb.mass;
                break;
            case ForceMode.Acceleration:
                _additionalVelocity += force * Time.fixedDeltaTime;
                break;
            case ForceMode.VelocityChange:
                _additionalVelocity = force;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 追加の力をリセットします。
    /// </summary>
    public void ResetAdditionalVelocity()
    {
        _additionalVelocity = Vector3.zero;
    }
}
