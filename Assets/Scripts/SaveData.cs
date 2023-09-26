using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    // [Header("저장 데이터에 포함되있는 데이터 목록")]
    //TODO: 저장 데이터에 어떤 데이터가 들어가야하는지 정할것
    [Header("플레이어 마지막 포지션")]
    public Vector3 lastest_p_transform;
}
