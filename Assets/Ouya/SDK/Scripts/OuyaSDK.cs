// Unity JNI reference: http://docs.unity3d.com/Documentation/ScriptReference/AndroidJNI.html
// JNI Spec: http://docs.oracle.com/javase/1.5.0/docs/guide/jni/spec/jniTOC.html
// Android Plugins: http://docs.unity3d.com/Documentation/Manual/Plugins.html#AndroidPlugins 
// Unity Android Plugin Guide: http://docs.unity3d.com/Documentation/Manual/PluginsForAndroid.html

using System;
using System.Collections.Generic;
using UnityEngine;

public static class OuyaSDK
{
    /// <summary>
    /// The developer ID assigned
    /// </summary>
    static private string m_developerId = string.Empty;

     /*
     * Before this app will run, you must define some purchasable items on the developer website. Once
     * you have defined those items, put their SKUs in the List below.
     *
     * The SKUs below are those in our developer account. You should change them.
     */
    public static List<Purchasable> PRODUCT_IDENTIFIER_LIST = new List<Purchasable>() {
        new Purchasable("long_sword"),
        new Purchasable("sharp_axe"),
        new Purchasable("__DECLINED__THIS_PURCHASE")
    };

    #region Key Codes

    public const int KEYCODE_BUTTON_A = 96;
    public const int KEYCODE_BUTTON_B = 97;
    public const int KEYCODE_BUTTON_X = 99;
    public const int KEYCODE_BUTTON_Y = 100;
    public const int KEYCODE_BUTTON_L1 = 102;
    public const int KEYCODE_BUTTON_L2 = 104;
    public const int KEYCODE_BUTTON_R1 = 103;
    public const int KEYCODE_BUTTON_R2 = 105;
    public const int KEYCODE_BUTTON_L3 = 106;
    public const int KEYCODE_BUTTON_R3 = 107;
    public const int KEYCODE_BUTTON_START = 108;
    public const int AXIS_X = 0;
    public const int AXIS_Y = 1;
    public const int AXIS_Z = 11;
    public const int AXIS_RZ = 14;
    public const int KEYCODE_DPAD_UP = 19;
    public const int KEYCODE_DPAD_DOWN = 20;
    public const int KEYCODE_DPAD_LEFT = 21;
    public const int KEYCODE_DPAD_RIGHT = 22;
    public const int KEYCODE_DPAD_CENTER = 23;

    #endregion

    #region OUYA controller

    public const int BUTTON_O = KEYCODE_BUTTON_A;
    public const int BUTTON_U = KEYCODE_BUTTON_X;
    public const int BUTTON_Y = KEYCODE_BUTTON_Y;
    public const int BUTTON_A = KEYCODE_BUTTON_B;

    public const int BUTTON_LB = KEYCODE_BUTTON_L1;
    public const int BUTTON_LT = KEYCODE_BUTTON_L2;
    public const int BUTTON_RB = KEYCODE_BUTTON_R1;
    public const int BUTTON_RT = KEYCODE_BUTTON_R2;
    public const int BUTTON_L3 = KEYCODE_BUTTON_L3;
    public const int BUTTON_R3 = KEYCODE_BUTTON_R3;

    public const int BUTTON_SYSTEM = KEYCODE_BUTTON_START;

    public const int AXIS_LSTICK_X = AXIS_X;
    public const int AXIS_LSTICK_Y = AXIS_Y;
    public const int AXIS_RSTICK_X = AXIS_Z;
    public const int AXIS_RSTICK_Y = AXIS_RZ;

    public const int BUTTON_DPAD_UP = KEYCODE_DPAD_UP;
    public const int BUTTON_DPAD_RIGHT = KEYCODE_DPAD_RIGHT;
    public const int BUTTON_DPAD_DOWN = KEYCODE_DPAD_DOWN;
    public const int BUTTON_DPAD_LEFT = KEYCODE_DPAD_LEFT;
    public const int BUTTON_DPAD_CENTER = KEYCODE_DPAD_CENTER;

