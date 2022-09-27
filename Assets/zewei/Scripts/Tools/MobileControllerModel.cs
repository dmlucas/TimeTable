using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MobileControllerModel : MonoBehaviour
{

    //float xSpeed = 100f;
    //float ySpeed = 100f;
    float x = 0f;
    float y = 0f;
    float Speed = 0.01f;

    /// <summary>  
    /// ��һ�ΰ��µ�λ��  
    /// </summary>  
    private Vector3 first = Vector3.zero;
    /// <summary>  
    /// ������קλ�ã��ڶ��ε�λ�ã�  
    /// </summary>  
    private Vector3 second = Vector3.zero;
    /// <summary>  
    /// ��ת�ĽǶ�  
    /// </summary>  
    //private float angle = 3f;
    /// <summary>  
    /// ��¼��ǰ�ķ���
    /// </summary>
    public GameObject model;

    Vector2 oldPosition1;
    Vector2 oldPosition2;

    void Update()
    {
        if (Input.anyKey)
        {
            //��ֻ��һ�δ���
            if (Input.touchCount == 1)
            {
                //�������ͣ�����
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    //�ƶ���ק
                    //��ȡx��
                    x = Input.GetAxis("Mouse X") * Speed;
                    //��ȡy��
                    y = Input.GetAxis("Mouse Y") * Speed;
                    model.transform.Translate(-x, y, 0);//*Time.deltaTime

                    //��ת
                    //��ȡx��
                    //x = Input.GetAxis("Mouse X") * xSpeed;
                    //��ȡy��
                    //y = Input.GetAxis("Mouse Y") * ySpeed;
                    //��ģ�ͽ�������������ת
                    //model.transform.Rotate(Vector3.up * x * Time.deltaTime, Space.World);
                    //model.transform.Rotate(Vector3.left * -y * Time.deltaTime, Space.World);
                }

            }
            //�����δ���
            if (Input.touchCount > 1)
            {
                //���δ������л���
                if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
                {
                    //��ȡ��һ���������δ�����λ��
                    Vector2 tempPosition1 = Input.GetTouch(0).position;
                    Vector2 tempPosition2 = Input.GetTouch(1).position;
                    //�Ŵ�
                    if (isEnlarge(oldPosition1, oldPosition2, tempPosition1, tempPosition2))
                    {
                        float oldScale = model.transform.localScale.x;
                        float newScale = oldScale * 1.025f;
                        model.transform.localScale = new Vector3(newScale, newScale, newScale);
                    }
                    else//��С
                    {
                        float oldScale = model.transform.localScale.x;
                        float newScale = oldScale / 1.025f;
                        model.transform.localScale = new Vector3(newScale, newScale, newScale);

                    }
                    //������һ�δ������λ�ã����ڶԱ�   
                    oldPosition1 = tempPosition1;
                    oldPosition2 = tempPosition2;
                }
            }
        }

    }
    /// <summary>
    /// �Ƚ����ε�λ�ã���С�������зŴ�����С
    /// </summary>
    /// <param name="oP1"></param>
    /// <param name="oP2"></param>
    /// <param name="nP1"></param>
    /// <param name="nP2"></param>
    /// <returns></returns>
    bool isEnlarge(Vector2 oP1, Vector2 oP2, Vector2 nP1, Vector2 nP2)
    {
        //����������һ�δ��������λ���뱾�δ��������λ�ü�����û�������   
        var leng1 = Mathf.Sqrt((oP1.x - oP2.x) * (oP1.x - oP2.x) + (oP1.y - oP2.y) * (oP1.y - oP2.y));
        var leng2 = Mathf.Sqrt((nP1.x - nP2.x) * (nP1.x - nP2.x) + (nP1.y - nP2.y) * (nP1.y - nP2.y));
        if (leng1 < leng2)
        {
            //�Ŵ�����   
            return true;
        }
        else
        {
            //��С����   
            return false;
        }
    }
}
