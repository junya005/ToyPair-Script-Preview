using UnityEngine;

/// <summary>
/// JoyConをテストするためのコンポーネント
/// </summary>
public class TestJoyConInputConnector : MonoBehaviour
{
    [Header("コンポーネント参照")]
    [SerializeField, Tooltip("ジョイコン入力コンポーネント")]
    private JoyconInput _joyconInput;

    [SerializeField, Tooltip("プレイヤー操作コンポーネント")]
    private PlayerHandle _playerHandle;

    [SerializeField, Tooltip("プレイヤー操作コンポーネント")]
    private PlayerHandle _playerHandle2;

    private bool _isInitializedSlope = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        if (!_isInitializedSlope)
        {
            _joyconInput.ResetSlope(0);
            _joyconInput.ResetSlope(1);
            _isInitializedSlope = true;
        }

        ExecuteSetInputValues(0);
        ExecuteSetInputValues(1);
    }

    /// <summary>
    /// プレイヤーのハンドルに入力値を渡す
    /// </summary>
    /// <param name="playerIndex">プレイヤーのインデックス</param>
    private void ExecuteSetInputValues(int playerIndex)
    {
        PlayerHandle playerHandle = playerIndex == 0 ? _playerHandle : _playerHandle2;

        playerHandle.SetMoveInput(_joyconInput.MoveInputVector[playerIndex]);
        playerHandle.SetJumpInput(_joyconInput.JumpInput[playerIndex]);
        playerHandle.SetBrakeInput(_joyconInput.BrakeInput[playerIndex]);
        playerHandle.SetBrakeInputRelesed(_joyconInput.BrakeReleaseInput[playerIndex]);
        playerHandle.SetChargeDushInput(_joyconInput.ChargeDushInput[playerIndex]);
        playerHandle.SetChargeDushInputRelesed(_joyconInput.ChargeDushReleaseInput[playerIndex]);
        playerHandle.SetInteractInput(_joyconInput.InteractInput[playerIndex]);
    }
}
