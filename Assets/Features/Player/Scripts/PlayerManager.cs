using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// プレイヤーの管理を行うクラス
/// </summary>
[DefaultExecutionOrder(1000)]
public class PlayerManager : MonoBehaviour
{
    [Header("オブジェクト参照")]
    [SerializeField, Tooltip("プレイヤー参照コンポーネント(プレイヤー1)")]
    private PlayerReferenceGetter _player1;

    [SerializeField, Tooltip("プレイヤー参照コンポーネント(プレイヤー2)")]
    private PlayerReferenceGetter _player2;

    [SerializeField, Tooltip("カメラターゲットグループ制御コンポーネント")]
    private CameraTargetGroupController _cameraTargetGroupController;

    [SerializeField, Tooltip("ステージの進行経路を示したSpline Container")]
    private SplineContainer _splineContainer;

    [Header("パラメータ")]
    [SerializeField, Tooltip("テレポートが発動するプレイヤー間の距離")]
    private Vector3 _teleportDistance = new Vector3(15.0f, 15.0f, 15.0f);

    private Dictionary<int, float> _nearestPoints = new Dictionary<int, float>();

    /// <summary>テレポート機能を使用するかどうか</summary>
    public bool UseTeleport { get; private set; } = true;

    /// <summary>片方のプレイヤーが死亡した時にもう片方をリスポーンさせるか</summary>
    public bool IsBothRespawn { get; private set; } = false;

    // キャッシュ用の変数
    private PlayerController _player1Controller;
    private PlayerController _player2Controller;
    private GameObject _player1Body;
    private GameObject _player2Body;

