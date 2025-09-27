using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour 
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Collider2D playerCollider;
    [Header("Settings")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float fireRate;
    [SerializeField] private float muzzleFlashDuration;

    private bool shouldFire;
    private float previousFireTime;
    private float muzzleFlashTimer;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)  return;
        inputReader.PrimaryFireEvent += HandleFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputReader.PrimaryFireEvent -= HandleFire;
    }

    private void Update()
    {
        if(!IsOwner) return;
        if (!shouldFire) return;
        if (Time.time  < (1/fireRate) + previousFireTime) return;
        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);
        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);
        previousFireTime = Time.time;
    }
    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPoint, Vector3 direction)
    {
        GameObject  projectileInstance = Instantiate(clientProjectilePrefab, spawnPoint, Quaternion.identity);
        projectileInstance.transform.up = direction;
        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());
        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }
        SpawnDummyProjectileClientRpc(spawnPoint, direction);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPoint, Vector3 direction)
    {
        if(IsOwner) return;
        SpawnDummyProjectile(spawnPoint, direction);
    }

    private void HandleFire(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }
    private void SpawnDummyProjectile(Vector3 spawnPoint, Vector3 direction)
    {
        muzzleFlash.SetActive(true);
        //muzzleFlashTimer = muzzleFlashDuration;
        StartCoroutine(DisableMuzzleFlash());
        GameObject  projectileInstance = Instantiate(clientProjectilePrefab, spawnPoint, Quaternion.identity);
        projectileInstance.transform.up = direction;
        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());
        if (projectileInstance.TryGetComponent<DealDamageContact>(out DealDamageContact dealDamageContact))
        {
            dealDamageContact.SetOwner(OwnerClientId);
        }
        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * projectileSpeed;
        }
    }

    IEnumerator DisableMuzzleFlash()
    {
        yield return new WaitForSeconds(muzzleFlashDuration);
        muzzleFlash.SetActive(false);
    }
}
