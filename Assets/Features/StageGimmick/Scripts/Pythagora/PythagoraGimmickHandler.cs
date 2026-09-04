using System;
using UnityEngine;

public class PythagoraGimmickHandler : MonoBehaviour, IPlayerHandle
{
    [SerializeField, Tooltip("デバックコントロールを有効化する")]
    private bool _useDebbugControl = false;

    private int _controlPlayerIndex;

    public event Action OnJumpInput;
    public event Action<int> OnInteractInput;

    private void OnDestroy()
    {
        OnJumpInput = null;
        OnInteractInput = null;
    }

    private void Update()
    {
        if (_useDebbugControl)
            SetJumpInput(Input.GetKeyDown(KeyCode.G));
    }

    /// <summary>
    /// 移動入力を受け取ります。
    /// </summary>
    /// <param name="inputValue">入力値</param>
    public void SetMoveInput(Vector2 inputValue)
    {

    }

    /// <summary>
    /// ジャンプ入力を受け取ります。
    /// </summary>
    /// <param name="jumpInput">入力値</param>
    public void SetJumpInput(bool jumpInput)
    {
        if (jumpInput)
            OnJumpInput?.Invoke();
    }

    /// <summary>
    /// ブレーキ入力を受け取ります
    /// </summary>
    /// <param name="brakeInput">入力値</param>
    public void SetBrakeInput(bool brakeInput)
    {

    }

    /// <summary>
    /// ブレーキを離した時の入力を受け取ります
    /// </summary>
    /// <param name="brakeInputRelesed">入力値</param>
    public void SetBrakeInputRelesed(bool brakeInputRelesed)
    {

    }

    /// <summary>
    /// 溜めダッシュ入力を受け取ります。
    /// </summary>
    /// <param name="chargeDushInput">入力値</param>
    public void SetChargeDushInput(bool chargeDushInput)
    {

    }

    /// <summary>
    /// 溜めダッシュを離した時の入力を受け取ります
    /// </summary>
    /// <param name="chargeDushInputRelesed">入力値</param>
    public void SetChargeDushInputRelesed(bool chargeDushInputRelesed)
    {

    }

    /// <summary>
    /// インタラクト時の入力を受け取ります
    /// </summary>
    /// <param name="interactInput">入力値</param>
    public void SetInteractInput(bool interactInput)
    {
        if (interactInput)
            OnInteractInput?.Invoke(_controlPlayerIndex);
    }

    public void SetControlPlayerIndex(int playerIndex)
    {
        _controlPlayerIndex = playerIndex;
    }
}
