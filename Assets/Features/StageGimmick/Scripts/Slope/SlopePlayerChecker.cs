using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// プレイヤーが坂に触れたかを管理するコンポーネント
/// </summary>
public class SlopePlayerChecker : MonoBehaviour
{
    private Dictionary<int, PlayerController> _hitPlayers = new Dictionary<int, PlayerController>();

    /// <summary>坂に触れているプレイヤーのハッシュ配列</summary>
    public Dictionary<int, PlayerController> HitPlayers { get { return _hitPlayers; } }

    public event Action<PlayerController> OnRemovedPlayer;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out var component))
        {
            _hitPlayers.Add(component.PlayerIndex, component);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerController>(out var component))
        {
            OnRemovedPlayer?.Invoke(component);
            _hitPlayers.Remove(component.PlayerIndex);
        }
    }
}
