using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class movement : MonoBehaviour
{
    [Header("이동 설정")]
    public float speed = 5f;
    public float jumpForce = 10f;

    [Header("효과음 설정")]
    public float volume = 1.0f;
    public AudioClip clip;
    private AudioSource audioSource;

    [Header("UI 연결")]
    public GameObject nextButton;
    public GameObject gameOverPanel;

    private Rigidbody2D rb;
    private bool isSceneLoading = false;
    private bool isGameOver = false;

    private float deadLineY = -3.0f;

    private CinemachineCamera cm;
    private float initialX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        // ❌ [삭제됨] Inventory.instance.items.Clear(); 
        // 게임 시작 시 아이템을 지우면 버튼이 안 뜹니다. 만약 초기화가 필요하면 Inventory의 Reset 기능을 쓰세요.

        if (nextButton != null) nextButton.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        Time.timeScale = 1f;

        GameObject vCamObj = GameObject.Find("CinemachineCamera");
        if (vCamObj != null)
        {
            cm = vCamObj.GetComponent<CinemachineCamera>();
            initialX = vCamObj.transform.position.x;
        }

        // 씬 시작 시 이미 10개 이상일 수도 있으니 한 번 체크해줍니다.
        CheckInventoryAndShowButton();
    }

    void Update()
    {
        if (isGameOver) return;

        float x = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(x * speed, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(rb.linearVelocity.y) < 0.01f)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        if (cm != null && cm.Follow != null)
        {
            cm.transform.position = new Vector3(initialX, transform.position.y, cm.transform.position.z);
        }

        if (transform.position.y < deadLineY)
        {
            TriggerGameOver();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isGameOver) return;
        if (collision.gameObject.name == "GameOverZone") { TriggerGameOver(); return; }

        string itemName = collision.gameObject.name.ToLower();
        if (itemName.Contains("mushroom") || itemName.Contains("pepperoni") ||
            itemName.Contains("cheese") || itemName.Contains("ham"))
        {
            // Inventory 스크립트의 AddItem을 호출하여 리스트에 추가하고 저장함
            Inventory.instance.AddItem(collision.gameObject.name);
            Destroy(collision.gameObject);

            if (audioSource != null && clip != null) audioSource.PlayOneShot(clip, volume);

            // 아이템을 먹을 때마다 버튼 조건 확인
            CheckInventoryAndShowButton();
        }
    }

    private void TriggerGameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            if (cm != null) cm.Follow = null;
            if (gameOverPanel != null) gameOverPanel.SetActive(true);
            StartCoroutine(WaitAndStopTime(0.3f));
        }
    }

    private IEnumerator WaitAndStopTime(float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        Time.timeScale = 0f;
    }

    private void CheckInventoryAndShowButton()
    {
        if (Inventory.instance == null || nextButton == null) return;

        int mCount = 0;
        int pCount = 0;

        foreach (string item in Inventory.instance.items)
        {
            string l = item.ToLower();
            if (l.Contains("mushroom")) mCount++;
            if (l.Contains("pepperoni")) pCount++;
        }

        // 콘솔 창에서 실시간으로 개수를 확인할 수 있게 로그 추가
        Debug.Log($"버섯: {mCount}, 페퍼로니: {pCount}");

        if (mCount >= 10 && pCount >= 10)
        {
            nextButton.SetActive(true);
        }
    }

    public void OnNextButtonClick()
    {
        if (!isSceneLoading)
        {
            Time.timeScale = 1f;
            isSceneLoading = true;
            // 아이템을 다 모아서 다음 단계로 갈 때만 인벤토리를 비우고 싶다면 여기서 Clear를 호출하세요.
            Inventory.instance.ResetInventory();
            SceneManager.LoadScene("ResultScene");
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}