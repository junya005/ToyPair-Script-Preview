using UnityEngine;

/// <summary>
/// パンチ動作のインタラクト
/// </summary>
public class PunchInteract : MonoBehaviour, IInteractable
{
    [SerializeField, Tooltip("パンチ操作のハンドラを設定してください")]
    private PunchHandler _punchHandler;

    [SerializeField]
    private InteractPopUpGuid interactPopUpGuid;

    private bool _isControl = false;


    [SerializeField] private GameObject _player1;
    [SerializeField] private GameObject _player2;

    [SerializeField] private GameObject _moveTarget;

    [SerializeField] private PlayerHandle _player1Handle;
    [SerializeField] private PlayerHandle _player2Handle;

    [SerializeField] private PlayerController _player1Controller;
    [SerializeField] private PlayerController _player2Controller;

    [SerializeField] private PhysicsLocomotion _player1PhysicsLocomotion;
    [SerializeField] private PhysicsLocomotion _player2PhysicsLocomotion;


    public void OnInteract()
    {

    }

    public void OnInteract(int playerIndex)
    {
        if (_punchHandler == null) return;

        // 操作中であればハンドラをリセット、そうで無ければハンドラを設定
        if (_isControl == true)
        {
            InputManager.Instance?.ResetHandle(playerIndex);
            if(playerIndex == 0)
            {
                EnablePlayerMove(_player1Handle);
                _player1PhysicsLocomotion.RestartMove();
            }
            else
            {
                EnablePlayerMove(_player2Handle);
                _player2PhysicsLocomotion.RestartMove();
            }
            _isControl = false;
        }
        else if (_punchHandler != null && _isControl == false)
        {
            InputManager.Instance?.SetCurrentHandle(playerIndex, _punchHandler);
            _punchHandler.SetControlPlayerIndex(playerIndex);
            if(playerIndex == 0)
            {
                interactPopUpGuid.Player1Hit();
                _player1PhysicsLocomotion.Stop();
                DisablePlayerMove(_player1Handle);
                MoveTarget(playerIndex);
            }
            else
            {
                interactPopUpGuid.Player2Hit();
                _player2PhysicsLocomotion.Stop();
                DisablePlayerMove(_player2Handle);
                MoveTarget(playerIndex);
            }
            _isControl = true;
        }
    }

    private void MoveTarget(int index)
    {
        if (index == 0)
        {
            _player1.transform.position = _moveTarget.transform.position;
        }
        else if (index == 1)
        {
            _player2.transform.position = _moveTarget.transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            interactPopUpGuid?.PlayerHit();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player1") || other.CompareTag("Player2"))
        {
            interactPopUpGuid?.Reset();
        }
    }

    private void DisablePlayerMove(PlayerHandle handle)
    {
        handle.SetEnableInput(PlayerHandle.PlayerInputType.Move, false);
        handle.SetEnableInput(PlayerHandle.PlayerInputType.Jump, false);
        handle.SetEnableInput(PlayerHandle.PlayerInputType.Brake, false);
        handle.SetEnableInput(PlayerHandle.PlayerInputType.ChargeDush, false);
    }

    private void EnablePlayerMove(PlayerHandle handle)
    {
        handle.SetEnableInput(PlayerHandle.PlayerInputType.Move, true);
        handle.SetEnableInput(PlayerHandle.PlayerInputType.Jump, true);
        handle.SetEnableInput(PlayerHandle.PlayerInputType.Brake, true);
        handle.SetEnableInput(PlayerHandle.PlayerInputType.ChargeDush, true);
    }
}
