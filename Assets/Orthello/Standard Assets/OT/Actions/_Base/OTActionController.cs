using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// <b><a href="http://www.wyrmtale.com/products/unity3d-components/orthello-pro" target="_blank" >PRO</a></b> 
/// : Controller that can execute actions or action trees
/// </summary>
public class OTActionController : OTController {

    /// <summary>
    /// Action delegate
    /// </summary>
    /// <param name="action">The action of the event</param>
    public delegate void ActionDelegate(OTAction action);
    
    /// <summary>
    /// Called when an action starts
    /// </summary>
    public ActionDelegate onStartAction = null;
    /*
    /// <summary>
    /// Called when an action pauzes
    /// </summary>
    public ActionDelegate onPauzeAction = null;
    /// <summary>
    /// Called when a pauzed action resumes
    /// </summary>
    public ActionDelegate onResumeAction = null;
     */
    /// <summary>
    /// Called when an action stops
    /// </summary>
    public ActionDelegate onStopAction = null;

    /// <summary>
    /// Action tree delegate
    /// </summary>
    /// <param name="actionTree">The action tree of the event</param>
    public delegate void ActionTreeDelegate(OTActionTree actionTree);

    /// <summary>
    /// Called when an action tree starts
    /// </summary>
    public ActionTreeDelegate onStartTree = null;
    /*
    /// <summary>
    /// Called when an action tree pauzes
    /// </summary>
    public ActionTreeDelegate onPauzeTree = null;
    /// <summary>
    /// Called when a pauzed action tree resumes
    /// </summary>
    public ActionTreeDelegate onResumeTree = null;
     */
    /// <summary>
    /// Called when an action tree stops
    /// </summary>
    public ActionTreeDelegate onStopTree = null;

    Dictionary<string, OTAction> actions = new Dictionary<string, OTAction>();
    List<OTAction> actionList = new List<OTAction>();
    List<string> actionNames = new List<string>();
    List<OTAction> runningActions = new List<OTAction>();

    List<string> nextNames = new List<string>();
    List<float> nextSpeeds = new List<float>();
    List<int> nextCounts = new List<int>();

    Dictionary<string, OTActionTree> actionTrees = new Dictionary<string, OTActionTree>();
    OTActionTree runningTree = null;
    List<OTAction> runningTreeActions = new List<OTAction>();
    List<OTActionTreeElement> runningTreeElements = new List<OTActionTreeElement>();
    float treeSpeed = 1;
    int treeCount = 1;

    /// <summary>
    /// Action controller constructor 
    /// </summary>
    /// <param name="owner">Object that has to be controlled</param>
    /// <param name="name">Name of this controller</param>
    public OTActionController(Object owner, string name)
        : base(owner, name)
    {
    }

    /// <summary>
    /// Action controller constructor 
    /// </summary>
    /// <param name="name">Name of this controller</param>
    public OTActionController(string name)
        : base(name)
    {
    }

    /// <summary>
    /// Action controller constructor 
    /// </summary>
    public OTActionController()
        : base()
    {
    }

    
    protected override void Initialize()
    {
        Actions();
    }

    
    protected virtual void Actions()
    {
        Add("Wait", new OTActionWait(1));
        Add("Destroy", new OTActionDestroy());
        ActionTrees();
    }

    
    protected virtual void ActionTrees()
    {        
    }

    /// <summary>
    /// Finds an action tree
    /// </summary>
    /// <param name="name">Name of the action tree to find</param>
    /// <returns>Action tree or null if none was found</returns>
    public OTActionTree FindTree(string name)
    {
        name = name.ToLower();
        if (actionTrees.ContainsKey(name))
            return actionTrees[name];
        else
            return null;
    }

