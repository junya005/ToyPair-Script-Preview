using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

/// <summary>
/// プレイヤーのteleport処理を行うクラス
/// </summary>
public class PlayerTeleport : MonoBehaviour
{
    [Header("パラメータ")]
    [SerializeField, Tooltip("テレポートにかかる時間")]
    private float _duration = 1.0f;

    private Rigidbody _rb;

    /// <summary>テレポート中かどうか</summary>
    public bool IsTeleporting { get; private set; } = false;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// プレイヤーのテレポートを実行します
    /// </summary>
    /// <param name="pos">テレポート先座標</param>
    /// <param name="cb">テレポート後に行うコールバック</param>
    /// <param name="cancellationToken">キャンセル用トークン</param>
    public void Execute(Vector3 pos, Action cb = null, CancellationToken cancellationToken = default)
    {
        TeleportAsync(pos, cb, cancellationToken).Forget();
        IsTeleporting = true;
    }

    /// <summary>
    /// テレポートを非同期で実行します
    /// </summary>
    /// <param name="pos">テレポート先座標</param>
    /// <param name="cb">テレポート後に行うコールバック</param>
    /// <param name="cancellationToken">キャンセル用トークン</param>
    /// <returns></returns>
    private async UniTask TeleportAsync(Vector3 pos, Action cb = null, CancellationToken cancellationToken = default)
    {
        float time = 0.0f;
        _rb.isKinematic = true;
        Vector3 startPos = this.transform.position;
        Vector3 targetPos = pos;

        while (time < _duration)
        {
            this.transform.position = Vector3.Lerp(startPos, targetPos, time / _duration);
            time += Time.deltaTime;
            await UniTask.Yield();
        }

        cb?.Invoke();

        _rb.isKinematic = false;
        IsTeleporting = false;
    }
}
