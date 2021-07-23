using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject[] stages;
    public GameObject UIClearMsg;
    public GameObject UIOverMsg;
    public GameObject restartButton;
    public Image[] UIPlayerLife;
    public Text UIStageText;
    public Text UIPoint;
    public Player player;
    public int playerLife;
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;

    private void Awake() {
        Screen.SetResolution(1024, 768, true);
    }

    private void Update() {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            HealthDown();
            PlayerReposition();
        }
    }

    void PlayerReposition() {
        player.transform.position = new Vector3(-12, 4, 0);
        player.VelocityZero();
    }

    public void HealthDown() {
        UIPlayerLife[playerLife - 1].color = new Color(1, 1, 1, 0.2f);
        playerLife--;
        if (playerLife == 0) {
            //Player Dead
            GameOver();
        }
    }

    public void NextStage() {
        if (stageIndex < stages.Length - 1) {
            //Next Stage

            totalPoint += stagePoint;
            stagePoint = 0;

            stages[stageIndex].SetActive(false);
            stageIndex++;
            stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStageText.text = "Stage " + (stageIndex + 1).ToString();
        }
        else {
            //Game Clear
            GameClear();
        }
    }

    void GameOver() {
        player.OnDie();
        UIOverMsg.SetActive(true);
        restartButton.SetActive(true);
    }

    void GameClear() {
        Time.timeScale = 0;
        UIClearMsg.SetActive(true);
        restartButton.SetActive(true);
    }

    public void Restart() {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }
}
