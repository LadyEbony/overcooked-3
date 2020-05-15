using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class whiteRabbit : MonoBehaviour
{
    
    private string[] m_buttonNames = new string[] { "Idle", "Run", "Run Turn Left"};
    private Animator m_animator;

    public GameObject[] wayPoints;
    int tarPosition;
    public float speed;
    float WPradius = 0.001f;

    public float actionTime;
    bool nextPosition;

    // Start is called before the first frame update
    // Use this for initialization
    void Start()
    {
        m_animator = GetComponent<Animator>();
        tarPosition = 1;
        //nextPosition = true;
        //actionTime = 30.0f;

    }

    private void OnCollisionEnter(Collision collision)
    {
        print("Line 31: helloe");
    }
    private void OnTriggerEnter(Collider other)
    {
        print("Line 31: helloe");
        nextPosition = false;
        actionTime = 60.0f;

        while (actionTime > 0)
        {
            //print("line 47: helloe");
            m_animator.SetInteger("AnimIndex", 0);
            m_animator.SetTrigger("Next");
            actionTime -= Time.deltaTime;
        }
        nextPosition = true;
    }

    // Update is called once per frame
    void Update()
    {
        //print("ActionTime: " + actionTime);
        //if (nextPosition == true)
        //{
        /*
            if (Vector3.Distance(wayPoints[tarPosition].transform.position, transform.position) < WPradius)
            {
                tarPosition++;
                if (tarPosition >= wayPoints.Length)
                {
                    tarPosition = 0;
                }
            }*/
        //}
        if(tarPosition == 1)
        {
            actionTime = 60.0f;

            while (actionTime > 0)
            {
                print("time: " + actionTime);
                //print("line 47: helloe");
                m_animator.SetInteger("AnimIndex", 0);
                m_animator.SetTrigger("Next");
                actionTime -= Time.deltaTime;
            }
                m_animator.SetInteger("AnimIndex", 1);
                m_animator.SetTrigger("Next");
                transform.position = Vector3.MoveTowards(transform.position, wayPoints[tarPosition].transform.position, Time.deltaTime * speed);
            
        }
        
    }
}
