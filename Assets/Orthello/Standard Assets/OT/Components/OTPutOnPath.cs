using UnityEngine;
using System.Collections;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Add this 'script' component to a OTObject to setup path movement in the editor
/// </summary>
[ExecuteInEditMode]
public class OTPutOnPath : MonoBehaviour {

    /// <summary>
    /// Shape to use as a path
    /// </summary>
    public OTShape shape = null;
    /// <summary>
    /// Movement duration in seconds
    /// </summary>
    public float duration = 1;
    /// <summary>
    /// Movement speed in pixels/units per second ( <=0 is ignored
    /// </summary>
    public float speed = 0;
    /// <summary>
    /// Object will be looping
    /// </summary>
    public bool looping = true;
    /// <summary>
    /// Add rotation to the object 
    /// </summary>
    public float addRotation = 0;
    /// <summary>
    /// Object Heading type
    /// </summary>
    public OTPathController.UpVector upVector = OTPathController.UpVector.Follow;
    /// <summary>
    /// Manual position control
    /// </summary>
    public bool customControl = false;
    /// <summary>
    /// Manual position 
    /// </summary>
    public float customPosition = 0.0f;
    /// <summary>
    /// object to target when upVector = Target
    /// </summary>
    public GameObject targetObject = null;
    /// <summary>
    /// Control movement preview
    /// </summary>
    public int previewProgress = 0;

    private OTPathController sp = null;
    private OTShape _shape = null;
    private float _duration = 1;
    private float _speed = 0;
    private bool _looping = true;
    private float _addRotation = 0;
    private OTPathController.UpVector _upVector = OTPathController.UpVector.Follow;

    private OTObject sprite;

    void Clean()
    {
        _shape = shape;
        _duration = duration;
        _speed = speed;
        _looping = looping;
        _addRotation = addRotation;
        _upVector = upVector;
    }

    void SetController()
    {
        sprite = GetComponent<OTObject>();
        if (sprite!=null)
            sp = shape.IsPathFor(sprite, duration, looping, addRotation);
        else
            sp = shape.IsPathFor(gameObject, duration, looping, addRotation);
		
		if (speed > 0)
			sp.duration = shape.DurationWithSpeed(speed);
        sp.customControl = customControl;
        sp.customPosition = customPosition;
        sp.upVector = upVector;
		sp.targetObject = targetObject;
    }

	// Use this for initialization
	void Start () {
        if (shape != null && sp == null)
            SetController();
        Clean();
	}

    void Update()
    {
        if (shape != null && sp == null)
            SetController();

        if (customPosition < 0) customPosition = 0;
        if (customPosition > 1) customPosition = 1;

        if (sp != null)
        {
            if (_shape != shape)
            {
                SetController();
                Clean();
            }
            else
            {
                if (_duration!=duration || _speed!=speed || _looping!=looping || _upVector!=upVector || _addRotation!=addRotation)
                {
					if (speed > 0)
						sp.duration = shape.DurationWithSpeed(speed);
					else
                    	sp.duration = duration;
                    sp.upVector = upVector;
                    sp.addRotation = addRotation;
                    sp.looping = looping;
                    Clean();
                }
            }

            if (!Application.isPlaying)
            {
                if (previewProgress < 0) previewProgress = 0;
                if (previewProgress > 100) previewProgress = 100;
                sp.customControl = true;
                sp.customPosition = previewProgress / 100f;
            }
        }

    }
	
}
