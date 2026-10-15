using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

/// <summary>
/// プレイヤーの操作クラス
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CheckGround))]
[RequireComponent(typeof(PlayerHandle))]
[RequireComponent(typeof(PhysicsLocomotion))]
[RequireComponent(typeof(VelocityLimiter))]
[RequireComponent(typeof(VelocityBrake))]
[RequireComponent(typeof(ChargeDush))]
[RequireComponent(typeof(PhysicsJump))]
[RequireComponent(typeof(PlayerInteract))]
[RequireComponent(typeof(PlayerDeath))]
[RequireComponent(typeof(PlayerRespawn))]
[RequireComponent(typeof(PlayerView))]
[RequireComponent(typeof(PlayerStan))]
[RequireComponent(typeof(PlayerFrontalWallCollisionDetector))]
[RequireComponent(typeof(PlayerTeleport))]
[DefaultExecutionOrder(-100)]
public class PlayerController : MonoBehaviour, IGetPlayerStatus
{
    // 操作可能なプレイヤーのインデックス
    private static int _playerIndexCount = 0;
    public int PlayerIndex { get; private set; }

    /// <summary>プレイヤーが死亡状態か</summary>
    public bool IsDeath { get; private set; }

    /// <summary>プレイヤーがテレポート中か</summary>
    public bool IsTeleporting { get { return _playerTeleport.IsTeleporting; } }

    /// <summary>プレイヤーがダッシュ中かどうか</summary>
    public bool IsDushing { get { return _chargeDush.IsDushing; } }

    /// <summary>プレイヤーの現在速度</summary>
    public Vector3 CurrentVelocity { get { return _rb.linearVelocity; } }

    public event Action PlayerRespawnEvent;

    private CancellationTokenSource _animationCancellationTokenSource;

    // 外部コンポーネント
    private Rigidbody _rb;
    private CheckGround _checkGround;
    private PlayerHandle _playerHandle;
    private PhysicsLocomotion _physicsLocomotion;
    private VelocityLimiter _velocityLimiter;
    private PhysicsJump _physicsJump;
    private ChargeDush _chargeDush;
    private VelocityBrake _velocityBrake;
    private PlayerInteract _playerInteract;
    private PlayerDeath _playerDeath;
    private PlayerRespawn _playerRespawn;
    private PlayerView _playerView;
    private PlayerStan _playerStan;
    private PlayerFrontalWallCollisionDetector _playerFrontalWallCollisionDetector;
    private PlayerTeleport _playerTeleport;

    // 入力値
    private Vector2 _moveInput;
    private bool _jumpInput;
    private PlayerInputState _brakeInput;
    private PlayerInputState _chargeDushInput;
    private bool _interactInput;

    // デバッグ用
    [Header("デバックオプション")]
    [SerializeField, Tooltip("デバック用のウィンドウを表示する")]
    private bool _showDebbugWindow = false;

    private int _windowId = 0;

    private Rect _windowRect = new Rect(0, 0, 200, 400);

    private string _windowTitle = "プレイヤー移動のパラメータ";

    [SerializeField] private PlayerManager _playerManager;

    [Header("Parameter")]
    [SerializeField, Tooltip("プレイヤーインデックスを自動で振り分ける")]
    private bool _useAutoIndex = false;

    [SerializeField, Range(0, 1), Tooltip("手動指定時のプレイヤーインデックス")]
    private int _playerIndex;

    #region Unity Event

    private void Awake()
    {
        // コンポーネントを取得
        // (RequireComponentで指定しているため、必ず取得できる)
        _rb = GetComponent<Rigidbody>();
        _checkGround = GetComponent<CheckGround>();
        _playerHandle = GetComponent<PlayerHandle>();
        _physicsLocomotion = GetComponent<PhysicsLocomotion>();
        _velocityLimiter = GetComponent<VelocityLimiter>();
        _physicsJump = GetComponent<PhysicsJump>();
        _chargeDush = GetComponent<ChargeDush>();
        _velocityBrake = GetComponent<VelocityBrake>();
        _playerInteract = GetComponent<PlayerInteract>();
        _playerDeath = GetComponent<PlayerDeath>();
        _playerRespawn = GetComponent<PlayerRespawn>();
        _playerView = GetComponent<PlayerView>();
        _playerStan = GetComponent<PlayerStan>();
        _playerFrontalWallCollisionDetector = GetComponent<PlayerFrontalWallCollisionDetector>();
        _playerTeleport = GetComponent<PlayerTeleport>();

        if (IsExistComponents())
        {
            // プレイヤーインデックスの振り分け
            AssignPlayerIndex();
        }
    }

