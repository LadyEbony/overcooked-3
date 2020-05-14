using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    public GameObject[] wayPoints;
    int curPosition;
    public float speed;
    float WPradius = 1;

    //public GameObject hoot;
    public AudioSource source;
    public AudioClip horn;
    public float waitTime;


    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(-90, 0, -126.3f);
        waitTime = 15.0f;
    }

    // Update is called once per frame
    void Update()
    {
        waitTime -= Time.deltaTime;

        if (Vector3.Distance(wayPoints[curPosition].transform.position, transform.position) < WPradius)
        {
            curPosition++;
            if (curPosition >= wayPoints.Length)
            {
                curPosition = 0;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, wayPoints[curPosition].transform.position, Time.deltaTime * speed);
        //transform.rotation = transform.rotation * Quaternion.Euler(0,0,-0.12f);
        transform.RotateAround(transform.position, new Vector3(0,1,0), -8.05f * Time.deltaTime);

        if(waitTime < 0.0f)
        {
            source.PlayOneShot(horn, 2.0f);
            waitTime = 60.0f;
        }
        


    }
    
}
