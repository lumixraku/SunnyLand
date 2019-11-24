using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    public Collider2D coll;
    public LayerMask ground;  //指的layer  而不是 sort layer
    public Text textCherryCount;
    public AudioSource jumpAudio;
    //public ScenceManager

    public float speed;
    public float jumpForce;




    public int cherryCount;
    private bool justInjure;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    // 而Fixedupdate在每个渲染帧之间的时间间隔是相等的
    void FixedUpdate()
    {
        movement();
        SwitchAnimation();
    }

    void movement()
    {
        float horizonInputValue = Input.GetAxis("Horizontal");
        float verticalInputValue = Input.GetAxis("Vertical");
        float faceDirect = Input.GetAxisRaw("Horizontal"); // only -1 and 1 and 0

        if (justInjure)
        {
            return;
        }
        if (horizonInputValue != 0.0f)
        {
            rb.velocity = new Vector2(horizonInputValue * speed * Time.fixedDeltaTime, rb.velocity.y);
            animator.SetFloat("running", Mathf.Abs(faceDirect));
        }
        if (verticalInputValue != 0.0f)
        {

        }



        if (faceDirect != 0)
        {
            transform.localScale = new Vector3(faceDirect, 1, 1);
        }

        if (Input.GetButton("Jump") && coll.IsTouchingLayers(ground))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * Time.fixedDeltaTime);
            animator.SetBool("jumping", true);
            jumpAudio.Play();
        }
    }

    void SwitchAnimation()
    {
        if (rb.velocity.y < Mathf.Epsilon)
        {
            //print("falling" + animator.GetBool("falling") + " ::: " + rb.velocity.y);
            animator.SetBool("falling", true);
            animator.SetBool("jumping", false);
            animator.SetBool("idle", false);
        }


        // 从下降回到idle 状态 下面是我自己的实现
        // michel 使用的是LayerMask 
        //if (System.Math.Abs(rb.velocity.y) < Mathf.Epsilon)
        //{
        //    print("...." + animator.GetBool("falling"));
        //    // 之前是降落状态  然后y 为0的时候认为回到地面 状态设置为idle 可好？
        //    //if (!animator.GetBool("falling"))
        //    {
        //        print("set idle!");
        //        animator.SetBool("idle", true);
        //        animator.SetBool("falling", false);
        //        animator.SetBool("jumping", false);
        //    }


        //}

        if (System.Math.Abs(rb.velocity.y) <= Mathf.Epsilon)
        {
            print("System.Math.Abs(rb.velocity.y) < Mathf.Epsilon" + coll.IsTouchingLayers(ground).ToString());

            if (coll.IsTouchingLayers(ground))
            {
                //print("IsTouchingLayers");
                animator.SetBool("idle", true);
                animator.SetBool("falling", false);
                animator.SetBool("jumping", false);
            }
        }

        // 如果对tilemap 仅仅使用tilemap collider 的话  跑步的时候有抖动
        // 有时候跑着跑着 狐狸的y速度就很大  输出变为了 rb.velocity.y >  Mathf.Epsilon  0.361474  虽然没有跳  但是y速度明显大于 0.1
        // 这是因为每个tile 都是独立的碰撞体
        // 就像一块块瓷砖一样  实际上地面并不是那么平整
        // 所以我们再给tilemap添加一个composite collider(组合碰撞体)组件.添加此组件会自动为其添加rigidbody2D刚体组件
        // 这样地面就有重力了  会往下掉
        // 所以勾选地面rigidbody的 static  这样地面旧固定住了
        if (rb.velocity.y > Mathf.Epsilon && !coll.IsTouchingLayers(ground))
        // if (rb.velocity.y > 0.1)
        //if (rb.velocity.y > 1 && !coll.IsTouchingLayers(ground))
        {
            //print("rb.velocity.y >  Mathf.Epsilon  " + rb.velocity.y + "  " + coll.IsTouchingLayers(ground).ToString());
            animator.SetBool("falling", false);
            animator.SetBool("jumping", true);
            animator.SetBool("idle", false);

        }


        if (System.Math.Abs(rb.velocity.x) < 0.1)
        {
            if (justInjure)
            {
                justInjure = false;
                animator.SetBool("getInjure", justInjure);
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Collection"))
        {
            //Destroy(collision.gameObject);
            this.collectionFeedback(collision);
            cherryCount = cherryCount + 1;
            textCherryCount.text = "" + cherryCount;
        }

        // 掉到世界范围外
        if (collision.CompareTag("DeadLine"))
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            // 声音全部关闭
            AudioSource[] audios = GetComponents<AudioSource>();
            for (int i = audios.Length - 1; i >= 0; i--)
            {
                audios[i].enabled = false;
            }
            Invoke("Restart", 1f);

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (animator.GetBool("falling"))
            {
                enemy.JumpOn();
                //Destroy(collision.gameObject);
                // 杀死敌人还有反弹跳跃的效果
                float shrinkJump = 0.5f;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce * shrinkJump * Time.fixedDeltaTime);
                animator.SetBool("jumping", true);
            }
            else
            {
                justInjure = true;
                animator.SetBool("getInjure", justInjure);

                //这里都是玩家受到伤害  //受到伤害会往行进的反反向弹开
                if (transform.position.x < collision.gameObject.transform.position.x)
                {
                    rb.velocity = new Vector2(-1f * speed * Time.fixedDeltaTime, rb.velocity.y);
                }
                if (transform.position.x > collision.gameObject.transform.position.x)
                {
                    rb.velocity = new Vector2(speed * Time.fixedDeltaTime, rb.velocity.y);
                }

            }
        }
    }

    void collectionFeedback(Collider2D collision)
    {
        Collection collect = collision.gameObject.GetComponent<Collection>();
        collect.PlayFeedback();
    }


    //重制当前场景
    void Restart()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

}
