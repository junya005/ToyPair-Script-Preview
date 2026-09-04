using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 入力の傾きを表示するためのコンポーネント
/// </summary>
public class DisplaySlopeToObject : MonoBehaviour
{
    private const float DEBUG_INPUT_VALUE = 1.0f;

    [Header("オブジェクト参照")]
    [SerializeField, Tooltip("傾きを表示するオブジェクト")]
    private Transform _targetTransform;

    [SerializeField, Tooltip("プレイヤー本体のオブジェクト")]
    private GameObject _player;

    [SerializeField, Tooltip("傾き表示モデルのレンダラー")]
    private MeshRenderer _renderer;

    [Header("デバック用オプション")]
    [SerializeField, Tooltip("デバック用の入力を使用するか")]
    private bool _useDebugControl = false;

    private Vector2 _moveInput = Vector2.zero;
    private IInputGetter _inputGetter;
    private IGetPlayerStatus _playerStatus;

    private void Awake()
    {
        if(_player == null){return;}
        _inputGetter = _player.GetComponent<IInputGetter>();
        _playerStatus = _player.GetComponent<IGetPlayerStatus>();
    }

    void Update()
    {
        // デバック用操作が有効であれば入力を受け取る
        if (_useDebugControl)
        {
            DebugControl();
        }
        else if (_inputGetter != null)
        {
            // プレイヤーの入力管理コンポーネントが設定されている場合、
            // 入力値を自動で反映
            _moveInput = _inputGetter.GetMoveInput();
        }

        if(_playerStatus != null || _renderer != null)
        {
            // プレイヤーの状態管理コンポーネントがある場合、
            // 自動的に傾き表示有効/無効化
            _renderer.enabled = !_playerStatus.IsDeath;
        }

        Vector2 input = new Vector2(_moveInput.x, _moveInput.y);
        Display(input);
    }

    /// <summary>
    /// デバック用のコントロール処理
    /// </summary>
    private void DebugControl()
    {
        var keyboard = Keyboard.current;

        if (keyboard.dKey.isPressed)
        {
            _moveInput.x = DEBUG_INPUT_VALUE;
        }
        else if (keyboard.aKey.isPressed)
        {
            _moveInput.x = -DEBUG_INPUT_VALUE;
        }
        else
        {
            _moveInput.x = 0.0f;
        }

        if (keyboard.wKey.isPressed)
        {
            _moveInput.y = DEBUG_INPUT_VALUE;
        }
        else if (keyboard.sKey.isPressed)
        {
            _moveInput.y = -DEBUG_INPUT_VALUE;
        }
        else
        {
            _moveInput.y = 0.0f;
        }
    }

    /// <summary>
    /// 入力値を設定する
    /// </summary>
    /// <param name="input">設定したい入力値</param>
    public void SetMoveInput(Vector2 input)
    {
        Mathf.Clamp(input.x, -1.0f, 1.0f);
        Mathf.Clamp(input.y, -1.0f, 1.0f);

        _moveInput = input;
    }

    /// <summary>
    /// 現在の傾きを表示
    /// </summary>
    public void Display(Vector2 input)
    {
        // 入力値の傾きを計算して、オブジェクトに適用(回転で表現)
        Transform target = _targetTransform != null ? _targetTransform : this.transform;
        Vector3 direction = Vector3.down - new Vector3(input.x, 0.0f, input.y);
        Quaternion rotation = Quaternion.LookRotation(direction);
        target.rotation = rotation;
    }
}
