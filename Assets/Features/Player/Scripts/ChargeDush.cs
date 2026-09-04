using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// チャージダッシュを行うクラス
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(VelocityLimiter))]
[RequireComponent(typeof(CheckGround))]
public class ChargeDush : MonoBehaviour
{
    [System.Serializable]
    private class ChargeStep
    {
        [Tooltip("溜めにかかる時間(秒)")]
        public float chargeSecond;

        [Tooltip("チャージ後の速度")]
        public float speedMultiplier;
    }

    private const float CHARGE_SPEED_MULTIPLIER = 0.3f;

    [Header("コンポーネント参照")]
    [SerializeField]
    private Rigidbody _rb;
    [SerializeField]
    private VelocityLimiter _velocityLimiter;
    [SerializeField]
    private CheckGround _checkGround;

    [Header("パラメーター")]
    [SerializeField, Tooltip("ダッシュの溜め段階リスト")]
    private List<ChargeStep> _chargeStepList = new List<ChargeStep>();

    [SerializeField, Tooltip("ダッシュ後、減速までの時間(秒)")]
    private float _dushHoldSec = 1.0f;

    [SerializeField, Tooltip("チャージ中の移動速度倍率")]
    private float _chargeSpeedMultiplier = 0.3f;

    [SerializeField, Tooltip("ダッシュ後の減速の強さ")]
    private float _dushDecelerationPower = 1.5f;

    private bool _isCanceledChargeDash = false;
    private float _chargeTime = 0.0f;
    private bool _isCharging = false;

    public bool IsCharging { get { return _isCharging; } }

    private bool _isDushing = false;
    private float _dushTime = 0.0f;

    public bool IsDushing { get { return _isDushing; } }

    private bool _isBoosting = false;

    public bool IsBoosting { get { return _isBoosting; } }

    private Dictionary<int, bool> _isTriggeredMap = new Dictionary<int, bool>();

    // 演出トリガー用のイベントハンドラ
    public event Action OnStartedCharge;
    public event Action<int> OnCharged;
    public event Action OnChargeEnded;
    public event Action<float> OnDushed;

    private void Awake()
    {
        _chargeStepList.Sort((a, b) => b.chargeSecond.CompareTo(a.chargeSecond));
        _isTriggeredMap.Clear();
        for (int i = 0; i < _chargeStepList.Count; i++)
        {
            _isTriggeredMap.Add(i, false);
        }

        if (_rb == null)
            _rb = GetComponent<Rigidbody>();

        if (_velocityLimiter == null)
            _velocityLimiter = GetComponent<VelocityLimiter>();

        if (_checkGround == null)
            _checkGround = GetComponent<CheckGround>();
    }

    private void Oestroy()
    {
        OnStartedCharge = null;
        OnCharged = null;
        OnChargeEnded = null;
        OnDushed = null;
    }

    /// <summary>
    /// 溜めダッシュ後の減速処理
    /// </summary>
    public void DecelerationOnFixedUpdate()
    {
        if (!IsExistsComponents()) { return; }

        if (_isDushing)
        {
            _dushTime += Time.deltaTime;

            if (_dushTime < _dushHoldSec) { return; }

            if (_velocityLimiter.CurrentMaxSpeed > _velocityLimiter.BaseMaxSpeed)
            {
                _isBoosting = false;
                // _velocityLimiter.ResetCurrentMaxSpeedToBaseMaxSpeed();
                _velocityLimiter.SetCurrentMaxSpeed(_velocityLimiter.CurrentMaxSpeed - Time.deltaTime * _dushDecelerationPower);
            }
            else
            {
                _velocityLimiter.ResetCurrentMaxSpeedToBaseMaxSpeed();
                ResetDushState();
            }
        }
    }

    /// <summary>
    /// 溜める
    /// </summary>
    /// <remarks>
    /// ダッシュボタンが押されている時のイベントに登録してください
    /// </remarks>
    public void Charge()
    {
        if (!IsExistsComponents()) { return; }
        if (_isCanceledChargeDash) { return; }

        if (!_isCharging)
        {
            if (_checkGround.IsGrounded)
                _rb.AddForce(Vector3.up * 2.0f, ForceMode.Impulse);
            _velocityLimiter.ResetCurrentMaxSpeedToBaseMaxSpeed();
            ResetDushState();
            _isCharging = true;
            OnStartedCharge?.Invoke();
        }

        // 演出用トリガーの起動
        for (int i = 0; i < _chargeStepList.Count; i++)
        {
            if (_chargeTime > _chargeStepList[i].chargeSecond && !_isTriggeredMap[i])
            {
                OnCharged?.Invoke(_chargeStepList.Count - 1 - i);
                _isTriggeredMap[i] = true;
                break;
            }
        }

        _chargeTime += Time.deltaTime;
        _velocityLimiter.SetCurrentMaxSpeedByMultiply(CHARGE_SPEED_MULTIPLIER);
    }

    /// <summary>
    /// ダッシュする
    /// </summary>
    /// <remarks>
    /// ダッシュボタンが離された時のイベントに登録してください
    /// </remarks>
    /// <param name="moveInput"></param>
    public void Dush(Vector2 moveInput)
    {
        if (!IsExistsComponents()) { return; }

        for (int i = 0; i < _isTriggeredMap.Count; i++)
        {
            _isTriggeredMap[i] = false;
        }

        foreach (ChargeStep step in _chargeStepList)
        {
            if (_chargeTime > step.chargeSecond && !_isCanceledChargeDash)
            {
                _velocityLimiter.Accelerate(step.speedMultiplier, moveInput);
                _isDushing = true;
                _isBoosting = true;
                _dushTime = 0.0f;
                ResetChargeState();
                OnDushed?.Invoke(_dushHoldSec);
                return;
            }
        }

        _velocityLimiter.ResetCurrentMaxSpeedToBaseMaxSpeed();
        ResetChargeState();
    }

    /// <summary>
    /// チャージダッシュをキャンセルする
    /// </summary>
    public void CancelChargeDush()
    {
        if (!IsExistsComponents()) { return; }
        if (!_isCharging) return;

        _velocityLimiter.ResetCurrentMaxSpeedToBaseMaxSpeed();
        _isCanceledChargeDash = true;

    }

    /// <summary>
    /// ダッシュ状態をリセットします
    /// </summary>
    private void ResetDushState()
    {
        _isDushing = false;
        _dushTime = 0.0f;
    }

    /// <summary>
    /// チャージ状態をリセットします
    /// </summary>
    private void ResetChargeState()
    {
        _isCharging = false;
        _isCanceledChargeDash = false;
        _chargeTime = 0.0f;
        OnChargeEnded?.Invoke();
    }

    /// <summary>
    /// 全てのコンポーネントが存在しているかを確認します
    /// </summary>
    /// <returns>存在しているか</returns>
    private bool IsExistsComponents()
    {
        return _rb != null &&
                _velocityLimiter != null &&
                _checkGround != null;
    }
}
