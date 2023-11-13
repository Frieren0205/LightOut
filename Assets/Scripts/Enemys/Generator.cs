using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Playables;
using System.Linq;

public class Generator : MonoBehaviour
{

    public int GeneratorHP; // 초기 밸런싱 15
    private BoxCollider col;

    public EmissiveTest test;

    public ChinemachineManager chinemachineManager;
    private DOTweenAnimation doanimation;
    private SpriteRenderer sprender;
    private bool isHit = false;
    public bool isEmergency = false;
    // Start is called before the first frame update
    private void OnEnable() 
    {
        col = this.gameObject.GetComponent<BoxCollider>();
        sprender = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        doanimation = this.gameObject.GetComponentInChildren<DOTweenAnimation>();
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
        if(other.gameObject.tag == "PlayerAttack" && !isHit)
        {
            Debug.Log("피격 인식");
            GeneratorHP -= 1;
            OnHit();
        }
    }
    
    public void OnHit()
    {
        StartCoroutine(Hitable());
    }
    // 히트 시 무적시간
    IEnumerator Hitable()
    {
        isHit = true;
        sprender.color = new Color(0.4f,0.4f,0.4f,1);
        doanimation.DORestart();
        yield return new WaitForSecondsRealtime(1.25f);
        isHit = false;
        sprender.color = new Color(1,1,1,1);
    }

    IEnumerator OnTerminate()
    {
        col.enabled = false;
        yield return new WaitForSeconds(1.25f); // 파괴 애니메이션 길이만큼 딜레이
        // chinemachineManager.playableDirector.Play();
        // yield return new WaitForSecondsRealtime(8);
        // chinemachineManager.playableDirector.enabled = false;
        // test.TurnOff();
        LevelManager.Instance.subtera_generator_list.RemoveAt(0);
        Destroy(this.gameObject);
        //Debug.Log("파괴");
    }
}