    public enum InputAction
    {
        None,
        GenericMotionEvent,
        KeyDown,
        KeyUp,
        TouchEvent,
        TrackballEvent
    }

    public enum KeyEnum
    {
        NONE = -1,
        BUTTON_O = OuyaSDK.BUTTON_O,
        BUTTON_U = OuyaSDK.BUTTON_U,
        BUTTON_Y = OuyaSDK.KEYCODE_BUTTON_Y,
        BUTTON_A = OuyaSDK.BUTTON_A,
        BUTTON_LB = OuyaSDK.BUTTON_LB,
        BUTTON_LT = OuyaSDK.KEYCODE_BUTTON_L2,
        BUTTON_RB = OuyaSDK.BUTTON_RB,
        BUTTON_RT = OuyaSDK.BUTTON_RT,
        BUTTON_L3 = OuyaSDK.BUTTON_L3,
        BUTTON_R3 = OuyaSDK.BUTTON_R3,
        BUTTON_SYSTEM = OuyaSDK.BUTTON_SYSTEM,
        AXIS_LSTICK_X = OuyaSDK.AXIS_LSTICK_X,
        AXIS_LSTICK_Y = OuyaSDK.AXIS_LSTICK_Y,
        AXIS_RSTICK_X = OuyaSDK.AXIS_RSTICK_X,
        AXIS_RSTICK_Y = OuyaSDK.AXIS_RSTICK_Y,
        BUTTON_DPAD_UP = OuyaSDK.BUTTON_DPAD_UP,
        BUTTON_DPAD_RIGHT = OuyaSDK.BUTTON_DPAD_RIGHT,
        BUTTON_DPAD_DOWN = OuyaSDK.BUTTON_DPAD_DOWN,
        BUTTON_DPAD_LEFT = OuyaSDK.BUTTON_DPAD_LEFT,
        BUTTON_DPAD_CENTER = OuyaSDK.BUTTON_DPAD_CENTER
    }

    public enum AxisEnum
    {
        NONE = -1,
        AXIS_LSTICK_X,
        AXIS_LSTICK_Y,
        AXIS_RSTICK_X,
        AXIS_RSTICK_Y,
        AXIS_LTRIGGER,
        AXIS_RTRIGGER,
    }

    public enum OuyaPlayer
    {
        player1=1,
        player2=2,
        player3=3,
        player4=4,
        none=0,
    }

    #endregion

    /// <summary>
    /// Listener for the product list
    /// </summary>
    private static OuyaSDK.CancelIgnoringIapResponseListener<List<OuyaSDK.Product>> m_productListListener = null;
    public static OuyaSDK.CancelIgnoringIapResponseListener<List<OuyaSDK.Product>> getProductListListener()
    {
        return m_productListListener;
    }
    public static void registerProductListListener(OuyaSDK.CancelIgnoringIapResponseListener<List<OuyaSDK.Product>> productListListener)
    {
        m_productListListener = productListListener;
    }

    /// <summary>
    /// Listener for request purchase responses
    /// </summary>
    private static OuyaSDK.CancelIgnoringIapResponseListener<OuyaSDK.Product> m_requestPurchaseListener = null;
    public static OuyaSDK.CancelIgnoringIapResponseListener<OuyaSDK.Product> getRequestPurchaseListener()
    {
        return m_requestPurchaseListener;
    }
    public static void registerRequestPurchaseListener(OuyaSDK.CancelIgnoringIapResponseListener<OuyaSDK.Product> requestPurchaseListener)
    {
        m_requestPurchaseListener = requestPurchaseListener;
    }

    /// <summary>
    /// Listener for the input buttons
    /// </summary>
    private static OuyaSDK.InputButtonListener<OuyaSDK.InputButtonEvent> m_inputButtonListener = null;
    public static OuyaSDK.InputButtonListener<OuyaSDK.InputButtonEvent> getInputButtonListener()
    {
        return m_inputButtonListener;
    }
    public static void registerInputButtonListener(OuyaSDK.InputButtonListener<OuyaSDK.InputButtonEvent> inputButtonListener)
    {
        m_inputButtonListener = inputButtonListener;
    }

