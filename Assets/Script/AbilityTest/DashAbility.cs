
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DashAbility : Ability
{
    public float dashVelocity;
    public override void Activate(GameObject parent)
    {
        PlayerMovement_2 movement = parent.GetComponent<PlayerMovement_2>();
        movement.acceleration = dashVelocity;
    }

    public override void BeginCooldown(GameObject parent)
    {
        PlayerMovement_2 movement = parent.GetComponent<PlayerMovement_2>();
        movement.acceleration = movement.normalAcceleration;
    }
}
