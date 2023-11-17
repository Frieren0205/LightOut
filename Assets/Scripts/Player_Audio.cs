using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Audio : MonoBehaviour
{

    #region 플레이어 효과음 모음
    [Header("플레이어 발소리 배열")]
    public AudioClip[] player_footstep_list;
    [Header("플레이어 포복이동 소리 배열")]
    public AudioClip[] player_crawlsound_list;
    [Header("플레이어 공격 사운드 배열")]
    public AudioClip[] player_attacksound_list;
    #endregion

    public AudioSource player_audio_source;
    public void FootStep()
    {
        player_audio_source.PlayOneShot(player_footstep_list[Random.Range(0,3)]);
    }

    public void AttackSound(int attackcount)
    {
        switch(attackcount)
        {
            case 0:
            {
                player_audio_source.PlayOneShot(player_attacksound_list[0]);
                break;
            }
            case 1:
            {
                player_audio_source.PlayOneShot(player_attacksound_list[1]);
                break;
            }
            case 2:
            {
                player_audio_source.PlayOneShot(player_attacksound_list[2]);
                break;
            }
        }
    }

    public void CrawlSound()
    {
        player_audio_source.PlayOneShot(player_crawlsound_list[Random.Range(0,3)]);
    }
}