    private void Start()
    {
        if (!IsExistComponents()) { return; }

        // 外部コンポーネントに処理を登録
        _playerDeath.OnDeathEvent += (playerIndex) =>
        {
            Death();
        };

        _chargeDush.OnStartedCharge += () =>
        {
            _playerView.StartLoopEffect("EF_Player_Dash_Charge");
        };

        _chargeDush.OnCharged += (stepIndex) =>
        {
            _playerView.PlayEffectForDuration($"EF_Player_Dash_Impact_{stepIndex}", 1.0f);
        };

        _chargeDush.OnChargeEnded += () =>
        {
            _playerView.StopLoopEffect("EF_Player_Dash_Charge");
        };

        _chargeDush.OnDushed += (dushSec) =>
        {
            _playerView.PlayEffectForDuration("EF_Player_Dash_Activate", dushSec);
            _playerView.PlayEffectForDuration("EF_Player_Dash_Windbreak", dushSec);
        };

        _checkGround.OnAirborne +=() =>
        {
            _playerView.SetAnimTrigger("Jump");
        };

        // プレイヤー操作の処理を登録
        SubscribeInputProcess();

        // プレイヤー操作を有効化
        _playerHandle.EnableInput();
    }

    private void Update()
    {
        // アニメーション値をセット
        SetAnimationValuesOnUpdate();

        // 空中にいるときに煙エフェクトを無効化
        if (!_checkGround.IsGrounded)
        {
            _playerView.StopLoopEffect("EF_Player_WalkSmoke");
        }
        else
        {
            _playerView.StartLoopEffect("EF_Player_WalkSmoke");
        }

        _playerView.SetMoveInputValueToCharacterHead(_moveInput);
        Vector3 headDirection = _playerView.GetHeadDerection();
        Quaternion effectRot = Quaternion.Euler(
            Quaternion.LookRotation(headDirection).eulerAngles +
            new Vector3(0, -90, 0)
            );

        // エフェクトの方向をセット
        _playerView.SetEffectLocalRotation("EF_Player_Dash_Activate", effectRot);
        _playerView.SetEffectLocalRotation("EF_Player_Dash_Windbreak", effectRot);
    }

    private void FixedUpdate()
    {
        // 速度制限
        LimitSpeed();

        if (!_playerStan.IsStan)
        {
            // 移動処理
            Move(_moveInput);
            _physicsLocomotion.UseUndersteer(_chargeDush.IsBoosting);

            // ブレーキ処理
            Brake(_brakeInput);

            // ジャンプ処理
            Jump(_jumpInput);

            Vector3 headDirection = _playerView.GetHeadDerection();

            // 溜めダッシュ処理
            ChargeDush(new Vector2(
                    headDirection.x,
                    headDirection.z
                ),
                _chargeDushInput);

            // インタラクト処理
            Interact(_interactInput);
        }

        // スタンのチェックと実行
        CheckStan();
    }

    private void OnDestroy()
    {
        PlayerRespawnEvent = null;
    }

    private void OnGUI()
    {
        // デバック表示
        if (_showDebbugWindow)
            ShowParameters();
    }

    #endregion

    #region デバック

    /// <summary>
    /// デバック用のパラメータ情報を表示するウィンドウを表示する
    /// </summary>
    private void ShowParameters()
    {
        _windowRect = GUI.Window(_windowId, _windowRect, (id) =>
        {
            GUILayout.Label($"速度: {_rb.linearVelocity}");
            GUILayout.Label($"速さ制限: {_velocityLimiter.CurrentMaxSpeed}");
            GUILayout.Label($"水平方向の加速度: {_physicsLocomotion.Acceleration * _moveInput}");
            GUILayout.Label($"ジャンプ力: {_physicsJump.JumpPower}");

            GUILayout.Label($"床に設置しているか: {_checkGround.IsGrounded}");

            GUILayout.Label($"移動入力: {_moveInput}");
            GUILayout.Label($"ブレーキ入力: {_brakeInput}");
            GUILayout.Label($"溜めダッシュ入力: {_chargeDushInput}");

            GUI.DragWindow();
        }, _windowTitle);
    }

    #endregion

    /// <summary>
    /// 外部コンポーネントがすべて存在するか確認する
    /// </summary>
    /// <returns>存在したか</returns>
    private bool IsExistComponents()
    {
        return _rb != null &&
                _checkGround != null &&
                _playerHandle != null &&
                _velocityLimiter != null &&
                _physicsLocomotion != null &&
                _physicsJump != null &&
                _chargeDush != null &&
                _velocityBrake != null &&
                _playerInteract != null;
    }

