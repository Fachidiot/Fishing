using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 화면 페이드 인/아웃 효과를 담당하는 컴포넌트입니다.
/// </summary>
public class ScreenFader : MonoBehaviour
{
    [SerializeField] private Image m_FadeImage = null;
    [SerializeField] private float m_DefaultDuration = 1f;

    private void Awake()
    {
        if (m_FadeImage != null)
        {
            // 시작 시 화면을 완전히 검게 설정
            Color c = m_FadeImage.color;
            c.a = 1f;
            m_FadeImage.color = c;
            m_FadeImage.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 화면을 서서히 밝게 만듭니다 (Fade In)
    /// </summary>
    public void FadeIn(float duration = -1f, Action onComplete = null)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(1f, 0f, duration > 0 ? duration : m_DefaultDuration, onComplete));
    }

    /// <summary>
    /// 화면을 서서히 어둡게 만듭니다 (Fade Out)
    /// </summary>
    public void FadeOut(float duration = -1f, Action onComplete = null)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(0f, 1f, duration > 0 ? duration : m_DefaultDuration, onComplete));
    }

    private IEnumerator FadeRoutine(float startAlpha, float endAlpha, float duration, Action onComplete)
    {
        if (m_FadeImage == null) yield break;

        float elapsed = 0f;
        Color c = m_FadeImage.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            m_FadeImage.color = c;
            yield return null;
        }

        c.a = endAlpha;
        m_FadeImage.color = c;
        
        if (endAlpha <= 0f) m_FadeImage.gameObject.SetActive(false);
        else m_FadeImage.gameObject.SetActive(true);

        onComplete?.Invoke();
    }
}
