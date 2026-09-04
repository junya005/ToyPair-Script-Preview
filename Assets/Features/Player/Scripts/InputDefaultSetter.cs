using UnityEngine;

/// <summary>
/// 操作ハンドラのデフォルト値を設定するコンポーネント
/// </summary>
public class InputDefaultSetter : MonoBehaviour
{
    [SerializeField, Tooltip("Player1 Controller")]
    private PlayerController _p1Controller;

    [SerializeField, Tooltip("Player2 Controller")]
    private PlayerController _p2Controller;

    [SerializeField, Tooltip("Joycon Input")]
    private JoyconInput _joyconInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputManager.Instance?.SetDefaultHandle(_p1Controller.PlayerIndex, _p1Controller.GetPlayerHandle(), true);
        InputManager.Instance?.SetDefaultHandle(_p2Controller.PlayerIndex, _p2Controller.GetPlayerHandle(), true);
        InputManager.Instance?.SetCurrentInput(_joyconInput);
    }
}
