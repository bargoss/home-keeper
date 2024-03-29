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

    public float CycleDuration = 1;
    private float m_LastShoot = 0;
    private float m_LookX = 0;
    private float m_LookY = 0;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space)&& Time.time - m_LastShoot > CycleDuration)
        {
            GetComponent<ShooterGOView>().ShootAnimation(CycleDuration);
            m_LastShoot = Time.time;
        }

        m_LookX += Input.GetAxis("Horizontal");
        m_LookY += Input.GetAxis("Vertical");

        var look = Vector3.forward;
        look = Quaternion.Euler(Vector3.right * m_LookY) * look;
        look = Quaternion.Euler(Vector3.up * m_LookX) * look;
        
        GetComponent<ShooterGOView>().UpdateLookDirection(look);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Time.timeScale -= 0.2f;
            if (Time.timeScale <= 0)
            {
                Time.timeScale = 0.1f;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Time.timeScale += 0.2f;
            if (Time.timeScale >= 1)
            {
                Time.timeScale = 1;
            }
        }
    }
}
