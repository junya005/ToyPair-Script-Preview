using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System;
using System.Threading;
using UnityEngine;

/// <summary>ピタゴラギミックのドアコンポーネント</summary>
public class PythagoraGimmickDoor : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField, Tooltip("開閉状態の初期値を入力してください")]
    private bool _isOpen;

    [SerializeField, Tooltip("開扉時の回転値。TransformのRotationから、Copy Quaternionの数値をペーストすることを推奨します")]
    private Quaternion _openRot = Quaternion.identity;
    [SerializeField, Tooltip("閉扉時の回転値。TransformのRotationから、Copy Quaternionの数値をペーストすることを推奨します")]
    private Quaternion _closeRot = Quaternion.identity;



    [SerializeField, Tooltip("回転アニメーションの秒数")]
    private float _rotAnimTime;

    private CancellationTokenSource _rotAnimCancellationTokenSource;

    [SerializeField]
    private Collider _collider;

    private void Start()
    {
        // 初期状態を設定
        if (_isOpen)
        {
            this.transform.rotation = _openRot;
        }
        else
        {
            this.transform.rotation = _closeRot;
        }
    }

    /// <summary>
    /// 開閉状態を反転させ、対応するアニメーションを再生する
    /// </summary>
    [Button]
    public void Switch()
    {
        _isOpen = !_isOpen;

        // キャンセル用にトークンを新規作成する
        if (_rotAnimCancellationTokenSource != null)
        {
            _rotAnimCancellationTokenSource.Cancel();
            _rotAnimCancellationTokenSource = null;
        }

        _rotAnimCancellationTokenSource = new CancellationTokenSource();
        CancellationToken token = _rotAnimCancellationTokenSource.Token;

        UpdateStateAsync(token).Forget();
    }

    /// <summary>
    /// 開閉状態を非同期で更新します
    /// </summary>
    private async UniTask UpdateStateAsync(CancellationToken token = default)
    {
        if (_isOpen)
        {
            // 開く前に当たり判定を無効化
            if (_collider != null)
                _collider.enabled = false;

            await UpdateDoorRotationAsync(_closeRot, _openRot, _rotAnimTime, token);
        }
        else
        {
            await UpdateDoorRotationAsync(_openRot, _closeRot, _rotAnimTime, token);

            // 閉じた後に当たり判定を有効化
            if (_collider != null)
                _collider.enabled = true;
        }
    }

    /// <summary>
    /// 開閉アニメーションを非同期で行う
    /// </summary>
    /// <param name="startRot">回転開始値</param>
    /// <param name="endRot">回転終了値</param>
    /// <param name="duration">アニメーション全体の秒数</param>
    private async UniTask UpdateDoorRotationAsync(Quaternion startRot, Quaternion endRot, float duration, CancellationToken token = default)
    {
        try
        {
            float time = duration;

            while (time > 0.0f)
            {
                Quaternion angle = Quaternion.Slerp(endRot, startRot, time / duration);
                this.transform.rotation = angle;

                time -= Time.deltaTime;
                await UniTask.Yield(token);
            }
        }
        catch (OperationCanceledException ex)
        {
            Debug.Log($"[PytagoraGimmick] 回転アニメーションはキャンセルされました。\n{ex}");
        }
        finally
        {
            // どの場合でも正常な終了状態にする
            this.transform.rotation = endRot;
        }
    }
}
