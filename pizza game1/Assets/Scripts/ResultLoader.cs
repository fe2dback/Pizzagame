using UnityEngine;

public class ResultLoader : MonoBehaviour
{
    // 유니티 인스펙터 창에서 활성화할 버튼 오브젝트를 드래그해서 넣어주세요.
    public GameObject nextButton;

    void Start()
    {
        // 처음에는 버튼을 숨긴 상태로 시작합니다.
        if (nextButton != null)
        {
            nextButton.SetActive(false);
        }

        // 씬이 시작될 때 인벤토리 데이터를 다시 한 번 최신화합니다.
        if (Inventory.instance != null)
        {
            Inventory.instance.LoadInventory();
        }
    }

    void Update()
    {
        if (Inventory.instance != null && nextButton != null)
        {
            // 각각 10개 이상인지 확인
            int mushroomCount = Inventory.instance.GetItemCount("Mushroom");
            int pepperoniCount = Inventory.instance.GetItemCount("Pepperoni");

            // 디버깅을 위해 콘솔에 개수를 찍어보고 싶다면 아래 주석을 해제하세요.
            // Debug.Log($"M: {mushroomCount}, P: {pepperoniCount}");

            if (mushroomCount >= 10 && pepperoniCount >= 10)
            {
                // 버튼이 꺼져있을 때만 켭니다 (매 프레임 켜는 과부하 방지)
                if (!nextButton.activeSelf)
                {
                    nextButton.SetActive(true);
                    Debug.Log("모든 재료 수집 완료! 버튼 활성화.");
                }
            }
        }
    }
}