using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SlopePlayerChecker))]
public class SlopeController : MonoBehaviour
{
    private SlopePlayerChecker _slopePlayerChecker;
    private SlopeEffectGrantor _slopeEffectGrantor;
    private SlopeConditionsChecker _slopeConditionsChecker;

    [Header("坂の角度")]
    [SerializeField] private float _slopeAngle = 30f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _slopePlayerChecker = GetComponent<SlopePlayerChecker>();
        _slopeEffectGrantor = new SlopeEffectGrantor();
        _slopeConditionsChecker = new SlopeConditionsChecker();

        _slopePlayerChecker.OnRemovedPlayer += (player) =>
        {
            _slopeEffectGrantor.StopGrantEffect(player);
        };
    }

    // Update is called once per frame
    void Update()
    {
        Dictionary<int, PlayerController> players = _slopePlayerChecker.HitPlayers;

        // 坂の角度からローカル法線を作る
        Vector3 localSlopeNormal = Quaternion.Euler(_slopeAngle, 0f, 0f) * Vector3.up;

        // 親のRotationも含めてワールド方向へ変換
        Vector3 slopeNormal = transform.TransformDirection(localSlopeNormal).normalized;

        foreach (var player in players)
        {
            Vector3 velocity =
               player.Value.CurrentVelocity.normalized;

            bool condition =
                _slopeConditionsChecker.CheckCondition(
                    velocity,
                    slopeNormal,
                    player.Value.IsDushing
                );

            if (condition)
                _slopeEffectGrantor.GrantEffect(player.Value, slopeNormal);
        }
    }
}