    // 木村が追加
    // 仮で死んだ回数をここにカウント
    public int DieCount { get; private set; } = 0;
    public int RedPlayerDieCount;
    public int BluePlayerDieCount;
    public int CoinCount = 0;
    public int StarCount = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_player1 == null || _player2 == null || _cameraTargetGroupController == null) { return; }

        // プレイヤーのコントローラーと本体オブジェクトをキャッシュ
        _player1Controller = _player1.GetPlayerController();
        _player2Controller = _player2.GetPlayerController();
        _player1Body = _player1.GetBody();
        _player2Body = _player2.GetBody();

        // ニアレストポイントの初期位置を追加
        if (_splineContainer != null)
        {
            try
            {
                _nearestPoints = new Dictionary<int, float>();

                SplineUtility.GetNearestPoint(_splineContainer.Spline, _player1Body.transform.position,
                                                out var player1NearestPoint, out var t1);
                _nearestPoints.Add(
                    _player1Controller.PlayerIndex,
                    t1
                    );

                SplineUtility.GetNearestPoint(_splineContainer.Spline, _player2Body.transform.position,
                                                out var player2NearestPoint, out var t2);
                _nearestPoints.Add(
                    _player2Controller.PlayerIndex,
                    t2
                    );
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"次のエラーが発生したため、NearestPointの登録を中止しました: \n{ex}");
            }

            //DisableTeleport();
        }

        // リスポーン時にカメラに追加されるよう設定
        _player1Controller.PlayerRespawnEvent += () =>
        {
            Debug.Log("Respawning Player1");
            //_cameraTargetGroupController.AddTarget(_player1Body.transform);
        };

        _player2Controller.PlayerRespawnEvent += () =>
        {
            Debug.Log("Respawning Player2");
            //_cameraTargetGroupController.AddTarget(_player2Body.transform);
        };
    }

    private void Update()
    {
        if (_splineContainer != null)
        {
            // プレイヤー座標をスプライン上の座標に変換して格納します
            SplineUtility.GetNearestPoint(_splineContainer.Spline, _player1Body.transform.position,
                                            out var player1NearestPoint, out var t1);
            _nearestPoints[_player1Controller.PlayerIndex] = t1;

            SplineUtility.GetNearestPoint(_splineContainer.Spline, _player2Body.transform.position,
                                            out var player2NearestPoint, out var t2);
            _nearestPoints[_player2Controller.PlayerIndex] = t2;
        }

        // 両方のプレイヤーをリスポーンさせる処理
        if (IsBothRespawn == true)
        {
            if (_player1Controller.IsDeath && !_player2Controller.IsDeath)
            {
                _player2Controller.ExecuteDeath();
            }

            if (_player2Controller.IsDeath && !_player1Controller.IsDeath)
            {
                _player1Controller.ExecuteDeath();
            }
        }

        if (UseTeleport == false) { return; }
        if (_player1Controller.IsTeleporting || _player2Controller.IsTeleporting) { return; }

        // プレイヤー同士の距離を判定し、一定距離以上になったらテレポートを実行します
        Vector3 distance = GetPlayerDicetance();
        if (distance.x > _teleportDistance.x || distance.y > _teleportDistance.y || distance.z > _teleportDistance.z)
        {
            PlayerController targetTransform = GetPlayerControllerInBack();
            Vector3 teleportPos = GetPlayerPosInFront();

            targetTransform.Teleport(teleportPos);
        }
    }

    /// <summary>テレポート機能を有効にします</summary>
    public void EnableTeleport()
    {
        UseTeleport = true;
    }

    /// <summary>テレポート機能を無効にします</summary>
    public void DisableTeleport()
    {
        UseTeleport = false;
    }

    public void SetIsBothRespawn(bool value)
    {
        IsBothRespawn = value;
    }

    /// <summary>
    /// プレイヤー同士の距離を取得します
    /// </summary>
    /// <returns>float型</returns>
    public Vector3 GetPlayerDicetance()
    {
        Vector3 player1Pos = _player1Body.transform.position;
        Vector3 player2Pos = _player2Body.transform.position;

        float x = Mathf.Abs(player1Pos.x - player2Pos.x);
        float y = Mathf.Abs(player1Pos.y - player2Pos.y);
        float z = Mathf.Abs(player1Pos.z - player2Pos.z);

        Vector3 distanceVector = new Vector3(x, y, z);

        return distanceVector;
    }

    /// <summary>
    /// 前にいるプレイヤーの座標を取得します
    /// </summary>
    /// <remarks>
    /// 位置が離れた際のテレポート処理に使用します
    /// </remarks>
    /// <returns>Vector3型</returns>
    private Vector3 GetPlayerPosInFront()
    {
        return GetPlayerTransformInFront().position;
    }

    /// <summary>
    /// 前にいるプレイヤーのTransformを取得します
    /// </summary>
    /// <remarks>
    /// SplineContainerが設定されていない場合は、プレイヤー1のTransformを返します
    /// </remarks>
    public Transform GetPlayerTransformInFront()
    {
        if (_splineContainer == null)
        {
            return _player1Body.transform;
        }

        if (_nearestPoints[_player1Controller.PlayerIndex] >
                _nearestPoints[_player2Controller.PlayerIndex])
        {
            return _player1Body.transform;
        }

        return _player2Body.transform;
    }

    /// <summary>
    /// 後ろにいるプレイヤーのPlayerControllerを取得します
    /// </summary>
    /// <remarks>
    /// SplineContainerが設定されていない場合は、プレイヤー2のPlayerControllerを返します
    /// </remarks>
    public PlayerController GetPlayerControllerInBack()
    {
        if (_splineContainer == null)
        {
            return _player2Controller;
        }

        if (_nearestPoints[_player1Controller.PlayerIndex] >
                _nearestPoints[_player2Controller.PlayerIndex])
        {
            return _player2Controller;
        }

        return _player1Controller;
    }

    public void IncrementDieCount(string playerTag)
    {
        DieCount++;

        if (playerTag == "Player1")
        {
            RedPlayerDieCount++;
        }
        else if (playerTag == "Player2")
        {
            BluePlayerDieCount++;
        }
    }
}
