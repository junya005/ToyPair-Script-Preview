using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// プレイヤーのスタン時の制御を行うクラス
/// </summary>
public class PlayerStan : MonoBehaviour
{
    /// <summary>プレイヤーがスタン中か</summary>
    public bool IsStan { get; private set; }

    [Header("パラメーター")]
    [SerializeField, Tooltip("スタン秒数")]
    private float _duration = 1.0f;

    private CancellationTokenSource _stanCancellationTokenSource;

    /// <summary>
    /// スタンを実行します。
    /// </summary>
    /// <remarks>
    /// このメゾットを介して非同期処理が実行されます。
    /// </remarks>
    /// <param name="locomotion"></param>
    public void Execute(PhysicsLocomotion locomotion, Action cb = null)
    {
        IsStan = true;
        locomotion.Stop();

        if (_stanCancellationTokenSource != null)
        {
            _stanCancellationTokenSource.Cancel();
            _stanCancellationTokenSource = null;
        }

        _stanCancellationTokenSource = new CancellationTokenSource();
        CancellationToken token = _stanCancellationTokenSource.Token;

        StanAsync(locomotion, _duration, cb, token).Forget();
    }

    /// <summary>
    /// 非同期でスタンを実行します
    /// </summary>
    /// <param name="locomotion">移動コンポーネント</param>
    /// <param name="duration">スタン秒数</param>
    private async UniTask StanAsync(PhysicsLocomotion locomotion, float duration,
                                    Action cb,CancellationToken cancellationToken = default)
    {
        try
        {
            await UniTask.WaitForSeconds(duration, cancellationToken: cancellationToken);
            locomotion.RestartMove();
            IsStan = false;
            cb?.Invoke();
        }
        catch (System.OperationCanceledException ex)
        {
            Debug.Log($"[PlayerStan] 非同期処理はキャンセルされました。:\n{ex}");
            locomotion.RestartMove();
            IsStan = false;
            cb?.Invoke();
        }
        catch (System.Exception ex)
        {
            Debug.Log($"[PlayerStan] 非同期処理で予期せぬエラーが発生しました。:\n{ex}");
            locomotion.RestartMove();
            IsStan = false;
            cb?.Invoke();
        }
    }

    /// <summary>
    /// スタン状態を強制的にキャンセルします
    /// </summary>
    public void CancelStan()
    {
        if (_stanCancellationTokenSource != null)
        {
            _stanCancellationTokenSource.Cancel();
            _stanCancellationTokenSource = null;
        }
    }
}
