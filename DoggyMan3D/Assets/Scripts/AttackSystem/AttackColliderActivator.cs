using UnityEngine;

[RequireComponent(typeof(GameEntityObject))]
public class AttackColliderActivator : MonoBehaviour
{

    public GameObject attackCollider1;
    public GameObject attackCollider2;

    private GameEntityObject _entity;
    private float startTime1, startTime2;
    private float endTime1, endTime2;

    public void Start()
    {
        _entity = GetComponent<GameEntityObject>();
        if (attackCollider1 != null)
        {
            attackCollider1.SetActive(false);
        }
        if (attackCollider2 != null)
        {
            attackCollider2.SetActive(false);
        }
    }

    public void ActivateAttack1(float time)
    {
        attackCollider1.SetActive(true);
        Debug.Log("Attack collider: " + _entity.GetActiveAttackDamage() + " / ID: " + _entity.GetAttackID());
        attackCollider1.GetComponent<AttackCollider>().Damage = _entity.GetActiveAttackDamage();
        startTime1 = 0;
        endTime1 = time;
    }

    public void ActivateAttack2(float time)
    {
        attackCollider2.SetActive(true);
        attackCollider2.GetComponent<AttackCollider>().Damage = _entity.GetActiveAttackDamage();
        startTime2 = 0;
        endTime2 = time;
    }

    private void Update()
    {
        if (attackCollider1 != null)
        {
            if (attackCollider1.activeSelf)
            {
                startTime1 += Time.deltaTime;
                if (startTime1 > endTime1)
                {
                    attackCollider1.SetActive(false);
                }
            }
        }

        if (attackCollider2 != null)
        {
            if (attackCollider2.activeSelf)
            {
                startTime2 += Time.deltaTime;
                if (startTime2 > endTime2)
                {
                    attackCollider2.SetActive(false);
                }
            }
        }
    }

}