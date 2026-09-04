using UnityEngine;
using System.Collections.Generic;

public class PythagoraGimmickInteract : MonoBehaviour, IInteractable
{
    private const float INTERVAL = 0.5f;

    [SerializeField]
    private PythagoraGimmickHandler _handler;

    private bool _isControl = false;

    private float _intervalTime;

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
            InputManager.Instance.ResetHandle(playerIndex);

            _isControl = false;

            return;
        }
        else
        {
            InputManager.Instance?.SetCurrentHandle(playerIndex, _handler);

            _handler.SetControlPlayerIndex(playerIndex);
            _isControl = true;
        }
    }

    private void StartInterval()
    {
        _intervalTime = INTERVAL;
    }
}
