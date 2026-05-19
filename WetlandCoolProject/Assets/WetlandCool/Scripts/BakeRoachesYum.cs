using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using LOP;

//In play mode, you can toggle a roach object on/off to bake the roaches if this component is added. Obviously only works if attached to an object with a RoachController component.
//You can even bake the Wetland Aspect roaches if you load that scene and add this component to them.
//Basically, for each roach cluster object in the map, I open the editor, bake the roaches, copy the Roaches array, then exit play mode and paste it.
//If you wish to add roaches to your own map, note that having the roach "cone" faced down, like perfectly 90 degrees facing the ground, will cause every roach to start at the same point, rather than being spread out. You need to angle the cone for best results.
//There is probably a way to bake roaches without going into play mode, but this is just what I knew how to do. Feel free to use this for your own roach baking endeavours if you would like.
//Sometimes i dip my roaches in a vat of boiling acid and they scream loud as FUUUUUUUCK


public class BakeRoachesYum : MonoBehaviour
{
#if UNITY_EDITOR
    
    void Awake()
    {
        //The intended use for this component is to attach it to your roach object, then repeatedly toggle its active state to re-bake the roaches. if you injected the roach params, doing this will revert the roach params to null every time you try to bake the roaches, which obviously is not ideal. So, the Addressable Injector component, if it exists, is removed to prevent that.
        //A null roach params field is auto-filled with the default roach params OnEnable, so you don't need to worry about filling it out manually.
        if (gameObject.GetComponent<AddressableInjector>())
        {
            GameObject.Destroy(gameObject.GetComponent<AddressableInjector>());
        }
    }
    void OnEnable()
    {
        RoachParams rpDefault = Addressables.LoadAssetAsync<RoachParams>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Common_Props.rpDefaultRoach_asset).WaitForCompletion();
        if (gameObject.GetComponent<RoachController>().roachParams == null)
        {
            gameObject.GetComponent<RoachController>().roachParams = rpDefault;

        }
        gameObject.GetComponent<RoachController>().BakeRoaches();
    }
#endif
}