using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectManager : MonoBehaviour
{
    private static GameObjectManager instance;
    public static GameObjectManager GetInstance()
    {
        return instance;
    }
    public GameObject[] gameObjects;
    public Material[] materials;
    public AudioSource[] audioSources;
    
    public void Awake()
    {
        instance = this;
    }

    public Transform GetWelcomePanel()
    {
        return gameObjects[0].transform;
    }
    public Transform GetUI01()
    {
        return gameObjects[1].transform;
    }

    public Transform GetUI02()
    {
        return gameObjects[2].transform;
    }

    public Transform GetUI03()
    {
        return gameObjects[9].transform;
    }
    public Transform GetUI04()
    {
        return gameObjects[10].transform;
    }
    public Transform GetUI05()
    {
        return gameObjects[11].transform;
    }
    public Transform GetCheckTriggerDiv()
    {
        return gameObjects[12].transform;
    }
    public Transform GetCheckStatusDiv()
    {
        return gameObjects[13].transform;
    }
    public Transform GetButton01()
    {
        return gameObjects[3].transform;
    }

    public Transform GetButton02()
    {
        return gameObjects[4].transform;
    }

    public Transform GetButton03()
    {
        return gameObjects[5].transform;
    }

    public Transform GetButton04()
    {
        return gameObjects[6].transform;
    }

    public Transform GetSelectorPanelsParent()
    {
        return gameObjects[7].transform;
    }
    public Transform GetModelsParent()
    {
        return gameObjects[8].transform;
    }

    public Transform GetQrcodesParent()
    {
        return gameObjects[14].transform;
    }
    public Transform GetPaysParent()
    {
        return gameObjects[15].transform;
    }
    public Transform GetOrdersParent()
    {
        return gameObjects[16].transform;
    }
    // public Transform GetDebugMenu()
    // {
    //     return FindObjectOfType<DebugMenu>().transform;
    //     // return gameObjects[17].transform;
    // }
    public Transform GetAdjustModelAssembly()
    {
        return GameObject.Find("DmCoreRoot/DebugMenu/Canvas/Panel/Scroll View/Viewport/Content/AdjustModel").transform;
    }
    public Material GetSelectorPanelsMatUnselected()
    {
        return materials[0];
    }
    public Material GetSelectorPanelsMatSelected()
    {
        return materials[1];
    }
    public AudioSource GetClickAudio()
    {
        return audioSources[0];
    }
}