    /// <summary>
    /// 入力値をすべてリセットします
    /// </summary>
    public void ResetInputValues()
    {
        _moveInput = Vector2.zero;
        _jumpInput = false;
        _brakeInput = PlayerInputState.Idle;
        _chargeDushInput = PlayerInputState.Idle;
        _interactInput = false;
    }

    /// <summary>
    /// 入力に対する処理を登録します
    /// </summary>
    private void SubscribeInputProcess()
    {
        // 入力コンポーネントに処理を登録
        _playerHandle.OnMoveInputChanged += (inputValue) =>
        {
            _moveInput = inputValue;
        };

        _playerHandle.OnJumpInputChanged += (jumpInput) =>
        {
            if (jumpInput)
            {
                _jumpInput = true;
            }
        };

        _playerHandle.OnBrakeInputChanged += (brakeInput) =>
        {
            if (_brakeInput == PlayerInputState.Idle)
            {
                _brakeInput = PlayerInputState.Hold;
            }
        };

        _playerHandle.OnBrakeInputRelesed += (brakeInputRelesed) =>
        {
            if (_brakeInput == PlayerInputState.Hold)
            {
                _brakeInput = PlayerInputState.Release;
            }
        };

        _playerHandle.OnChargeDushInputChanged += (chargeDushInput) =>
        {
            if (_chargeDushInput == PlayerInputState.Idle)
            {
                _chargeDushInput = PlayerInputState.Hold;
            }
        };

        _playerHandle.OnChargeDushInputRelesed += (chargeDushInputRelesed) =>
        {
            if (_chargeDushInput == PlayerInputState.Hold)
            {
                _chargeDushInput = PlayerInputState.Release;
            }
        };

        _playerHandle.OnInteractInputChanged += (interactInput) =>
        {
            Debug.Log($"Interact Input : {interactInput} frame:{Time.frameCount}");

            if (interactInput)
            {
                _interactInput = true;
            }
        };
    }

    /// <summary>
    /// 速度制限を掛ける
    /// </summary>
    private void LimitSpeed()
    {
        if (_velocityLimiter == null) return;
        if (_chargeDush == null) return;

        _velocityLimiter.LimitOnFixedUpdate();
        _chargeDush.DecelerationOnFixedUpdate();

        // 水平になった時に即時に留まるようにグリップを適用
        if (_moveInput == Vector2.zero && !_chargeDush.IsDushing && _rb.isKinematic == false)
        {
            _physicsLocomotion.ApplyGrip(0.9f);
        }
    }

    /// <summary>
    /// 移動処理を行う
    /// </summary>
    /// <param name="direction">方向</param>
    private void Move(Vector2 direction)
    {
        if (_physicsLocomotion == null) { return; }

        _physicsLocomotion.MoveOnFixedUpdate(direction);
    }

    /// <summary>
    /// ブレーキを掛ける
    /// </summary>
    /// <param name="brakeInput">ブレーキ入力状態</param>
    private void Brake(PlayerInputState brakeInput)
    {
        if (_velocityBrake == null) return;
        if (_chargeDush == null) return;

        // 溜めダッシュ中はブレーキ不可
        if (_chargeDush.IsCharging) { return; }

        if (brakeInput == PlayerInputState.Hold)
        {
            _velocityBrake.Brake();
            _playerView.PlaySoundsLoop(PlayerSounds.SoundType.Brake);
        }
        if (brakeInput == PlayerInputState.Release)
        {
            _velocityBrake.StopBraking();
            _brakeInput = PlayerInputState.Idle;
            _playerView.StopSoundsLoop();
        }
    }

    /// <summary>
    /// ジャンプする
    /// </summary>
    /// <param name="jumpInput">ジャンプ入力</param>
    private void Jump(bool jumpInput)
    {
        if (_physicsJump == null) return;
        if (_chargeDush == null) return;

        if (jumpInput)
        {
            _physicsJump.Execute();
            _chargeDush.CancelChargeDush();
            _jumpInput = false;

            if (_checkGround.IsGrounded)
                _playerView.PlaySoundsOneShot(PlayerSounds.SoundType.Jump);
        }
    }

    /// <summary>
    /// 溜めダッシュを行う
    /// </summary>
    /// <param name="moveInput">ダッシュの方向</param>
    /// <param name="chargeDushInput">チャージ入力状態</param>
    private void ChargeDush(Vector2 moveInput, PlayerInputState chargeDushInput)
    {
        if (_chargeDush == null) return;
        if (_velocityBrake == null) return;

        // ブレーキ中は溜めダッシュ不可
        if (_velocityBrake.IsBreaking) { return; }

        if (chargeDushInput == PlayerInputState.Hold)
        {
            _chargeDush.Charge();
            _playerView.PlaySoundsLoop(PlayerSounds.SoundType.Dush_Charge);
        }
        if (chargeDushInput == PlayerInputState.Release)
        {
            _chargeDush.Dush(moveInput);
            _chargeDushInput = PlayerInputState.Idle;
            _playerView.PlaySoundsOneShot(PlayerSounds.SoundType.Dush_Relese_1);
        }
    }

