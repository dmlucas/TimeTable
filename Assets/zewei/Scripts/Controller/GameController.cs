using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
//using TMPro;
using UnityEngine;
using Touch = UnityEngine.Touch;

public class GameController : MonoBehaviour
{

    // seri
    public class v2data
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 localScale;
        public v2data(Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            this.position = position;
            this.rotation = rotation;
            this.localScale = localScale;
        }
        public static v2data GetData(Transform transform)
        {
            v2data v2Data = new v2data(
                transform.position,
                transform.rotation,
                transform.localScale
                );
            return v2Data;
        }
        public static void SetData(ref v2data _v2Data, Transform transform)
        {
            _v2Data = v2data.GetData(transform);
        }
        public static void AssignV2Data(v2data v2Data, Transform transform)
        {
            transform.position = v2Data.position;
            transform.rotation = v2Data.rotation;
            transform.localScale = v2Data.localScale;
        }
    }
    [SerializeField]
    public v2data v2Data;

    public GameObjectManager manager;
    public Camera camera;
    public GameObject debugMenu;
    //TextMeshPro textMeshPro;
    void Start()
    {
        Init();
        ModelRecord();
    }

    void Init()
    {
        var v2 = transform.parent;
        if (PlayerPrefs.HasKey("v2sc"))
        {
            // v2 scale
            v2.localScale = Vector3.one * PlayerPrefs.GetFloat("v2sc");
        }
        if (v2Data == null)
            v2data.SetData(ref v2Data, v2);
        else
        {
            v2data.AssignV2Data(v2Data, v2);
        }
        if (camera == null)
            camera = Camera.main;
        if (camera == null)
            camera = GameObject.Find("DmCoreRoot/ARFoundationSession/ARFoundationCamera").GetComponent<Camera>();

        //TextMeshPro
        if (manager == null)
            manager = GameObjectManager.GetInstance();

        try
        {
            if (debugMenu == null)
                debugMenu = GameObject.Find("DmCoreRoot/DebugMenu");
            // register adjust parameter
            var adText = manager.GetAdjustModelAssembly().GetChild(0).GetComponent<UnityEngine.UI.Text>();
            var adBtn1 = manager.GetAdjustModelAssembly().GetChild(1).GetComponent<UnityEngine.UI.Button>();
            var adBtn2 = manager.GetAdjustModelAssembly().GetChild(2).GetComponent<UnityEngine.UI.Button>();
            var adBtn3 = manager.GetAdjustModelAssembly().GetChild(3).GetComponent<UnityEngine.UI.Button>();
            var adBtn4 = manager.GetAdjustModelAssembly().GetChild(4).GetComponent<UnityEngine.UI.Button>();
            var adBtn5 = manager.GetAdjustModelAssembly().GetChild(5).GetComponent<UnityEngine.UI.Button>();
            var adBtn6 = manager.GetAdjustModelAssembly().GetChild(6).GetComponent<UnityEngine.UI.Button>();

            var adBtn7 = manager.GetAdjustModelAssembly().GetChild(7).GetComponent<UnityEngine.UI.Button>();
            var adBtn8 = manager.GetAdjustModelAssembly().GetChild(8).GetComponent<UnityEngine.UI.Button>();

            // save to playerPrefs
        }
        catch { }
        
        manager.GetWelcomePanel().DoAnimationScaleAlphaHide(() =>
        {
            manager.GetWelcomePanel().gameObject.SetActive(true);
            manager.GetWelcomePanel().DelayCall(1f, transform => {
                transform.DoAnimationScaleAlphaShow(() => { });
            });
        });

        manager.GetUI01().DoAnimationScaleAlphaHide(() =>
        {
            manager.GetUI01().gameObject.SetActive(false);
        });
        manager.GetUI02().DoAnimationScaleAlphaHide(() =>
        {
            manager.GetUI02().gameObject.SetActive(false);
        });
        manager.GetUI03().DoAnimationScaleAlphaHide(() =>
        {
            manager.GetUI03().gameObject.SetActive(false);
        });
        manager.GetUI04().DoAnimationScaleAlphaHide(() =>
        {
            manager.GetUI04().gameObject.SetActive(false);
        });
        manager.GetUI05().DoAnimationScaleAlphaHide(() =>
        {
            manager.GetUI05().gameObject.SetActive(false);
        });
    }
    // slide menu param
    private Vector2 downVec;
    private bool isValidDown;

    private void FixedUpdate()
    {
        // if (Input.GetMouseButtonDown(0))
        // {
        //     isValidDown = true;
        //     downVec = Input.mousePosition;
        //     if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit))
        //     {
        //         if (raycastHit.transform.CompareTag("Model"))
        //         {
        //             isValidDown = false;
        //         }
        //     }
        // }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            downVec = Input.mousePosition;
            isValidDown = true;
            if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit))
            {
                if (raycastHit.transform.CompareTag("Model"))
                {
                    isValidDown = false;
                }
            }
            // enter ui1
            manager.GetWelcomePanel().DoClickModelSelfEvent(camera, Input.mousePosition, raycastHit => {
                manager.GetClickAudio().Play();
                
                manager.GetWelcomePanel().DoAnimationScaleAlphaHide(() => {
                    manager.GetWelcomePanel().gameObject.SetActive(false);
                    manager.GetUI01().gameObject.SetActive(true);
                    manager.GetUI01().DoAnimationScaleAlphaShow(() => {

                    });
                });
            });

            // enter ui2
            manager.GetButton02().DoClickModelSelfEvent(camera, Input.mousePosition, raycastHit => {
                // insert 
                ModelReset();
                manager.GetClickAudio().Play();
                
                manager.GetUI01().DoAnimationScaleAlphaHide(() => {
                    manager.GetUI01().gameObject.SetActive(false);
                    manager.GetUI02().gameObject.SetActive(true);
                    manager.GetUI02().DoAnimationScaleAlphaShow(() => {
                        
                    });
                });
            });

            // ui2 back to ui1
            manager.GetButton03().DoClickModelSelfEvent(camera, Input.mousePosition, raycastHit => {
                manager.GetClickAudio().Play();

                manager.GetUI02().DoAnimationScaleAlphaHide(() => {
                    manager.GetUI02().gameObject.SetActive(false);
                    manager.GetUI01().gameObject.SetActive(true);
                    manager.GetUI01().DoAnimationScaleAlphaShow(() => {

                    });
                });
            });

            // in ui2 -> show selected
            SelectModelAction();

            // ui2 -> ui3(order)
            manager.GetButton04().DoClickModelSelfEvent(camera, Input.mousePosition, raycastHit => {
                // output 
                print("enter order ");
                manager.GetClickAudio().Play();
                manager.GetUI02().DoAnimationScaleAlphaHide(() => {
                    manager.GetUI02().gameObject.SetActive(false);
                    manager.GetUI03().gameObject.SetActive(true);
                    manager.GetUI03().DoAnimationScaleAlphaShow(() => {

                    });

                });
            });

            // ui3 -> ui4(pay success)
            transform.DoClickModelEvent(camera, Input.mousePosition, raycastHit => {
                if (raycastHit.transform.name == "Tips" &&
                raycastHit.transform.parent.name == "ui-03")
                {
                    manager.GetClickAudio().Play();

                    manager.GetUI03().DoAnimationScaleAlphaHide(() => {
                        manager.GetUI03().gameObject.SetActive(false);
                        manager.GetUI04().gameObject.SetActive(true);
                        manager.GetUI04().DoAnimationScaleAlphaShow(() => {

                        });

                    });
                }
            });

            // ui4->ui5 (order success)
            transform.DoClickModelEvent(camera, Input.mousePosition, raycastHit => {
                //print(raycastHit.transform.root.name);
                try
                {
                    if (raycastHit.transform.parent.parent.name == "ui-04")
                    {
                        manager.GetClickAudio().Play();

                        manager.GetUI04().DoAnimationScaleAlphaHide(() =>
                        {
                            manager.GetUI04().gameObject.SetActive(false);
                            manager.GetUI05().gameObject.SetActive(true);
                            //DoAnimationMoveAlphaShow
                            manager.GetUI05().DoAnimationScaleAlphaShow(() =>
                            {

                            });

                        });
                    }
                }
                catch { }
            });

            // check action
            for (int i = 0; i < manager.GetCheckTriggerDiv().childCount; i++)
            {
                int idx = i;
                var child = manager.GetCheckTriggerDiv().GetChild(idx);
                child.DoClickModelSelfEvent(camera, Input.mousePosition, raycastHit => {

                    if (manager.GetCheckStatusDiv().GetChild(idx).gameObject.activeSelf)
                        return;

                    for (int j = 0; j < manager.GetCheckStatusDiv().childCount; j++)
                    {
                        int jdx = j;
                        var childJ = manager.GetCheckStatusDiv().GetChild(jdx);
                        if (childJ.gameObject.activeSelf)
                        {
                            childJ.DoAnimationScaleAlphaHide(() => {
                                childJ.gameObject.SetActive(false);
                            }, 0, 1.1f, 0.3f, false);
                            manager.GetCheckStatusDiv().GetChild(idx).DoAnimationScaleAlphaHide(() => {
                                manager.GetCheckStatusDiv().GetChild(idx).gameObject.SetActive(true);
                                manager.GetCheckStatusDiv().GetChild(idx).DoAnimationScaleAlphaShow(null, 0, 1.1f, 0.3f);
                            }, 0, 1.1f, 0, false);
                        }
                    }

                }) ;
            }
            // order page click to welcome page
            for (int i = 0; i < manager.GetOrdersParent().childCount; i++)
            {
                var child = manager.GetOrdersParent().GetChild(i);
                child.DoClickModelSelfEvent(camera, Input.mousePosition, _=> {
                    manager.GetClickAudio().Play();
                    Init();
                });
            }
        }

        // 滑动选择菜单
        //if (Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved))
        //    print("slider " + Input.GetAxis("Mouse X")) ;
        if (manager.GetUI02().gameObject.activeSelf)
        {
            if (Input.GetMouseButtonUp(0))
            {
                
                float dx = Input.mousePosition.x - downVec.x;
                if (Mathf.Abs(dx) > 5 && isValidDown)
                {
                    if (!selectedActionLock)
                    {
                        outIdx += -(int)Mathf.Sign(dx);
                        if (outIdx < 0)
                        {
                            outIdx = manager.GetSelectorPanelsParent().childCount - 1;
                        }
                        if (outIdx > manager.GetSelectorPanelsParent().childCount - 1)
                        {
                            outIdx = 0;
                        }
                        SwitchMenu(manager.GetSelectorPanelsParent().GetChild(outIdx), outIdx);
                    }
                }
            }
#if  UNITY_IOS || UNITY_ANDROID
            {
                // if (Input.touchSupported)
                // {
                //     if (Input.touchCount > 0)
                //     {
                //         Touch touch = Input.touches[0];
                //         if (touch.phase == TouchPhase.Moved)
                //         {
                //             if (Mathf.Abs(touch.deltaPosition.x) > 3)
                //             {
                //                 if (!selectedActionLock)
                //                 {
                //                     outIdx += -(int)Mathf.Sign(touch.deltaPosition.x);
                //                     if (outIdx < 0)
                //                     {
                //                         outIdx = manager.GetSelectorPanelsParent().childCount - 1;
                //                     }
                //                     if (outIdx > manager.GetSelectorPanelsParent().childCount - 1)
                //                     {
                //                         outIdx = 0;
                //                     }
                //                     SwitchMenu(manager.GetSelectorPanelsParent().GetChild(outIdx), outIdx);
                //                 }
                //              
                //             }
                //         }
                //     }
                // }
            }
#elif UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                float dy = Input.GetAxis("Mouse X");
                if (Mathf.Abs(dy) != 0)
                {
                    if (!selectedActionLock)
                    {
                        outIdx += -(int)Mathf.Sign(dy);
                        if (outIdx < 0)
                        {
                            outIdx = manager.GetSelectorPanelsParent().childCount - 1;
                        }

                        if (outIdx > manager.GetSelectorPanelsParent().childCount - 1)
                        {
                            outIdx = 0;
                        }
                        SwitchMenu(manager.GetSelectorPanelsParent().GetChild(outIdx), outIdx);
                    }
                }
            }   
