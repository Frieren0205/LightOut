using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Generator : MonoBehaviour
{

    public int GeneratorHP; // 초기 밸런싱 15
    private BoxCollider col;

    public EmissiveTest test;

    public ChinemachineManager chinemachineManager;
    private DOTweenAnimation doanimation;
    private SpriteRenderer sprender;
    public Sprite original_generator_sprite;
    public Sprite damage_generator_sprite;
    private bool isHit = false;
    public bool isEmergency = false;
    public bool isLevelUP_Point = false;

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
       if(GeneratorHP <= 2)
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
        sprender.sprite = damage_generator_sprite;
        doanimation.DORestart();
        yield return new WaitForSecondsRealtime(1.25f);
        isHit = false;
        sprender.color = new Color(1,1,1,1);
        if(isEmergency)
        {
            sprender.sprite = damage_generator_sprite;
        }
        else
        {
            sprender.sprite = original_generator_sprite;
        }
    }

    IEnumerator OnTerminate()
    {
      
        col.enabled = false;
        yield return new WaitForSeconds(1.25f); // 파괴 애니메이션 길이만큼 딜레이 
        // chinemachineManager.playableDirector.Play();
        // yield return new WaitForSecondsRealtime(8);
        // chinemachineManager.playableDirector.enabled = false;
        // test.TurnOff();
        LevelManager.Instance.generator_list.RemoveAt(LevelManager.Instance.generator_list.IndexOf(this.gameObject.GetComponent<Generator>()));
        if(isLevelUP_Point == true) GameManager.Instance.interactionManager.if_Generator_Destory();
        Destroy(this.gameObject);
        //Debug.Log("파괴");
    }
}
