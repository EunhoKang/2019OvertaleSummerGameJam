using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Canvas : MonoBehaviour
{
    public Text uptext;
    public Text downtext;
    public Text lefttext;
    public Text righttext;
    public Text mousetext;
    public PlayerMovement player;
    public Text current;
    public Text max;
    public Text health;
    public Text score;
    public Text scoreleft;
    public Image gameover;

    public void keyChange()
    {
        uptext.text = player.jumpbutton;
        downtext.text = player.crouchbutton;
        lefttext.text = player.leftbutton;
        righttext.text = player.rightbutton;
        mousetext.text = player.shootbutton;
    }

    private void Update()
    {
        current.text = MapManager.mapmanager.clearCount.ToString();
        max.text = MapManager.mapmanager.clearItemCount.ToString();
        health.text = MapManager.mapmanager.playerHealth.ToString();
        score.text = MapManager.mapmanager.totalscore.ToString();
        if (MapManager.mapmanager.isGameOver)
        {
            gameover.gameObject.SetActive(true);
            score.color = new Color(1, 1, 1, 1);
            scoreleft.color = new Color(1, 1, 1, 1);
        }
    }
}