#endif
    
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (!selectedActionLock)
                {
                    outIdx += -(int)Mathf.Sign(-1);
                    if (outIdx < 0)
                    {
                        outIdx = manager.GetSelectorPanelsParent().childCount - 1;
                    }

                    if (outIdx > manager.GetSelectorPanelsParent().childCount - 1)
                    {
                        outIdx = 0;
                    }

                    SwitchMenu(manager.GetSelectorPanelsParent().GetChild(outIdx), outIdx);
                }
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                if (!selectedActionLock)
                {
                    outIdx += -(int)Mathf.Sign(1);
                    if (outIdx < 0)
                    {
                        outIdx = manager.GetSelectorPanelsParent().childCount - 1;
                    }

                    if (outIdx > manager.GetSelectorPanelsParent().childCount - 1)
                    {
                        outIdx = 0;
                    }

                    SwitchMenu(manager.GetSelectorPanelsParent().GetChild(outIdx), outIdx);
                }
            }
            
        }
    }


    Transform[] selectorPanels;
    Transform[] selectorRelatedModels;
    Transform selectedObject;
    bool selectedActionLock = false;

    private int outIdx = 1;
    // do action when selecting
    void SelectModelAction()
    {
        if (selectedActionLock)
            return;
        if (selectorPanels == null)
        {
            int spCount = manager.GetSelectorPanelsParent().childCount;
            selectorPanels = new Transform[spCount];
            selectorRelatedModels = new Transform[spCount];
            for (int i = 0; i < spCount; i++)
            {
                selectorPanels[i] = manager.GetSelectorPanelsParent().GetChild(i);
                selectorPanels[i].AttachOriginTransform();
                selectorRelatedModels[i] = manager.GetModelsParent().GetChild(i);
            }
            // set selected
            int middle = (int)spCount / 2;
            var ot = selectorPanels[middle].GetComponent<OriginTransform>();
            selectorPanels[middle].DOScale(ot.localScale * 1.2f, 0);
            // set center object
            selectedObject = selectorPanels[middle];
        }

        for (int i = 0; i < manager.GetSelectorPanelsParent().childCount; i++)
        {
            var child = manager.GetSelectorPanelsParent().GetChild(i);
            int idx = i;
            child.DoClickModelSelfEvent(camera, Input.mousePosition, raycastHit => {
                
                SwitchMenu(raycastHit.transform, idx);
            });
        }
    }

    void SwitchMenu(Transform raycastHit, int idx)
    {
        if (manager.GetModelsParent().GetChild(idx).gameObject.activeSelf)
            return;
        manager.GetClickAudio().Play();
        outIdx = idx;
        // 显示对应的菜单面片
        for (int n = 0; n < manager.GetQrcodesParent().childCount; n++)
        {
            manager.GetQrcodesParent().GetChild(n).gameObject.SetActive(idx == n);
            manager.GetPaysParent().GetChild(n).gameObject.SetActive(idx == n);
            manager.GetOrdersParent().GetChild(n).gameObject.SetActive(idx == n);
        }
        // lock
        selectedActionLock = true;
        var ot = raycastHit.transform.GetComponent<OriginTransform>();
        raycastHit.transform.DOScale(ot.localScale * 1.2f, .3f);

        bool isLeftObject = raycastHit.transform.position.x < selectedObject.position.x;
        int signDirect = isLeftObject ? -1 : 1;

        Transform rightObject = null;
        for (int m = 0; m < manager.GetSelectorPanelsParent().childCount; m++)
        {
            if (!manager.GetSelectorPanelsParent().GetChild(m).Equals(raycastHit.transform)
            && !manager.GetSelectorPanelsParent().GetChild(m).Equals(selectedObject))
            {
                rightObject = manager.GetSelectorPanelsParent().GetChild(m);
            }
            else
            {
                //var ot1 = manager.GetSelectorPanelsParent().GetChild(m).GetComponent<OriginTransform>();
                //manager.GetSelectorPanelsParent().GetChild(m).DOScale(ot1.localScale, .3f);
            }
        }

        // change mat status
        raycastHit.transform.GetComponent<MeshRenderer>().material = manager.GetSelectorPanelsMatSelected();
        selectedObject.GetComponent<MeshRenderer>().material = manager.GetSelectorPanelsMatUnselected();
        rightObject.GetComponent<MeshRenderer>().material = manager.GetSelectorPanelsMatUnselected();

        var rightPos = rightObject.position;
        var leftPos = raycastHit.transform.position;
        var centerPos = selectedObject.position;

        var rightRot = rightObject.rotation;
        var leftRot = raycastHit.transform.rotation;
        var centerRot = selectedObject.rotation;

        // 1
        raycastHit.transform.DOMove(centerPos, .3f);
        raycastHit.transform.DORotateQuaternion(centerRot, .3f);
        // 2
        selectedObject.DOMove(rightPos, .3f);
        selectedObject.DORotateQuaternion(rightRot, .3f);

        // scale 1
        selectedObject.DOScale(selectedObject.GetComponent<OriginTransform>().localScale, .3f);
        rightObject.DOScale(rightObject.GetComponent<OriginTransform>().localScale, .3f);
        rightObject.DORotateQuaternion(leftRot, .3f);

        // update selectedObject
        selectedObject = raycastHit.transform;
        rightObject.DoFadeAll(0, .3f);
        rightObject.DOBlendableLocalMoveBy(signDirect * Vector3.left * 0.1f, .3f)
        .OnComplete(delegate {
            rightObject.position = leftPos;
            rightObject.DOBlendableLocalMoveBy(-signDirect * Vector3.left * 0.1f, 0)
            .OnComplete(delegate
            {
                rightObject.gameObject.SetActive(true);
                rightObject.DoFadeAll(1, .3f);
                rightObject.DOBlendableLocalMoveBy(signDirect * Vector3.left * 0.1f, .3f)
                .OnComplete(delegate {
                    if (selectedActionLock)
                        selectedActionLock = false;
                });
            });
        });

        for (int j = 0; j < manager.GetModelsParent().childCount; j++)
        {
            int jdx = j;
            var childJ = manager.GetModelsParent().GetChild(j);
            if (idx == jdx)
            {
                continue;
            }
            var child2 = manager.GetSelectorPanelsParent().GetChild(j);
            var ot2 = child2.GetComponent<OriginTransform>();
            child2.DOScale(ot2.localScale, .3f);

            childJ.DoFadeAll(0, .3f, () => {
                childJ.gameObject.SetActive(false);
                manager.GetModelsParent().GetChild(idx).gameObject.SetActive(true);
                // reset position and ratation
                ModelReset();
                // show
                manager.GetModelsParent().GetChild(idx).transform.DoFadeAll(1, .3f, () =>
                {
                    if (selectedActionLock)
                        selectedActionLock = false;
                });
            });
        }
    }

    class ModelData
    {
        public Vector3 localPostion;
        public Quaternion localRotation;
        public ModelData(Transform transform)
        {
            this.localPostion = transform.localPosition;
            this.localRotation = transform.localRotation;
        }

        public static void Resume(Transform transform, ModelData modelData)
        {
            transform.GetComponent<Rigidbody>().isKinematic = true;
            transform.localPosition = modelData.localPostion;
            transform.localRotation = modelData.localRotation;
            transform.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    private List<ModelData> modelDatas = new List<ModelData>();
    // record pos and rot
    public void ModelRecord()
    {
        
        for (int i = 0; i < manager.GetModelsParent().childCount; i++)
        {
            var tf = manager.GetModelsParent().GetChild(i);
            for (int j = 0; j < tf.childCount; j++)
            {
                if (tf.GetChild(j).name.StartsWith("Model0"))
                {
                    modelDatas.Add(new ModelData(tf.GetChild(j)));
                }
            }
        }
    }
    public void ModelReset()
    {
        int ct = 0;
        for (int i = 0; i < manager.GetModelsParent().childCount; i++)
        {
            var tf = manager.GetModelsParent().GetChild(i);
            for (int j = 0; j < tf.childCount; j++)
            {
                if (tf.GetChild(j).name.StartsWith("Model0"))
                {
                    ModelData.Resume(tf.GetChild(j), modelDatas[ct]);
                    ct++;
                }
            }
            
        }
    }
}
