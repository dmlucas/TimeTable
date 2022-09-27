using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.Rendering;
using Assets.Scripts.Tools;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.IO;

public enum AnimationType{
    Move,
    Rotate,
    Scale,
    Alpha,
    Color,
}
public enum AnimationContainType
{
    Self,
    Childs,
    SelfAndChilds
}
public enum SpaceType
{
    Self,
    World
}
public static class OtherExtension{

    public static void SetAlphaProperty(this Transform transform)
    {
        var meshRenderer = transform.GetComponent<MeshRenderer>();
        meshRenderer.material.SetMaterialRenderingModeEx(RenderingMode.Fade);
        UrpOperator.SetChangeWallTransparencyProperty(meshRenderer.material, true);
    }
    public static void SetAlphaValue(this Transform transform, float value)
    {
        var meshRenderer = transform.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            Color color = meshRenderer.material.color;
            meshRenderer.material.color = new Color(color.r, color.g, color.b, value);
        }
    }
    public static float GetAlphaValue(this Transform transform)
    {
        var meshRenderer = transform.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            Color color = meshRenderer.material.color;
            return color.a;
        }
        return 0;
    }

    public static void DoFadeAll(this Transform transform, float endValue, float duration, UnityAction unityAction = null, Material material = null, string noOpaqueTfNameContainStr = "-transparentTf", float noOpaqueTfNameMaxAlpha = 1)
    {
        var meshRenderer = transform.GetComponentsInChildren<MeshRenderer>();
        
        for (int i = 0; i < meshRenderer.Length; i++)
        {
            try
            {
                var mr = meshRenderer[i];
                if (mr == null)
                    continue;
                if (mr.transform.name.Contains("-dontDeal"))
                    continue;
                if (material != null)
                {
                    mr.material = material;
                }
                else
                {
                    mr.material.SetMaterialRenderingModeEx(endValue == 0 ? RenderingMode.Fade : RenderingMode.Opaque);
                }

                UrpOperator.SetChangeWallTransparencyProperty(mr.material, endValue == 0);
                //var endValue2 = endValue;
                if (mr.transform.name.Contains(noOpaqueTfNameContainStr))
                {
                    mr.material.SetMaterialRenderingModeEx(RenderingMode.Fade);
                    UrpOperator.SetChangeWallTransparencyProperty(mr.material, true);
                    endValue = endValue != 0 ? noOpaqueTfNameMaxAlpha : endValue;
                }
                bool isLast = i == meshRenderer.Length - 1;
                mr.material.DOFade(endValue, duration).OnComplete(() => {
                    if (isLast)
                    {
                        unityAction?.Invoke();
                    }
                });
            }
            catch { }
        }
    }
    public static OriginTransform AttachOriginTransform(this Transform transform)
    {
        if (transform.GetComponent<OriginTransform>() == null)
        {
            transform.gameObject.AddComponent<OriginTransform>();
            var ot = transform.GetComponent<OriginTransform>();
            ot.localPosition = transform.localPosition;
            ot.position = transform.position;
            ot.localScale = transform.localScale;
            ot.localRotation = transform.localRotation;
            ot.rotation = transform.rotation;
            ot.localEulerAngles = transform.localEulerAngles;
        }
        return transform.GetComponent<OriginTransform>();
    }
    public static void DelayCall(this Transform transform, float delayMs, UnityAction<Transform> unityAction)
    {
        int v = 0;
        DOTween.To(()=>v, x => v = x, 1, delayMs).OnComplete(() => {
            unityAction?.Invoke(transform);
        });
    }

    [System.Obsolete]
    public static IEnumerator ImageTexture(this Image image, string url)
    {
        //downloading
        WWW www = new WWW(url);
        //waiting download
        yield return www.isDone && www.error != null;
        Texture2D tex = new Texture2D(www.texture.width, www.texture.height);
        www.LoadImageIntoTexture(tex);
        image.sprite = Sprite.Create(tex, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.zero);
        yield return null;
    }

    static async Task LoadByFSAsync(string path, Image image)
    {
        byte[] result;
        using (FileStream SourceStream = File.Open(path, FileMode.Open))
        {
            result = new byte[SourceStream.Length];
            await SourceStream.ReadAsync(result, 0, (int)SourceStream.Length);
        }
        Texture2D tx = new Texture2D(512, 512);
        tx.LoadImage(result);
        //result = null;
        image.sprite = Sprite.Create(tx, new Rect(0, 0, tx.width, tx.height), new Vector2(0.5f, 0.5f));
    }

}