    /// <summary>
    /// プレイヤーがスタンの条件を満たしたか確認する
    /// スタンの実行まで行います
    /// </summary>
    /// <remarks>
    /// FixedUpdateで呼ばれることを想定しています
    /// </remarks>
    private void CheckStan()
    {
        if (_chargeDush == null) return;
        if (_playerStan == null) return;
        if (_playerFrontalWallCollisionDetector == null) return;
        if (_playerView == null) return;

        if (_playerFrontalWallCollisionDetector.IsCollition)
        {
            if (!_playerStan.IsStan && _chargeDush.IsDushing)
            {
                _playerView.PlaySoundsOneShot(PlayerSounds.SoundType.Collision_1);
                _playerView.PlaySoundsLoop(PlayerSounds.SoundType.Stan);

                Action cb = () =>
                {
                    _playerView.StopSoundsLoop();
                };

                _playerStan.Execute(_physicsLocomotion, cb);
            }
        }

    }

    /// <summary>
    /// インタラクトを行う
    /// </summary>
    /// <param name="interactInput">インタラクト入力</param>
    private void Interact(bool interactInput)
    {
        if (_playerInteract == null) return;
        if (_chargeDush == null) return;
        if (_checkGround == null) return;

        if (interactInput)
        {
            _interactInput = false;

            // 空中、溜めダッシュ中はインタラクト不可
            if (!_checkGround.IsGrounded) { return; }
            if (_chargeDush.IsDushing) { return; }

            if (/*_playerInteract.Execute() &&*/ _playerInteract.Execute(PlayerIndex))
            {
                _playerView.PlaySoundsOneShot(PlayerSounds.SoundType.Interact);

                //_playerHandle.DisableInput();
                Debug.Log("インタラクト実行");
                // 木村が修正　インタラクト解除を可能にするため
                /*if (_playerHandle.IsEnableInput)
                {
                    _playerHandle.DisableInput();
                }
                else
                {
                    _playerHandle.EnableInput();
                    RestartMove();
                }*/

                ResetInputValues();
            }
        }
    }

    /// <summary>
    /// 移動を再開する
    /// </summary>
    public void RestartMove()
    {
        if (_physicsLocomotion == null) return;

        _physicsLocomotion.RestartMove();
    }

    /// <summary>
    /// 操作可能なプレイヤーのインデックスを振り分けます
    /// </summary>
    private void AssignPlayerIndex()
    {
        if (_useAutoIndex)
        {
            PlayerIndex = _playerIndexCount;
            _playerIndexCount++;
        }
        else
        {
            PlayerIndex = _playerIndex;
        }

        _playerDeath.SetPlayerIndex(PlayerIndex);
        _playerRespawn.SetCharacterIndex(PlayerIndex);

        Debug.Log($"Index Assigned: {PlayerIndex}");
    }

    /// <summary>
    /// プレイヤーの死亡処理
    /// </summary>
    private void Death()
    {
        CancelAnimation();
        CancellationToken cancellationToken = GetNewAnimationCancellationToken();

        // 木村が追加
        // 死亡の回数をカウントする処理を追加
        if (_playerManager != null)
            _playerManager.IncrementDieCount(gameObject.tag);

        _playerView.PlaySoundsOneShot(PlayerSounds.SoundType.Death);

        Action cb = () =>
        {
            Respawn();
        };

        ResetPhysicsValues();

        IsDeath = true;
        _playerView.EnableMesh(false);
        _playerView.SetEnablePlayerSubMesh(false);
        _playerHandle.DisableInput();
        _velocityBrake.StopBraking();
        _chargeDush.CancelChargeDush();
        _physicsLocomotion.Stop();
        _playerView.PlayDeathAnimationAsync(2.0f, cb, cancellationToken).Forget();
    }

    /// <summary>
    /// プレイヤーのリスポーン処理
    /// </summary>
    private void Respawn()
    {
        CancelAnimation();
        CancellationToken cancellationToken = GetNewAnimationCancellationToken();

        Action cb = () =>
        {
            _playerHandle.EnableInput();
            _physicsLocomotion.RestartMove();
            _playerDeath.SetIsExecute(true);
        };

        IsDeath = false;
        _playerView.EnableMesh(true);
        _playerView.SetEnablePlayerSubMesh(true);
        _playerRespawn.Execute();
        PlayerRespawnEvent?.Invoke();
        _playerView.PlayRespawnAnimationAsync(1.0f, cb, cancellationToken).Forget();
    }

