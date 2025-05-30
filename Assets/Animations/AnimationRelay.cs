using UnityEngine;

public class AnimationRelay : MonoBehaviour
{
    public Unit parentUnit;

    public void OnDeathAnimationComplete()
    {
        parentUnit?.OnDeathAnimationCompleteRpc();
    }
}