using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("�̗͐ݒ�")]
    public int maxHP = 100;
    public int currentHP;


    [Header("UI")]
    public Image hpBar;

    [Header("�C�x���g")]
    public UnityEvent onDeath;
    public UnityEvent onDamage;

    public bool isDead => currentHP <= 0;

    private void Start()
    {
        currentHP = maxHP;
        UpdateHPUI();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        onDamage?.Invoke();
        UpdateHPUI();

        if (currentHP <= 0)
        {
            HandleDeath();
        }
    }

    void HandleDeath()
    {
        Debug.Log($"{gameObject.name} �����j����܂����I");
        onDeath?.Invoke();
        // �������~�߂��薳�G���Ȃǂ̉��o��������
        GetComponent<PlayerController>().enabled = false;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }

    void UpdateHPUI()
    {
        if (hpBar != null)
        {
            hpBar.fillAmount = (float)currentHP / maxHP;
        }
    }
}