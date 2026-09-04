using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーのリスポーン機能を提供するコンポーネント
/// </summary>
public class PlayerRespawn : MonoBehaviour, IRespawnable
{
    private int _characterIndex = 0;

    /// <summary>リスポーン処理中か</summary>
    public bool IsRespawning { get; private set; } = false;

    private Vector3 _respawnPoint = Vector3.zero;

    // 
    public event Action OnRespawn;

    /// <summary>
    /// プレイヤーの現在のリスポーン座標を保存しておくDictionary
    /// </summary>
    /// <remarks>
    /// staticにすることで、複数プレイヤー間のリスポーン座標の設定状況を把握できるようにしています
    /// </remarks>
    private static Dictionary<int, Vector3> _posRegisters = new Dictionary<int, Vector3>();

    private void Awake()
    {
        // ゲーム開始時にリスポーン座標の登録を初期化しておく
        _posRegisters.Clear();
    }

    /// <summary>
    /// キャラクターのインデックスを設定します
    /// </summary>
    /// <param name="index">設定する数値</param>
    public void SetCharacterIndex(int index)
    {
        _characterIndex = index;
    }

    /// <summary>
    /// スポーン地点を設定します
    /// </summary>
    /// <param name="point">リスポーン座標</param>
    public void SetRespawnPoint(Vector3 point)
    {
        _respawnPoint = point;

        // 設定した座標をDictionaryに登録
        if (!_posRegisters.ContainsKey(_characterIndex))
        {
            _posRegisters.Add(_characterIndex, point);
        }
        else
        {
            _posRegisters[_characterIndex] = point;
        }

        Debug.Log($"現在のリスポーン位置{_posRegisters[_characterIndex]}");
    }

    /// <summary>
    /// リスポーン座標が登録済みか調べます
    /// </summary>
    /// <param name="spawnPoint">比較するリスポーン座標</param>
    /// <returns>登録されていたか</returns>
    public bool IsRegisteredSpawnPos(Vector3 spawnPoint)
    {
        bool respawnPointIsRegistered = false;

        // すでに登録されているかを確認
        foreach (var pos in _posRegisters)
        {
            if (pos.Value == spawnPoint)
            {
                respawnPointIsRegistered = true;
                break;
            }
        }

        return respawnPointIsRegistered;
    }

    /// <summary>
    /// リスポーン処理を実行します
    /// </summary>
    public void Execute()
    {
        this.transform.position = _respawnPoint;
        this.transform.rotation = Quaternion.identity;

        OnRespawn?.Invoke();

        // TODO: リスポーン演出の実行（フラグも同時に変更する）
    }
}
