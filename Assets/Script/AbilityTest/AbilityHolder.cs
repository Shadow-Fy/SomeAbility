using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    public Ability ability;
    private float cooldownTime;
    private float activeTime;

    enum AbilityState
    {
        ready,active,cooldown
    }

    private AbilityState state = AbilityState.ready;
    public KeyCode key;
    

    void Start()
    {
        
    }

 
    void Update()
    {
        switch (state)
        {
            case AbilityState.ready:
                if (Input.GetKeyDown(key))
                {
                    ability.Activate(gameObject);
                    state = AbilityState.active;
                    activeTime = ability.activeTime;
                }
                break;
            case AbilityState.active:
                if (activeTime >= 0)
                {
                    activeTime -= Time.deltaTime;
                }
                else
                {                    
                    ability.BeginCooldown(gameObject);
                    cooldownTime = ability.cooldownTime;
                    state = AbilityState.cooldown;
                }
                break;
            case AbilityState.cooldown:
                if (cooldownTime > 0)
                {
                    cooldownTime -= Time.deltaTime;
                }
                else
                {
                    state = AbilityState.ready;
                }
                break;
        }
    }
}