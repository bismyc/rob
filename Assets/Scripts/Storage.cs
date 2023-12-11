using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Storage
{
    const string typeKey = "type";
    const string posX = "posX";
    const string posY = "posY";
    const string posZ = "posZ";
    const string scaX = "scaX";
    const string scaY = "scaY";
    const string scaZ = "scaZ";

    const int numOfTypes = 2;


    public List<Tuple<int, Vector3, Vector3>> LoadSavedAssets()
    {
        List<Tuple<int, Vector3, Vector3>> savedAssets = new List<Tuple<int, Vector3, Vector3>>();


        for (int i = 0; i < numOfTypes; i++)
        {
            int num = PlayerPrefs.GetInt(typeKey + i.ToString(), 0);
            for (int j = 0; j < num; j++)
            {
                string key = typeKey + i.ToString() + j.ToString();
                Tuple<int, Vector3, Vector3> savedAssetData = new Tuple<int, Vector3, Vector3>(
                i,
                new Vector3(PlayerPrefs.GetFloat(key + posX), PlayerPrefs.GetFloat(key + posY), PlayerPrefs.GetFloat(key + posZ)),
                new Vector3(PlayerPrefs.GetFloat(key + scaX), PlayerPrefs.GetFloat(key + scaY), PlayerPrefs.GetFloat(key + scaZ))
                );
                savedAssets.Add(savedAssetData);
            }
        }


        return savedAssets;
    }

    public void SaveAsset(Tuple<int, Vector3, Vector3> asset)
    {
        string currTypeKey = typeKey + asset.Item1.ToString(); //type_0, type_1 etc
        int num = PlayerPrefs.GetInt(currTypeKey);  
        string key = currTypeKey + num.ToString(); //type_0_0

        PlayerPrefs.SetFloat(key + posX, asset.Item2.x); //type_0_0_posX, type_0_1_posX etc
        PlayerPrefs.SetFloat(key + posY, asset.Item2.y);
        PlayerPrefs.SetFloat(key + posZ, asset.Item2.z);

        PlayerPrefs.SetFloat(key + scaX, asset.Item3.x);
        PlayerPrefs.SetFloat(key + scaY, asset.Item3.y);
        PlayerPrefs.SetFloat(key + scaZ, asset.Item3.z);

        PlayerPrefs.SetInt(currTypeKey, num + 1);
    }

    public void ResetAllAssetData()
    {
        for (int i = 0; i < numOfTypes; i++)
        {
            int num = PlayerPrefs.GetInt(typeKey + i.ToString(), 0);
            for (int j = 0; j < num; j++)
            {
                string key = typeKey + i.ToString() + j.ToString();
                PlayerPrefs.SetFloat(key + posX, 0.0f);
                PlayerPrefs.SetFloat(key + posY, 0.0f);
                PlayerPrefs.SetFloat(key + posZ, 0.0f);

                PlayerPrefs.SetFloat(key + scaX, 0.0f);
                PlayerPrefs.SetFloat(key + scaY, 0.0f);
                PlayerPrefs.SetFloat(key + scaZ, 0.0f);
                
            }
            PlayerPrefs.SetInt(typeKey + i.ToString(), 0);
        }
    }
}
