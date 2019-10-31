using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axis : MonoBehaviour {

    public CharacterController2D controller;
    public Transform axis;
    public Transform tanPoint;
    public Transform flip;
    public Transform[] Weapons;

    bool isright = true;
    bool ismouseright;
    Vector3 mouseposition;

    public static float GetAngle(Vector3 vStart, Vector3 vEnd)
    {
        Vector3 v = vEnd - vStart;
        return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
    }

    void Update()
    {
        mouseposition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mouseposition.x > flip.position.x)
        {
            ismouseright = true;
        }
        else
        {
            ismouseright = false;
        }
        if (ismouseright && !isright)
        {
            Flip();
            controller.Flip(true);
        }
        else if (!ismouseright && isright)
        {
            Flip();
            controller.Flip(false);
        }
        axis.rotation = Quaternion.Euler(0f, 0f, GetAngle(flip.position, mouseposition));
    }

    void Flip()
    {
        isright = !isright;
        for(int i=0; i < Weapons.Length; i++)
        {
            Weapons[i].Rotate(180f, 0f, 0f);
        }
        //transform.Rotate(180f, 0f, 0f);
    }
}
