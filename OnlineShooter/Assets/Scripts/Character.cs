using System;
using UnityEngine;
using UnityEngine.Networking;
[RequireComponent(typeof(CharacterController))]
public abstract class Character : NetworkBehaviour
{
    protected Action OnUpdateAction { get; set; }
    protected abstract RayShooter fireAction { get; set; }
    [SyncVar] protected Vector3 serverPosition;
    [SyncVar] protected Quaternion serverRotation;
    [SyncVar] protected int CountPlayers;
    [SyncVar] private int money = 100;
    [SyncVar] protected int ServerHealth;

    protected virtual void Initiate()
    {
        OnUpdateAction += Movement;
        OnUpdateAction += HealthUpdater;
    }    

    private void Update()
    {
        OnUpdate();
    }

    private void OnUpdate()
    {
        OnUpdateAction?.Invoke();
    }

    [Command]
    protected void CmdUpdatePosition(Vector3 position,Quaternion rotation)
    {
        serverPosition = position;
        serverRotation = rotation;
    }

    [Command]
    protected void CmdSetStartHealth(int health)
    {
        ServerHealth = health;
    }

    [Command]
    protected void CmdShoot()
    {
        fireAction.ServerShoot();
    }


    public void UpdateHealth()
    {
        ServerHealth -= 5;

        if(ServerHealth <= 0)
        {
            ServerHealth = 0;
            NetworkServer.DisconnectAll();
        }
    }

    public abstract void Movement();
    public abstract void HealthUpdater();

}
