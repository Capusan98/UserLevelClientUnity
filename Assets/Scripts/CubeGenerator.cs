using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class CubeGenerator : MonoBehaviour
{

    public static CubeGenerator Instance { get; private set; }
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public GameObject CubePrefab;

    public Transform CubesParentTransform;

    public MapsJson Maps;

    [System.Serializable]
    public struct MapsJson
    {
        public List<CubeList> Maps;
    }

    [System.Serializable]
    public struct CubeList
    {
        public string name;
        public List<Cube> cubes;
    }

    [System.Serializable]
    public struct Cube
    {
        public float x;
        public float y;
        public Cube(float x, float y)
        {
            this.x = x; this.y = y;
        }
    }

    public void GenerateRandomCubes()
    {
        DeleteCubes();

        var numberOfCubes=Random.Range(0, 50);
        
        for(int i = 0; i < numberOfCubes; i++)
        {
            SpawnCube(Random.Range(0,9),Random.Range(0,9));
        }
    }

    private void SpawnCube(float x, float y)
    {
        var cube=Instantiate(CubePrefab, CubesParentTransform);
        cube.transform.localPosition=new Vector3(x,0,y);
    }

    public void DeleteCubes()
    {
        foreach(Transform obj in CubesParentTransform)
        {
            Destroy(obj.gameObject);
        }
    }

    public void GenerateCubesFromList(CubeList _cubeList)
    {
        foreach (var cube in _cubeList.cubes)
        {
            SpawnCube(cube.x, cube.y);
        }
    }

    public string SaveObjectsToText()
    {
        CubeList newCubeList;
        newCubeList.cubes = new List<Cube>();
        newCubeList.name = UiManager.Instance.MapNameInputField.text;

        foreach (Transform obj in CubesParentTransform)
        {
            newCubeList.cubes.Add(new Cube(obj.localPosition.x, obj.localPosition.z));
        }

        string text=JsonUtility.ToJson(newCubeList);
        
        return text;
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    //Debug.Log("Success: ");
                    //Debug.Log(webRequest.downloadHandler.text);
                    string mapsString = "{ \"Maps\":"+ webRequest.downloadHandler.text+" }";
                    Maps = JsonUtility.FromJson<MapsJson>(mapsString);
                    UiManager.Instance.FillMapMenu();
                    //Debug.Log(Maps.Maps[0].cubes[0].x);
                    break;
            }
        }
    }


    public void SendCurrentMapToTheServer()
    {
        StartCoroutine(Upload());
    }

    IEnumerator Upload()
    {
        string postData = SaveObjectsToText();
        var test=JsonUtility.FromJson<CubeList>(postData);
        //Debug.Log(postData);
        using (UnityWebRequest www = UnityWebRequest.Put("https://localhost:7050/CubeList", postData))
        {
            www.method = "POST";
            www.SetRequestHeader("accept", "text/plain");
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                StartCoroutine(GetRequest("https://localhost:7050/CubeList"));
            }
        }
    }

    public void SpawnMapFromIndex(int index)
    {
        DeleteCubes();
        GenerateCubesFromList(Maps.Maps[index]);
    }

    private void Start()
    {
        StartCoroutine(GetRequest("https://localhost:7050/CubeList"));
    }
}
