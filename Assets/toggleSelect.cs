using UnityEngine;
using UnityEngine.UI;

public class ToggleAndButton : MonoBehaviour
{
    public Toggle toggle; // 토글 UI 요소를 연결하기 위한 변수
    public Button submitButton; // 버튼 UI 요소를 연결하기 위한 변수

    void Start()
    {
        // 버튼 클릭 시 이벤트를 트리거하기 위한 함수 추가
        submitButton.onClick.AddListener(OnSubmitButtonClick);
    }

    void OnSubmitButtonClick()
    {
        if (toggle.isOn)
        {
            Debug.Log("선택된 캐릭터: " + toggle.gameObject.name); // 토글의 이름을 콘솔에 출력
        }
    }
}
