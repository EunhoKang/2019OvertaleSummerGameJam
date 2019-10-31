using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {
    public CharacterController2D controller;
    public GameObject[] Weapons;


	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Weapon1"))
        {
            SwitchWeapon(0);

        }
        if (Input.GetButton("Weapon2"))
        {

            SwitchWeapon(1);

        }
        if (Input.GetButton("Weapon3"))
        {
            SwitchWeapon(2);

        }
        if (Input.GetButton("Weapon4"))
        {
            SwitchWeapon(3);
        }
        if (Input.GetButton("Weapon5"))
        {
            SwitchWeapon(4);
        }

    }

    void SwitchWeapon(int num)
    {
        if (Weapons.Length <= num)
        {
            return;
        }

        for (int i=0; i<Weapons.Length; i++)
        {
            Weapons[i].SetActive(false);
        }
        
        Weapons[num].SetActive(true);

    }
}