    /// <summary>
    /// Listener for the input axis
    /// </summary>
    private static OuyaSDK.InputAxisListener<OuyaSDK.InputAxisEvent> m_inputAxisListener = null;
    public static OuyaSDK.InputAxisListener<OuyaSDK.InputAxisEvent> getInputAxisListener()
    {
        return m_inputAxisListener;
    }
    public static void registerInputAxisListener(OuyaSDK.InputAxisListener<OuyaSDK.InputAxisEvent> inputAxisListener)
    {
        m_inputAxisListener = inputAxisListener;
    }

    /// <summary>
    /// The constructor
    /// </summary>
    /// <param name="developerId"></param>
    public static void initialize(string developerId)
    {
        m_developerId = developerId;
        JavaSetDeveloperId();

        JavaInit();
    }

    /// <summary>
    /// Access the developer id
    /// </summary>
    /// <returns></returns>
    public static string getDeveloperId()
    {
        return m_developerId;
    }

    #region Mirror Java API

    public class GenericListener<T>
    {
        public delegate void SuccessDelegate(T data);

        public SuccessDelegate onSuccess = null;

        public delegate void FailureDelegate(int errorCode, String errorMessage);

        public FailureDelegate onFailure = null;
    }

    public class CancelIgnoringIapResponseListener<T> : GenericListener<T>
    {
    }

    public static void requestProductList(List<Purchasable> purchasables, CancelIgnoringIapResponseListener<List<Product>> productListListener)
    {
        JavaGetProductsAsync();
    }

    public static void requestPurchase(Purchasable purchasable, CancelIgnoringIapResponseListener<Product> requestPurchaseListener)
	{
        JavaRequestPurchaseAsync(purchasable);
    }

    public class InputButtonListener<T> : GenericListener<T>
    {
    }

    public class InputAxisListener<T> : GenericListener<T>
    {
    }

    public class DeviceListener<T> : GenericListener<T>
    {
    }


    #endregion

    #region Data containers

    public class Purchasable
    {
        public string productId = string.Empty;
        public Purchasable(string productId)
        {
            this.productId = productId;
        }
        public string getProductId()
        {
            return productId;
        }
        public static implicit operator string(Purchasable rhs)
        {
            return rhs.getProductId();
        }
        public static implicit operator Purchasable(string rhs)
        {
            return new Purchasable(rhs);
        }
    }

    [Serializable]
    public class Product
    {
        public string identifier = string.Empty;
        public string name = string.Empty;
        public int priceInCents = 0;

        public string getIdentifier()
        {
            return identifier;
        }

        public string getName()
        {
            return name;
        }

        public int getPriceInCents()
        {
            return priceInCents;
        }

        public void setIdentifier(string identifier)
        {
            this.identifier = identifier;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public void setPriceInCents(int priceInCents)
        {
            this.priceInCents = priceInCents;
        }
    }

    public class InputButtonEvent
    {
        public InputButtonEvent(InputAction inputAction, KeyEnum keyCode, OuyaPlayer player)
        {
            m_inputAction = inputAction;
            m_keyCode = keyCode;
            m_player = player;
        }

        private InputAction m_inputAction = InputAction.None;
        public InputAction getKeyAction()
        {
            return m_inputAction;
        }

        private KeyEnum m_keyCode = 0;
        public KeyEnum getKeyCode()
        {
            return m_keyCode;
        }

        private OuyaPlayer m_player;
        public OuyaPlayer getPlayer()
        {
            return m_player;
        }
    }

    public class InputAxisEvent
    {
        private int m_keyCode;

        public InputAxisEvent(InputAction inputAction, OuyaSDK.AxisEnum axisEnum, float axis, OuyaPlayer player)
        {
            m_inputAction = inputAction;
            m_axisCode = axisEnum;
            m_axis = axis;
            m_player = player;
        }

