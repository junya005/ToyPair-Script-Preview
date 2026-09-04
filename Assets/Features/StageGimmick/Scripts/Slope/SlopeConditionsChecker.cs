using UnityEngine;

public class SlopeConditionsChecker
{
    public bool CheckCondition(Vector3 targetDirection, Vector3 slopeNormal, bool isDush)
    {
        Vector3 horizotalDirection = new Vector3(targetDirection.x, 0, targetDirection.z);
        float dot = Vector3.Dot(horizotalDirection, slopeNormal);
        return dot < -0.001f && !isDush;
    }
}
