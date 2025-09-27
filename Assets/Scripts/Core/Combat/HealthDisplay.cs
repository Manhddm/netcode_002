using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private Image healhBarImage;
    public override void OnNetworkSpawn()
    {
        if (!IsClient) return;
        health.CurrenHealth.OnValueChanged += HandleHealthChanged;
        HandleHealthChanged(0, health.CurrenHealth.Value);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient) return;
        health.CurrenHealth.OnValueChanged -= HandleHealthChanged;
    }
    private void HandleHealthChanged(int oldHealth, int newHealth)
    {
        float healthNormalized = (float)newHealth / health.maxHealth;
        healhBarImage.fillAmount = healthNormalized;
    }
}
