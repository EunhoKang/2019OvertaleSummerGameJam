using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfo : MonoBehaviour
{
    public List<Transform> itemPositionList;
    public List<Transform> enemyPositionList;
    public GameObject item;
    private GameObject nowitem;
    public GameObject octoenemy;

    private List<Transform> currentitem;
    private List<Transform> currentenemy;

    public void Awake()
    {
        currentitem = new List<Transform>(itemPositionList);
        currentenemy = new List<Transform>(enemyPositionList);
        int rand = Random.Range(0, currentitem.Count);
        nowitem = Instantiate(item, currentitem[rand].position, Quaternion.identity);
    }

    public void itemSpawn()
    {
        if (currentitem.Count <= 0)
        {
            currentitem = new List<Transform>(itemPositionList);
        }

        int rand = Random.Range(0, currentitem.Count);
        nowitem.transform.position=currentitem[rand].position;
        currentitem.RemoveAt(rand);
    }
    public void enemySpawn()
    {
        if (currentenemy.Count <= 0)
        {
            currentenemy = new List<Transform>(enemyPositionList);
        }
        int rand = Random.Range(0, currentenemy.Count);
        Instantiate(octoenemy, currentenemy[rand].position,Quaternion.identity);
        currentenemy.RemoveAt(rand);
    }

}
