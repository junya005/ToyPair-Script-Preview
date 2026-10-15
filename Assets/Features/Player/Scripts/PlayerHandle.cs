using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// プレイヤーの操作クラス
/// </summary>
public class PlayerHandle : MonoBehaviour, IPlayerHandle, IInputGetter
{
    public enum PlayerInputType
    {
        Move,
        Jump,
        Brake,
        ChargeDush,
        Interact
    }

    [Header("デバックオプション")]
    [SerializeField, Tooltip("デバック用の操作を使用するか")]
    private bool _useDebbugControl = false;

    [Header("パラメーター")]
    [SerializeField, Tooltip("ジャイロのデットゾーン（入力有効の閾値）")]
    private float _deadZone = 0.1f;

    public Vector2 MoveInput { get; private set; } = Vector2.zero;

    public bool IsEnableInput { get; private set; } = true;

    private Dictionary<PlayerInputType, bool> _enableInputs = new Dictionary<PlayerInputType, bool>();

    private Vector2 _moveInput = Vector2.zero;

    // 入力イベント
    public event Action<Vector2> OnMoveInputChanged;
    public event Action<bool> OnJumpInputChanged;
    public event Action<bool> OnBrakeInputChanged;
    public event Action<bool> OnBrakeInputRelesed;
    public event Action<bool> OnChargeDushInputChanged;
    public event Action<bool> OnChargeDushInputRelesed;
    public event Action<bool> OnInteractInputChanged;

    private void Start()
    {
        _enableInputs.Add(PlayerInputType.Move, true);
        _enableInputs.Add(PlayerInputType.Jump, true);
        _enableInputs.Add(PlayerInputType.Brake, true);
        _enableInputs.Add(PlayerInputType.ChargeDush, true);
        _enableInputs.Add(PlayerInputType.Interact, true);
    }

    private void Update()
    {
        // デバック用コントロール
        if (_useDebbugControl)
        {
            ControlForDebug();
        }
    }

    private void OnDestroy()
    {
        // 登録イベントを破棄
        OnMoveInputChanged = null;
        OnJumpInputChanged = null;
        OnBrakeInputChanged = null;
        OnBrakeInputRelesed = null;
        OnChargeDushInputChanged = null;
        OnChargeDushInputRelesed = null;
        OnInteractInputChanged = null;
    }

    /// <summary>
    /// デバック用の入力処理
    /// </summary>
    private void ControlForDebug()
    {
        var keyboard = Keyboard.current;

        // 移動入力
        Vector2 inputAxis = new Vector2(0, 0);

        if (_enableInputs[PlayerInputType.Move])
        {
            if (keyboard.wKey.isPressed)
            {
                inputAxis.y = 1;
            }
            if (keyboard.sKey.isPressed)
            {
                inputAxis.y = -1;
            }
            if (keyboard.aKey.isPressed)
            {
                inputAxis.x = -1;
            }
            if (keyboard.dKey.isPressed)
            {
                inputAxis.x = 1;
            }
        }

        SetMoveInput(inputAxis);

        // ジャンプ入力
        SetJumpInput(keyboard.spaceKey.wasPressedThisFrame);

        // ブレーキ入力
        SetBrakeInput(keyboard.shiftKey.isPressed);

        // ブレーキ離す入力
        SetBrakeInputRelesed(keyboard.shiftKey.wasReleasedThisFrame);

        // 溜めダッシュ入力
        SetChargeDushInput(keyboard.eKey.isPressed);

        // 溜めダッシュ離す入力
        SetChargeDushInputRelesed(keyboard.eKey.wasReleasedThisFrame);

        // インタラクト入力
        SetInteractInput(keyboard.fKey.wasPressedThisFrame);
    }

    /// <summary>
    /// 移動入力を受け取ります。
    /// </summary>
    /// <param name="inputValue">入力値</param>
    public void SetMoveInput(Vector2 inputValue)
    {
        if (!_enableInputs[PlayerInputType.Move])
        {
            Debug.Log("Move入力無効");
            _moveInput = Vector2.zero;
            var localAction = OnMoveInputChanged;
            if (localAction != null)
            {
                localAction.Invoke(_moveInput);
            }
            return;
        }

        // デットゾーンを適用
        inputValue = new Vector2(
            Mathf.Abs(inputValue.x) < _deadZone ? 0 : inputValue.x,
            Mathf.Abs(inputValue.y) < _deadZone ? 0 : inputValue.y
        );

        Vector2 clampedInputValue = Vector2.ClampMagnitude(inputValue, 1);
        _moveInput = clampedInputValue;

        MoveInput = clampedInputValue;

        var action = OnMoveInputChanged;
        if (action != null)
        {
            action.Invoke(clampedInputValue);
        }
        else
        {
            Debug.LogWarning("[PlayerHandle] OnMoveInputChanged is null");
        }
    }

