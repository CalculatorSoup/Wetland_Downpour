using RoR2;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FSCStage;
using UnityEngine;
using UnityEngine.UIElements;

namespace FSCStage.Content
{
    public class AudioMuffleZone : MonoBehaviour
    {
        // this script was based on the music muffling script used in Fogbound Lagoon. Credit to John Lagoon
        // it is largely the same but applies muffling if the camera is in a volume, rather than if it is below a specified elevation

        void OnEnable()
        {
            if (FSCStage.waterMuffle.Value) On.RoR2.MusicController.RecalculateHealth += MusicController_RecalculateHealth;
        }

        void OnDisable()
        {
            if (FSCStage.waterMuffle.Value) On.RoR2.MusicController.RecalculateHealth -= MusicController_RecalculateHealth;
        }

        //player cameras don't have collider components, so you can't use OnTriggerEnter/Exit to see if it entered the collider.
        //Instead, get the collider's bounds, then return whether the given position is inside the bounds
        public static bool IsInsideCollider(Collider collider, Vector3 position)
        {
            return collider.bounds.Contains(position);
        }

        //muffle music on a camera if the player object's CharacterBody component is in the player list
        private void MusicController_RecalculateHealth(On.RoR2.MusicController.orig_RecalculateHealth orig, RoR2.MusicController self, GameObject playerObject)
        {
            orig(self, playerObject);

            if (self.targetCamera)
            {
                if (IsInsideCollider(gameObject.GetComponent<BoxCollider>(), self.targetCamera.transform.localPosition))
                {
                    self.rtpcPlayerHealthValue.value -= 100;
                    self.rtpcPlayerHealthValue.value = Mathf.Max(self.rtpcEnemyValue.value, -100);
                }
            }

        }
    }

}