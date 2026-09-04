using System.Collections.Generic;
using UnityEngine;

public interface IPlayerInput
{
    Dictionary<int, Vector2> MoveInputVector { get; }
    Dictionary<int, bool> JumpInput { get; }
    Dictionary<int, bool> BrakeInput { get; }
    Dictionary<int, bool> BrakeReleaseInput { get; }
    Dictionary<int, bool> ChargeDushInput { get; }
    Dictionary<int, bool> ChargeDushReleaseInput { get; }
    Dictionary<int, bool> InteractInput { get; }
}
