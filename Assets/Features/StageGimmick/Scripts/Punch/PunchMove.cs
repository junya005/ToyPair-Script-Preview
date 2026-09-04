using UnityEngine;

/// <summary>
/// パンチの左右移動を行うクラス
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PunchMove : MonoBehaviour
{
    [Header("パラメータ")]
    [SerializeField, Tooltip("移動するX座標の最小値")]
    private float _min;

    [SerializeField, Tooltip("移動するX座標の最大値")]
    private float _max;

    [SerializeField, Tooltip("一回で移動する力の量")]
    private float _force = 1.5f;

    [SerializeField, Tooltip("徐々に戻る力の量"), Min(0)]
    private float _returnForce = 0.1f;

    [Header("デバック用オプション")]
    [SerializeField, Tooltip("デバック用の操作を使用する")]
    private bool _useDebugControl = false;

    private Rigidbody _rb;

    [Header("JoyCon入力")]
    [SerializeField] private JoyConInGameProvider _joyConProvider;

    [SerializeField] private PlayerSpin _player1Spin;
    [SerializeField] private PlayerSpin _player2Spin;

    [SerializeField] private PunchGimmickIntaracter _punchGimmickInteracter;



    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // デバッグ用
        if (_useDebugControl)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Move();
            }

            return;
        }

        // 操作中でなければ何もしない
        if (!_punchGimmickInteracter.IsOperating)
        {
            return;
        }

        if (_punchGimmickInteracter.CurrentPlayer == null)
        {
            return;
        }

        // Player1
        if (_punchGimmickInteracter.CurrentPlayer.CompareTag("Player1"))
        {
            if (_joyConProvider.GetP1Return())
            {
                Move();
                _player1Spin.Execute();
            }
        }
        // Player2
        else if (_punchGimmickInteracter.CurrentPlayer.CompareTag("Player2"))
        {
            if (_joyConProvider.GetP2Return())
            {
                Move();
                _player2Spin.Execute();
            }
        }



    }

    void FixedUpdate()
    {
        this.LimitPosOnFixedUpdate();

        _rb.AddForce(new Vector3(-_returnForce, 0, 0), ForceMode.VelocityChange);
    }

    /// <summary>
    /// 座標の制限を掛ける
    /// </summary>
    /// <remarks>
    /// FixedUpdateで呼ばれることを想定
    /// </remarks>
    private void LimitPosOnFixedUpdate()
    {
        float limitVecX = this.transform.localPosition.x;
        if (this.transform.localPosition.x < _min)
        {
            limitVecX = _min;
        }
        else if (this.transform.localPosition.x > _max)
        {
            limitVecX = _max;
        }

        this.transform.localPosition = new Vector3(limitVecX, this.transform.localPosition.y, this.transform.localPosition.z);
    }

    /// <summary>
    /// 移動する
    /// </summary>
    public void Move()
    {
        _rb.linearVelocity = new Vector3(0f, _rb.linearVelocity.y, _rb.linearVelocity.z);

        _rb.AddForce(Vector3.right * _force, ForceMode.Impulse);
    }
}
