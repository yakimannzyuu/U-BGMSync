using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
/// <summary>
/// イベント会場向けBGM再生スクリプト
/// 再生の同期が可能です。
/// </summary>
[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class BGMSync : UdonSharpBehaviour
{
    [SerializeField]    AudioSource source;
    [SerializeField]    AudioClip[] clips;

    [UdonSynced(UdonSyncMode.None)]
    float playTime = 0;

    [UdonSynced(UdonSyncMode.None)]
    int bgmNumber = 0;

    [UdonSynced(UdonSyncMode.None)]
    bool isPlay = false;

    [SerializeField]    Text ViewText;
    [SerializeField]    Text DebText;

    // 同期を要求する。
    void RequestSync()
    {
        AddDebug("RequestSync");
        ViewText.text = "RequestSync Owner:" + Networking.GetOwner(gameObject).displayName;
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "Sync");
    }

    // 同期する。
    public void Sync()
    {
        if(Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
        {
            playTime = source.time;
            AddDebug("Sync" + playTime.ToString());
            RequestSerialization();
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PlayUpdate");
        }
        else
        {
            RequestSync();
        }
    }

    // 再生状態を更新する。
    public void PlayUpdate()
    {
        AddDebug("PlayUpdate");
        source.clip = clips[bgmNumber];
        if(isPlay)
        {
            source.Play();
        }
        else
        {
            source.Stop();
        }
        source.time = playTime;

        if(ViewText != null)
        {
            string bgmName = clips[bgmNumber].name;
            ViewText.text = $"{bgmName}\nisPlay = {isPlay}    time = {playTime}";
        }
    }

    //
    public int BgmNumber
    {
        get{ return bgmNumber; }
        set{
            if(bgmNumber != value)
            {
                int num = Mathf.Max(0, value) % clips.Length;
                bgmNumber = num;
                source.time = 0;
                Sync();
            }
        }
    }

    public bool IsPlay
    {
        get{ return isPlay; }
        set{
            if(isPlay != value)
            {
                isPlay = value;
                Sync();
            }
        }
    }
    
    public override void Interact()
    {
        IsPlay = !IsPlay;
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if(Networking.LocalPlayer == player)
        {
            RequestSync();
        }
    }

    public void AddDebug(string str)
    {
        if(DebText == null){return;}
        DebText.text += "\n" + str;
    }
}

/*
要件メモ

BGM再生/停止が同期される
再生時間の同期が行われる。
音楽切り替えが同期される。

*/