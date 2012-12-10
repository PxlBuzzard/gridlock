using UnityEngine;
using System.Collections;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Controller to move an object along an path, formed by an OTShape.
/// </summary>
public class OTPathController : OTMovementController
{
	/// <summary>
	/// PathController delegate
	/// </summary>
	/// <param name="path">
	/// Shape that forms the path
	/// </param>
	/// <param name="owner">
	/// owner Object that is on the path
	/// </param>
    public delegate void PathDelegate(OTShape path, Object owner);

    /// <summary>
    /// This delegate will be called when the moving object reaches the finish
    /// </summary>
    /// <remarks>
    /// If the object is looping , the OnStart will be called each time the object 
    /// is at the end position
    /// </remarks>
    public PathDelegate onFinish = null;
    /// <summary>
    /// This delegate will be called when the moving object starts on the path.
    /// </summary>
    /// <remarks>
    /// If the object is looping , the OnStart will be called each time the object 
    /// is at the start position
    /// </remarks>
    public PathDelegate onStart = null;

    OTShape shape;
    /// <summary>
    /// Movement duration iun seconds
    /// </summary>
    public float duration;
    /// <summary>
    /// Movement easing function
    /// </summary>
    public OTEase easing;
    /// <summary>
    /// Object will be looping
    /// </summary>
    public bool looping = false;
    /// <summary>
    /// Object will move in opposite direction
    /// </summary>
    public bool oppositeDirection
    {
        get
        {
            return _oppositeDirection;
        }
        set
        {
			if (value!=oppositeDirection)
			{
	            _time = duration - time;
	            _oppositeDirection = value;
				
				if (_time == duration)
					_time = 0;
				
			}
        }
    }
    /// <summary>
    /// Sets or gets the position on the path (0-1)
    /// </summary>
    public float position
    {
        get
        {
			if (oppositeDirection)
				return 1-(_time / duration);
			else				
            	return _time / duration;
        }
        set
        {
            _time = duration * value;
            customPosition = value;
        }
    }

	/// <summary>
	/// Gets or sets the target object to point to
	/// </summary>
	public GameObject targetObject
	{
		get
		{
			return _targetObject;
		}
		set
		{
			_targetObject = value;
		}
	}
	
	
    /// <summary>
    /// Handle movement manually
    /// </summary>
    public bool customControl = false;
    /// <summary>
    /// Manual position in path percentage (0-100)
    /// </summary>
    public float customPosition = 0;

    private bool _oppositeDirection = false;

    /// <summary>
    /// How will the object be heading
    /// </summary>
    public enum UpVector
    {
        /// <summary>
        /// Shape's up vector is not touched
        /// </summary>
        None,
        /// <summary>
        /// Shape's up vector will point along the path
        /// </summary>
        Follow,
        /// <summary>
        /// Shape's up vector will point inward
        /// </summary>
        InWard,
        /// <summary>
        /// Shape's up vector will point outward
        /// </summary>
        OutWard,
        /// <summary>
        /// Shape's up vector will point at a provided object
        /// </summary>
        Target
    }
    /// <summary>
    /// How will the object be heading
    /// </summary>
    public UpVector upVector = UpVector.Follow;
    /// <summary>
    /// Add additional rotation
    /// </summary>
    public float addRotation = 0;
	
	GameObject _targetObject = null;
	
	override protected void Initialize()
	{		
		if (_targetObject == null) _targetObject = shape.gameObject;
        previousPosition = shape.startPosition;
		SetPosition();
        HandleOrientation(shape.GetPosition(0.1f));		
	}

    /// <summary>
    /// Path controller constructor
    /// </summary>
    /// <param name="owner">Owner object that must be moved</param>
    /// <param name="name">Name of this controller</param>
    /// <param name="shape">Path shape</param>
    /// <param name="duration">Movement duration in seconds</param>
    /// <param name="easing">Movement easing function</param>
    public OTPathController(Object owner, string name, OTShape shape, float duration, OTEase easing)
        : base(owner, name)
    {
        this.shape = shape;
        this.duration = duration;
        this.easing = easing;
    }

