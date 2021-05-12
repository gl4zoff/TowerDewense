using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Аналогичен скрипту "Hero"
    [SerializeField] float speed;
    [SerializeField] float AttackRadius;
    [SerializeField] float TimeBetweenAttack;
    [SerializeField] int AttackDamage;
    [SerializeField] int Health;
    [SerializeField] GameObject Arrow;
    [SerializeField] AudioSource sound;

    public int Cost;
    public SpriteRenderer sr;
    private Animator animator;
    private bool canAttack;
    private bool deth = false;
    private float attackCounter;
    private Hero targetHero = null;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        attackCounter = TimeBetweenAttack;
        Manager.RegisterEnemy(this);
        animator = GetComponent<Animator>();
        sound = GetComponent<AudioSource>();
    }

    void Update()
    {  
        attackCounter -= Time.deltaTime;

        if (targetHero == null)
        {
            Hero nearestHero = GetNearestHero();
            if (nearestHero != null && Vector2.Distance(transform.position, nearestHero.transform.position) < AttackRadius)
                targetHero = nearestHero;
        }
        else
        {
            if (attackCounter <= 0)
            {
                canAttack = true;
                attackCounter = TimeBetweenAttack;
            }

            if (Vector2.Distance(transform.position, targetHero.transform.position) > AttackRadius)
                targetHero = null;
        }
        if (transform.position.x < 0)
        {
            Manager.lose = true;
        }
    }
    void FixedUpdate()
    {
        if (canAttack)
            Attack();
        if(!deth && targetHero == null)
            transform.Translate(transform.right * -speed * Time.fixedDeltaTime);
    }
    void Attack()
    {
        if (!deth)
        {
            sound.Play();
            GameObject arrow = Instantiate(Arrow) as GameObject;
            arrow.transform.position = transform.position;

            if (targetHero == null)
                Destroy(arrow);
            else
            {
                animator.SetTrigger("Attack");
                StartCoroutine(MoveArrow(arrow));
                targetHero.HeroHit(AttackDamage);
                targetHero = null;
                canAttack = false;
            }
        }
    }

    private IEnumerator MoveArrow(GameObject arrow)
    {
        while (GetTargetDistance(targetHero) > 0.20f && arrow != null && targetHero != null)
        {
            var direction = targetHero.transform.position - transform.position;
            var angleDiretion = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            arrow.transform.rotation = Quaternion.AngleAxis(angleDiretion, Vector3.forward);
            arrow.transform.position = Vector2.MoveTowards(arrow.transform.position, targetHero.transform.position, 5f * Time.deltaTime);
            yield return null;
        }
        if (arrow != null || targetHero == null)
            Destroy(arrow);
    }

    private float GetTargetDistance(Hero hero)
    {
        if (hero == null)
        {
            hero = GetNearestHero();
            if (hero == null)
                return 0f;
        }
        return Mathf.Abs(Vector2.Distance(transform.position, hero.transform.position));
    }

    //Противники в зоне поражения
    private List<Hero> GetHeroesInRange()
    {
        List<Hero> heroesInRange = new List<Hero>();

        foreach (Hero hero in Manager.heroes)
        {
            if (Vector2.Distance(transform.position, hero.gameObject.transform.position) < AttackRadius)
            {
                heroesInRange.Add(hero);
            }
        }
        return heroesInRange;
    }
    //Ближайший противник
    private Hero GetNearestHero()
    {
        Hero nearestHero = null;
        float smallestDistance = float.PositiveInfinity;
        foreach (Hero hero in GetHeroesInRange())
        {
            if (Vector3.Distance(transform.position, hero.gameObject.transform.position) < smallestDistance)
            {
                smallestDistance = Vector2.Distance(transform.position, hero.transform.position);
                nearestHero = hero;
            }
        }
        return nearestHero;
    }
    public void EnemyHit(int attackDamage)
    {
        if (attackDamage < Health)
        {
            Health -= attackDamage;
            animator.SetTrigger("Hit");
        }
        else
        {
            Manager.UnregisterEnemy(this);
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
