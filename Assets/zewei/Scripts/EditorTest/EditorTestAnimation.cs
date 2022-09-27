using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.Internal;
using UnityEngine.Networking;
using LitJson;
using System.Text;
using Assets.Scripts.Tools;
using System;

//[ExecuteAlways]
public class EditorTestAnimation : MonoBehaviour
{
    public static IEnumerator SendWebRequest<T>(string url, T dataClass, UnityAction<string> unityAction)
    {
        // 接口地址
        // string url = "http://ccs.websol.cn";
        // post数据 通过序列化获得字符串
        string postData = JsonMapper.ToJson(dataClass);
        // Post网络请求
        using (UnityWebRequest request = UnityWebRequest.Post(url, UnityWebRequest.kHttpVerbPOST))
        {
            byte[] postBytes = Encoding.UTF8.GetBytes(postData);
            request.uploadHandler = new UploadHandlerRaw(postBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.ConnectionError
                && request.result != UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.downloadHandler.text);
                unityAction.Invoke(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"发起网络请求失败： 确认过闸接口 -{request.error}");
            }
        }
    }

    [Header("welcome")]
    public Transform m_panelWelcome;

    [Header("call update once")]
    public bool isExecute = false;

    /// <summar
    // show welcome panel
    public void PanelWelcomeShow(UnityAction unityAction=null)
    {
        Vector3 scale = m_panelWelcome.localScale;
        m_panelWelcome.DOScale(scale * 0.8f, 0);
        m_panelWelcome.GetComponent<MeshRenderer>().material.DOFade(0, 0)
            .OnComplete(delegate {
                m_panelWelcome.GetComponent<MeshRenderer>().material.DOFade(1f, .3f);
                m_panelWelcome.DOScale(1.2f * scale, .3f).OnComplete(() => {
                    m_panelWelcome.DOScale(1f * scale, .3f).OnComplete(() => {
                        unityAction?.Invoke();
                    });
                });
            });
    }
    
    public void PanelWelcomeHide(UnityAction unityAction = null)
    {
        Vector3 scale = m_panelWelcome.localScale;
        m_panelWelcome.DOScale(1.2f * scale, .3f).OnComplete(() => {
            m_panelWelcome.DOLocalRotate(new Vector3(0, 90, 45), .3f);
            m_panelWelcome.GetComponent<MeshRenderer>().material.DOFade(0, .3f)
                .OnComplete(delegate {
                    unityAction?.Invoke();
                });
        });

    }

    public Material material;

    public new Camera camera;
    
    void Start()
    {
        //StartCoroutine(SendWebRequest("http://ccs.websol.cn"));
    }

    void Update()
    {
        //transform.LookAt(camera.transform);
        //transform.Rotate(Vector3.up * 180, Space.World);
        if (Input.GetMouseButtonDown(0))
        {



            return;
            transform.DoClickModelSelfEvent(Camera.main, Input.mousePosition, raycastHit => {
                transform.DoAnimationRotateAlphaHide(Vector3.right  * 90, () => {
                    transform.DelayCall(.3f, tf => {
                        transform.DoAnimationRotateAlphaShow(() => { });
                    });
                });
            });

            return;
            transform.DoClickModelEvent(Camera.main, Input.mousePosition, _ => {
                print(_.transform.name);
                _.transform.root.DOBlendableMoveBy(Vector3.right * 3, .5f);
                _.transform.root.DoFadeAll(0, .5f, null, material);
            });
        }

        if (isExecute)
        {
            isExecute = false;
            PanelWelcomeShow(() => {
                PanelWelcomeHide();
            });
        }

    }

    class Affirm { 
        
    }

}