        private InputAction m_inputAction = InputAction.None;
        public InputAction getInputAction()
        {
            return m_inputAction;
        }

        private AxisEnum m_axisCode = AxisEnum.NONE;
        public AxisEnum getAxisCode()
        {
            return m_axisCode;
        }

        private float m_axis = 0f;
        public float getAxis()
        {
            return m_axis;
        }

        private OuyaPlayer m_player;
        public OuyaPlayer getPlayer()
        {
            return m_player;
        }
    }

    #endregion

    #region Java Interface

    private const string JAVA_CLASS = "tv.ouya.sdk.OuyaUnityPlugin";

    private static void JavaInit()
    {
        // attach our thread to the java vm; obviously the main thread is already attached but this is good practice..
        AndroidJNI.AttachCurrentThread();

        // first we try to find our main activity..
        IntPtr cls_Activity = AndroidJNI.FindClass("com/unity3d/player/UnityPlayer");
        IntPtr fid_Activity = AndroidJNI.GetStaticFieldID(cls_Activity, "currentActivity", "Landroid/app/Activity;");
        IntPtr obj_Activity = AndroidJNI.GetStaticObjectField(cls_Activity, fid_Activity);
        Debug.Log("obj_Activity = " + obj_Activity);

        // create a JavaClass object...
        IntPtr cls_JavaClass = AndroidJNI.FindClass("tv/ouya/sdk/OuyaUnityPlugin");
        IntPtr mid_JavaClass = AndroidJNI.GetMethodID(cls_JavaClass, "<init>", "(Landroid/app/Activity;)V");
        IntPtr obj_JavaClass = AndroidJNI.NewObject(cls_JavaClass, mid_JavaClass, new jvalue[] { new jvalue() { l = obj_Activity } });
        Debug.Log("JavaClass object = " + obj_JavaClass);
    }

    private static void JavaSetDeveloperId()
    {
        // again, make sure the thread is attached..
        AndroidJNI.AttachCurrentThread();

        AndroidJNI.PushLocalFrame(0);

        try
        {
            Debug.Log("JavaSetDeveloperId");
            using (AndroidJavaClass ajc = new AndroidJavaClass(JAVA_CLASS))
            {
                ajc.CallStatic<String>("setDeveloperId", new object[] { m_developerId + "\0" });
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("OuyaSDK.JavaSetDeveloperId exception={0}", ex));
        }
        finally
        {
            AndroidJNI.PopLocalFrame(IntPtr.Zero);
        }
    }

    private static void JavaGetProductsAsync()
    {
        // again, make sure the thread is attached..
        AndroidJNI.AttachCurrentThread();

        AndroidJNI.PushLocalFrame(0);

        try
        {
            Debug.Log("JavaGetProductsAsync");

            using (AndroidJavaClass ajc = new AndroidJavaClass(JAVA_CLASS))
            {
                ajc.CallStatic<String>("getProductsAsync");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("OuyaSDK.JavaGetProductsAsync exception={0}", ex));
        }
        finally
        {
            AndroidJNI.PopLocalFrame(IntPtr.Zero);
        }
    }

    private static void JavaRequestPurchaseAsync(Purchasable purchasable)
    {
        // again, make sure the thread is attached..
        AndroidJNI.AttachCurrentThread();

        AndroidJNI.PushLocalFrame(0);

        try
        {
            Debug.Log(string.Format("JavaRequestPurchaseAsync purchasable: {0}", purchasable.getProductId()));

            using (AndroidJavaClass ajc = new AndroidJavaClass(JAVA_CLASS))
            {
                ajc.CallStatic<String>("requestPurchaseAsync", new object[] { purchasable.getProductId() + "\0" });
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(string.Format("OuyaSDK.JavaRequestPurchaseAsync exception={0}", ex));
        }
        finally
        {
            AndroidJNI.PopLocalFrame(IntPtr.Zero);
        }
    }

    #endregion
}