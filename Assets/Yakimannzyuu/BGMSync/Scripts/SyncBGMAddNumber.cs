
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SyncBGMAddNumber : UdonSharpBehaviour
{
    [SerializeField]    SyncBGM bgm = null;
    [SerializeField]    int addNumber = 1;
    
    public override void Interact()
    {
        bgm.BGMNumber += addNumber;
        bgm.Sync();
    }
}
