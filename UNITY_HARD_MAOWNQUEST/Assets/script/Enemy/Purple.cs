﻿using UnityEngine;

public class Purple : MonoBehaviour
{
    #region 欄位
    [Header("移動速度"), Range(0, 100)]
    public float speed = 1f;
    [Header("攻擊力"), Range(0, 100)]
    public float attack = 10f;
    [Header("攻擊冷卻"), Range(0, 30)]
    public float cd = 3;
    [Header("血量"), Range(0, 1000)]
    public float hp = 100f;
    [Header("偵測範圍"), Range(0, 50)]
    public float radiusTrack = 5;
    [Header("攻擊範圍"), Range(0, 30)]
    public float radiusAttack = 2;
    [Header("偵測地板位移與半徑")]
    public Vector3 groundoffset;
    public float groundRadius = 0.1f;


    private Transform player;
    private Rigidbody2D rig;
    private Animator ani;

    /// <summary>
    /// Cd時間
    /// </summary>
    private float timer;

    /// <summary>
    /// 原始速度
    /// </summary>
    private float speedOriginal;

    #endregion

    #region 事件
    private void Start()
    {
        ani = GetComponent<Animator>();
        rig = GetComponent<Rigidbody2D>();

        player = GameObject.Find("Player").transform;

        timer = cd;
        speedOriginal = speed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, radiusTrack);

        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, radiusAttack);

        Gizmos.color = new Color(0.6f, 0.9f, 1, 0.7f);
        Gizmos.DrawSphere(transform.position + transform.right * groundoffset.x + transform.up * groundoffset.y, groundRadius);
    }

    private void Update()
    {
        Move();

    }
    #endregion

    #region 方法
    /// <summary>
    /// 移動
    /// </summary>
    private void Move()
    {
        float dis = Vector3.Distance(player.position, transform.position);

        if (dis <= radiusAttack)
        {
            Attack();
        }

        else if (dis <= radiusTrack)
        {
            rig.velocity = transform.right * speed * Time.deltaTime;
            ani.SetBool("走路開關", speed != 0);
            LookAtPlayer();
            CheckGround();
        }

        else
        {
                ani.SetBool("走路開關", false);
        }
    }

    /// <summary>
    /// 攻擊
    /// </summary>
    private void Attack()
    {
        ani.SetBool("走路開關", false);
        
        if (timer <= cd)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0;
            ani.SetTrigger("攻擊觸發");
        }
    }

    /// <summary>
    /// 面相玩家
    /// </summary>
    private void LookAtPlayer()
    {
        if (transform.position.x > player.position.x)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }

        else
        {
            transform.eulerAngles = Vector3.zero;
        }
    
        
    }
    
    /// <summary>
    /// 移動區域
    /// </summary>
    private void CheckGround()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position + transform.right * groundoffset.x + transform.up * groundoffset.y, groundRadius, 1 << 8);

        if (hit && (hit.name == "地板" || hit.name == "跳台"))
        {
            print("可以向前");
            speed = speedOriginal;
        }

        else
        {
            print("不可以向前");
            speed = 0;
        }

    }

    /// <summary>
    /// 死亡
    /// </summary>
    private void Dead() 
    {
        ani.SetBool("死亡", true);
    }

    /// <summary>
    /// 受傷
    /// </summary>
    /// <param name="damge">接收到的傷害</param>
    public void Hit(float damge)
    {
        hp -= damge;

        if (hp <= 0) Dead();
    }
    #endregion
}

