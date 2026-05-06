using RoR2;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Networking;
public class GravityZone : MonoBehaviour
{
    private List<CharacterBody> characterBodies = new List<CharacterBody>();
    public float gravityCoefficient;

    // this script was based on the anti gravity script used in Fogbound Lagoon. Credit to John Lagoon (JaceDaDorito)
    // it is largely the same but applies velocity if the character is in a volume, rather than if they are below a specified elevation.
    // this allows for having many different water sources at different elevations, as opposed to one big ocean.

    // add character body to the list of bodies to be affected by water after it enters the collider
    public void OnTriggerEnter(Collider other)
    {
        CharacterBody body = other.GetComponent<CharacterBody>();
        if ((bool)body && !characterBodies.Contains(body) && body.characterMotor != null)
        {
            if (body.characterMotor.useGravity)
            {
                characterBodies.Add(body);
            }
        }
    }

    // adds character bodies that are in the collider but weren't caught by OnTriggerEnter
    public void OnTriggerStay(Collider other)
    {
        CharacterBody body = other.GetComponent<CharacterBody>();
        if ((bool)body && !characterBodies.Contains(body) && body.characterMotor != null)
        {
            if (body.characterMotor.useGravity)
            {
                characterBodies.Add(body);
            }
        }
    }

    // remove character body from the list of bodies to be affected by water after it leaves the collider
    public void OnTriggerExit(Collider other)
    {
        CharacterBody body = other.GetComponent<CharacterBody>();
        if (characterBodies.Contains(body))
        {
            characterBodies.Remove(body);
        }
    }

    // apply anti-gravity
    void FixedUpdate()
    {
        foreach (CharacterBody body in characterBodies)
        {
            //there was an issue where Breaching Fin would cause enemies to rise above the water before falling back down. kind of fitting given the item's name but probably shouldn't happen. If they have any of these debuffs they won't get affected by the gravity coefficient, which fixes that
            bool breaching = body.HasBuff(DLC2Content.Buffs.KnockBackActiveWindow) || body.HasBuff(DLC2Content.Buffs.KnockUpHitEnemies) || body.HasBuff(DLC2Content.Buffs.KnockUpHitEnemiesJuggleCount);

            if (!body.characterMotor.isGrounded && !breaching)
            {
                body.characterMotor.velocity.y += -Physics.gravity.y * gravityCoefficient * Time.fixedDeltaTime;
            }
        }
    }
}