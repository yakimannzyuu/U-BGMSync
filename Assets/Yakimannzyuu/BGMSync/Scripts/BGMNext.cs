
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BGMNext : UdonSharpBehaviour
{
    [SerializeField] BGMSync sync;
    
    public override void Interact()
    {
        sync.BgmNumber++;
    }
}
