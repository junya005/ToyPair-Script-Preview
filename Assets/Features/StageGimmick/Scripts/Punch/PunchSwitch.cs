using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

/// <summary>
/// パンチギミックのスイッチを操作するクラス
/// </summary>
public class PunchSwitch : MonoBehaviour, IPushAble
{
    // Vector3にすることで、角度がついたオブジェクトにも対応できるようにしています。
    [Header("パラメータ")]
    [SerializeField, Tooltip("押された後の高さのオフセット、0の場合は相対座標が使用されます")]
    private Vector3 _pushedOffset;

    public event Action PushEvent;

    private Vector3 _initLocalPos;

    private bool _isPushed = false;

    /// <summary>
    /// 押されているかをチェックする
    /// </summary>
    public bool IsPushed
    {
        get { return _isPushed; }
    }

    void Start()
    {
        _initLocalPos = this.transform.localPosition;
    }

    private void OnDestroy()
    {
        PushEvent = null;
    }

    /// <summary>
    /// スイッチを押す
    /// </summary>
    public void Push()
    {
        if (_isPushed) { return; }

        Action cb = () =>
        {
            _isPushed = true;
            PushEvent?.Invoke();
        };
        PushAsync(cb: cb).Forget();
    }

    /// <summary>
    /// スイッチを押すアニメーションを非同期で実行する
    /// </summary>
    /// <param name="cb">コールバック(Actionで受け取り)</param>
    private async UniTask PushAsync(Action cb = null, CancellationToken cancellationToken = default)
    {
        float duration = 0.5f;
        Vector3 startPos = _initLocalPos;
        Vector3 endPos = _initLocalPos - _pushedOffset;
        float time = 0.0f;

        while (time < duration)
        {
            this.transform.localPosition = Vector3.Lerp(
                startPos,
                endPos,
                time / duration
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

        cb?.Invoke();
    }
}
