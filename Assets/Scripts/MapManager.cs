using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public static MapManager mapmanager;
    private void Awake()
    {
        if (mapmanager == null)
        {
            mapmanager=this;
        }
        else if (mapmanager != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this);
        fxtime = new WaitForSeconds(healthDecreaseCycle);
        Init();
    }

    public List<MapInfo> mapList;
    public int clearItemCount;
    public float initialHealth;
    public float healthDecreaseRate;
    public float healthDecreaseCycle;
    public int initialItemCount;
    public int initialEnemyCount;

    [HideInInspector]public int mapcount=0;
    private WaitForSeconds fxtime;

    [HideInInspector] public float playerHealth;
    [HideInInspector] public int clearCount = 0;
    [HideInInspector] public int totalscore=0;
    [HideInInspector] public MapInfo current;
    [HideInInspector] public bool isGameOver = false;

    void Init()
    {
        playerHealth = initialHealth;
        StartCoroutine(HealthReduce());
        GameObject cur=Instantiate(mapList[mapcount].gameObject, new Vector3(0, 0, 0), Quaternion.identity);
        current = cur.GetComponent<MapInfo>();
        for(int i = 0; i < initialItemCount; i++)
        {
            current.itemSpawn();
        }
        for (int i = 0; i < initialEnemyCount; i++)
        {
            current.enemySpawn();
        }
    }

    void OnLevelWasLoaded(int index)
    {
        mapcount++;
        clearItemCount += 4;
        if (mapcount >= mapList.Count)
        {
            mapcount = 1;
            healthDecreaseCycle++;
        }
        clearCount = 0;
        Init();
    }

    IEnumerator HealthReduce()
    {
        while (clearCount < clearItemCount)
        {
            playerHealth -= healthDecreaseRate;
            if (playerHealth <= 0)
            {
                GameOver();
            }

            yield return fxtime;
        }
        Invoke("Restart", 1.6f);
    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
    public void GameOver()
    {
        isGameOver = true;
        StopAllCoroutines();

    }

    public void itemSpawn()
    {
        current.itemSpawn();
    }
    public void enemySpawn()
    {
        current.enemySpawn();
    }
}
