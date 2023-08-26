using UnityEngine;
using System;

public class HuntState : GhostState
{
    public static event Action onStart;
    public override void Init(GhostControler ghost)
    {
        onStart?.Invoke();
    }
    public override void Run()
    {

    }
}