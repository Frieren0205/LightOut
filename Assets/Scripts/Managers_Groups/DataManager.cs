using UnityEngine;
using System.IO;

public class DataManager : MonoBehaviour
{
    static GameObject container;
    static DataManager _instance;
    public static DataManager Instance
    {
        get
        {
            if(!_instance)
            {
                container = GameObject.FindFirstObjectByType<DataManager>().gameObject;
                _instance = container.GetComponent<DataManager>();
                DontDestroyOnLoad(container);
            }
            return _instance;
        }
    }
    string GamaDataFileName = "svdt.json";

    public SaveData saveData = new SaveData();

    public void LoadFromSaveData()
    {
        // 세이브 데이터 파일 경로 
        string filepath = Application.persistentDataPath + "/" + GamaDataFileName;
        // 저장된 파일이 있을 경우
        if(File.Exists(filepath))
        {
            string FromJsonData = File.ReadAllText(filepath);
            saveData = JsonUtility.FromJson<SaveData>(FromJsonData);
            Debug.Log(filepath);
            //TODO : 파일 로드를 성공했음을 표기하기
        }
    }

    //TODO : 새 데이터 작성도 넣어야 할 지 체크하기. . .?
    public void SavetoSaveData()
    {
        string ToJsonData = JsonUtility.ToJson(saveData, true);
        string filepath = Application.persistentDataPath + "/" + GamaDataFileName;

        GameManager.Instance.SavePlayerData();
        File.WriteAllText(filepath, ToJsonData);
        //올바르게 저장이 되었는지 체크하기
    }

}
