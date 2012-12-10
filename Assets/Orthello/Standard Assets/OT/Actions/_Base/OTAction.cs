using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Action base class
/// </summary>
public class OTAction  {

    /// <summary>
    /// Action name
    /// </summary>
    public string name
    {
        get
        {
            return _name;
        }
    }

    /// <summary>
    /// Action duration
    /// </summary>
    public float duration
    {
        get
        {
            return _duration;
        }
        set
        {
            _duration = value;
        }
    }

    /// <summary>
    /// Action speed - Inc- decrease time.
    /// </summary>
    public float speed
    {
        get
        {
            return _speed;
        }
        set
        {
            _speed = value;
        }
    }

    /// <summary>
    /// Number of executes
    /// </summary>
    public float count
    {
        get
        {
            return _count;
        }
        set
        {
            _count = value;
        }
    }

    /// <summary>
    /// Action is running when true
    /// </summary>
    public bool running
    {
        get
        {
            if (pauze)
                return false;
            else
                return _running;
        }
    }

    /// <summary>
    /// Action is pauzed when true
    /// </summary>
    public bool pauzed
    {
        get
        {
            return pauze;
        }
    }

    
    public string _name = "";
    
    public float updateFrequency = 0.0f;

    
    protected virtual bool Execute()
    {
        return true;
    }

    
    protected virtual void Initialize()
    {
        return;
    }

    bool initialized = false;
    float deltaTime = 0.0f;
    private bool _running = false;
    private bool pauze = false;

    
    protected float _duration = 1;
    
    protected float _speed = 1;
    
    protected float _count = 0;
    
    protected object owner;
    
    protected string ownerActionName = "action";
    
    protected float time = 0.0f;

    
    public void SetOwner(object owner, string name)
    {
        this.owner = owner;
        this.ownerActionName = name;
    }

    
    public bool Update(float deltaTime)
    {
        if (!initialized)
        {
            Initialize();
            initialized = true;
        }
        if (running)
        {
            this.deltaTime += deltaTime;
            if (this.deltaTime > updateFrequency)
            {
                time += this.deltaTime * _speed;
                this.deltaTime = 0;
                if (Execute())
                {
                    if (count > 1)
                    {
                        count--;
                        Start();
                    }
                    else
                        if (count == -1)
                            Start();
                        else
                        {
                            count = 1;
                            return true;
                        }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Starts this action
    /// </summary>
    public void Start()
    {
        time = 0;
        initialized = false;
        _running = true;
    }

    /// <summary>
    /// Pauzes the action
    /// </summary>
    public void Pauze()
    {
        pauze = true;
    }

    /// <summary>
    /// Resumes the action
    /// </summary>
    public void Resume()
    {
        pauze = false;
    }

    /// <summary>
    /// Stops the action
    /// </summary>
    public void Stop()
    {
        _running = false;
    }

    /// <summary>
    /// Completes the action
    /// </summary>
    public void Complete()
    {
        time = duration;
        Execute();
        Stop();
    }

    /// <summary>
    /// Action constructor
    /// </summary>
    /// <param name="name">Action name</param>
    public OTAction(string name)
    {
        _name = name;
    }

}
