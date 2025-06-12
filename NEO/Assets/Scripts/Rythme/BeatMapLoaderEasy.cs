using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

public class BeatMapLoaderEasy : MonoBehaviour
{
    void Start()
    {
        TextAsset mapFile = Resources.Load<TextAsset>("Maps/EasyStandard");
        if (mapFile == null)
        {
            return;
        }

        string jsonString = mapFile.text;
        BeatSaberMap map = JsonConvert.DeserializeObject<BeatSaberMap>(jsonString);
    }
}
