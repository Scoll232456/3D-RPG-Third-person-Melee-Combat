using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    MeeleFighter meleeFighter;
    private void Awake()
    {
        meleeFighter = GetComponent<MeeleFighter>();
    }
    private void Update()
    {
        if (Input.GetButtonDown("CommanAttack")) 
        {
            meleeFighter.TryToAttack();
        }
        
    }
}