static class AnimationExtension {

    public static void DoAnimationDisplayHorizontal(this Transform transform, bool show, float animationDuration, UnityAction unityAction = null) {
        AnimationIClass.GetInstance().AnimationDisplayHorizontal(transform, show, animationDuration, unityAction);
    }
    public static void DoAnimationDisplayVertical(this Transform transform, bool show, float animationDuration, UnityAction unityAction = null)
    {
        AnimationIClass.GetInstance().AnimationDisplayVertical(transform, show, animationDuration, unityAction);
    }
    public static void DoAnimationDisplayScale(this Transform transform, bool show, Vector3 direction, float animationDuration, UnityAction unityAction = null)
    {
        AnimationIClass.GetInstance().AnimationDisplayScale(transform, show, direction, animationDuration, unityAction);
    }
    public static void DoAnimationDisplayColor(this Transform transform, Color color, float opacity, float animationDuration, UnityAction unityAction = null)
    {
        AnimationIClass.GetInstance().AnimationDisplayColor(transform, color, opacity, animationDuration, unityAction);
    }
    public static void DoAnimationRotate(this Transform transform, Vector3 endValue, float animationDuration, UnityAction unityAction = null)
    {
        AnimationIClass.GetInstance().AnimationRotate(transform, endValue, animationDuration, unityAction);
    }
    public static void DoAnimationScaleAlphaShow(this Transform transform, UnityAction unityAction = null,
        float minValue = 0.85f, float maxValue = 1.1f, float duration = 0.3f, bool isPunch = true)
    {
        AnimationIClass.GetInstance().AnimationScaleAlphaShow(transform, unityAction, minValue, maxValue, duration, isPunch);
    }
    public static void DoAnimationScaleAlphaHide(this Transform transform, UnityAction unityAction = null,
        float minValue = 0.85f, float maxValue = 1.1f, float duration = 0.3f, bool isPunch = true)
    {
        AnimationIClass.GetInstance().AnimationScaleAlphaHide(transform, unityAction, minValue, maxValue, duration, isPunch);
    }

    public static void DoAnimationMoveAlphaShow(this Transform transform, Vector3 moveOffset, UnityAction unityAction = null,
        float overflow = 0.1f, SpaceType spaceType = SpaceType.Self, float duration = 0.3f)
    {
        AnimationIClass.GetInstance().AnimationMoveAlphaShow(transform, moveOffset, unityAction,
        overflow, spaceType, duration);
    }
    public static void DoAnimationMoveAlphaHide(this Transform transform, Vector3 moveOffset, UnityAction unityAction = null,
        float overflow = 0.1f, SpaceType spaceType = SpaceType.Self, float duration = 0.3f)
    {
        AnimationIClass.GetInstance().AnimationMoveAlphaHide(transform, moveOffset, unityAction,
        overflow, spaceType, duration);
    }

    public static void DoAnimationRotateAlphaShow(this Transform transform, UnityAction unityAction = null, float duration = 0.3f)
    {
        AnimationIClass.GetInstance().AnimationRotateAlphaShow(transform, unityAction, duration);
    }
    public static void DoAnimationRotateAlphaHide(this Transform transform, Vector3 rotateValue, UnityAction unityAction = null, float duration = 0.3f)
    {
        AnimationIClass.GetInstance().AnimationRotateAlphaHide(transform, rotateValue, unityAction, duration);
    }


}
static class InteractionExtension
{
    public static void DoClickModelEvent(this Object @object, Camera camera, Vector3 inputPosition, UnityAction<RaycastHit> unityAction)
    {
        InteractionIClass.GetInstance().ClickModelEvent(camera, inputPosition, unityAction);
    }
    
