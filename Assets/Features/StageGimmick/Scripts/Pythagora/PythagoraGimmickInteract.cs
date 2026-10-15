using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class PythagoraGimmickInteract : MonoBehaviour, IInteractable
{
    private const float INTERVAL = 0.5f;

    [SerializeField]
    private PythagoraGimmickHandler _handler;

    private bool _isControl = false;

    private float _intervalTime;

    [SerializeField] private InteractPopUpGuid interactPopUpGuid;

    [SerializeField] private PythagoraClaypipeMover _pythagoraClaypipeMover;

    [SerializeField] private GameObject _player1;
    [SerializeField] private GameObject _player2;

    [SerializeField] private GameObject _moveTarget;

    [SerializeField] private PlayerHandle _player1Handle;
    [SerializeField] private PlayerHandle _player2Handle;

    [SerializeField] private PlayerController _player1Controller;
    [SerializeField] private PlayerController _player2Controller;

    [SerializeField] private PhysicsLocomotion _player1PhysicsLocomotion;
    [SerializeField] private PhysicsLocomotion _player2PhysicsLocomotion;

    private int _controlPlayerIndex = -1;

    private void Awake()
    {
        if (_handler != null)
        {
            _handler.OnInteractInput += (playerIndex) =>
            {
                OnInteract(playerIndex);
            };
        }
    }

    private void Update()
    {
        if (_intervalTime > 0)
        {
            _intervalTime -= Time.deltaTime;
        }
    }

    public void OnInteract()
    {

    }

    public void OnInteract(int playerIndex)
    {
        if (_intervalTime > 0) return;

        StartInterval();

        if (_isControl)
        {
            // 操作中のプレイヤー以外は無視
            if (_controlPlayerIndex != playerIndex)
            {
                return;
            }


            // 操作解除
            InputManager.Instance.ResetHandle(playerIndex);

            if(playerIndex == 0)
            {
                _player1PhysicsLocomotion.RestartMove();
                EnablePlayerMove(_player1Handle);
            }
            else if(playerIndex == 1)
            {
                _player2PhysicsLocomotion.RestartMove();
                EnablePlayerMove(_player2Handle);
            }


            _isControl = false;
            _controlPlayerIndex = -1;

            _pythagoraClaypipeMover.MoveToInitial().Forget();

            return;
        }


        // 操作開始
        InputManager.Instance?.SetCurrentHandle(playerIndex, _handler);

        _handler.SetControlPlayerIndex(playerIndex);


        if (playerIndex == 0)
        {
            interactPopUpGuid?.Player1Hit();
            _player1PhysicsLocomotion.Stop();
            DisablePlayerMove(_player1Handle);
            MoveTarget(playerIndex);

        }
        else if (playerIndex == 1)
        {
            interactPopUpGuid?.Player2Hit();
            _player2PhysicsLocomotion.Stop();
            DisablePlayerMove(_player2Handle);
            MoveTarget(playerIndex);
        }


        _isControl = true;
        _controlPlayerIndex = playerIndex;

        _pythagoraClaypipeMover.MoveToTarget().Forget();
    }

    private void StartInterval()
    {
        _intervalTime = INTERVAL;
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
