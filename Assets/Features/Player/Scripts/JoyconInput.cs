using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class JoyconInput : MonoBehaviour, IPlayerInput
{

    private Joycon j1, j2;
    private Joycon _leftJoycon;
    private Joycon _rightJoycon;


    private Dictionary<int, Quaternion> _currentCenter = new Dictionary<int, Quaternion>();

    // 移動入力値が入ったDictionary群（intはプレイヤーインデックス）
    public Dictionary<int, Vector2> MoveInputVector { get; private set; } = new Dictionary<int, Vector2>();
    public Dictionary<int, bool> JumpInput { get; private set; } = new Dictionary<int, bool>();
    public Dictionary<int, bool> BrakeInput { get; private set; } = new Dictionary<int, bool>();
    public Dictionary<int, bool> BrakeReleaseInput { get; private set; } = new Dictionary<int, bool>();
    public Dictionary<int, bool> ChargeDushInput { get; private set; } = new Dictionary<int, bool>();
    public Dictionary<int, bool> ChargeDushReleaseInput { get; private set; } = new Dictionary<int, bool>();
    public Dictionary<int, bool> InteractInput { get; private set; } = new Dictionary<int, bool>();

    // 傾きリセットはここのみの特殊操作
    public Dictionary<int, bool> ResetSlopeInput { get; private set; } = new Dictionary<int, bool>();
    public Dictionary<int, bool> ResetSlopeRequest { get; private set; } = new Dictionary<int, bool>();

    private void Awake()
    {
        Initialize();
    }

    void Update()
    {
        AssignJoycons();
        //JoyConUtils.FixConnection(ref j1, ref j2);

        CheckInput(0);
        CheckInput(1);
    }

    /// <summary>
    /// joyconの左右を判別して変数に格納する
    /// </summary>
    private void AssignJoycons()
    {
        var joycons = JoyconManager.Instance.j;


        j1 = null; j2 = null;
        _leftJoycon = null;
        _rightJoycon = null;

#if PRO_CON

        j1 = joycons[0];
        j2 = joycons[1];

#else

        foreach (var joycon in joycons)
        {
            if (joycon.isLeft)
                _leftJoycon = joycon;
            else
                _rightJoycon = joycon;
        }

#endif
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Initialize()
    {
        // Dictionaryの初期化
        MoveInputVector.Add(0, new Vector2(0, 0));
        MoveInputVector.Add(1, new Vector2(0, 0));

        JumpInput.Add(0, false);
        JumpInput.Add(1, false);

        BrakeInput.Add(0, false);
        BrakeInput.Add(1, false);

        BrakeReleaseInput.Add(0, false);
        BrakeReleaseInput.Add(1, false);

        ChargeDushInput.Add(0, false);
        ChargeDushInput.Add(1, false);

        ChargeDushReleaseInput.Add(0, false);
        ChargeDushReleaseInput.Add(1, false);

        InteractInput.Add(0, false);
        InteractInput.Add(1, false);

        ResetSlopeInput.Add(0, false);
        ResetSlopeInput.Add(1, false);

        ResetSlopeRequest.Add(0, false);
        ResetSlopeRequest.Add(1, false);

        // 傾き数値の初期化
        // 横持ちだと初期の角度(Quaternion.identitiy)からの差分が発生するので、
        // 登録時に横持ちの角度を登録しておく
        _currentCenter.Add(0, Quaternion.Euler(new Vector3(-90f, 0f, 0f)));
        _currentCenter.Add(1, Quaternion.Euler(new Vector3(-90f, 0f, 0f)));
    }

    /// <summary>
    /// 入力があるかどうかをチェックする
    /// </summary>
    /// <param name="playerIndex">プレイヤーのインデックス</param>
    private void CheckInput(int playerIndex)
    {

        // どちらのジョイコンかを判別
#if PRO_CON
        if (JoyConUtils.CheckConnection(j1, j2))
        {

            Joycon j = playerIndex == 0 ? j1 : j2;
#else
        if (JoyConUtils.CheckConnection(_leftJoycon, _rightJoycon))
        {
            Joycon j = playerIndex == 0 ? _leftJoycon : _rightJoycon;
#endif

            // JoyConの傾きを計算
            Quaternion joyRotation = j.GetVector();

            // 基準値と現在の角度を互いに打ち消し、残った差分で傾き度合いを計算
            Quaternion relativeRot = Quaternion.Inverse(_currentCenter[playerIndex]) * joyRotation;
            Vector3 tilt = relativeRot * Vector3.up;

            // elseでプレイヤーが登録されていなかった時に登録処理を行う

            // 傾きリセット入力
            if (ResetSlopeInput.ContainsKey(playerIndex))
            {
#if PRO_CON
                Joycon.Button inputButton = Joycon.Button.X;
#else
                Joycon.Button inputButton = playerIndex == 0 ? Joycon.Button.DPAD_RIGHT : Joycon.Button.DPAD_LEFT;

#endif
                ResetSlopeInput[playerIndex] = j.GetButtonDown(inputButton);
            }
            else
            {
#if PRO_CON
                Joycon.Button inputButton = Joycon.Button.X;
#else
                Joycon.Button inputButton = playerIndex == 0 ? Joycon.Button.DPAD_RIGHT : Joycon.Button.DPAD_LEFT;

#endif
                ResetSlopeInput.Add(playerIndex, j.GetButtonDown(inputButton));
            }

            // 移動入力
            if (MoveInputVector.ContainsKey(playerIndex))
            {
#if PRO_CON
                float x = -tilt.z;
                float y = tilt.x;

                MoveInputVector[playerIndex] = new Vector2(y, -x);
#else
                float x = playerIndex == 0 ? -tilt.z : tilt.z;
                float y = playerIndex == 0 ? tilt.x : -tilt.x;

                MoveInputVector[playerIndex] = new Vector2(x, y);
#endif
            }
            else
            {
#if PRO_CON
                float x = -tilt.z;
                float y = tilt.x;

                MoveInputVector[playerIndex] = new Vector2(y, -x);
#else
                float x = playerIndex == 0 ? -tilt.z : tilt.z;
                float y = playerIndex == 0 ? tilt.x : -tilt.x;

                MoveInputVector[playerIndex] = new Vector2(x, y);
#endif
            }

            // ジャンプ入力
            if (JumpInput.ContainsKey(playerIndex))
            {
#if PRO_CON
                Joycon.Button inputButton = Joycon.Button.B;
#else
                Joycon.Button inputButton = playerIndex == 0 ? Joycon.Button.DPAD_LEFT : Joycon.Button.DPAD_RIGHT;
#endif

                JumpInput[playerIndex] = j.GetButtonDown(inputButton);
            }
            else
            {
#if PRO_CON
                Joycon.Button inputButton = Joycon.Button.B;
#else
                Joycon.Button inputButton = playerIndex == 0 ? Joycon.Button.DPAD_LEFT : Joycon.Button.DPAD_RIGHT;
#endif
                JumpInput.Add(playerIndex, j.GetButtonDown(inputButton));
            }


#if PRO_CON

            // ブレーキ入力
            if (BrakeInput.ContainsKey(playerIndex))
            {
                BrakeInput[playerIndex] = j.GetButtonDown(Joycon.Button.L)
                || j.GetButtonDown(Joycon.Button.R);
            }
            else
            {
                BrakeInput.Add(playerIndex, j.GetButtonDown(Joycon.Button.L)
                || j.GetButtonDown(Joycon.Button.R));
            }

            // ブレーキ離す入力
            if (BrakeReleaseInput.ContainsKey(playerIndex))
            {
                BrakeReleaseInput[playerIndex] = j.GetButtonUp(Joycon.Button.L)
                || j.GetButtonUp(Joycon.Button.R);
            }
            else
            {
                BrakeReleaseInput.Add(playerIndex, j.GetButtonUp(Joycon.Button.L)
                || j.GetButtonUp(Joycon.Button.R));
            }

#else
            // ブレーキ入力
            if (BrakeInput.ContainsKey(playerIndex))
            {
                BrakeInput[playerIndex] = j.GetButtonDown(Joycon.Button.SL)
                || j.GetButtonDown(Joycon.Button.SR);
            }
            else
            {
                BrakeInput.Add(playerIndex, j.GetButtonDown(Joycon.Button.SL)
                || j.GetButtonDown(Joycon.Button.SR));
            }

            // ブレーキ離す入力
            if (BrakeReleaseInput.ContainsKey(playerIndex))
            {
                BrakeReleaseInput[playerIndex] = j.GetButtonUp(Joycon.Button.SL)
                || j.GetButtonUp(Joycon.Button.SR);
            }
            else
            {
                BrakeReleaseInput.Add(playerIndex, j.GetButtonUp(Joycon.Button.SL)
                || j.GetButtonUp(Joycon.Button.SR));
            }
#endif

            // チャージダッシュ入力
            if (ChargeDushInput.ContainsKey(playerIndex))
            {
#if PRO_CON
                Joycon.Button inputButton = Joycon.Button.Y;
#else
                Joycon.Button inputButton = playerIndex == 0 ? Joycon.Button.DPAD_UP : Joycon.Button.DPAD_DOWN;
#endif

                ChargeDushInput[playerIndex] = j.GetButtonDown(inputButton);
            }
            else
            {
#if PRO_CON
                Joycon.Button inputButton = Joycon.Button.Y;
#else
                Joycon.Button inputButton = playerIndex == 0 ? Joycon.Button.DPAD_UP : Joycon.Button.DPAD_DOWN;
#endif
                ChargeDushInput.Add(playerIndex, j.GetButtonDown(inputButton));
            }

            // チャージダッシュ離す入力
            if (ChargeDushReleaseInput.ContainsKey(playerIndex))
            {
#if PRO_CON
                Joycon.Button inputButton = Joycon.Button.Y;
#else
                Joycon.Button inputButton = playerIndex == 0 ? Joycon.Button.DPAD_UP : Joycon.Button.DPAD_DOWN;
#endif
                ChargeDushReleaseInput[playerIndex] = j.GetButtonUp(inputButton);
            }
            else
            {
#if PRO_CON
                Joycon.Button inputButton = Joycon.Button.Y;
#else
                Joycon.Button inputButton = playerIndex == 0 ? Joycon.Button.DPAD_UP : Joycon.Button.DPAD_DOWN;
#endif
                ChargeDushReleaseInput.Add(playerIndex, j.GetButtonUp(inputButton));
            }

            // インタラクト入力
            if (InteractInput.ContainsKey(playerIndex))
            {
#if PRO_CON
                Joycon.Button inputButton = Joycon.Button.A;
#else
                Joycon.Button inputButton = playerIndex == 0 ? Joycon.Button.DPAD_DOWN : Joycon.Button.DPAD_UP;
#endif

                InteractInput[playerIndex] = j.GetButtonDown(inputButton);
                //Debug.Log($"Interact({playerIndex}) : {InteractInput[playerIndex]}");
            }
            else
            {
#if PRO_CON
                Joycon.Button inputButton = Joycon.Button.A;
#else
                Joycon.Button inputButton = playerIndex == 0 ? Joycon.Button.DPAD_DOWN : Joycon.Button.DPAD_UP;
#endif
                InteractInput.Add(playerIndex, j.GetButtonDown(inputButton));
                //Debug.Log($"Interact({playerIndex}) : {InteractInput[playerIndex]}");
            }

            // ここで傾きリセット処理
            if (ResetSlopeInput[playerIndex])
            {
                _currentCenter[playerIndex] = joyRotation;
            }

            if (ResetSlopeRequest[playerIndex])
            {
                _currentCenter[playerIndex] = j.GetVector();

                // コントローラのベクトル取得までにラグがあるため、
                // ラグ中に取得した数値であれば次のフレームでもリセットを実行するようにする
                if (_currentCenter[playerIndex].w != 1.0f)
                    ResetSlopeRequest[playerIndex] = false;
            }
        }
    }

    /// <summary>
    /// 指定プレイヤーの傾きをリセットします
    /// </summary>
    /// <param name="playerIndex">プレイヤーの番号</param>
    public void ResetSlope(int playerIndex)
    {
        ResetSlopeRequest[playerIndex] = true;
    }
}
