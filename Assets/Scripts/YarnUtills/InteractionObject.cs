using UnityEngine;

[CreateAssetMenu(fileName = "InteractionData_", menuName = "ScriptableObjects/InteractionObject", order = 1)]
public class InteractionObject : ScriptableObject
{
    [Header("텍스트 이벤트 화자(말하고 있는 사람)")]
    public string[] TextEventName;
    
    [Header("말하고 있는 캐릭터 일러스트")]
    public Sprite[] EventCharacterIllust;

    [Header("텍스트 이벤트 내용")]
    [TextArea(20, int.MaxValue)]
    public string[] TextEventLog;


}