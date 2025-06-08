using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("‘Ì—ÍÝ’è")]
    public int maxHP = 100;
    public int currentHP;


    [Header("UI")]
    public Image hpBar;

    [Header("ƒCƒxƒ“ƒg")]
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
        Debug.Log($"{gameObject.name} ‚ªŒ‚”j‚³‚ê‚Ü‚µ‚½I");
        onDeath?.Invoke();
        // “®‚«‚ðŽ~‚ß‚½‚è–³“G‰»‚È‚Ç‚Ì‰‰o‚à‚±‚±‚Å
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