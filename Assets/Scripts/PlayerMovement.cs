using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public CharacterController2D controller;
	public Animator animator;
    public PrefabWeapon weapon;
    public GameObject canvas;

	public float runSpeed = 40f;
    public AudioClip shotSound;
    public AudioClip jumpSound;
    public AudioClip collectSound;

    private Canvas canvasScript;

	float horizontalMove = 0f;
	bool jump = false;
	bool crouch = false;

    List<string> keyList = new List<string> {"Q","W","E","R","T","Y","A","S","D","F","G","H","Z","X","C","V","B","N","LC","RC"};

    [HideInInspector] public string jumpbutton;
    [HideInInspector] public string leftbutton;
    [HideInInspector] public string rightbutton;
    [HideInInspector] public string crouchbutton;
    [HideInInspector] public string shootbutton;

    private void Start()
    {
        jumpbutton = "W";
        leftbutton = "A";
        rightbutton = "D";
        crouchbutton = "S";
        shootbutton = "LC";
        keyList.Remove("W");
        keyList.Remove("A");
        keyList.Remove("S");
        keyList.Remove("D");
        keyList.Remove("LC");
        canvasScript = canvas.GetComponent<Canvas>();
    }

    void Update () {

        //horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetButton(leftbutton))
        {
            if (Input.GetButton(rightbutton))
            {
                horizontalMove = 0;
            }
            else if (!Input.GetButton(rightbutton))
            {
                horizontalMove = -1;
            }
        }
        else
        {
            if (Input.GetButton(rightbutton))
            {
                horizontalMove = 1;
            }
            else if (!Input.GetButton(rightbutton))
            {
                horizontalMove = 0;
            }
        }
        horizontalMove *= runSpeed;

		animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

		if (Input.GetButtonDown(jumpbutton))
		{
            SoundManager.soundmanager.SFXPlay(jumpSound);
			jump = true;
			
		}
		if (Input.GetButton(crouchbutton))
		{
			crouch = true;
		} else if (Input.GetButtonUp(crouchbutton))
		{
			crouch = false;
		}
        if (Input.GetButtonDown(shootbutton))
        {
            SoundManager.soundmanager.SFXPlay(shotSound);
            weapon.Shoot();
        }
    }

	public void OnLanding ()
	{
		animator.SetBool("IsJumping", false);
	}
    public void OnJumping()
    {
        animator.SetBool("IsJumping", true);
    }

    public void OnCrouchingTrue()
    {
        animator.SetBool("IsCrouching", true);
    }
    public void OnCrouchingFalse()
    {
        animator.SetBool("IsCrouching", false);
    }
    void FixedUpdate ()
	{
		// Move our character
		controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
		jump = false;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer==15)
        {
            MapManager.mapmanager.clearCount++;
            MapManager.mapmanager.totalscore++;
            MapManager.mapmanager.playerHealth += (50-MapManager.mapmanager.mapcount);
            SoundManager.soundmanager.SFXPlay(collectSound);
            if (MapManager.mapmanager.playerHealth > 100)
            {
                MapManager.mapmanager.playerHealth = 100;
            }
            keySwap();
            MapManager.mapmanager.itemSpawn();
        }
    }

    public void keySwap()
    {
        int rand = Random.Range(0,5);
        int randkey = Random.Range(0, keyList.Count);
        if (rand == 0)
        {
            keyList.Add(leftbutton);
            leftbutton = keyList[randkey];
            keyList.RemoveAt(randkey);
        }else if (rand == 1)
        {
            keyList.Add(rightbutton);
            rightbutton = keyList[randkey];
            keyList.RemoveAt(randkey);
        }
        else if (rand == 2)
        {
            keyList.Add(jumpbutton);
            jumpbutton = keyList[randkey];
            keyList.RemoveAt(randkey);
        }
        else if (rand == 3)
        {
            keyList.Add(crouchbutton);
            crouchbutton = keyList[randkey];
            keyList.RemoveAt(randkey);
        }
        else if (rand == 4)
        {
            keyList.Add(shootbutton);
            shootbutton = keyList[randkey];
            keyList.RemoveAt(randkey);
        }
        canvasScript.keyChange();
    }
}
