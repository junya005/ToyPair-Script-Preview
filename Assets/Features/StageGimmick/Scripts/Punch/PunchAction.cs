using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

/// <summary>
/// パンチの動作を行うクラス
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PunchAction : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField, Tooltip("パンチ後の最大高さ")]
    private float _maxOffset;

    [SerializeField, Tooltip("パンチアニメーション全体の長さ")]
    private float _duration;

    [
    SerializeField,
    Tooltip("パンチの前半のアニメーション時間割合(低くすればするほどパンチの出が早くなります。)"),
    Range(0.01f, 0.99f)
    ]
    private float _beforeRaito;

    private Vector3 _initPos;

    private Rigidbody _rb;

    private CancellationTokenSource _cancellationTokenSource;

    [SerializeField] private PunchGimmickPusher punchGimmickPusher;

    [SerializeField] private AnimationSE sE;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        if (punchGimmickPusher != null)
            punchGimmickPusher.OnGimmickClear += Punch;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _initPos = this.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Punch();
        }
    }

    private void FixedUpdate()
    {
        LimitPosOnFixedUpdate();
    }

    private void OnDestroy()
    {
        if (punchGimmickPusher != null)
        {
            punchGimmickPusher.OnGimmickClear -= Punch;
        }

        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }

    /// <summary>
    /// パンチを実行します
    /// </summary>
    public void Punch()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
        }

        _cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = _cancellationTokenSource.Token;

        PunchAsync(_duration, _beforeRaito, cancellationToken).Forget();
    }

    /// <summary>
    /// パンチマシーンの座標を制限します
    /// </summary>
    /// <remarks>
    /// FixedUpdateで呼ばれることを想定しています
    /// </remarks>
    private void LimitPosOnFixedUpdate()
    {
        float limitVecY = this.transform.localPosition.y;

        if (this.transform.localPosition.y > _initPos.y + _maxOffset)
        {
            limitVecY = _initPos.y + _maxOffset;
        }

        this.transform.localPosition = new Vector3(this.transform.localPosition.x, limitVecY, this.transform.localPosition.z);
    }

    /// <summary>
    /// パンチアニメーションを非同期で実行します
    /// </summary>
    /// <param name="duration">アニメーション全体の秒数</param>
    /// <param name="beforeRaito">全体の秒数の内、パンチの振り上げ動作に使う秒数の割合</param>
    private async UniTask PunchAsync(float duration, float beforeRaito, CancellationToken cancellationToken)
    {
        float beforeDuration = duration * beforeRaito;
        float afterDuration = duration * (1 - beforeRaito);
        float startPosY = _initPos.y;
        float targetPosY = _initPos.y + _maxOffset;
        float time = 0.0f;

        sE.PlaySE(1);

        while (time < beforeDuration)
        {
            this.transform.localPosition = Vector3.Lerp(
                new Vector3(this.transform.localPosition.x, startPosY, this.transform.localPosition.z),
                new Vector3(this.transform.localPosition.x, targetPosY, this.transform.localPosition.z),
                time / beforeDuration
            );

            time += Time.deltaTime;

            try
            {
                await UniTask.Yield();
            }
            catch (OperationCanceledException ex)
            {
                Debug.Log($"非同期処理は正常に終了しました。: {ex}");
                return;
            }
        }

        time = 0.0f;

        while (time < afterDuration)
        {
            this.transform.localPosition = Vector3.Lerp(
                new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, this.transform.localPosition.z),
                new Vector3(this.transform.localPosition.x, startPosY, this.transform.localPosition.z),
                time / afterDuration
            );

            time += Time.deltaTime;

            try
            {
                await UniTask.Yield();
            }
            catch (OperationCanceledException ex)
            {
                Debug.Log($"非同期処理は正常に終了しました。: {ex}");
                return;
            }
        }
    }


}
