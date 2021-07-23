using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    public GameManager gameManager;
    public float maxSpeed;
    public int jumpPower;

    Rigidbody2D rigid;
    CircleCollider2D circleCollider;
    Animator anim;
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;
    bool onDamaging;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate() {
        float h = onDamaging ? 0 : Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed) {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < -maxSpeed) {
            rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y);
        }

        if (rigid.velocity.y < -0.00001f) {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Ground"));

            if (rayHit.collider != null) {
                if (rayHit.distance < 0.5f) {
                    anim.SetBool("isJumping", false);
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping") && !onDamaging) {
            PlaySound("JUMP");

            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping", true);
        }

        if (Input.GetButtonUp("Horizontal")) {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        if (Input.GetButton("Horizontal") && !onDamaging) {
            spriteRenderer.flipX = rigid.velocity.x < 0;
        }

        if (Mathf.Abs(rigid.velocity.x) < 0.3f) {
            anim.SetBool("isWalking", false);
        }
        else {
            anim.SetBool("isWalking", true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Item") {
            PlaySound("ITEM");

            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze) {
                gameManager.stagePoint += 50;
            }
            else if (isSilver) {
                gameManager.stagePoint += 150;
            }
            else if (isGold) {
                gameManager.stagePoint += 300;
            }

            //DeActive Item
            collision.gameObject.SetActive(false);
        }
        else if (collision.tag == "Finish") {
            //Stage Clear
            PlaySound("FINISH");

            gameManager.NextStage();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Enemy") {
            if (rigid.velocity.y < -0.0001f && transform.position.y > collision.transform.position.y) {
                OnAttack(collision.transform);
            }
            else {
                OnDamaged(collision.transform.position);
            }
        }
    }

    void OnAttack(Transform enemy) {
        PlaySound("ATTACK");

        rigid.AddForce(Vector2.up * 7, ForceMode2D.Impulse);

        Enemy enemyLogic = enemy.gameObject.GetComponent<Enemy>();
        enemyLogic.OnDamaged();
        gameManager.stagePoint += enemyLogic.point;
    }

    void OnDamaged(Vector3 enemy) {
        PlaySound("DAMAGED");

        gameManager.HealthDown();
        onDamaging = true;
        int enemyDir = transform.position.x - enemy.x > 0 ? 1 : -1;
        

        rigid.AddForce(new Vector2(enemyDir, 1) * 10, ForceMode2D.Impulse);

        //Animation
        anim.SetTrigger("doDamaged");

        //Immortal
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        gameObject.layer = 11;

        Invoke("OffDamaged", 2);
        Invoke("OffDamaging", 1f);
    }

    void OffDamaged() {
        if (gameManager.playerLife == 0) {
            return;
        }
        spriteRenderer.color = new Color(1, 1, 1, 1);
        gameObject.layer = 10;
    }

    void OffDamaging() {
        onDamaging = false;
    }

    public void VelocityZero() {
        rigid.velocity = Vector2.zero;
    }

    public void OnDie() {
        PlaySound("DIE");

        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        spriteRenderer.flipY = true;
        circleCollider.enabled = false;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    void PlaySound(string action) {
        switch (action) {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
        }
        audioSource.Play();
    }
}
