using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 入力処理を統括するクラス
/// </summary>
public class InputManager : SingletonMonoBehaviour<InputManager>
{
    /// 入力を受け取る側のインターフェース
    public Dictionary<int, IPlayerHandle> CurrentHandles { get; private set; } = new Dictionary<int, IPlayerHandle>();

    /// 入力を検知する側のインターフェース
    public IPlayerInput CurrentInput { get; private set; }

    private Dictionary<int, IPlayerHandle> _defaultHandle = new Dictionary<int, IPlayerHandle>();

    private void Start()
    {
        if (CurrentInput == null)
        {
            Debug.LogWarning("[InputManager] CurrentInput is null.");
        }
    }

    private void Update()
    {
        if (CurrentInput == null) return;

        var currentHandles = new Dictionary<int, IPlayerHandle>(CurrentHandles);
        foreach (var handle in currentHandles)
        {
            ExecuteSetInputValues(handle.Key, handle.Value);
        }
    }

    /// <summary>
    /// 入力値をプレイヤーのハンドルに渡します
    /// </summary>
    /// <param name="playerIndex">プレイヤーのインデックス</param>
    private void ExecuteSetInputValues(int playerIndex, IPlayerHandle playerHandle)
    {
        playerHandle.SetMoveInput(CurrentInput.MoveInputVector[playerIndex]);
        playerHandle.SetJumpInput(CurrentInput.JumpInput[playerIndex]);
        playerHandle.SetBrakeInput(CurrentInput.BrakeInput[playerIndex]);
        playerHandle.SetBrakeInputRelesed(CurrentInput.BrakeReleaseInput[playerIndex]);
        playerHandle.SetChargeDushInput(CurrentInput.ChargeDushInput[playerIndex]);
        playerHandle.SetChargeDushInputRelesed(CurrentInput.ChargeDushReleaseInput[playerIndex]);
        playerHandle.SetInteractInput(CurrentInput.InteractInput[playerIndex]);
    }

    /// <summary>
    /// 現在のプレイヤーの操作対象を設定します
    /// </summary>
    /// <param name="handle">操作可能なコンポーネント</param>
    public void SetCurrentHandle(int playerIndex, IPlayerHandle handle)
    {
        if (handle == null) return;

        if (CurrentHandles.ContainsKey(playerIndex))
        {
            CurrentHandles[playerIndex] = handle;
        }
        else
        {
            CurrentHandles.Add(playerIndex, handle);
        }
    }

    /// <summary>
    /// 現在の入力検知コンポーネントを設定します
    /// </summary>
    /// <param name="input">入力受け取り可能なコンポーネント</param>
    public void SetCurrentInput(IPlayerInput input)
    {
        CurrentInput = input;
    }

    /// <summary>
    /// プレイヤーのハンドルをリセットします
    /// </summary>
    public void ResetHandle(int playerIndex)
    {
        if (CurrentHandles.ContainsKey(playerIndex) &&
            _defaultHandle.ContainsKey(playerIndex))
        {
            CurrentHandles[playerIndex] = _defaultHandle[playerIndex];
        }
    }

    /// <summary>
    /// プレイヤーのデフォルトハンドルを設定します
    /// </summary>
    /// <param name="playerIndex">対象プレイヤーのインデックス</param>
    /// <param name="handle">対象プレイヤーのハンドル</param>
    /// <param name="isSetCurrent">そのまま現在のハンドルに設定するか</param>
    public void SetDefaultHandle(int playerIndex, IPlayerHandle handle, bool isSetCurrent)
    {
        if (handle == null)
        {
            Debug.LogWarning($"[InputManager] デフォルト値の設定に失敗しました。: {handle}");
            return;
        }

        if (_defaultHandle.ContainsKey(playerIndex))
        {
            _defaultHandle[playerIndex] = handle;
        }
        else
        {
            _defaultHandle.Add(playerIndex, handle);
        }

        if (isSetCurrent)
            SetCurrentHandle(playerIndex, handle);

        Debug.Log($"[InputManager] Setted Default Handle.: {playerIndex}, {handle}");
    }
}
