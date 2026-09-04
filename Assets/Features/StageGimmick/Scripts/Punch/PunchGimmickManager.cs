using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// パンチギミック全体を制御するクラス
/// </summary>
public class PunchGimmickManager : MonoBehaviour
{
    [Header("オブジェクト参照")]
    [SerializeField, Tooltip("押されるボタンのリストを設定してください")]
    private List<PunchSwitch> _punchSwitches;

    public event Action AllPushedSwitchEvent;

    [SerializeField] private TwoSwitchGImmickOnlyWall onlyWall;


    private void Start()
    {
        if (onlyWall != null)
            AllPushedSwitchEvent += onlyWall.Hide;

        foreach (var punchSwitch in _punchSwitches)
        {
            // ボタンが押された時にすべて押されたか確認する処理が発動するようイベントを登録
            punchSwitch.PushEvent += () =>
            {
                if (IsAllSwitchPushed())
                {
                    OnAllPushed();
                }
            };
        }
    }

    private void OnDestroy()
    {
        AllPushedSwitchEvent = null;
    }

    /// <summary>
    /// 全てのボタンが押されたかチェックします
    /// </summary>
    /// <returns>すべてのボタンが押されたか</returns>
    private bool IsAllSwitchPushed()
    {
        foreach (var punchSwitch in _punchSwitches)
        {
            if (!punchSwitch.IsPushed)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 全てのボタンが押された時に発動する処理
    /// </summary>
    private void OnAllPushed()
    {
        if (IsAllSwitchPushed())
            AllPushedSwitchEvent?.Invoke();
    }
}
