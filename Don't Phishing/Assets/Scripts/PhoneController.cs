using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneController : MonoBehaviour
{
    [SerializeField]
    private Animator m_animator = null;

    private bool m_enable = false;
    private bool m_disable = false;

    private string m_animIDEnable = "Enable";
    private string m_animIDDisable = "Disable";

    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
        UpdateAnimationProcess();
    }

    private void Initialize()
    {
        // 인스펙터에서 할당되지 않았을 경우를 대비한 캐싱 및 예외 처리
        if (m_animator == null)
        {
            m_animator = GetComponent<Animator>();
        }

        if (m_animator == null)
        {
            Debug.LogError("[PhoneController] Animator가 할당되지 않았습니다.");
        }
    }

    private void UpdateAnimationProcess()
    {
        if (m_disable)
        {
            if (CheckEndAnimation(m_animIDDisable))
            {
                m_disable = false;
            }
        }
        
        if (m_enable)
        {
            if (CheckEndAnimation(m_animIDEnable))
            {
                m_enable = false;
            }
        }
    }

    public void Enable()
    {
        if (m_animator == null) return;
        
        m_animator.SetBool(m_animIDEnable, true);
        m_enable = true;
        m_disable = false;
    }

    public void Disable()
    {
        if (m_animator == null) return;

        m_animator.SetBool(m_animIDEnable, false);
        m_disable = true;
        m_enable = false;
    }

    private bool CheckEndAnimation(string name)
    {
        if (m_animator == null) return false;

        AnimatorStateInfo stateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
        
        if (stateInfo.IsName(name))
        {
            float _animTime = stateInfo.normalizedTime;
            
            // 애니메이션이 끝났는지 확인 (normalizedTime >= 1.0f)
            if (_animTime >= 1.0f)
            {
                return true;
            }
        }
        return false;
    }
}
