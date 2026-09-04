using UnityEngine;

public class SlopeEffectGrantor
{
    public void GrantEffect(PlayerController playerController, Vector3 slopeNormal)
    {
        // プレイヤーの今の速度全部消しちゃってるから一気に押し戻しちゃってて仕様の挙動と少し違うので変更
        /*Vector3 horizontalNormal = Vector3.ProjectOnPlane(Vector3.up, slopeNormal);
        playerController.AddForce(-playerController.CurrentVelocity -
                                horizontalNormal * playerController.CurrentVelocity.magnitude,
                                ForceMode.VelocityChange
                                );*/
        Vector3 velocity = playerController.CurrentVelocity;
        // 坂の面に沿った速度
        Vector3 slopeVelocity =　Vector3.ProjectOnPlane(velocity, slopeNormal);

        // 現在の速度から、坂の面に沿った速度だけを残す
        Vector3 correctionVelocity = slopeVelocity - velocity;

        playerController.AddForce(
            correctionVelocity,
            ForceMode.VelocityChange
        );
    }

    public void StopGrantEffect(PlayerController playerController)
    {
        playerController.ResetForce();
    }
}
