using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Task 창의 빈 공간을 클릭했을 때 다시 홈 화면으로 돌아가게 하는 스크립트입니다.
/// </summary>
public class CloseTaskAction : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[CloseTaskAction] 배경 클릭됨 -> 홈 화면으로 복귀");
        
        // OSManager의 상태를 Idle로 돌리면, 
        // OSManager가 자동으로 TaskBar를 비활성화하고 MainScreen을 켭니다.
        OSManager.Instance.EndApp();
    }
}
