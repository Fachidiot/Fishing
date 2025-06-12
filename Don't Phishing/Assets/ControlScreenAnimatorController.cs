using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlScreenAnimatorController : MonoBehaviour
{
    private Animator m_animator;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    public void MediaToggle()
    {

    }
}
