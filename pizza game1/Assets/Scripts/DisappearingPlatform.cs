using UnityEngine;
using System.Collections;

public class DisappearingPlatform : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool isDisappearing = false;

    [Header("설정")]
    [SerializeField] private float fadeDuration = 1.0f; // 사라지는 시간

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDisappearing) return;

        // 1. 이름에 '!'가 포함되어 있고 부딪힌 대상이 'Player'인지 확인
        if (gameObject.name.Contains("!") && collision.gameObject.CompareTag("Player"))
        {
            // 2. 충돌 지점들을 검사
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // contact.normal.y가 -0.8보다 작아야 '확실히 위에서 아래로' 밟은 것으로 간주
                // (옆면은 보통 0에 가깝고, 아랫면은 1에 가깝습니다)
                if (contact.normal.y < -0.8f)
                {
                    StartCoroutine(FadeAndDestroy());
                    break;
                }
            }
        }
    }

    IEnumerator FadeAndDestroy()
    {
        isDisappearing = true;
        float currentTime = 0f;
        Color startColor = spriteRenderer.color;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeDuration);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        gameObject.SetActive(false);
    }
}