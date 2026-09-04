using UnityEngine;

public class TestInputManager : MonoBehaviour
{
    [SerializeField]
    private PlayerController _playerController1;

    [SerializeField]
    private PlayerController _playerController2;

    [SerializeField]
    private PythagoraGimmickHandler _pythagoraGimmickHandler;

    [SerializeField]
    private JoyconInput _joyconInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        InputManager.Instance?.SetDefaultHandle(_playerController1.PlayerIndex,
                                                _playerController1.GetPlayerHandle(),
                                                true);
        InputManager.Instance?.SetDefaultHandle(_playerController2.PlayerIndex,
                                                _playerController2.GetPlayerHandle(),
                                                true);
        InputManager.Instance?.SetCurrentInput(_joyconInput);
    }
}