    /// <summary>
    /// Checks if an action or action tree is running
    /// </summary>
    /// <param name="name">Action name or action tree name</param>
    /// <returns>True if found and running</returns>
    public bool IsRunning(string name)
    {
        name = name.ToLower();
        if (actionTrees.ContainsKey(name))
        {
            if (runningTree != null && runningTree.name == name)
                return true;
        }
        else
        {
            if (actionNames.Contains(name))
            {
                if (actions[name].running)
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Adds an action to this action controller
    /// </summary>
    /// <param name="name">Action name</param>
    /// <param name="action">Action object</param>
    public void Add(string name, OTAction action)
    {
        name = name.ToLower();
        if (!actions.ContainsKey(name))
        {
            actions.Add(name, action);
            actionList.Add(action);
            actionNames.Add(name);
            action.SetOwner(owner, name);
        }
    }

    /// <summary>
    /// Adds an action tree to this action controller
    /// </summary>
    /// <param name="name">Action tree name</param>
    /// <param name="actionTree">Action tree object</param>
    public void Add(string name, OTActionTree actionTree)
    {
        name = name.ToLower();
        if (!actionTrees.ContainsKey(name))
        {
            actionTrees.Add(name, actionTree);
            actionTree.SetName(name);
        }
    }

    /// <summary>
    /// Adds the action tree of an tree element
    /// </summary>
    /// <param name="name">Action tree name</param>
    /// <param name="element">Action tree element</param>
    public void Add(string name, OTActionTreeElement element)
    {
        Add(name, element.tree);
    }

    void RunTree(OTActionTree tree)
    {
        if (runningTree != null)
        {
            while (runningTreeActions.Count > 0)
            {
                OTAction a = runningTreeActions[0];
                a.Complete();
                if (onStopAction != null)
                    onStopAction(a);
                runningTreeActions.Remove(a);
            }
            if (onStopTree != null)
                onStopTree(runningTree);
            if (!CallBack("onStopTree", new object[] { runningTree }))
                CallBack("OnStopTree", new object[] { runningTree });
            runningTreeElements.Clear();
        }
        runningTree = tree;
        if (onStartTree != null)
            onStartTree(runningTree);
        if (!CallBack("onStartTree", new object[] { runningTree }))
            CallBack("OnStartTree", new object[] { runningTree });
        if (owner != null)
            RunTreeElements(tree.rootElements);
    }

    void RunTreeElements(List<OTActionTreeElement> elements)
    {
        for (int e = 0; e < elements.Count; e++)
        {
            OTActionTreeElement el = elements[e];
            string name = el.name.ToLower();
            if (actions.ContainsKey(name))
            {
                if (runningActions.Contains(actions[name]))
                {
                    actions[name].Stop();
                    if (onStopAction != null)
                        onStopAction(actions[name]);
                    runningActions.Remove(actions[name]);
                }
                if (el.duration>0)
                    actions[name].duration = el.duration;
                actions[name].speed = treeSpeed;
                actions[name].count = 1;
                actions[name].Start();
                runningTreeActions.Add(actions[name]);
                runningTreeElements.Add(el);
                if (onStartAction != null)
                    onStartAction(actions[name]);
                if (!CallBack("onStartAction", new object[] { actions[name] }))
                    CallBack("OnStartAction", new object[] { actions[name] });
            }
        }
    }

    
    public override void SetOwner(Object owner)
    {
        base.SetOwner(owner);
        for (int a = 0; a < actionList.Count; a++)
            actionList[a].SetOwner(owner,actionNames[a]);
    }
	
	/// <summary>
	/// Stop all running actions
	/// </summary>
    public void Stop()
    {
        runningTreeActions.Clear();
        runningActions.Clear();
        runningTree = null;
    }

    void RunAction(OTAction action)
    {
        if (!runningTreeActions.Contains(action))
        {
            action.Start();
            if (!runningActions.Contains(action))
                runningActions.Add(action);
            if (onStartAction != null)
                onStartAction(action);
            if (!CallBack("onStartAction", new object[] { action }))
                CallBack("OnStartAction", new object[] { action });
        }
    }

    /// <summary>
    /// Runs an action or action tree
    /// </summary>
    /// <param name="name">Name of action or action tree</param>
    /// <param name="speed">Run speed</param>
    /// <param name="count">Number of times to run</param>
	public void Run(string name, float speed, int count)
	{
        if (speed <= 0) speed = 1;
        name = name.ToLower();
        if (actionTrees.ContainsKey(name))
        {
            if (runningTree != null && runningTree.name == name && runningTreeActions.Count>0)
            {
                if (treeCount == -1) return;
                else
                    Next(name, speed, count);
                return;
            }
            this.treeSpeed = speed;
            this.treeCount = count;
            RunTree(actionTrees[name]);
        }
        else
        {
            if (actions.ContainsKey(name))
            {
                actions[name].speed = speed;
                actions[name].count = count;
                RunAction(actions[name]);
            }
            else
            {
                nextNames.Add(name);
                nextSpeeds.Add(speed);
                nextCounts.Add(count);
            }
        }
	}

    /// <summary>
    /// Runs an action or action tree
    /// </summary>
    /// <param name="name">Name of action or action tree</param>
    public void Run(string name)
    {
        Run(name, speed, 1);
    }

    /// <summary>
    /// Runs an action or action tree
    /// </summary>
    /// <param name="name">Name of action or action tree</param>
    /// <param name="speed">Run speed</param>
    public void Run(string name, float speed)
    {
        Run(name, speed, 1);
    }

    /// <summary>
    /// Sets an action or action tree in the running queue
    /// </summary>
    /// <param name="name">Name of action or tree to add</param>
    /// <param name="speed">Run speed</param>
    /// <param name="count">Number of times to run</param>
    /// <param name="onlyOne">This action or tree can only appear one time in the queue</param>
    public void Next(string name, float speed, int count, bool onlyOne)
    {
        if (speed <= 0) speed = 1;
        if (runningActions.Count == 0 && runningTreeActions.Count == 0)
            Run(name, speed, count);
        else
        {
            if (!onlyOne || (onlyOne && !nextNames.Contains(name)))
            {
                nextNames.Add(name);
                nextSpeeds.Add(speed);
                nextCounts.Add(count);
            }
        }         
    }

    /// <summary>
    /// Sets an action or action tree in the running queue
    /// </summary>
    /// <param name="name">Name of action or tree to add</param>
    /// <param name="speed">Run speed</param>
    /// <param name="count">Number of times to run</param>
    public void Next(string name, float speed, int count)
    {
        Next(name, speed, count, true);
    }

    /// <summary>
    /// Sets an action or action tree in the running queue
    /// </summary>
    /// <param name="name">Name of action or tree to add</param>
    /// <param name="speed">Run speed</param>
    public void Next(string name, float speed)
    {
        Next(name, speed, 1, true);
    }

    /// <summary>
    /// Sets an action or action tree in the running queue
    /// </summary>
    /// <param name="name">Name of action or tree to add</param>
    public void Next(string name)
    {
        Next(name, speed, 1, true);
    }

    
    protected override void Update()
    {
        base.Update();

        int r = 0;
        // update single running actions
        if (runningActions.Count > 0)
        {
            r = 0;
            while (r < runningActions.Count)
            {
                OTAction a = runningActions[r];
                if (a.Update(deltaTime))
                {
                    if (runningActions.Count == 0) return;

                    if (a.count > 0) a.count--;
                    if (a.count > 0 || a.count == -1)
                        a.Start();
                    else
                    {
                        a.Stop();
                        if (onStopAction != null)
                            onStopAction(a);
                        if (!CallBack("onStopAction", new object[] { a }))
                            CallBack("OnStopAction", new object[] { a });
                        runningActions.Remove(a);
                    }
                }
                else
                    r++;
                if (runningActions.Count == 0) return;
            }
        }
        // update running tree actions
        if (runningTreeActions.Count > 0)
        {
            string currentRuningTree = runningTree.name;
            r = 0;
            List<OTActionTreeElement> closedElements = new List<OTActionTreeElement>();
            while (r < runningTreeActions.Count)
            {
                OTAction a = runningTreeActions[r];
                if (a.Update(deltaTime))
                {
					if (runningTree == null) return;
                    if (runningTree.name != currentRuningTree || runningTreeActions.Count==0)
                        return;

                    a.Stop();
                    if (onStopAction != null)
                        onStopAction(a);
                    if (!CallBack("onStopAction", new object[] { a }))
                        CallBack("OnStopAction", new object[] { a });

                    closedElements.Add(runningTreeElements[r]);
                    runningTreeActions.Remove(a);
                    runningTreeElements.RemoveAt(r);
                }
                else
                {
                    if (runningTree.name != currentRuningTree || runningTreeActions.Count==0)
                        return;
                    r++;
                }
            }

            if (closedElements.Count > 0)
            {
                while (closedElements.Count > 0)
                {
                    OTActionTreeElement el = closedElements[0];
                    if (el.children.Count > 0)
                        RunTreeElements(el.children);
                    closedElements.Remove(el);
                }
            }

            if (runningTreeActions.Count == 0)
            {
                // tree has ended
                if (treeCount > 1 || treeCount == -1)
                {
                    if (treeCount > 1)
                        treeCount--;
                    RunTree(runningTree);
                }
                else
                {
                    if (onStopTree != null)
                        onStopTree(runningTree);
                    runningTree = null;
                }
            }
        }
        else
        {
            if (runningTree != null && owner != null)
                RunTree(runningTree);
        }


        if (nextNames.Count != 0 && runningTreeActions.Count == 0 && runningActions.Count == 0 && owner!=null )
        {
            Run(nextNames[0], nextSpeeds[0], nextCounts[0]);
            nextNames.RemoveAt(0);
            nextSpeeds.RemoveAt(0);
            nextCounts.RemoveAt(0);
        }

    }

}
