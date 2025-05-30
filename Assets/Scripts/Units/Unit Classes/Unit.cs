using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

abstract public class Unit : NetworkBehaviour
{
    public int armyId;

    [SerializeField] private NetworkVariable<float> health = new NetworkVariable<float>(10);
    [SerializeField] private NetworkVariable<int> gridSpeed = new NetworkVariable<int>(3); // How many tiles a unit can move in a turn. Will be shown as "Speed" in game.
    [SerializeField] private NetworkVariable<float> movementSpeed = new NetworkVariable<float>(1.0f); // How fast the unit moves from tile to tile in real time.
    [HideInInspector] private NetworkVariable<int> remainingSpeed = new NetworkVariable<int>(0);

    [HideInInspector] private NetworkVariable<int> poisonedTurns = new NetworkVariable<int>(0);
    [HideInInspector] private NetworkVariable<int> stunnedTurns = new NetworkVariable<int>(0);

    public bool flying;

    protected List<Ability> abilities = new List<Ability>(); 
    public Ability selectedAbility = null;
    public int abilityID = -1;
    [HideInInspector] private NetworkVariable<bool> usedAbility = new NetworkVariable<bool>(false);

    GameManager gameManager;
    protected Animator animator;
    [SerializeField] private NetworkVariable<int> animationState = new NetworkVariable<int>(
    0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private AnimState lastAppliedAnimState = AnimState.Idle;

    private NetworkVariable<FixedString32Bytes> syncedTag = new NetworkVariable<FixedString32Bytes>(
        "", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    protected int id;
    int ID {  get { return id; } set { id = value; } }

    public enum AnimState
    {
        Idle = 0,
        Walking = 1,
        Ability1 = 2,
        Ability2 = 3,
        Dead = 4
    }

    private void Update()
    {
        UpdateAnimationFromNetwork();
    }

    private void UpdateAnimationFromNetwork()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null) return;
        }

        AnimState current = (AnimState)animationState.Value;

        // Only update if the animation state changed
        if (current == lastAppliedAnimState) return;

        lastAppliedAnimState = current; // Track this state

        switch (current)
        {
            case AnimState.Idle:
                animator.SetBool("walking", false);
                break;
            case AnimState.Walking:
                animator.SetBool("walking", true);
                break;
            case AnimState.Ability1:
                animator.SetTrigger("Ability1");
                break;
            case AnimState.Ability2:
                animator.SetTrigger("Ability2");
                break;
            case AnimState.Dead:
                animator.SetBool("dead", true);
                break;
        }
    }

    [Rpc(SendTo.Server)]
    public void SetAnimStateRpc(int newState)
    {
        animationState.Value = newState;
    }

    public void SetAnimState(AnimState state)
    {
        if (IsServer)
            animationState.Value = (int)state;
        else
            SetAnimStateRpc((int)state);
    }

    public Ability GetAbilityFromID(int id)
    {
        abilityID = id;
        return abilities[id];
    }

    [Rpc(SendTo.Server)]
    public void CheckDeathRpc()
    {
        if(health.Value <= 0)
        {
            SetAnimState(AnimState.Dead);
        }
    }

    [Rpc(SendTo.Server)]
    public void SetHealthRpc(float newHealth)
    {
        health.Value = newHealth;
    }
    public float GetHealth() { return health.Value; }

    [Rpc(SendTo.Server)]
    public void SetGridSpeedRpc(int newSpeed)
    {
        gridSpeed.Value = newSpeed;
    }
    public int GetGridSpeed() { return gridSpeed.Value; }

    [Rpc(SendTo.Server)]
    public void SetMoveSpeedRpc(float newSpeed)
    {
        movementSpeed.Value = newSpeed;
    }
    public float GetMoveSpeed() { return movementSpeed.Value; }

    [Rpc(SendTo.Server)]
    public void SetRemainingSpeedRpc(int newSpeed)
    {
        remainingSpeed.Value = newSpeed;
    }
    public int GetRemainingSpeed() { return remainingSpeed.Value; }

    [Rpc(SendTo.Server)]
    public void SetPoisonedTurnsRpc(int newTurns)
    {
        poisonedTurns.Value = newTurns;
    }
    public int GetPoisonedTurns() { return poisonedTurns.Value; }

    [Rpc(SendTo.Server)]
    public void SetStunnedTurnsRpc(int newTurns)
    {
        stunnedTurns.Value = newTurns;
    }
    public int GetStunnedTurns() { return stunnedTurns.Value; }

    [Rpc(SendTo.Server)]
    public void SetUsedAbilityRpc(bool newUsed)
    {
        usedAbility.Value = newUsed;
    }
    public bool GetUsedAbility() {  return usedAbility.Value; }

    [Rpc(SendTo.Server)]
    public void OnDeathAnimationCompleteRpc()
    {
        if (CompareTag("Player1"))
        {
            gameManager.playerOneArmy.RemoveAt(armyId);
        }
        else
        {
            gameManager.playerTwoArmy.RemoveAt(armyId);
        }

        if (IsServer)
        {
            var netObj = GetComponent<NetworkObject>();
            netObj.Despawn(true);
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn(); // Good practice

        SetUsedAbilityRpc(false);

        gameManager = FindFirstObjectByType<GameManager>();

        if (!string.IsNullOrEmpty(syncedTag.Value.ToString()))
        {
            gameObject.tag = syncedTag.Value.ToString();
        }
    }

    public void SetTagOnServer(string newTag)
    {
        if (IsServer)
        {
            syncedTag.Value = newTag;
            gameObject.tag = newTag;
        }
    }

    public void PlayAbilityAnim(int abilityId)
    {
        switch (abilityId)
        {
            case 0:
                SetAnimState(AnimState.Ability1);
                break;
            case 1:
                SetAnimState(AnimState.Ability2);
                break;
            default:
                Debug.Log("Unknown ability animation triggered.");
                break;
        }
    }
}