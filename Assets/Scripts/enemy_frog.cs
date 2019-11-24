using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_frog : Enemy
{

    private Rigidbody2D rb;
    //private Animator animator;// 继承父类
    private Collider2D coll;

    public LayerMask ground;


    public Transform leftPoint;
    public Transform rightPoint;
    private float leftPointValue;
    private float rightPointValue;


    public float speedx; // default 5
    public float speedy; // default 0

    public float moveDirection; // default 1 默认向右边移动  




    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        leftPointValue = leftPoint.gameObject.transform.position.x;
        rightPointValue = rightPoint.gameObject.transform.position.x;
        //Debug.LogFormat("left {0}  right {1}", leftPointValue, rightPointValue);

        //animator = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        //ground = GetComponent<LayerMask>(); //必须继承自MonoBehaviour 才可以这么使用

        //animator.SetBool("idle", true);
    }

    // Update is called once per frame
    void Update()
    {
        AnimationCheck();


    }

    void AnimationCheck()
    {

        if (coll.IsTouchingLayers(ground))
        {
            if (animator.GetBool("falling"))
            {
                ChangeAnimation("idle");
            }

        }
        else
        {
            if (animator.GetBool("jumping") && rb.velocity.y < Mathf.Epsilon)
            {
                ChangeAnimation("falling");
            }

        }




        //调整青蛙的脸的方向
        transform.localScale = new Vector2(-1 * moveDirection, 1);
        if (transform.position.x < leftPointValue)
        {
            moveDirection = 1;
        }
        if (transform.position.x > rightPointValue) 
        {
            moveDirection = -1;
        }
    }


    // 利用 animation event 将在 idle 之后执行 jump 函数 
    void frogJump()
    {

        if (coll.IsTouchingLayers(ground))
        {
            //rb.AddForce(new Vector2(0, jumpForce));
            //Debug.LogFormat("frog pos {0}", transform.position.x);
            rb.velocity = new Vector2(moveDirection * speedx, speedy );
            ChangeAnimation("jumping");
        }
    }


    void ChangeAnimation(string toState)
    {
        switch(toState)
        {
            case "jumping":
                animator.SetBool("jumping", true);
                animator.SetBool("falling", false);
                animator.SetBool("idle", false);
                break;
            case "idle":
                animator.SetBool("idle", true);
                animator.SetBool("jumping", false);
                animator.SetBool("falling", false);
                break;
            case "falling":
                animator.SetBool("falling", true);
                animator.SetBool("jumping", false);
                animator.SetBool("idle", false);
                break;
        }
            
    }


}

