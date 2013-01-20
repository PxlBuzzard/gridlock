//http://code.google.com/p/google-gson/

package tv.ouya.demo.OuyaUnityApplication;

import android.app.Activity;
//import android.hardware.input.InputManager; //API 16
import android.os.Bundle;
import android.util.Log;
import android.view.KeyEvent;
import android.view.MotionEvent;
import android.view.InputDevice;
import android.widget.FrameLayout;
import android.widget.LinearLayout.LayoutParams;
import android.widget.RelativeLayout;

import com.google.gson.Gson;

import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;
import com.unity3d.player.UnityPlayerNativeActivity;
import com.unity3d.player.UnityPlayerProxyActivity;
import java.util.ArrayList;

public class OuyaUnityApplication extends Activity
{
	//the Unity Player
	private UnityPlayer m_UnityPlayer;
	
	//indicates the Unity player has loaded
	private Boolean m_enableUnity = true;

	//private InputManager.InputDeviceListener m_inputDeviceListener = null;

	protected void onCreate(Bundle savedInstanceState) 
	{
		super.onCreate(savedInstanceState);

		// Create the UnityPlayer
        m_UnityPlayer = new UnityPlayer(this);
        int glesMode = m_UnityPlayer.getSettings().getInt("gles_mode", 1);
        boolean trueColor8888 = false;
        m_UnityPlayer.init(glesMode, trueColor8888);
        setContentView(R.layout.main);

        // Add the Unity view
        FrameLayout layout = (FrameLayout) findViewById(R.id.unityLayout);
        LayoutParams lp = new LayoutParams (LayoutParams.FILL_PARENT, LayoutParams.FILL_PARENT);
        layout.addView(m_UnityPlayer.getView(), 0, lp);

		// Set the focus
        RelativeLayout mainLayout = (RelativeLayout) findViewById(R.id.mainLayout);
		mainLayout.setFocusableInTouchMode(true);

		// listen for controller changes - http://developer.android.com/reference/android/hardware/input/InputManager.html#registerInputDeviceListener%28android.hardware.input.InputManager.InputDeviceListener,%20android.os.Handler%29
		//m_inputDeviceListener = new InputManager.InputDeviceListener();
		//InputManager.registerInputDeviceListener (m_inputDeviceListener, inputDeviceListener);

		//Get a list of all device id's and assign them to players.
		ArrayList<Device> devices = checkDevices();
		GenericSendMessage("onSetDevices", devices);
		
	}

	void inputDeviceListener()
	{
	}

	private ArrayList<Device> checkDevices(){
		//Get a list of all device id's and assign them to players.
		ArrayList<Device> devices = new ArrayList<Device>();
		int[] deviceIds = InputDevice.getDeviceIds();
		//Log.i("Unity-Devices", "length:" + deviceIds.length );
		int controllerCount = 1;
		for (int count=0; count < deviceIds.length; count++)
		{
			InputDevice d = InputDevice.getDevice(deviceIds[count]);
			if (!d.isVirtual())
			{
				if (d.getName().toUpperCase().indexOf("OUYA GAME CONTROLLER") != -1 ||
					d.getName().toUpperCase().indexOf("MICROSOFT X-BOX 360 PAD") != -1 ||
					d.getName().toUpperCase().indexOf("IDROID:CON") != -1 ||
					d.getName().toUpperCase().indexOf("USB CONTROLLER") != -1)
				{
					Device device = new Device();
					device.id = d.getId();
					device.player = controllerCount;
					device.name = d.getName();
					devices.add(device);
					controllerCount++;
				}
				else
				{
					Device device = new Device();
					device.id = d.getId();
					device.player = 0;
					device.name = d.getName();
					devices.add(device);
				}
			}
		}
		return devices;
	}

