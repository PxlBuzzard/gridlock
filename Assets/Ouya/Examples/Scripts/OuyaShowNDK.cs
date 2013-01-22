using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class OuyaShowNDK : MonoBehaviour
{
    #region NDK wrapper

    /// <summary>
    /// Isolate DLL calls
    /// </summary>
    private struct AndroidPlugin
    {
        [DllImport("lib-ouya-ndk")]
        // EXPORT_API void AndroidGetHelloWorld(long* size)
        private static extern IntPtr AndroidGetHelloWorld(out long size);

        /// <summary>
        /// Wrap the dll call
        /// </summary>
        /// <returns></returns>
        public static string InvokeAndroidGetHelloWorld()
        {
            try
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        long size;
                        IntPtr result = AndroidGetHelloWorld(out size);
                        byte[] bytes = new byte[size];
                        for (int index = 0; index < size; ++index)
                        {
                            bytes[index] = Marshal.ReadByte(result, index);
                        }
                        InvokeAndroidReleaseMemory(result);
                        return System.Text.UTF8Encoding.UTF8.GetString(bytes);
                    default:
                        return string.Empty;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("Failed to invoke AndroidGetHelloWorld: {0}", ex));
                return string.Empty;
            }
        }

        [DllImport("lib-ouya-ndk")]
        // EXPORT_API void AndroidReleaseMemory(char* buffer)
        private static extern void AndroidReleaseMemory(IntPtr buffer);

        /// <summary>
        /// Release unmanaged memory
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="size"></param>
        public static void InvokeAndroidReleaseMemory(IntPtr buffer)
        {
            try
            {
                AndroidReleaseMemory(buffer);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("Failed to invoke AndroidReleaseMemory: {0}", ex));
            }
        }

        [DllImport("lib-ouya-ndk", CallingConvention = CallingConvention.Cdecl)]
        // EXPORT_API void AndroidExampleFunction1(unsigned char* a, int b, int* d)
        private static extern void AndroidExampleFunction1(byte[] a, int b, out int c);

        /// <summary>
        /// Wrap the dll boundary
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public static void InvokeAndroidExampleFunction1(byte[] a, int b, out int c)
        {
            try
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        AndroidExampleFunction1(a, b, out c);
                        break;
                    default:
                        c = 0;
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("Failed to invoke InvokeAndroidExampleFunction1: {0}", ex));
                c = 0;
            }
        }
    }

    #endregion

    #region Hello World Test

    /// <summary>
    /// Get the hello world message
    /// </summary>
    private string m_helloWorld = string.Empty;

    #endregion

    /// <summary>
    /// Display returned value from NDK
    /// </summary>
    private int m_c = 0;

    /// <summary>
    /// Implements MonoBehaviour
    /// </summary>
    private void OnGUI()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                GUILayout.Space(20);
                if (GUILayout.Button("Clear Results"))
                {
                    m_c = -1;
                    m_helloWorld = string.Empty;
                }

                GUILayout.Space(20);
                if (GUILayout.Button("Invoke Android Hello World"))
                {
                    m_helloWorld = AndroidPlugin.InvokeAndroidGetHelloWorld();
                }
                GUILayout.Space(20);

                GUILayout.Label(string.Format("Hello World result: {0}", m_helloWorld));

                GUILayout.Space(20);
                if (GUILayout.Button("Invoke Android ExampleFunction1"))
                {
                    byte[] a = ASCIIEncoding.ASCII.GetBytes("Hello some string in bytes");
                    AndroidPlugin.InvokeAndroidExampleFunction1(a, 2, out m_c);
                }

                GUILayout.Space(20);
                GUILayout.Label(string.Format("ExampleFunction1 result: {0}", m_c));
                break;
        }
    }
}