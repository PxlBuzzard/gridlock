using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class OuyaMenuAdmin : MonoBehaviour
{
    private static Vector3 m_pos = Vector3.zero;
    private static Vector3 m_euler = Vector3.zero;

    [MenuItem("OUYA/Generate OUYA Unity Plugin for Packager")]
    public static void MenuGeneratePluginJar()
    {
        UpdatePaths();

        CompileApplicationClasses();
        BuildApplicationJar();

        AssetDatabase.Refresh();
    }

    [MenuItem("OUYA/Copy Transform")]
    public static void MenuCopyTransform()
    {
        if (Selection.activeGameObject)
        {
            m_pos = Selection.activeGameObject.transform.position;
            m_euler = Selection.activeGameObject.transform.rotation.eulerAngles;
        }
    }

    [MenuItem("OUYA/Set Transform")]
    public static void MenuSetTransform()
    {
        if (Selection.activeGameObject)
        {
            Selection.activeGameObject.transform.position = m_pos;
            Selection.activeGameObject.transform.rotation = Quaternion.Euler(m_euler);
        }
    }

    private static string m_pathUnityProject = string.Empty;
    private static string m_pathUnityEditor = string.Empty;
    private static string m_pathUnityJar = string.Empty;
    private static string m_pathJDK = string.Empty;
    private static string m_pathToolsJar = string.Empty;
    private static string m_pathJar = string.Empty;
    private static string m_pathJavaC = string.Empty;
    private static string m_pathSrc = string.Empty;
    private static string m_pathSDK = string.Empty;
    private static string m_pathOuyaSDKJar = string.Empty;
    private static string m_pathGsonJar = string.Empty;

    private static void UpdatePaths()
    {
        m_pathUnityProject = new DirectoryInfo(Directory.GetCurrentDirectory()).FullName;
        m_pathUnityEditor = new FileInfo(EditorApplication.applicationPath).Directory.FullName;
        switch (Application.platform)
        {
            case RuntimePlatform.OSXEditor:
                m_pathUnityJar = string.Format("{0}/{1}", m_pathUnityEditor, OuyaPanel.PATH_UNITY_JAR_MAC);
                break;
            case RuntimePlatform.WindowsEditor:
                m_pathUnityJar = string.Format("{0}/{1}", m_pathUnityEditor, OuyaPanel.PATH_UNITY_JAR_WIN);
                break;
        }
        m_pathSrc = string.Format("{0}/Assets/Plugins/Android/src", m_pathUnityProject);
        m_pathSDK = EditorPrefs.GetString(OuyaPanel.KEY_PATH_ANDROID_SDK);
        m_pathJDK = EditorPrefs.GetString(OuyaPanel.KEY_PATH_JAVA_JDK);
        switch (Application.platform)
        {
            case RuntimePlatform.OSXEditor:
                m_pathToolsJar = string.Format("{0}/Contents/Classes/classes.jar", m_pathJDK);
                m_pathJar = string.Format("{0}/Contents/Commands/{1}", m_pathJDK, OuyaPanel.FILE_JAR_MAC);
                m_pathJavaC = string.Format("{0}/Contents/Commands/{1}", m_pathJDK, OuyaPanel.FILE_JAVAC_MAC);
                break;
            case RuntimePlatform.WindowsEditor:
                m_pathToolsJar = string.Format("{0}/lib/tools.jar", m_pathJDK);
                m_pathJar = string.Format("{0}/{1}/{2}", m_pathJDK, OuyaPanel.REL_JAVA_PLATFORM_TOOLS, OuyaPanel.FILE_JAR_WIN);
                m_pathJavaC = string.Format("{0}/{1}/{2}", m_pathJDK, OuyaPanel.REL_JAVA_PLATFORM_TOOLS, OuyaPanel.FILE_JAVAC_WIN);
                break;
        }
        m_pathOuyaSDKJar = string.Format("{0}/Assets/Plugins/Android/libs/ouya-sdk.jar", m_pathUnityProject);
        m_pathGsonJar = string.Format("{0}/Assets/Plugins/Android/libs/gson-2.2.2.jar", m_pathUnityProject);
    }

    private static string GetPathAndroidJar()
    {
        return string.Format("{0}/platforms/android-{1}/android.jar", m_pathSDK, (int)PlayerSettings.Android.minSdkVersion);
    }

    static void CompileApplicationClasses()
    {
        string pathClasses = string.Format("{0}/Assets/Plugins/Android/Classes", m_pathUnityProject);
        if (!Directory.Exists(pathClasses))
        {
            Directory.CreateDirectory(pathClasses);
        }

        string includeFiles = string.Format("\"{0}/OuyaUnityPlugin.java\" \"{0}/TestOuyaFacade.java\"", m_pathSrc);
        string jars = string.Empty;

        if (File.Exists(m_pathToolsJar))
        {
            Debug.Log(string.Format("Found Java tools jar: {0}", m_pathToolsJar));
        }
        else
        {
            Debug.LogError(string.Format("Failed to find Java tools jar: {0}", m_pathToolsJar));
            return;
        }

        if (File.Exists(GetPathAndroidJar()))
        {
            Debug.Log(string.Format("Found Android jar: {0}", GetPathAndroidJar()));
        }
        else
        {
            Debug.LogError(string.Format("Failed to find Android jar: {0}", GetPathAndroidJar()));
            return;
        }

        if (File.Exists(m_pathGsonJar))
        {
            Debug.Log(string.Format("Found GJON jar: {0}", m_pathGsonJar));
        }
        else
        {
            Debug.LogError(string.Format("Failed to find GSON jar: {0}", m_pathGsonJar));
            return;
        }

        if (File.Exists(m_pathUnityJar))
        {
            Debug.Log(string.Format("Found Unity jar: {0}", m_pathUnityJar));
        }
        else
        {
            Debug.LogError(string.Format("Failed to find Unity jar: {0}", m_pathUnityJar));
            return;
        }

        switch (Application.platform)
        {
            case RuntimePlatform.OSXEditor:
                jars = string.Format("\"{0}:{1}:{2}:{3}:{4}\"", m_pathToolsJar, GetPathAndroidJar(), m_pathGsonJar, m_pathUnityJar, m_pathOuyaSDKJar);

                OuyaPanel.RunProcess(m_pathJavaC, string.Format("-g -source 1.6 -target 1.6 {0} -classpath {1} -bootclasspath {1} -d \"{2}\"",
                    includeFiles,
                    jars,
                    pathClasses));
                break;
            case RuntimePlatform.WindowsEditor:
                jars = string.Format("\"{0}\";\"{1}\";\"{2}\";\"{3}\";\"{4}\"", m_pathToolsJar, GetPathAndroidJar(), m_pathGsonJar, m_pathUnityJar, m_pathOuyaSDKJar);

                OuyaPanel.RunProcess(m_pathJavaC, string.Format("-g -source 1.6 -target 1.6 {0} -classpath {1} -bootclasspath {1} -d \"{2}\"",
                    includeFiles,
                    jars,
                    pathClasses));
                break;
        }
    }

    static void BuildApplicationJar()
    {
        string pathClasses = string.Format("{0}/Assets/Plugins/Android/Classes", m_pathUnityProject);
        OuyaPanel.RunProcess(m_pathJar, pathClasses, string.Format("cvfM OuyaUnityPlugin.jar tv/"));
        string pathAppJar = string.Format("{0}/OuyaUnityPlugin.jar", pathClasses);
        string pathDest = string.Format("{0}/Assets/Plugins/Android/OuyaUnityPlugin.jar", m_pathUnityProject);
        if (File.Exists(pathDest))
        {
            File.Delete(pathDest);
        }
        if (File.Exists(pathAppJar))
        {
            File.Move(pathAppJar, pathDest);
        }
        if (Directory.Exists(pathClasses))
        {
            Directory.Delete(pathClasses, true);
        }
    }
}