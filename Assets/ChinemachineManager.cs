using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class ChinemachineManager : MonoBehaviour
{

    [Header("타임라인 리스트")]
    public TimelineAsset[] Timelinelist;
    public PlayableDirector playableDirector;

    public void OnGeneratorTerminate()
    {
        playableDirector.playableAsset = Timelinelist[0];
    }
}
