using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// リスポーン地点を設定するためのコンポーネント
/// </summary>
public class RespawnPosSetter : MonoBehaviour
{
    [Header("オブジェクト参照")]
    [SerializeField, Tooltip("リスポーン地点にあるオブジェクトを設定してください")]
    private List<Transform> _respawnPosList = new List<Transform>();

    private bool _hitPlayer1 = false;
    private bool _hitPlayer2 = false;

    void OnTriggerEnter(Collider other)
    {
        // 当たったオブジェクトがリスポーン可能なコンポーネントを搭載しているかチェック
        if (other.TryGetComponent<IRespawnable>(out var component))
        {
            if (_respawnPosList.Count == 0) { return; }

            // 新しいリスポーン座標を登録する
            Vector3 targetPos = this.transform.position;
            foreach (var pos in _respawnPosList)
            {
                // すでに登録されていた場合は登録しない
                if (component.IsRegisteredSpawnPos(pos.position))
                {
                    continue;
                }

                if (targetPos == this.transform.position) { targetPos = pos.position; }
            }

            if (targetPos == this.transform.position) { return; }

            component.SetRespawnPoint(targetPos);
        }

        if (other.CompareTag("Player1"))
        {
            _hitPlayer1 = true;

            if(_hitPlayer1 && _hitPlayer2)
            {
                this.gameObject.SetActive(false);
            }
        }

        if (other.CompareTag("Player2"))
        {
            _hitPlayer2 = true;
            if (_hitPlayer1 && _hitPlayer2)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