    /// <summary>
    /// ジャンプ入力を受け取ります。
    /// </summary>
    /// <param name="jumpInput">入力値</param>
    public void SetJumpInput(bool jumpInput)
    {
        if (!_enableInputs[PlayerInputType.Jump]) { return; }

        if (!jumpInput) { return; }

        var action = OnJumpInputChanged;
        if (action != null)
        {
            action.Invoke(jumpInput);
        }
        else
        {
            Debug.LogWarning("[PlayerHandle] OnJumpInputChanged is null");
        }
    }

    /// <summary>
    /// ブレーキ入力を受け取ります
    /// </summary>
    /// <param name="brakeInput">入力値</param>
    public void SetBrakeInput(bool brakeInput)
    {
        if (!_enableInputs[PlayerInputType.Brake]) { return; }

        if (!brakeInput) { return; }

        var action = OnBrakeInputChanged;
        if (action != null)
        {
            action.Invoke(brakeInput);
        }
        else
        {
            Debug.LogWarning("[PlayerHandle] OnBrakeInputChanged is null");
        }
    }

    /// <summary>
    /// ブレーキを離した時の入力を受け取ります
    /// </summary>
    /// <param name="brakeInputRelesed">入力値</param>
    public void SetBrakeInputRelesed(bool brakeInputRelesed)
    {
        if (!brakeInputRelesed) { return; }

        var action = OnBrakeInputRelesed;
        if (action != null)
        {
            action.Invoke(brakeInputRelesed);
        }
        else
        {
            Debug.LogWarning("[PlayerHandle] OnBrakeInputRelesed is null");
        }
    }

    /// <summary>
    /// 溜めダッシュ入力を受け取ります。
    /// </summary>
    /// <param name="chargeDushInput">入力値</param>
    public void SetChargeDushInput(bool chargeDushInput)
    {
        if (!_enableInputs[PlayerInputType.ChargeDush]) { return; }

        if (!chargeDushInput) { return; }

        var action = OnChargeDushInputChanged;
        if (action != null)
        {
            action.Invoke(chargeDushInput);
        }
        else
        {
            Debug.LogWarning("[PlayerHandle] OnChargeDushInputChanged is null");
        }
    }

    /// <summary>
    /// 溜めダッシュを離した時の入力を受け取ります
    /// </summary>
    /// <param name="chargeDushInputRelesed">入力値</param>
    public void SetChargeDushInputRelesed(bool chargeDushInputRelesed)
    {
        if (!chargeDushInputRelesed) { return; }

        var action = OnChargeDushInputRelesed;
        if (action != null)
        {
            action.Invoke(chargeDushInputRelesed);
        }
        else
        {
            Debug.LogWarning("[PlayerHandle] OnChargeDushInputRelesed is null");
        }
    }

    /// <summary>
    /// インタラクト時の入力を受け取ります
    /// </summary>
    /// <param name="interactInput">入力値</param>
    public void SetInteractInput(bool interactInput)
    {
        //Debug.Log("SetInteractInput called with value: " + interactInput);
        // 木村が修正　インタラクトの解除を可能にするため↓のコードをコメントアウトした
        //if (!IsEnableInput) { return; }

        if (!interactInput) { return; }

        var action = OnInteractInputChanged;
        if (action != null)
        {
            action.Invoke(interactInput);
        }
        else
        {
            Debug.LogWarning("[PlayerHandle] OnInteractInputChanged is null");
        }
    }

    /// <summary>
    /// 入力を有効にします
    /// </summary>
    public void EnableInput()
    {
        IsEnableInput = true;
        foreach (var key in _enableInputs.Keys.ToList())
        {
            _enableInputs[key] = true;
        }
    }

    /// <summary>
    /// 入力を無効にします
    /// </summary>
    public void DisableInput()
    {
        IsEnableInput = false;
        foreach (var key in _enableInputs.Keys.ToList())
        {
            _enableInputs[key] = false;
        }
        Debug.Log("PlayerHandle DisableInput 実行");
    }

    /// <summary>
    /// 特定の操作の入力を有効、無効化します
    /// </summary>
    /// <param name="key"></param>
    public void SetEnableInput(PlayerInputType key, bool value)
    {
        if (_enableInputs.ContainsKey(key))
        {
            _enableInputs[key] = value;
        }
    }

    public Vector2 GetMoveInput()
    {
        return _moveInput;
    }
}