    public static void DoClickModelSelfEvent(this Object @object, Camera camera, Vector3 inputPosition, UnityAction<RaycastHit> unityAction)
    {
        InteractionIClass.GetInstance().ClickModelSelfEvent(@object as Transform, camera, inputPosition, unityAction);
    }
    

}
public interface IAnimation
{
    /// <summary>
    /// Horizontal scale
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="show"></param>
    /// <param name="animationDuration"></param>
    public virtual void AnimationDisplayHorizontal(Transform transform, bool show, float animationDuration, UnityAction unityAction = null){}
    /// <summary>
    /// Vertical scale
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="show"></param>
    /// <param name="animationDuration"></param>
    public virtual void AnimationDisplayVertical(Transform transform, bool show, float animationDuration, UnityAction unityAction = null){}
    /// <summary>
    /// scale and move
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="show"></param>
    /// <param name="direction"></param>
    /// <param name="animationDuration"></param>
    public virtual void AnimationDisplayScale(Transform transform, bool show, Vector3 direction, float animationDuration, UnityAction unityAction = null){}
    /// <summary>
    /// change color
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="color"></param>
    /// <param name="opacity"></param>
    /// <param name="animationDuration"></param>
    public virtual void AnimationDisplayColor(Transform transform, Color color, float opacity, float animationDuration, UnityAction unityAction = null){}

    /// <summary>
    /// rotate
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="point"></param>
    /// <param name="axis"></param>
    /// <param name="angle"></param>
    public virtual void AnimationRotate(Transform transform, Vector3 endValue, float animationDuration, UnityAction unityAction){}

    // general animation
    public virtual void AnimationGeneric()
    {

    }
    public virtual void AnimationScaleAlphaShow(Transform transform, UnityAction unityAction, float minValue, float maxValue, float duration, bool isPunch){}
    public virtual void AnimationScaleAlphaHide(Transform transform, UnityAction unityAction, float minValue, float maxValue, float duration, bool isPunch){}

    public virtual void AnimationMoveAlphaShow(Transform transform, Vector3 moveOffset, UnityAction unityAction,
        float overflow, SpaceType spaceType, float duration){}
    public virtual void AnimationMoveAlphaHide(Transform transform, Vector3 moveOffset, UnityAction unityAction,
        float overflow, SpaceType spaceType, float duration){}

    public virtual void AnimationRotateAlphaShow(Transform transform, UnityAction unityAction = null, float duration = 0.3f){}
    public virtual void AnimationRotateAlphaHide(Transform transform, Vector3 rotateValue, UnityAction unityAction = null, float duration = 0.3f){}

}
public class AnimationIClass : IAnimation
{
    private static AnimationIClass instance;
    public static AnimationIClass GetInstance()
    {
        if (instance == null)
            instance = new AnimationIClass();
        return instance;
    }
    public void AnimationDisplayHorizontal(Transform transform, bool show, float animationDuration, UnityAction unityAction)
    {
        var tfs = transform.GetComponentsInChildren<Transform>();
        float opacity = show ? 1 : 0;
        foreach (var tf in tfs)
        {
            tf.GetComponent<Renderer>().material.DOFade(opacity, animationDuration);
        }
        transform.DOScaleX(opacity, animationDuration);
    }

    public void AnimationDisplayVertical(Transform transform, bool show, float animationDuration, UnityAction unityAction)
    {
        var tfs = transform.GetComponentsInChildren<Transform>();
        float opacity = show ? 1 : 0;
        foreach (var tf in tfs)
        {
            tf.GetComponent<Renderer>().material.DOFade(opacity, animationDuration);
        }
        transform.DOScaleY(opacity, animationDuration);
    }

    public void AnimationDisplayColor(Transform transform, Color color, float opacity, float animationDuration, UnityAction unityAction)
    {
        //transform.GetComponent<Renderer>().sharedMaterial.do;
        var rdrs = transform.GetComponentsInChildren<MeshRenderer>();
        // 0 opaque 1 cutout 2 fade 3 transparent
        foreach (var rdr in rdrs)
        {
            rdr.material.DOFloat(2, "_Mode", 0).OnComplete(() => {
                rdr.UpdateGIMaterials();
                rdr.material.DOColor(new Color(color.r, color.g, color.b, opacity), animationDuration);
                //rdr.material.DOFade(opacity, animationDuration);
            });
        }
    }

    public void AnimationDisplayScale(Transform transform, bool show, Vector3 direction, float animationDuration, UnityAction unityAction)
    {
        var targetPos = transform.position + direction;
        int factor = show ? 1 : 0;
        transform.DOScale(factor, animationDuration);
        transform.DOMove(targetPos, animationDuration);
    }

    public void AnimationRotate(Transform transform, Vector3 endValue, float animationDuration, UnityAction unityAction = null)
    {
        transform.DORotate(endValue, animationDuration);
    }

