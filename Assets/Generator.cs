using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{

    public int GeneratorHP; // 초기 밸런싱 15
    private BoxCollider col;
    public bool isEmergency = false;
    // Start is called before the first frame update
    private void OnEnable() 
    {
        col = this.gameObject.GetComponent<BoxCollider>();
    }
    // Update is called once per frame
    void Update()
    {
       if(GeneratorHP <= 5)
       {
            isEmergency = true;
       }
       if(GeneratorHP == 0)
       {
            // 발전기가 파괴되었을 때
            StartCoroutine(OnTerminate());
       }
    }
    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "PlayerAttack")
        {
            Debug.Log("피격 인식");
            GeneratorHP -= 1;
        }
    }
    
    public void OnHit()
    {

    }
    // 히트 시 무적시간

    IEnumerator OnTerminate()
    {
        col.enabled = false;
        yield return new WaitForSeconds(1.5f); // 파괴 애니메이션 길이만큼 딜레이
        this.gameObject.SetActive(false);
        Debug.Log("파괴");
    }
}
