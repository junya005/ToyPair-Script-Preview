using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

/// <summary>ピタゴラギミック全体を制御するコンポーネント</summary>
[RequireComponent(typeof(PythagoraGimmickHandler))]
public class PythagoraGimmickController : MonoBehaviour
{
    private const float INTERVAL = 0.1f;

    [SerializeField, Tooltip("ドアのコンポーネントリスト")]
    private List<PythagoraGimmickDoor> _doors;

    private PythagoraGimmickHandler _handler;

    private float _intervalTime;

    [SerializeField] private AnimationSE se;

    private void Awake()
    {
        _handler = this.GetComponent<PythagoraGimmickHandler>();
    }

    private void Start()
    {
        if (_handler != null)
        {
            _handler.OnJumpInput += () =>
            {
                if (_intervalTime > 0) return;
                OnSwitch();
            };
        }
    }

    private void Update()
    {
        // 連続入力による意図しない挙動を防ぐためインターバルを導入しています
        if (_intervalTime > 0)
        {
            _intervalTime -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 全てのドアの開閉状態を反転させる
    /// </summary>
    [Button]
    public void OnSwitch()
    {
        StartInterval();

        se.PlaySE(1);
        foreach (var door in _doors)
        {
            door.Switch();
        }
    }

    /// <summary>
    /// インターバルを開始する
    /// </summary>
    private void StartInterval()
    {
        _intervalTime = INTERVAL;
    }
}
