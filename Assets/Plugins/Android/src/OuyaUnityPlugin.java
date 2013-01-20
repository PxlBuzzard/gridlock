package tv.ouya.sdk;

import android.app.Activity;
import android.content.Intent;
import android.util.Log;

import java.io.File;

import com.unity3d.player.UnityPlayer;

public class OuyaUnityPlugin
{
	private static Activity m_activity;

	private static TestOuyaFacade m_test = null;

	private static String m_developerId = "";

	public OuyaUnityPlugin(Activity currentActivity)
	{
		Log.i("Unity", "Constructor called with currentActivity = " + currentActivity);
		m_activity = currentActivity;

		InitializeTest();
	}

	private static void InitializeTest()
	{
		try		
		{
			if (null == m_test)
			{
				Log.i("Unity", "m_test is null");
			}

			if (null == m_activity)
			{
				Log.i("Unity", "m_activity is null");
			}

			if (null == m_test &&
				null != m_activity)
			{
				if (m_developerId == "")
				{
					Log.i("Unity", "m_developerId is empty, requesting");
					UnityPlayer.UnitySendMessage("OuyaGameObject", "RequestDeveloperId", null);
				}
				else
				{
					Log.i("Unity", "m_developerId is valid,  constructing TestOuyaFacade");
					m_test = new TestOuyaFacade(m_activity);
				}
			}
		}
		catch (Exception ex) 
		{
			Log.i("Unity", "InitializeTest exception: " + ex.toString());
		}
	}

	public static String setDeveloperId(String developerId)
	{
		try
		{
			Log.i("Unity", "setDeveloperId developerId: " + developerId);

			m_developerId = developerId;

			InitializeTest();
			return "";
		}
		catch (Exception ex) 
		{
			Log.i("Unity", "setDeveloperId exception: " + ex.toString());
		}
		return "";
	}

	public static String getProductsAsync()
	{
		try
		{
			Log.i("Unity", "getProductsAsync");

			InitializeTest();

			if (null == m_test)
			{
				Log.i("Unity", "m_test is null");
			}
			else
			{
				Log.i("Unity", "m_test is valid");
				m_test.getProductsAsync();
			}
		}
		catch (Exception ex) 
		{
			Log.i("Unity", "getProductsAsync exception: " + ex.toString());
		}
		return "";
	}

	public static String requestPurchaseAsync(String sku)
	{
		try
		{
			Log.i("Unity", "requestPurchaseAsync sku: " + sku);
		
			InitializeTest();

			if (null == m_test)
			{
				Log.i("Unity", "m_test is null");
			}
			else
			{
				Log.i("Unity", "m_test is valid");
				m_test.requestPurchase(sku);
			}
		}
		catch (Exception ex) 
		{
			Log.i("Unity", "requestPurchaseAsync exception: " + ex.toString());
		}
		return "";
	}
}