using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 홈 바에서 위로 드래그하는 제스처를 감지하여 Task 화면을 띄워주는 클래스입니다.
/// </summary>
public class TaskAction : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 startPosition;
    private const float swipeThreshold = 20f; // 더 민감하게 조정 (50 -> 20)

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = eventData.position;
        Debug.Log($"[TaskAction] 드래그 시작 위치: {startPosition}");
    }

    public void OnDrag(PointerEventData eventData)
    {
        // OnEndDrag를 활성화하기 위해 반드시 구현되어야 함
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 endPosition = eventData.position;
        float dragDistanceY = endPosition.y - startPosition.y;

        Debug.Log($"[TaskAction] 드래그 종료. 총 이동 거리(Y): {dragDistanceY}");

        // 위쪽으로 20픽셀 이상만 움직여도 Task 창 실행
        if (dragDistanceY > swipeThreshold)
        {
            Debug.Log($"[TaskAction] 제스처 성공! Task 상태 전환 시도");
            OSManager.Instance.SetTaskStatus();
        }
    }
}
