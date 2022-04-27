using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataApi;
using UnityEngine.Networking;


public class restApiDataView : MonoBehaviour
{
    public static restApiDataView _instance;
    public GameObject loadingIconPrefab;
    GameObject _spawnedLoadingIcon;
    public List<leaderboardModel.assetClass> leaderboadData;
    public leaderboardModel.assetClass[] leaderboardArray;
    public List<leaderboardModel.assetClass> dailyLeaderboadData;
    public leaderboardModel.assetClass[] dailyLeaderboardArray;
    [SerializeField] LeaderBoardControllerRestApi leaderboardControllerRestApi;
    private void Awake()
    {
        _instance = this;
        leaderboadData = new List<leaderboardModel.assetClass>();
        if (leaderboardControllerRestApi == null)
        {
            leaderboardControllerRestApi = GameObject.FindObjectOfType<LeaderBoardControllerRestApi>();
        }
    }
 
    // Start is called before the first frame update
    //callable function versions of the plain request for the leaderboard
    public async void DisplayLeaderboard()
    {
        

    }
    public async void DisplayDailyLeaderboard()
    {
       
    }
    public  void DisplayDailyLeaderboardRestApi()
    {
        getDailyLeaderboardFronRestApi();
        
    }
    public  void DisplayLeaderboardRestApi()
    {
        getLeaderboardFronRestApi();
      

    }
    public void getLeaderboardFronRestApi()
    {
        StartCoroutine(getLeaderboardFromApi("https://api.cryptofightclub.io/game/sdk/chicken/leaderboard/alltime", "all"));
    }
    public void getDailyLeaderboardFronRestApi()
    {
        StartCoroutine(getLeaderboardFromApi("https://api.cryptofightclub.io/game/sdk/chicken/leaderboard/daily","daily"));
    }
    public IEnumerator getLeaderboardFromApi(string url,string type)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url.ToString()))
        {
            request.method = UnityWebRequest.kHttpVerbGET;
            request.SetRequestHeader("Accept", "application/json");
            yield return request.SendWebRequest();
            if (request.error == null)
            {
                if (type == "daily")
                {
                    checkLeadboardDaily(request.downloadHandler.text);

                }
                else
                {
                    checkLeadboardAllTime(request.downloadHandler.text);

                }
                Debug.Log(request.downloadHandler.text);

            }
            else
            {
                Debug.Log("error in server");
            }
        }





    }
 
        public void checkLeadboardAllTime(string url)
    {

        string MatchData = fixJsonName(url);
        Debug.Log(MatchData);
        leaderboardArray = JsonUtil.fromJson<leaderboardModel.assetClass[]>(url);
        if (leaderboardArray != null)
        {
            if (leaderboardArray.Length > 0)
            {
                leaderboadData.Clear();
                for (int i = 0; i < leaderboardArray.Length; i++)
                {
                    leaderboadData.Add(leaderboardArray[i]);
                }
            }
        }

       // displayLeaderBoard("LEADERBOARD");
        if (leaderboardControllerRestApi != null)
            leaderboardControllerRestApi.UpDateLeaderBoardAllTimeRestApi(leaderboardArray, "LEADERBOARD");
    }
    public void checkLeadboardDaily(string url)
    {

        string MatchData = fixJsonName(url);
        Debug.Log(MatchData);
        dailyLeaderboardArray = JsonUtil.fromJson<leaderboardModel.assetClass[]>(url);
        if (dailyLeaderboardArray != null)
        {
            if (dailyLeaderboardArray.Length > 0)
            {
                dailyLeaderboadData.Clear();
                for (int i = 0; i < dailyLeaderboardArray.Length; i++)
                {
                    dailyLeaderboadData.Add(dailyLeaderboardArray[i]);
                }
            }
        }
        //displayLeaderBoard("Daily LEADERBOARD");
        if(leaderboardControllerRestApi!=null)
        leaderboardControllerRestApi.UpDateLeaderBoardDailyRestApi(dailyLeaderboardArray, "Daily LEADERBOARD");

    }
    public void displayLeaderBoard(string type)
    {
        _spawnedLoadingIcon = Instantiate(loadingIconPrefab, GameObject.Find("Canvas").transform);
        var query = new Dictionary<string, object>();
        query.Add("NumResults", 10);
        Destroy(_spawnedLoadingIcon);
        if(type== "LEADERBOARD")
        {
            UIManager._instance.SpawnLeaderboardRestApi(leaderboardArray, "LEADERBOARD");

        }
        else
        {
            UIManager._instance.SpawnLeaderboardRestApi(dailyLeaderboardArray, "Daily LEADERBOARD");
        }
    }
    public void setScore()
    {

    }
    public void getSessionCounter() 
    { 

    }
    public void setDataOnLeadModel(List<leaderboardModel.assetClass> mainModel, leaderboardObject localObject)
    {
        if (mainModel != null)
        {
            if (mainModel.Count > 0)
            {
                for (int i = 0;i < mainModel.Count; i++)
                {

                }
            }
        }
        
    }
    /*
    public LeaderboardUser (leaderboardModel.assetClass mainModel)
    {
        new LeaderboardUser user = new LeaderboardUser();
    }
    */
    string fixJsonName(string value)
    {
        value = value.Substring(0, value.Length - 1);
        value = value.Remove(0, 1);

        return value;
    }
    string fixJson(string value)
    {
        value = "{\"Items\":" + value + "}";
        return value;
    }
    public static class JsonUtil
    {

        /// <summary> Converts an object to a Json string </summary>.
        /// <param name="obj">object </param>
        public static string toJson<T>(T obj)
        {
            if (obj == null) return "null";

            if (typeof(T).GetInterface("IList") != null)
            {
                Pack<T> pack = new Pack<T>();
                pack.data = obj;
                string json = JsonUtility.ToJson(pack);
                return json.Substring(8, json.Length - 9);
            }

            return JsonUtility.ToJson(obj);
        }

        /// < summary > parse Json </summary >
        /// <typeparam name="T">type</typeparam>
        /// <param name="json">Json string </param>
        public static T fromJson<T>(string json)
        {
            if (json == "null" && typeof(T).IsClass) return default(T);

            if (typeof(T).GetInterface("IList") != null)
            {
                json = "{\"data\":{data}}".Replace("{data}", json);
                Pack<T> Pack = JsonUtility.FromJson<Pack<T>>(json);
                return Pack.data;
            }

            return JsonUtility.FromJson<T>(json);
        }

        /// < summary > inner packaging class </summary >
        private class Pack<T>
        {
            public T data;
        }

    }

}
