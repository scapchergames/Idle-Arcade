using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AnimationMotor))]
public class PlayerAttack : MonoBehaviour
{
    [Header("Detection")]
    public FloatVariable _attackRange;

    public LayerMask _attackMask;

    [Header("Attack")]
    public FloatVariable _attackDelay;
    public FloatVariable _damage;

    private WaitForSeconds _attackDelayWait;
    private AnimationMotor _animation;

    private void Start()
    {
        _animation = GetComponent<AnimationMotor>();
        _attackDelayWait = new WaitForSeconds(_attackDelay.Value);
        StartCoroutine(CheckTarget());
    }

    /// Check Target after given interval
    /// Optimized than Update or Fixed Update
    IEnumerator CheckTarget()
    {
        while (true)
        {
            FindTargets();
            yield return _attackDelayWait;
        }
    }

    /// Find Target
    void FindTargets()
    {
        Collider[] _colliders = new Collider[10];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, _attackRange.Value, _colliders, _attackMask);

        for (int i = 0; i < numColliders; i++)
        {
            if (Vector3.Dot(transform.position, _colliders[i].transform.position) > 0.5f)
            {
                _animation.Attack();

                if (_colliders[i].GetComponentInParent<TreeHealth>() != null)
                {
                    TreeHealth _health = _colliders[i].GetComponentInParent<TreeHealth>();
                    _health.GetDamage(_damage.Value);
                }
            }
        }
    }

    /// Draw Gizmos on the item
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange.Value);
    }
}