using UnityEngine;
using System.Collections;


public class enemy_eagle : Enemy
{

    public Transform flyTop, flyBottom;
    public float speed;


    private Rigidbody2D rb;
    private float limitUp, limitDown;
    private int direction = 1;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        //coll = GetComponent<Collider2D>();
        limitUp = flyTop.position.y;
        limitDown = flyBottom.position.y;
        //Destroy(top.gameObject);
        //Destroy(bottom.gameObject);
    }


    void Update()
    {
        Movement();
    }

    void Movement()
    {
        float currentY = transform.position.y;
        if (direction == 1 && currentY > limitUp)
        {
            direction = -1;
        }
        if (direction == -1 && currentY < limitDown)
        {
            direction = 1;
        }
        //Debug.LogFormat("speed  {0}", speed);
        //Debug.LogFormat("eagle pos {0}  {1}", transform.position.y, speed * direction);
        rb.velocity = new Vector2(rb.velocity.x, speed * direction  );
    }
}
