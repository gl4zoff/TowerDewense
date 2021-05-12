using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float AttackRadius;
    [SerializeField] private float TimeBetweenAttack;
    [SerializeField] private int AttackDamage;
    [SerializeField] private int Health;
    [SerializeField] private GameObject Arrow;
    [SerializeField] private AudioSource sound;

    public SpriteRenderer sr;
    private Animator animator;
    public bool deth = false;
    public int price;
    private bool canAttack;
    private float attackCounter;
    private Enemy targetEnemy = null;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        attackCounter = TimeBetweenAttack;
        Manager.RegisterHero(this);
        animator = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();
    }
    void Update()
    {
        attackCounter -= Time.deltaTime;

        if (targetEnemy == null)
        {
            Enemy nearestEnemy = GetNearestEnamy();
            if (nearestEnemy != null && Vector2.Distance(transform.position, nearestEnemy.transform.position) < AttackRadius)
                targetEnemy = nearestEnemy;
        }
        else
        {
            if (attackCounter <= 0)
            {
                canAttack = true;
                attackCounter = TimeBetweenAttack;
            }

            if (Vector2.Distance(transform.position, targetEnemy.transform.position) > AttackRadius)
                targetEnemy = null;
        }
        if (transform.position.x > 50)
        {
            Manager.win = true;
        }
    }
    void FixedUpdate()
    {
        if (canAttack)
            Attack();
        if (!deth && targetEnemy == null)
            transform.Translate(transform.right * speed * Time.fixedDeltaTime);
    }
    void Attack()
    {
        if (!deth)
        {
            sound.Play();
            GameObject arrow = Instantiate(Arrow) as GameObject;
            arrow.transform.position = transform.position;

            if (targetEnemy == null)
                Destroy(arrow);
            else
            {
                animator.SetTrigger("Attack");
                StartCoroutine(MoveArrow(arrow));
                targetEnemy.EnemyHit(AttackDamage);
                targetEnemy = null;
                canAttack = false;
            }
        }
    }

    private IEnumerator MoveArrow(GameObject arrow)
    {
        while(GetTargetDistance(targetEnemy)>0.20f && arrow != null && targetEnemy != null)
        {
            var direction = targetEnemy.transform.position - transform.position;
            var angleDiretion = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            arrow.transform.rotation = Quaternion.AngleAxis(angleDiretion, Vector3.forward);
            arrow.transform.position = Vector2.MoveTowards(arrow.transform.position, targetEnemy.transform.position, 5f * Time.deltaTime);
            yield return null;
        }
        if (arrow != null || targetEnemy == null)
            Destroy(arrow);
    }
    // Дистанция до цели
    private float GetTargetDistance(Enemy e)
    {
        if (e == null)
        {
            e = GetNearestEnamy();
            if (e == null)
                return 0f;
        }
        return Mathf.Abs(Vector2.Distance(transform.position, e.transform.position));
    }

    //Противники в зоне поражения
    private List<Enemy> GetEnemiesInRange()
    {
        List<Enemy> enemiesInRange = new List<Enemy>();

        foreach (Enemy enemy in Manager.enemies)
        {
            if (Vector2.Distance(transform.position, enemy.gameObject.transform.position) < AttackRadius)
            {
                enemiesInRange.Add(enemy);
            }
        }
        return enemiesInRange;
    }
    //Ближайший противник
    private Enemy GetNearestEnamy()
    {
        Enemy nearestEnemy = null;
        float smallestDistance = float.PositiveInfinity;
        foreach (Enemy enemy in GetEnemiesInRange())
        {
            if (Vector3.Distance(transform.position, enemy.gameObject.transform.position) < smallestDistance)
            {
                smallestDistance = Vector2.Distance(transform.position, enemy.transform.position);
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }
    //Получение урона
    public void HeroHit(int attackDamage)
    {
        if (attackDamage < Health)
        {
            animator.SetTrigger("Hit");
            Health -= attackDamage;
        }
        else
        {
            Manager.UnregisterHero(this);
            animator.SetBool("Die", true);
            deth = true;
            sr.sortingOrder = 1;
            StartCoroutine(Destroy());
        }
    }
    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(10);
        Destroy(this.gameObject);
    }
}
