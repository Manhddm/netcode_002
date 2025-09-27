using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Health : NetworkBehaviour
{
    [field:SerializeField] public int maxHealth { get; private set; } = 100;
    public NetworkVariable<int> CurrenHealth = new NetworkVariable<int>();
    private bool isDead ;
    public Action<Health> OnDie;
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        CurrenHealth.Value = maxHealth;
        isDead = false;
    }

    public void TakeDamage(int damage)
    {
        ModifyHealth(-damage);
    }
    public void RestoreHealth(int healAmount)
    {
        ModifyHealth(healAmount);
    }
    private void ModifyHealth(int value)
    {
        if (isDead) return;
        int newHealth = CurrenHealth.Value + value;
        CurrenHealth.Value = Mathf.Clamp(newHealth, 0, maxHealth);
        if (CurrenHealth.Value <= 0)
        {
            isDead = true;
            OnDie?.Invoke(this);
        }

    }
}