	private void ExtractDataMotionEvent (InputContainer box, MotionEvent event)
	{
		box.MotionEvent = event;
		
		box.ActionCode = event.getAction();
		box.ActionIndex = event.getActionIndex();
		box.ActionMasked = event.getActionMasked();
		
		box.AxisBrake = event.getAxisValue(MotionEvent.AXIS_BRAKE);
        box.AxisDistance = event.getAxisValue(MotionEvent.AXIS_DISTANCE);
        box.AxisGas = event.getAxisValue(MotionEvent.AXIS_GAS);
		box.AxisGeneric1 = event.getAxisValue(MotionEvent.AXIS_GENERIC_1);
		box.AxisGeneric2 = event.getAxisValue(MotionEvent.AXIS_GENERIC_2);
		box.AxisGeneric3 = event.getAxisValue(MotionEvent.AXIS_GENERIC_3);
		box.AxisGeneric4 = event.getAxisValue(MotionEvent.AXIS_GENERIC_4);
		box.AxisGeneric5 = event.getAxisValue(MotionEvent.AXIS_GENERIC_5);
		box.AxisGeneric6 = event.getAxisValue(MotionEvent.AXIS_GENERIC_6);
		box.AxisGeneric7 = event.getAxisValue(MotionEvent.AXIS_GENERIC_7);
		box.AxisGeneric8 = event.getAxisValue(MotionEvent.AXIS_GENERIC_8);
		box.AxisGeneric9 = event.getAxisValue(MotionEvent.AXIS_GENERIC_9);
		box.AxisGeneric10 = event.getAxisValue(MotionEvent.AXIS_GENERIC_10);
		box.AxisGeneric11 = event.getAxisValue(MotionEvent.AXIS_GENERIC_11);
		box.AxisGeneric12 = event.getAxisValue(MotionEvent.AXIS_GENERIC_12);
		box.AxisGeneric13 = event.getAxisValue(MotionEvent.AXIS_GENERIC_13);
		box.AxisGeneric14 = event.getAxisValue(MotionEvent.AXIS_GENERIC_14);
		box.AxisGeneric15 = event.getAxisValue(MotionEvent.AXIS_GENERIC_15);
		box.AxisGeneric16 = event.getAxisValue(MotionEvent.AXIS_GENERIC_16);
		box.AxisHatX = event.getAxisValue(MotionEvent.AXIS_HAT_X);
        box.AxisHatY = event.getAxisValue(MotionEvent.AXIS_HAT_Y);
        box.AxisHScroll = event.getAxisValue(MotionEvent.AXIS_HSCROLL);
        box.AxisLTrigger = event.getAxisValue(MotionEvent.AXIS_LTRIGGER);
        box.AxisOrientation = event.getAxisValue(MotionEvent.AXIS_ORIENTATION);
        box.AxisPressire = event.getAxisValue(MotionEvent.AXIS_PRESSURE);
        box.AxisRTrigger = event.getAxisValue(MotionEvent.AXIS_RTRIGGER);
        box.AxisRudder = event.getAxisValue(MotionEvent.AXIS_RUDDER);
        box.AxisRX = event.getAxisValue(MotionEvent.AXIS_RX);
        box.AxisRY = event.getAxisValue(MotionEvent.AXIS_RY);
        box.AxisRZ = event.getAxisValue(MotionEvent.AXIS_RZ);
        box.AxisSize = event.getAxisValue(MotionEvent.AXIS_SIZE);
        box.AxisThrottle = event.getAxisValue(MotionEvent.AXIS_THROTTLE);
        box.AxisTilt = event.getAxisValue(MotionEvent.AXIS_TILT);
        box.AxisToolMajor = event.getAxisValue(MotionEvent.AXIS_TOUCH_MAJOR);
        box.AxisToolMinor = event.getAxisValue(MotionEvent.AXIS_TOUCH_MINOR);
        box.AxisVScroll = event.getAxisValue(MotionEvent.AXIS_VSCROLL);
        box.AxisWheel = event.getAxisValue(MotionEvent.AXIS_WHEEL);
        box.AxisX = event.getAxisValue(MotionEvent.AXIS_X);
		box.AxisY = event.getAxisValue(MotionEvent.AXIS_Y);
		box.AxisZ = event.getAxisValue(MotionEvent.AXIS_Z);

		box.ButtonState = event.getButtonState();
		box.EdgeFlags = event.getEdgeFlags();
		box.Flags = event.getFlags();
		box.DeviceId = event.getDeviceId();
		InputDevice device = InputDevice.getDevice(box.DeviceId);
		if (null != device)
		{
			box.DeviceName = device.getName();
		}
		box.MetaState = event.getMetaState();
		box.PointerCount = event.getPointerCount();
		box.Pressure = event.getPressure();
		box.X = event.getX();
		box.Y = event.getY();
	}

	private void ExtractDataKeyEvent (InputContainer box, KeyEvent event)
	{
		box.KeyEvent = event;
		
		box.ActionCode = event.getAction();
		box.Flags = event.getFlags();
		box.DeviceId = event.getDeviceId();
		InputDevice device = InputDevice.getDevice(box.DeviceId);
		if (null != device)
		{
			box.DeviceName = device.getName();
		}
		box.MetaState = event.getMetaState();
	}

