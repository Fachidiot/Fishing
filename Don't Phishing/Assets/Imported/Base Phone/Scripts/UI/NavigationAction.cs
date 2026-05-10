using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 홈 버튼의 클릭(홈 이동)과 드래그(Task 이동)를 모두 처리하는 통합 클래스입니다.
/// </summary>
public class NavigationAction : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GameObject m_TaskBar;
    [SerializeField] private Camera m_BackgroundCaptureCamera;
    [SerializeField] private RenderTexture m_Texture;

    private string m_AppName;
    private Texture2D m_texture2D;
    private Vector2 startPosition;
    private bool isDragging = false;

    // 1. 클릭 처리 (짧게 누르기)
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDragging) return; // 드래그 중이었다면 클릭 무시

        Debug.Log("[NavigationAction] 홈 버튼 클릭됨 -> 홈 화면으로 이동");
        EndApp();
    }

    // 2. 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = eventData.position;
        isDragging = true;
        Debug.Log("[NavigationAction] 드래그 시작");
    }

    public void OnDrag(PointerEventData eventData) { }

    // 3. 드래그 종료 (위로 밀기)
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        float dragDistanceY = eventData.position.y - startPosition.y;

        Debug.Log($"[NavigationAction] 드래그 종료 (Y 이동거리: {dragDistanceY})");

        if (dragDistanceY > 30f) // 30픽셀 이상 위로 밀면 Task 창 실행
        {
            Debug.Log("[NavigationAction] 드래그 업 성공 -> Task 창 열기");
            OSManager.Instance.SetTaskStatus();
        }
    }

    public void EndApp()
    {
        RenderTexture.active = m_Texture;
        m_texture2D = new Texture2D(m_Texture.width, m_Texture.height);
        m_texture2D.ReadPixels(new Rect(0, 0, m_Texture.width, m_Texture.height), 0, 0);
        m_texture2D.Apply();

        m_AppName = AppManager.Instance.GetCurrentApp();
        if (m_AppName == string.Empty)
        {
            ResetApps();
            return;
        }
        
        StartCoroutine(ScreenCapture());
    }

    private void ResetApps()
    {
        AppManager.Instance.ResetApps();
        OSManager.Instance.EndApp();
    }

    private IEnumerator ScreenCapture()
    {
        yield return new WaitForEndOfFrame();

        byte[] byteArray = m_texture2D.EncodeToPNG();
        string savePath = Application.dataPath + "/Resources/Background/" + m_AppName + ".png";
        File.WriteAllBytes(savePath, byteArray);

        m_BackgroundCaptureCamera.gameObject.SetActive(false);
        if (Application.isPlaying) Destroy(m_texture2D);

        TaskManager.Instance.AddTask(m_AppName);
        m_AppName = string.Empty;
        ResetApps();
    }
}
