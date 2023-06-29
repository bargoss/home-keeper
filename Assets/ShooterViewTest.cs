using System.Collections;
using System.Collections.Generic;
using BulletCircle.GoViews;
using UnityEngine;

public class ShooterViewTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<ShooterView>().ShootAnimation(1);
        }
    }
}