    // composite animtion->

    public void AnimationScaleAlphaShow(Transform transform, UnityAction unityAction = null,
        float minValue = 0.85f, float maxValue = 1.1f, float duration = 0.3f, bool isPunch = true)
    {
        var ot = transform.AttachOriginTransform();
        Vector3 scale = ot.localScale;
        transform.DOScale(scale * minValue, 0);
        transform.DoFadeAll(1, duration, () => {
           
        });
        if (!isPunch)
        {
            transform.DOScale(1f * scale, duration / 2).OnComplete(() => {
                unityAction?.Invoke();
            });
        }
        else
        {
            transform.DOScale(maxValue * scale, duration).OnComplete(() => {
                transform.DOScale(1f * scale, duration).OnComplete(() => {
                    unityAction?.Invoke();
                });
            });
        }

    }

    public virtual void AnimationScaleAlphaHide(Transform transform, UnityAction unityAction = null,
        float minValue = 0.85f, float maxValue = 1.1f, float duration = 0.3f, bool isPunch = true)
    {
        var ot = transform.AttachOriginTransform();
        Vector3 scale = ot.localScale;
        if (!isPunch)
        {
            transform.DOScale(scale * minValue, duration).OnComplete(() => { unityAction?.Invoke(); });
            transform.DoFadeAll(0, duration);
        }
        else
        {
            transform.DOScale(maxValue * scale, duration / 2).OnComplete(() =>
            {
                //transform.DOLocalRotate(new Vector3(0, 90, 45), .3f);
                transform.DOScale(scale * minValue, duration).OnComplete(() => { unityAction?.Invoke(); });
                transform.DoFadeAll(0, duration);
            });
        }
    }
    public void AnimationMoveAlphaShow(Transform transform, Vector3 moveOffset, UnityAction unityAction = null, 
        float overflow = 0.1f, SpaceType spaceType = SpaceType.Self, float duration = 0.3f)
    {
        var ot = transform.AttachOriginTransform();
        Vector3 targetMove = ot.localPosition + moveOffset * overflow;
        transform.DoFadeAll(1, duration);
        transform.DOMove(targetMove, duration).OnComplete(() => {
            transform.DOMove(ot.localPosition, duration).OnComplete(() => {
                unityAction?.Invoke();
            });
        });

    }
    public void AnimationMoveAlphaHide(Transform transform, Vector3 moveOffset, UnityAction unityAction = null,
        float overflow = 0.1f, SpaceType spaceType = SpaceType.Self, float duration = 0.3f)
    {
        var ot = transform.AttachOriginTransform();
        Vector3 targetMove = ot.localPosition - moveOffset * overflow;
        transform.DOMove(targetMove, duration).OnComplete(() => {
            transform.DoFadeAll(0, duration);
            Vector3 targetMove2 = ot.localPosition + moveOffset;
            transform.DOMove(targetMove2, duration).OnComplete(() => {
                unityAction?.Invoke();
            });
        });
    }

    public void AnimationRotateAlphaShow(Transform transform, UnityAction unityAction = null, float duration = 0.3f)
    {
        var ot = transform.AttachOriginTransform();
        Quaternion localRotation = ot.localRotation;
        transform.DOLocalRotateQuaternion(localRotation, duration).OnComplete(() => {
            unityAction?.Invoke();
        });
        transform.DoFadeAll(1, duration);
    }
    public void AnimationRotateAlphaHide(Transform transform, Vector3 rotateValue, UnityAction unityAction = null, float duration = 0.3f)
    {
        var ot = transform.AttachOriginTransform();
        var localEulerAngles = ot.localEulerAngles;
        transform.DOLocalRotate(localEulerAngles + rotateValue, duration).OnComplete(() => {
            unityAction?.Invoke();
        });
        transform.DoFadeAll(0, duration);
    }


}

