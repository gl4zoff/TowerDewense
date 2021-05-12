using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    [SerializeField] private int AllEnemiesOnLevel; // Количество противников на уровне(сколько всего будет противников)
    [SerializeField] private int EnemiesOnLevel; // Максимальное количество противников, находящихся на уровне одновременно
    [SerializeField] private Enemy enemy;
    [SerializeField] private Hero[] hero;
    [SerializeField] private Text moneyText;
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private GameObject loseCanvas;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private int startMoney;

    private static int money;
    
    public static bool win = false;
    public static bool lose = false;

    private static float[] yEnemy = new float[1];
    private static float[] yHero = new float[1];
    public static List<Enemy> enemies = new List<Enemy>();
    public static List<Hero> heroes = new List<Hero>();
    void Awake()
    {
        Time.timeScale = 1;
        pauseCanvas.SetActive(false);
        winCanvas.SetActive(false);
        loseCanvas.SetActive(false);
        mainCanvas.SetActive(true);
        enemies = new List<Enemy>();
        heroes = new List<Hero>();
        lose = false;
        win = false;
    }
    void Start()
    {
        money = startMoney;
        for (int i = 0; i < EnemiesOnLevel; i++) // Спавн противников с начала игры
        {
            SpawnEnemy();
        }
        moneyText.text = money.ToString();
    }
    void Update()
    {
        if (enemies.Count < EnemiesOnLevel && AllEnemiesOnLevel > 0) // Спавн противником по ходу игры
        {
            SpawnEnemy();
        } 
        if (int.Parse(moneyText.text) != money)
        {
            moneyText.text = money.ToString();
        }
        if (win)
        {
            Time.timeScale = 0;
            winCanvas.SetActive(true);
            mainCanvas.SetActive(false);
        }
        if (lose)
        {
            Time.timeScale = 0;
            loseCanvas.SetActive(true);
            mainCanvas.SetActive(false);
        }
    }
    private void SpawnEnemy()
    {
        float y = Random.Range(-3, 3);
        Instantiate(enemy);
        enemy.gameObject.transform.position = new Vector3(50f, y, enemy.gameObject.transform.position.z);
        AllEnemiesOnLevel -= 1;
        AddValueInFloat(ref yEnemy, y);
        SortingArray(ref yEnemy);
        foreach (Enemy enemy in enemies)
        {
            for (int i = 0; i < yEnemy.Length; i++)
            {
                if (yEnemy[i] == enemy.gameObject.transform.position.y)
                    enemy.sr.sortingOrder = i;
            }
        }
    }
    public void SpawnHero(int h)
    {
        if (money >= hero[h].price)
        {
            float y = Random.Range(-3, 3);
            money -= hero[h].price;
            Instantiate(hero[h]);
            hero[h].gameObject.transform.position = new Vector3(0f, y, 0);
            moneyText.text = money.ToString();
            AddValueInFloat(ref yHero, y);
            SortingArray(ref yHero);
            foreach (Hero hero in heroes)
            {
                for(int i = 0; i < yHero.Length; i++)
                {
                    if (yHero[i] == hero.gameObject.transform.position.y)
                        hero.sr.sortingOrder = i;
                }     
            }
        }
    }
    public static void RegisterEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }
    public static void UnregisterEnemy(Enemy enemy)
    {
        money += enemy.Cost;
        enemies.Remove(enemy);
        for (int i = 0; i < yEnemy.Length; i++)
        {
            if (yEnemy[i] == enemy.gameObject.transform.position.y)
                DeleteFromArray(ref yEnemy, i);
        }
    }
    public static void RegisterHero(Hero hero)
    {
        heroes.Add(hero);
    }
    public static void UnregisterHero(Hero hero)
    {
        heroes.Remove(hero);
        for (int i = 0; i < yHero.Length; i++)
        {
            if (yHero[i] == hero.gameObject.transform.position.y)
                DeleteFromArray(ref yHero, i);
        }     
    }
    public void Pause()
    {
        Time.timeScale = 0;
        pauseCanvas.SetActive(true);
        mainCanvas.SetActive(false);
    }
    public void Resume()
    {
        Time.timeScale = 1;
        pauseCanvas.SetActive(false);
        mainCanvas.SetActive(true);
    }
    private void AddValueInFloat(ref float[] arr, float value)
    {
        float[] newArr = new float[arr.Length + 1];

        for (int i = 0; i < arr.Length; i++)
        {
            newArr[i] = arr[i];
        }

        newArr[newArr.Length - 1] = value;
        arr = newArr;
    }
    private static void SortingArray(ref float[] arr)
    {
        float[] newArr = arr;
        float n;
        for (int i = 0; i < arr.Length; i++)
        {
            for (int j = i + 1; j < arr.Length; j++)
            {
                if (newArr[i] < newArr[j])
                {
                    n = newArr[i];
                    newArr[i] = newArr[j];
                    newArr[j] = n;
                }
            }
        }
        arr = newArr;
    }
    private static void DeleteFromArray(ref float[] arr, int index)
    {
        float[] newArr = new float[arr.Length - 1];
        if (index != 0)
        {
            for (int i = 0; i < index; i++)
                newArr[i] = arr[i];

            for (int i = index + 1; i < arr.Length; i++)
            {
                int j = i - 1;
                newArr[j] = arr[i];
            }
        }
        else
        {
            for (int i = 0; i < newArr.Length; i++)
            {
                int j = i + 1;
                newArr[i] = arr[j];
            }
        }
        arr = newArr;
    }
}