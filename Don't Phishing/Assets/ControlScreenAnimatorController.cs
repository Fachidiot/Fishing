using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlScreenAnimatorController : MonoBehaviour
{
    private Animator m_animator = null;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        m_animator = GetComponent<Animator>();
        if (m_animator == null) Debug.LogError("[ControlScreenAnimatorController] Animator를 찾을 수 없습니다.");
    }

    public void MediaToggle()
    {
        // 로직 구현 필요 시 여기에 함수 호출 형태로 작성
    }
}