public class AnimationIClassExtension : AnimationIClass
{
    public override void AnimationScaleAlphaHide(Transform transform, UnityAction unityAction = null,
        float minValue = 0.85f, float maxValue = 1.1f, float duration = 0.3f, bool isPunch = true)
    {
        Debug.Log(789);
        var ot = transform.AttachOriginTransform();
        Vector3 scale = ot.localScale;
        if (!isPunch)
        {
            transform.DOScale(scale * minValue, duration).OnComplete(() => { unityAction?.Invoke(); });
            transform.DoFadeAll(0, duration);
        }
        else
        {
            transform.DOScale(maxValue * scale, duration / 2).OnComplete(() =>
            {
                //transform.DOLocalRotate(new Vector3(0, 90, 45), .3f);
                transform.DOScale(scale * minValue, duration).OnComplete(() => { unityAction?.Invoke(); });
                transform.DoFadeAll(0, duration);
            });
        }
    }
}

public interface IInteraction
{
    /// <summary>
    /// click object to trigger event
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="inputPosition"></param>
    /// <param name="action"></param>
    public void ClickModelEvent(Camera camera, Vector3 inputPosition, UnityAction<RaycastHit> action);

}

public class InteractionIClass : IInteraction
{
    private static InteractionIClass instance;

    public static InteractionIClass GetInstance()
    {
        if (instance == null)
            instance = new InteractionIClass();
        return instance;
    }

    public void ClickModelEvent(Camera camera, Vector3 inputPosition, UnityAction<RaycastHit> unityAction)
    {
        Ray ray = camera.ScreenPointToRay(inputPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            unityAction?.Invoke(hit);
        }
    }

    public void ClickModelSelfEvent(Transform transform, Camera camera, Vector3 inputPosition, UnityAction<RaycastHit> unityAction)
    {
        if (transform.GetComponent<BoxCollider>() == null)
        {
            if ( transform.GetComponent<SphereCollider>() == null)
            {
                transform.gameObject.AddComponent<BoxCollider>();
            }
        }
        if (transform.GetComponent<BoxCollider>() != null && 
            transform.GetComponent<BoxCollider>().isTrigger == false)
            transform.GetComponent<BoxCollider>().isTrigger = true;

        if (transform.GetComponent<SphereCollider>() != null && 
            transform.GetComponent<SphereCollider>().isTrigger == false)
            transform.GetComponent<SphereCollider>().isTrigger = true;

        if (transform.GetComponent<Rigidbody>() == null)
            transform.gameObject.AddComponent<Rigidbody>();
        if (transform.GetComponent<Rigidbody>().useGravity)
            transform.GetComponent<Rigidbody>().useGravity = false;

        Ray ray = camera.ScreenPointToRay(inputPosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (transform.Equals(hit.transform))
            {
                unityAction?.Invoke(hit);
            }
        }
    }

    public void CheckRadioEvent()
    {

    }

}

public class AnimationManager : MonoBehaviour
{

    float animationDuration = .3f;

    public delegate void AnimationLoop(Transform transform);
    public AnimationLoop animationLoop1;
    public void DoAnimationLoopVoid(Transform transform)
    {
        float alpha = transform.GetComponent<Renderer>().material.color.a;
        transform.GetComponent<Renderer>().material.SetFloat("_Mode", 2f);
        transform.GetComponent<Renderer>().UpdateGIMaterials();
        //transform.GetComponent<Renderer>().material.
        float animDur = 1.0f;
        float alphaMin = .4f;
        float alphaMax = 1.0f;
        float targetAlpha = alpha == 1 ? alphaMin : alphaMax;
        transform.GetComponent<Renderer>().material.DOFade(targetAlpha, animDur)
            .OnComplete(() => { 
                DoAnimationLoopVoid(transform);
            });
    }
    private void Awake()
    {
        animationLoop1 += new AnimationLoop(DoAnimationLoopVoid);
    }
    List<AnimationLoop> animationLoops;
    void Start()
    {
        //animationLoop1(transform);
    }

    void Update()
    {
        return;
        if (Input.GetMouseButtonDown(0))
        {
            gameObject.DoClickModelEvent(camera: Camera.main, inputPosition: Input.mousePosition, unityAction: raycastHit => {
                Debug.Log(raycastHit.point);
            });
        }

        if (Time.frameCount == 60)
        {
            Transform tf = GameObjectManager.GetInstance().gameObjects[0].gameObject.transform;
            //tf.DoAnimationDisplayHorizontal(show: false, animationDuration: animationDuration);
            //tf.DoAnimationDisplayScale(show: false, direction: new Vector3(2, 2, 0), animationDuration: animationDuration);
            tf.DoAnimationDisplayColor(color: Color.red, opacity: 0, animationDuration: animationDuration);
            
        }
    }
}
