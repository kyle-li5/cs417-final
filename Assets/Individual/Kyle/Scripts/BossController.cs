using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BossController : MonoBehaviour, ISpellTarget
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public NavMeshAgent agent;
    public Transform castPoint;

    [Header("Stats")]
    public float maxHealth = 1000f;
    public float currentHealth;
    public float optimalRange = 8f;
    public bool isPhaseTwo { get; private set; }

    // events to help interaction with boss from other scripts easier
    public event Action<float, float> OnHealthChanged; // (currentHealth, maxHealth)
    public event Action<ElementType, Vector3> OnElementHit;
    public event Action OnPhaseTwoStarted;
    public event Action OnBossDefeated;

    public IBossState CurrentState { get; private set; }

    // specific scripts corresponds to specific states. added temp states for now but commented them out to avoid errors
    //public CautiousState CautiousState { get; private set; }
    //public RangedAttackState RangedAttackState { get; private set; }

    // will add other states

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;

        //CautiousState = new CautiousState(this);
        //RangedAttackState = new RangedAttackState(this);
    }

    private void Start()
    {
        //ChangeState(CautiousState);
    }

    private void Update()
    {
        CurrentState?.Tick(Time.deltaTime);
    }

    public void ChangeState(IBossState newState)
    {
        if (CurrentState == newState) return;

        if (CurrentState != null)
        {
            CurrentState.Exit();
            if (!string.IsNullOrEmpty(CurrentState.StateFlagName))
                animator.SetBool(CurrentState.StateFlagName, false);
        }

        CurrentState = newState;

        if (CurrentState != null)
        {
            if (!string.IsNullOrEmpty(CurrentState.StateFlagName))
                animator.SetBool(CurrentState.StateFlagName, true);
            CurrentState.Enter();
        }
    }

    public void TakeSpellDamage(float amount, ElementType element, Vector3 hitPoint)
    {
        if (currentHealth <= 0f) return;

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnElementHit?.Invoke(element, hitPoint);

        HandleElementalReaction(element);

        if (currentHealth <= 0f)
        {
            OnBossDefeated?.Invoke();
            Destroy(gameObject, 3f);
            return;
        }

        if (!isPhaseTwo && currentHealth <= maxHealth * 0.5f)
        {
            isPhaseTwo = true;
            animator.SetBool("IsPhaseTwo", true);
            OnPhaseTwoStarted?.Invoke();
            // ChangeState(PhaseTwoTransitionState);
        }
    }

    private void HandleElementalReaction(ElementType element)
    {
        switch (element)
        {
            case ElementType.Fire:
                animator.SetTrigger("HitFire");
                break;
            case ElementType.Ice:
                animator.SetTrigger("HitIce");
                agent.speed = 2.0f;
                break;
            case ElementType.Lightning:
                animator.SetTrigger("Stagger");
                break;
            case ElementType.Earth:
                animator.SetTrigger("HitHeavy");
                break;
        }
    }

    // may replace with specific turning animations later
    public void FacePlayer(float turnSpeed = 5f)
    {
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0f;
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
        }
    }
}