    /// <summary>
    /// アニメーターコントローラーの値を設定します。
    /// </summary>
    private void SetAnimationValuesOnUpdate()
    {
        _playerView.SetAnimParameter("VelocityValue",
                                    _rb.linearVelocity.magnitude / _velocityLimiter.BaseMaxSpeed);
        _playerView.SetAnimParameter("IsCharge", _chargeDush.IsCharging);
        _playerView.SetAnimParameter("IsCrashed", _playerStan.IsStan);
    }

    /// <summary>
    /// アニメーションをキャンセルする
    /// </summary>
    public void CancelAnimation()
    {
        if (_animationCancellationTokenSource != null)
        {
            _animationCancellationTokenSource.Cancel();
            _animationCancellationTokenSource = null;
        }
    }

    /// <summary>
    /// アニメーションキャンセル用のトークンを取得する
    /// </summary>
    /// <returns>キャンセル用トークン</returns>
    private CancellationToken GetNewAnimationCancellationToken()
    {
        _animationCancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = _animationCancellationTokenSource.Token;
        return cancellationToken;
    }

    /// <summary>
    /// 現在の加速度を取得する
    /// </summary>
    /// <returns>現在の加速度</returns>
    public float GetCurrentAccelaration()
    {
        return _physicsLocomotion.CurrentAcceleration;
    }

    /// <summary>
    /// 現在の加速度を設定する
    /// </summary>
    /// <param name="value">設定値</param>
    public void SetCurrentAccelaration(float value)
    {
        _physicsLocomotion.SetCurrentAcceleration(value);
    }

    /// <summary>
    /// 加速度を係数に応じて設定する
    /// </summary>
    /// <param name="accel">指定する係数</param>
    public void SetAccelarationByMultiply(float accel)
    {
        _physicsLocomotion.SetCurrentAccelerationByMultiply(accel);
    }

    /// <summary>
    /// 加速度をリセットする
    /// </summary>
    public void ResetAccelaration()
    {
        _physicsLocomotion.ResetCurrentAcceleration();
    }

    /// <summary>
    /// 移動方向を取得する
    /// </summary>
    /// <returns>
    /// 移動方向(水平方向のみ[yは0固定]の正規化ベクトル)
    /// </returns>
    public Vector3 GetMoveDirection()
    {
        Vector3 retVal = new Vector3(_playerHandle.MoveInput.x, 0, _playerHandle.MoveInput.y);
        return retVal.normalized;
    }

    /// <summary>
    /// 回転をリセットする
    /// </summary>
    public void RestRotation()
    {
        // TODO: α版後に処理を適したクラスへ移動
        _rb.angularVelocity = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// 現在のプレイヤーに追加で力を加えます
    /// </summary>
    /// <remarks>
    /// このAddForceはRigidbodyに加算される形で動作します
    /// </remarks>
    /// <param name="force">加える力</param>
    /// <param name="forceMode">加える力のモード</param>
    public void AddForce(Vector3 force, ForceMode forceMode)
    {
        _velocityLimiter.AddForceToAdditionalVelocity(force, forceMode);
    }

    /// <summary>
    /// 追加で加えた力をリセットします
    /// </summary>
    public void ResetForce()
    {
        _velocityLimiter.ResetAdditionalVelocity();
    }

    /// <summary>
    /// スピンを実行します
    /// </summary>
    public void ExecuteSpin()
    {
        _playerView.ExecuteSpinAnim();
    }

    /// <summary>
    /// プレイヤーを特定位置にテレポートさせます
    /// </summary>
    public void Teleport(Vector3 pos, Action cb = null, CancellationToken cancellationToken = default)
    {
        if (_playerTeleport == null) { return; }
        if (_playerTeleport.IsTeleporting == true) { return; }
        _playerTeleport.Execute(pos, cb, cancellationToken);
    }

    public void ExecuteDeath()
    {
        _playerDeath.Execute();
    }

    /// <summary>
    /// 物理挙動に関する値をリセットします
    /// </summary>
    public void ResetPhysicsValues()
    {
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _velocityLimiter.ResetAdditionalVelocity();
    }

    /// <summary>
    /// プレイヤーのハンドラを取得します
    /// </summary>
    /// <returns></returns>
    public PlayerHandle GetPlayerHandle()
    {
        return _playerHandle;
    }
}
