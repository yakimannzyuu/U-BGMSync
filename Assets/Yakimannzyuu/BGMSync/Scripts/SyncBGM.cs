using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using System;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SyncBGM : UdonSharpBehaviour
{
    [SerializeField]    AudioSource source;
    [SerializeField]    AudioClip[] clips;
    [SerializeField]    bool[]      isLoops;

    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(BGMNumber))]
    int     bgmNumber = 0;
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(StartDate))]
    string startDate = "";
    [UdonSynced(UdonSyncMode.None), FieldChangeCallback(nameof(IsPlaying))]
    bool    isPlaying = false;

    [SerializeField]    string  statusViewFormat = "MM/dd/yyyy hh:mm:ss.fff tt";
    [SerializeField]    Text    statusViewText = null;

    DateTime date;
    [SerializeField]    Text    DebugLog;
    public void AddDebug(string str)
    {
        DebugLog.text += "\n" + str;
    }
    
    // 同期を要求する。
    public void RequestSync()
    {
        AddDebug("RequestSync");
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "Sync");
    }

    // 同期する。
    public void Sync()
    {
        if(Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
        {
            AddDebug("Sync Broadcast");
            date = JPNow();
            date.AddSeconds(-source.time);
            StartDate = date.ToString(statusViewFormat);
            RequestSerialization();
        }
        else
        {
            RequestSync();
        }
    }

    private readonly TimeZoneInfo TIMEZONE_JST =
        TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
    public DateTime JPNow()
    {
        return TimeZoneInfo.ConvertTime(DateTimeOffset.Now, TIMEZONE_JST).DateTime;
    }

    // Network Property
    public int BGMNumber
    {
        get => bgmNumber;
        set
        {
            bgmNumber = Mathf.Max(0, (value + clips.Length) % clips.Length);
            source.clip = clips[bgmNumber];
            source.loop = isLoops[Mathf.Max(0, bgmNumber % isLoops.Length)];
            AddDebug("BGMNumberPropaty : " + bgmNumber.ToString());
        }
    }

    public string StartDate
    {
        get => startDate;
        set
        {
            startDate = value;
            statusViewText.text = value;
            AddDebug("startDatePropaty : " + startDate.ToString());
            date = DateTime.ParseExact(value, statusViewFormat, null);
            source.time = (float)(JPNow() - date).TotalSeconds % clips[bgmNumber].length;
        }
    }

    public bool IsPlaying
    {
        get => isPlaying;
        set
        {
            isPlaying = value;
            if(isPlaying != source.isPlaying)
            {
                if(isPlaying){ source.Play(); }
                else{ source.Stop(); }
            }
            AddDebug("isPlayingPropaty : " + isPlaying.ToString());
        }
    }
    //

    // UdonSharpBehaviour
    void Start()
    {
        isPlaying = source.playOnAwake;
        Sync();
    }
    
    public override void Interact()
    {
        IsPlaying = !IsPlaying;
        Sync();
    }
}