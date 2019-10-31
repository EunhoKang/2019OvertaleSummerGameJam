using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPlatform : MonoBehaviour {

    private PlatformEffector2D effector;
    private bool isinput=false;

	// Use this for initialization
	void Start () {
        effector = GetComponent<PlatformEffector2D>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButton("Crouch") && !isinput)
        {
            if (Input.GetButtonDown("Jump"))
            {
                isinput = true;
                StartCoroutine(VerticalJump());
            }
        }
	}

    IEnumerator VerticalJump()
    {
        effector.rotationalOffset = 180f;
        for (int i=0; i<10; i++)
        {  
            yield return new WaitForSeconds(0.03f);
        }
        isinput = false;
        effector.rotationalOffset = 0f;
    }
}
