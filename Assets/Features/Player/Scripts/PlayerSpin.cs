using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// プレイヤーのスピン挙動を制御するコンポーネント
/// </summary>
public class PlayerSpin : MonoBehaviour
{
    private const float SPIN_ONCE_VALUE = 360.0f;

    /// <summary>ターゲットの初期回転値</summary>
    /// <remarks>エディタ終了時に開始時の状態に戻すために使用します。</remarks>
    private List<Vector3> _initRotList = new List<Vector3>();

    [Header("オブジェクト参照")]
    [SerializeField, Tooltip("回転対象のオブジェクトのリスト")]
    private List<Transform> _targetTransformList = new List<Transform>();

    [Header("パラメータ")]
    [SerializeField, Tooltip("1回転にかかる時間")]
    private float _duration = 0.2f;

    [SerializeField, Tooltip("回転回数"), Min(1)]
    private int _rotCount = 3;

    [Header("デバックオプション")]
    [SerializeField, Tooltip("デバック用の操作を使用するか")]
    private bool _useDebugControl = false;

    private CancellationTokenSource _cancellationTokenSource;

    private bool _isSpin = false;
    private bool _requestSpin = false;

    private void Start()
    {
        // 回転ターゲットの初期回転値をリストに追加
        foreach (var targetTransform in _targetTransformList)
        {
            _initRotList.Add(targetTransform.rotation.eulerAngles);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if(_useDebugControl)
        {
            // デバック用にPキーでスピン挙動
            if (Input.GetKeyDown(KeyCode.P))
            {
                Execute();
            }
        }

        // 先行入力があった場合、スピンを実行します
        if (_requestSpin)
        {
            _requestSpin = false;
            Execute();
        }
    }

    private void OnDestroy()
    {
        // エディタ終了時に非同期処理をキャンセル
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = null;
        }
    }

    /// <summary>
    /// スピンアニメーションを実行します
    /// </summary>
    public void Execute()
    {
        // 回転中は先行入力として受付
        if (_isSpin)
        {
            _requestSpin = true;
            return;
        }

        // 非同期処理が残っている可能性があるので、キャンセル
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = null;
        }

        // キャンセル用のトークン生成
        _cancellationTokenSource = new CancellationTokenSource();
        CancellationToken token = _cancellationTokenSource.Token;

        // 回転処理を非同期で実行
        SpinAsync(token).Forget();
    }

    /// <summary>
    /// 非同期でスピンアニメーションを実行します
    /// </summary>
    /// <param name="cancellationToken">キャンセル用トークン</param>
    private async UniTask SpinAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _isSpin = true;


            // 開始と終了地点データの作成
            List<Vector3> startRotList = new List<Vector3>();

            foreach (var targetTransform in _targetTransformList)
            {
                startRotList.Add(targetTransform.rotation.eulerAngles);
            }

            List<Vector3> targetRotList = new List<Vector3>();

            foreach (var targetTransform in _targetTransformList)
            {
                Vector3 targetRotEuler = targetTransform.rotation.eulerAngles + SPIN_ONCE_VALUE * Vector3.up;
                targetRotList.Add(targetRotEuler);
            }

            int count = _rotCount;
            float duration = _duration/count;

            // 回転アニメーションの実行
            for(int  j= 0; j < count ; ++j)
            {
                float time = 0.0f;
                while (time < duration)
                {
                    for (int i = 0; i < _targetTransformList.Count; ++i)
                    {
                        _targetTransformList[i].rotation = Quaternion.Euler(
                            Vector3.Lerp(
                                startRotList[i],
                                targetRotList[i],
                                time / duration
                            ));
                    }
                    time += Time.deltaTime;


                    await UniTask.Yield(cancellationToken: cancellationToken);
                }
                }

                // 回転前の位置に戻す
                for (int i = 0; i < _targetTransformList.Count; ++i)
                {
                    _targetTransformList[i].rotation = Quaternion.Euler(startRotList[i]);
                }

                _isSpin = false;
        }
        catch (OperationCanceledException ex)
        {
            // キャンセル時の処理
            _isSpin = false;

            for (int i = 0; i < _targetTransformList.Count; ++i)
            {
                _targetTransformList[i].rotation = Quaternion.Euler(_initRotList[i]);
            }

            Debug.Log($"正常にキャンセルされました: \n{ex}");
            return;
        }
    }
}
