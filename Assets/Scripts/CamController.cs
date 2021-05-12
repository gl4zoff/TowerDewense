using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamController : MonoBehaviour
{
    private float maxDistance = 0;
    private Hero targetHero = null;

    void FixedUpdate()
    {
        if (Manager.heroes.Count > 0)
        {
            if (targetHero == null)
            {
                maxDistance = 0;
                foreach (Hero hero in Manager.heroes) // Вычисление дальнего героя
                {
                    if (hero.gameObject.transform.position.x > maxDistance)
                    {
                        maxDistance = hero.gameObject.transform.position.x;
                        targetHero = hero;
                    }
                }
            }
            if (targetHero.deth)
            {
                targetHero = null;
            }

            if (targetHero != null)
            {
                Vector3 targetHeroPosition = new Vector3(targetHero.gameObject.transform.position.x + 2f, 0f, -10f);
                transform.position = Vector3.Lerp(transform.position, targetHeroPosition, 0.02f);
            }
        }
    }
}