	private void GenericSendMessage (String method, InputContainer box)
	{
		Gson gson = new Gson();
		String jsonData = gson.toJson(box);

		//Log.i("Unity", method + " jsonData=" + jsonData);
		if (m_enableUnity)
		{
			UnityPlayer.UnitySendMessage("OuyaGameObject", method, jsonData);
		}
	}

	private void GenericSendMessage (String method, ArrayList<Device> devices)
	{
		Gson gson = new Gson();
		String jsonData = gson.toJson(devices);

		Log.i("Unity", method + " jsonData=" + jsonData);
		if (m_enableUnity)
		{
			UnityPlayer.UnitySendMessage("OuyaGameObject", method, jsonData);
		}
	}

	@Override
	public boolean onKeyDown (int keyCode, KeyEvent event)
	{
		InputContainer box = new InputContainer();
		ExtractDataKeyEvent(box, event);
		box.KeyCode = keyCode;
		GenericSendMessage("onKeyDown", box);
		return true;
	}

	@Override
	public boolean onKeyUp (int keyCode, KeyEvent event)
	{
		InputContainer box = new InputContainer();
		ExtractDataKeyEvent(box, event);
		box.KeyCode = keyCode;
		GenericSendMessage("onKeyUp", box);
		return true;
	}

	@Override
	public boolean onGenericMotionEvent (MotionEvent event)
	{
		InputContainer box = new InputContainer();
		ExtractDataMotionEvent(box, event);
		GenericSendMessage("onGenericMotionEvent", box);
		return true;
	}

	@Override
	public boolean onTouchEvent (MotionEvent event)
	{
		InputContainer box = new InputContainer();
		ExtractDataMotionEvent(box, event);
		GenericSendMessage("onTouchEvent", box);
		return true;
	}

	@Override
	public boolean onTrackballEvent (MotionEvent event)
	{
		InputContainer box = new InputContainer();
		ExtractDataMotionEvent(box, event);
		GenericSendMessage("onTrackballEvent", box);
		return true;
	}

	public class Device
	{
		public int id;
		public int player;
		public String name;
	}

	public class InputContainer
	{
		public int KeyCode = 0;
		public KeyEvent KeyEvent = null;
		public MotionEvent MotionEvent = null;
		
		public int Action = 0;
        public int ActionCode = 0;
        public int ActionIndex = 0;
        public int ActionMasked = 0;
        
		public float AxisBrake = 0;
        public float AxisDistance = 0;
        public float AxisGas = 0;
        public float AxisGeneric1 = 0;
        public float AxisGeneric2 = 0;
        public float AxisGeneric3 = 0;
        public float AxisGeneric4 = 0;
        public float AxisGeneric5 = 0;
        public float AxisGeneric6 = 0;
        public float AxisGeneric7 = 0;
        public float AxisGeneric8 = 0;
        public float AxisGeneric9 = 0;
        public float AxisGeneric10 = 0;
        public float AxisGeneric11 = 0;
        public float AxisGeneric12 = 0;
        public float AxisGeneric13 = 0;
        public float AxisGeneric14 = 0;
        public float AxisGeneric15 = 0;
        public float AxisGeneric16 = 0;
        public float AxisHatX = 0;
        public float AxisHatY = 0;
        public float AxisHScroll = 0;
        public float AxisLTrigger = 0;
        public float AxisOrientation = 0;
        public float AxisPressire = 0;
        public float AxisRTrigger = 0;
        public float AxisRudder = 0;
        public float AxisRX = 0;
        public float AxisRY = 0;
        public float AxisRZ = 0;
        public float AxisSize = 0;
        public float AxisThrottle = 0;
        public float AxisTilt = 0;
        public float AxisToolMajor = 0;
        public float AxisToolMinor = 0;
        public float AxisVScroll = 0;
        public float AxisWheel = 0;
        public float AxisX = 0;
        public float AxisY = 0;
        public float AxisZ = 0;

		public int ButtonState = 0;
        public int DeviceId = 0;
        public String DeviceName = "";
        public int EdgeFlags = 0;
        public int Flags = 0;
        public int MetaState = 0;
        public int PointerCount = 0;
        public float Pressure = 0;
        public float X = 0;
        public float Y = 0;
	}
}