    /// <summary>
    /// Path controller constructor - Linear easing
    /// </summary>
    /// <param name="owner">Owner object that must be moved</param>
    /// <param name="name">Name of this controller</param>
    /// <param name="shape">Path shape</param>
    /// <param name="duration">Movement duration in seconds</param>
    public OTPathController(Object owner, string name, OTShape shape, float duration)
        : base(owner, name)
    {
        this.shape = shape;
        this.duration = duration;
        this.easing = OTEasing.Linear;
    }

    void HandleOrientation(Vector2 pos)
    {
		GameObject g = null;
        if (owner is OTObject)
		   g = (owner as OTObject).gameObject;
		else
        if (owner is GameObject)
		   g = (owner as GameObject);
		
		if (g == null) 
			return;
		
		Vector2 _upVector = Vector2.zero;			
		Matrix4x4 mx = new Matrix4x4();
        switch (upVector)
        {
            case UpVector.Follow:			
                _upVector = (pos - previousPosition).normalized;
                break;
			case UpVector.InWard:
                _upVector = (pos - previousPosition).normalized;
				mx.SetTRS(Vector3.zero,Quaternion.Euler(new Vector3(0,0,-90)),Vector3.one);
				_upVector = mx.MultiplyPoint3x4(_upVector);
				break;
			case UpVector.OutWard:
                _upVector = (pos - previousPosition).normalized;
				mx.SetTRS(Vector3.zero,Quaternion.Euler(new Vector3(0,0,90)),Vector3.one);
				_upVector = mx.MultiplyPoint3x4(_upVector);
				break;
            case UpVector.Target:
                _upVector = ((Vector2)targetObject.transform.position - previousPosition).normalized;
                break;
        }
        g.transform.up = _upVector;
        g.transform.rotation = Quaternion.Euler(0, 0,
            g.transform.rotation.eulerAngles.z + addRotation);
    }

    
    protected override void MoveStart()
    {
        if (shape==null) return;
        if (owner is OTObject)
            (owner as OTObject).position = shape.startPosition;
        else
            if (owner is GameObject)
                (owner as GameObject).transform.position = shape.startPosition;
        previousPosition = shape.startPosition;
        HandleOrientation(shape.GetPosition(0.1f));
        _time = duration * customPosition;

        if (onStart != null)
            onStart(shape,owner);
        if (!CallBack("onStart", new object[] { shape, owner }))
            CallBack("OnStart", new object[] { shape, owner });

    }
	
	private void SetPosition()
	{
        float pos = easing.ease(time, 0, 1, duration);
        if (oppositeDirection)
            pos = 1 - pos;
        Vector2 nPos = shape.GetPosition(pos);

        if (owner is OTObject)
            (owner as OTObject).position = nPos;
        else
            if (owner is GameObject)
                (owner as GameObject).transform.position = nPos;

        if (time > 0) 
            HandleOrientation(nPos);
		
	}

    
    protected override void Move()
    {
        if (shape == null) return;

        if (customControl)
        {
            _time = customPosition * duration;
            if (customPosition == 1)
                _time -= 0.001f;
        }

        if (time >= duration)
        {
            if (looping)
            {
                if (onFinish != null)
                    onFinish(shape, owner);
                if (!CallBack("onFinish", new object[] { shape, owner }))
                    CallBack("OnFinish", new object[] { shape, owner });
                while (time >= duration)
                    _time -= duration;
                if (onStart != null)
                    onStart(shape, owner);
                if (!CallBack("onStart", new object[] { shape, owner }))
                    CallBack("OnStart", new object[] { shape, owner });
            }
            else
            {
                _time = duration;
                if (onFinish != null)
                    onFinish(shape, owner);
                if (!CallBack("onFinish", new object[] { shape, owner }))
                    CallBack("OnFinish", new object[] { shape, owner });
				Stop();
            }
        }
		
		SetPosition();

    }

}
