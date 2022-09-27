using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Scripts.Tools
{
    // set material rendering mode
    public enum RenderingMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent
    }

    public interface IUtils
    {
        void SetMaterialRenderingMode(Material material, RenderingMode renderingMode);
    }

    public static class UtilsExtension
    {
        public static void SetMaterialRenderingModeEx(this Material material, RenderingMode renderingMode)
        {
            Utils.GetInstance().SetMaterialRenderingMode(material, renderingMode);
        }
    }

    public class Utils : MonoBehaviour, IUtils
    {
        private static Utils instance;
        public static Utils GetInstance()
        {
            if (instance == null)
                instance = new Utils();
            return instance;
        }

        public Material mat;

        public void SetMaterialRenderingMode(Material material, RenderingMode renderingMode)
        {
            switch (renderingMode)
            {
                case RenderingMode.Opaque:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
                case RenderingMode.Cutout:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 2450;
                    break;
                case RenderingMode.Fade:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
                case RenderingMode.Transparent:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
            }
        }

        //public static T ConverType2<T>(object value, T defaultValue)
        //{
        //    value = (object)defaultValue as string;
        //    return defaultValue;
        //}

        public static T ConvertType<T>(object value, T defaultValue)
        {
            try
            {
                return (T)ConvertToT<T>(value, defaultValue);
            }
            catch
            {
                return default(T);
            }
        }
        private static object ConvertToT<T>(object myvalue, T defaultValue)
        {
            TypeCode typeCode = System.Type.GetTypeCode(typeof(T));
            if (myvalue != null)
            {
                string value = Convert.ToString(myvalue);
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        bool flag = false;
                        if (bool.TryParse(value, out flag))
                        {
                            return flag;
                        }
                        break;
                    case TypeCode.Char:
                        char c;
                        if (Char.TryParse(value, out c))
                        {
                            return c;
                        }
                        break;
                    case TypeCode.SByte:
                        sbyte s = 0;
                        if (SByte.TryParse(value, out s))
                        {
                            return s;
                        }
                        break;
                    case TypeCode.Byte:
                        byte b = 0;
                        if (Byte.TryParse(value, out b))
                        {
                            return b;
                        }
                        break;
                    case TypeCode.Int16:
                        Int16 i16 = 0;
                        if (Int16.TryParse(value, out i16))
                        {
                            return i16;
                        }
                        break;
                    case TypeCode.UInt16:
                        UInt16 ui16 = 0;
                        if (UInt16.TryParse(value, out ui16))
                            return ui16;
                        break;
                    case TypeCode.Int32:
                        int i = 0;
                        if (Int32.TryParse(value, out i))
                        {
                            return i;
                        }
                        break;
                    case TypeCode.UInt32:
                        UInt32 ui32 = 0;
                        if (UInt32.TryParse(value, out ui32))
                        {
                            return ui32;
                        }
                        break;
                    case TypeCode.Int64:
                        Int64 i64 = 0;
                        if (Int64.TryParse(value, out i64))
                        {
                            return i64;
                        }
                        break;
                    case TypeCode.UInt64:
                        UInt64 ui64 = 0;
                        if (UInt64.TryParse(value, out ui64))
                            return ui64;
                        break;
                    case TypeCode.Single:
                        Single single = 0;
                        if (Single.TryParse(value, out single))
                        {
                            return single;
                        }
                        break;
                    case TypeCode.Double:
                        double d = 0;
                        if (Double.TryParse(value, out d))
                        {
                            return d;
                        }
                        break;
                    case TypeCode.Decimal:
                        decimal de = 0;
                        if (Decimal.TryParse(value, out de))
                        {
                            return de;
                        }
                        break;
                    case TypeCode.DateTime:
                        DateTime dt;
                        if (DateTime.TryParse(value, out dt))
                        {
                            return dt;
                        }
                        break;
                    case TypeCode.String:
                        if (!string.IsNullOrEmpty(value))
                        {
                            return value.ToString();
                        }
                        break;
                }
            }
            return defaultValue;
        }

    }
}
