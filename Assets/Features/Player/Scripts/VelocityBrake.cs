using System;
using UnityEngine;

/// <summary>
/// ブレーキ処理を行うクラス
/// </summary>
[RequireComponent(typeof(VelocityLimiter))]
public class VelocityBrake : MonoBehaviour
{
    private const float BRAKE_SPEED_MULTIPLIER = 0.3f;

    [Header("コンポーネント参照")]
    [SerializeField, Tooltip("速度制限コンポーネント")]
    private VelocityLimiter _velocityLimiter;

    private bool _isBreaking = false;

    public bool IsBreaking { get { return _isBreaking; } }

    private void Awake()
    {
        if (_velocityLimiter == null)
            _velocityLimiter = GetComponent<VelocityLimiter>();
    }

    /// <summary>
    /// FixedUpdate内で実行想定のブレーキメソッド
    /// </summary>
    /// <param name="breakInput">ブレーキ入力フラグ</param>
    public void BreakOnFixedUpdate(bool breakInput)
    {
        if (breakInput)
        {
            Brake();
        }
        else if (_isBreaking)
        {
            StopBraking();
        }
    }

    /// <summary>
    /// ブレーキを掛ける
    /// </summary>
    public void Brake()
    {
        if (_velocityLimiter == null) { return; }

        _velocityLimiter.SetCurrentMaxSpeedByMultiply(BRAKE_SPEED_MULTIPLIER);
        _isBreaking = true;
    }

    /// <summary>
    /// ブレーキを止める
    /// </summary>
    public void StopBraking()
    {
        if (_velocityLimiter == null) { return; }
        if (!_isBreaking) { return; }

        _velocityLimiter.ResetCurrentMaxSpeedToBaseMaxSpeed();
        _isBreaking = false;
    }
}
