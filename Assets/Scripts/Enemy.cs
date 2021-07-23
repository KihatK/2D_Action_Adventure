using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int point;
    public int nextMove;

    Rigidbody2D rigid;
    SpriteRenderer sprite;
    CircleCollider2D circleCollider;
    Animator anim;

    private void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();

        Invoke("Think", 5);
    }

    private void FixedUpdate() {
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Ground"));

        if (rayHit.collider == null) {
            Turn();
        }
    }

    void Turn() {
        nextMove *= -1;

        sprite.flipX = nextMove == 1;

        CancelInvoke();
        Invoke("Think", 5);
    }

    void Think() {
        nextMove = Random.Range(-1, 2);

        anim.SetInteger("isWalk", nextMove);

        if (nextMove != 0) {
            sprite.flipX = nextMove == 1;
        }

        float thinkTime = Random.Range(2f, 5f);

        Invoke("Think", thinkTime);
    }

    public void OnDamaged() {
        sprite.flipY = true;
        circleCollider.enabled = false;
        sprite.color = new Color(1, 1, 1, 0.4f);
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        Invoke("DeActive", 3);
    }

    void DeActive() {
        gameObject.SetActive(false);
    }

    private void OnDisable() {
        //When Enemy Die, Cancel All Invoke
        CancelInvoke();
    }
}
