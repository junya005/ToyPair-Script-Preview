using TMPro;
using UnityEngine;

/// <summary>
/// パンチギミックがすべて押されたかをテストするクラス
/// </summary>
public class TestAllSwitchPushed : MonoBehaviour
{
    [Header("オブジェクト参照")]
    [SerializeField, Tooltip("パンチギミック全体の制御クラス")]
    private PunchGimmickManager _punchGimmickManager;

    [SerializeField, Tooltip("すべて押された際に表示されるテキスト")]
    private TextMeshProUGUI _tmp;

    void Start()
    {
        _tmp.enabled = false;

        _punchGimmickManager.AllPushedSwitchEvent += () =>
        {
            _tmp.enabled = true;
        };
    }
}
