#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(OTSpline))]
public class OTSplineEditor : Editor 
{
    OTSpline spline;
    List<OTSplinePoint> splinePoints = new List<OTSplinePoint>();
    List<OTSplinePoint> selectedPoints = new List<OTSplinePoint>();
    List<int> selectedPointIndexes = new List<int>();
    Dictionary<string, Texture2D> editorImages = new Dictionary<string, Texture2D>();

    float scale = 1;
    int lastControl;
    bool initialized = false;

		
	Vector3 DrawControlPoint(Vector3 pos, Vector3 lineTo)
	{

        Handles.color = spline.lineColor;
		//Handles.DrawSolidDisc(pos,Vector3.forward,0.042f * scale);
        Handles.DrawLine(pos, lineTo);
        Handles.color = spline.handleColor;
        Handles.DrawSolidDisc(pos, Vector3.forward, 0.040f * scale);
        return Handles.FreeMoveHandle(pos, Quaternion.identity, 0.054f * scale, Vector3.zero, Handles.CircleCap);
	}
	
	Vector3 DrawPathPoint(Vector3 pos, bool selected)
	{
        Handles.color = spline.handleColor;
		Handles.DrawSolidDisc(pos,Vector3.forward,0.050f * scale);
        if (selected)
            Handles.color = spline.selectedColor;
        else
            Handles.color = spline.pointColor;
        Handles.DrawSolidDisc(pos, Vector3.forward, 0.045f * scale);
        return Handles.FreeMoveHandle(pos, Quaternion.identity, 0.045f * scale, Vector3.zero, Handles.CircleCap);
	}

    bool SelectedSplinePoint()
    {
        return false;
    }


    bool importTexture2DforGUI = false;
    Object[] LoadAssetsAtPath(string path, System.Type type)
    {
        List<Object> objects = new List<Object>();
        string loadPath = Path.GetFullPath(path);
        if (Directory.Exists(loadPath))
        {
            string[] fileEntries = Directory.GetFiles(loadPath);

            foreach (string fileName in fileEntries)
            {
                string fname = Path.GetFileName(fileName);
                if (type == typeof(Texture2D) && importTexture2DforGUI)
                {
					try
					{
	                    TextureImporter importer = TextureImporter.GetAtPath(path + "/" + fname) as TextureImporter;						
						if (importer != null)
						{
		                    if (importer.textureFormat != TextureImporterFormat.ARGB32 || importer.mipmapEnabled ||
		                        importer.npotScale != TextureImporterNPOTScale.None)
		                    {
		                        importer.textureFormat = TextureImporterFormat.ARGB32;
		                        importer.mipmapEnabled = false;
		                        importer.npotScale = TextureImporterNPOTScale.None;
		                        AssetDatabase.ImportAsset(path + "/" + fname, ImportAssetOptions.ForceUpdate);
		                    }
						}
					}
					catch(System.Exception)
					{						
					}
                }
                Object o = AssetDatabase.LoadAssetAtPath(path + "/"+ fname, type);
                if (o != null)
                    objects.Add(o);
            }
            return objects.ToArray();
        }
        return null;
    }

    Object[] LoadAssets(string path, System.Type type)
    {
        int pi = 0;
        Object[] assets = null;
        string  loadPath = "";
        while (assets == null)
        {
            switch(pi)
            {
                case 0: loadPath = "Assets/Orthello/Standard Assets/"+path; break;
                case 1: loadPath = "Assets/Standard Assets/" + path; break;
                case 2: loadPath = "Assets/Plugins/" + path; break;
                case 3: loadPath = "Assets/" + path; break;

            }
            assets = LoadAssetsAtPath(loadPath, type);
            pi++;
            if (pi == 4) break;
        }       
        return assets;
    }

    void LoadEditorImages()
    {
        importTexture2DforGUI = true;
        Object[] images = LoadAssets("Editor/Images", typeof(Texture2D));
        importTexture2DforGUI = false;
        if (images != null && images.Length > 0)
        {
            for (int i = 0; i < images.Length; i++)
                editorImages.Add(images[i].name, images[i] as Texture2D);
        }
    }

    void SetSplinePointType(OTSplinePoint point, OTSplinePoint.OTSplinePointType type)
    {
        if (point.pointType != type)
        {
            if (type == OTSplinePoint.OTSplinePointType.Align)
            {
                float d1 = point.ctrl1.magnitude;
                float d2 = point.ctrl2.magnitude;
                Vector2 v = point.ctrl1 - point.ctrl2;
                point.ctrl1 = (v.normalized * d1);
                point.ctrl2 = (v.normalized * d2) * -1;
            }
            point.pointType = type;
        }
    }

    void SetControlPoint(OTSplinePoint point, int ctrl)
    {
        float d1 = point.ctrl1.magnitude;
        float d2 = point.ctrl2.magnitude;
        Vector2 v;
        switch (ctrl)
        {
            case 1:
                v = point.ctrl2 * -1;
                point.ctrl1 = (v.normalized * d1);
                break;
            case 2:
                v = point.ctrl1 * -1;
                point.ctrl2 = (v.normalized * d2);
                break;
        }
    }


    void HandleEvents()
    {
        Event e = Event.current;
        switch (e.type)
        {
            case EventType.MouseDown :
                if (e.shift)
                {
                    // add spline point to selection if one under mouse
                    int sp = MouseSplinePoint();
                    if (sp != -1)
                    {
                        if (e.button == 0)
                        {
                            if (!selectedPointIndexes.Contains(sp))
                            {
                                selectedPointIndexes.Add(sp);
                                selectedPoints.Add(splinePoints[sp]);
                            }
                        }
                        else
                            if (e.button == 1)
                            {
                                if (selectedPointIndexes.Contains(sp))
                                    if (selectedPointIndexes.Count > 1)
                                    {
                                        selectedPointIndexes.Remove(sp);
                                        selectedPoints.Remove(splinePoints[sp]);
                                    }
                            }
                    }
                    lastControl = 0;
                }
                break;
        }
    }


    int MouseSplinePoint()
    {
        float dist;
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Plane p = new Plane(Vector3.forward, spline.transform.position.z);
        if (p.Raycast(ray, out dist))
        {
            Vector3 point = ray.origin + (ray.direction * dist);
            for (int sp = 0; sp < splinePoints.Count; sp++)
            {
                Vector2 checkpoint = spline.transform.position;
                checkpoint += splinePoints[sp].position;
                dist = Vector2.Distance(checkpoint, point);
                if (dist < (0.075f * scale))
                    return sp;
            }
        }
        return -1;
    }

    void MoveSplinePoints(Vector2 delta)
    {
        for (int p = 0; p < selectedPoints.Count; p++)
            selectedPoints[p].position += delta;
    }

    void CheckActiveSplinePoint()
    {
        if (GUIUtility.hotControl != lastControl && GUIUtility.hotControl != 0 && !Event.current.shift)
        {
            int sp = MouseSplinePoint();
            if (sp != -1)
            {
                selectedPointIndexes.Clear();
                selectedPoints.Clear();
                selectedPointIndexes.Add(sp);
                selectedPoints.Add(splinePoints[sp]);
            }
            lastControl = GUIUtility.hotControl;
        }
    }

    Vector3 _p(Vector2 pos)
    {
        return new Vector3(pos.x, pos.y, spline.transform.position.z);
    }

    void DrawBezier(OTSplinePoint p1, OTSplinePoint p2)
    {
        Vector2 lp = Vector2.zero;
        Handles.color = spline.lineColor;
        Vector2 sp = spline.transform.position;
        //loop through 100 steps of the curve
        for (float u = 0; u <= 1; u += (float)1 / spline.resolution)
        {
            Vector2 pos = spline.BezierPoint(p1,p2,u);
            if (u == 0)
                Handles.DrawLine(_p(sp + p1.position), _p(sp + pos));
            else
                Handles.DrawLine(_p(sp + lp), _p(sp + pos));
            lp = pos;
        }
        //Let the curve end on the second anchorPoint     
        Handles.DrawLine(_p(sp + lp), _p(sp + p2.position));
    }


    void OnSceneGUI()
    {

        if (spline.resolution < 1) spline.resolution = 1;

        Camera.current.orthographic = true;
        scale = Camera.current.orthographicSize/2.5f;

        if (editorImages.Count == 0)
            LoadEditorImages();

        if (!initialized)
            if (!Init())
                return;

		if (spline != null)
		{

            HandleEvents();
            CheckActiveSplinePoint();

			Vector2 p = spline.transform.position;
            spline.transform.position = new Vector3(p.x, p.y, spline.depth);

            Vector2 topPoint = Vector2.zero;

            Vector2 pw = Vector2.zero;
            Vector2 cw = Vector2.zero;

            for (int pi = 0; pi < splinePoints.Count - 1; pi++)
            {
                OTSplinePoint point1 = splinePoints[pi];
                OTSplinePoint point2 = splinePoints[pi+1];
                OTSplinePoint pointLast = splinePoints[splinePoints.Count - 1];

                if (pi == 0 && spline.closed)
                {
                    DrawBezier(pointLast, point1);

                    pw = DrawControlPoint(_p(p + point1.position + point1.ctrl1), _p(p + point1.position));
                    cw = point1.ctrl1;
                    point1.ctrl1 = pw - (p + point1.position);
                    if (point1.pointType == OTSplinePoint.OTSplinePointType.Align && !Vector2.Equals(cw, point1.ctrl1))
                        SetControlPoint(point1, 2);

                    pw = DrawControlPoint(_p(p + pointLast.position + pointLast.ctrl2), _p(p + pointLast.position));
                    cw = pointLast.ctrl2;
                    pointLast.ctrl2 = pw - (p + pointLast.position);
                    if (pointLast.pointType == OTSplinePoint.OTSplinePointType.Align && !Vector2.Equals(cw, pointLast.ctrl2))
                      SetControlPoint(pointLast, 1);

                }

                DrawBezier(point1, point2);

                pw = DrawControlPoint(_p(p + point1.position + point1.ctrl2), _p(p + point1.position));
                cw = point1.ctrl2;
                point1.ctrl2 = pw - (p + point1.position);
                if (point1.pointType == OTSplinePoint.OTSplinePointType.Align && !Vector2.Equals(cw, point1.ctrl2))
                    SetControlPoint(point1, 1);

                pw = DrawControlPoint(_p(p + point2.position + point2.ctrl1), _p(p + point2.position));
                cw = point2.ctrl1;
                point2.ctrl1 = pw - (p + point2.position);
                if (point2.pointType == OTSplinePoint.OTSplinePointType.Align && !Vector2.Equals(cw, point2.ctrl1))
                    SetControlPoint(point2, 2);

                pw = DrawPathPoint(_p(p + point1.position), selectedPoints.Contains(point1));
                if (!Vector2.Equals(point1.position, pw - p))
                    MoveSplinePoints((pw - p) - point1.position);

                point2 = splinePoints[pi + 1];
                pw = DrawPathPoint(_p(p + point2.position), selectedPoints.Contains(point2));
                if (!Vector2.Equals(point2.position, pw - p))
                    MoveSplinePoints((pw - p) - point2.position);

                if (point1.position.y > topPoint.y || Vector2.Equals(topPoint,Vector2.zero))
                    topPoint = point1.position;
                if (point2.position.y > topPoint.y)
                    topPoint = point2.position;


            }

            spline.points = splinePoints.ToArray();


            spline._pointPosition = selectedPoints[0].position;
            spline._pointType = selectedPoints[0].pointType;
            spline._pointIndex = selectedPointIndexes[0];

            Handles.BeginGUI();
            OnGUI();
            Handles.EndGUI();
			
			if (spline.meshMode == OTShape.MeshMode.Line)
				spline.MarkMeshDirty();
			
			UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(spline);
			
        }
    }


    OTSplinePoint SubdivideSpline(OTSplinePoint p1, OTSplinePoint p2)
    {
        OTSplinePoint np = new OTSplinePoint();

        // np.position = spline.BezierPoint(p1, p2, 0.5f);

        Vector2 c1 = p1.position + p1.ctrl2;
        Vector2 c2 = p2.position + p2.ctrl1;

        Vector2 pc1 = p1.position + (p1.ctrl2/2);
        Vector2 pc2 = p2.position + (p2.ctrl1/2);
        Vector2 cc = c1 + ((c2 - c1) / 2);
        Vector2 pcc1 = pc1 + ((cc - pc1) / 2);
        Vector2 pcc2 = pc2 + ((cc - pc2) / 2);

        np.position = pcc1 + ((pcc2 - pcc1) / 2);
        np.ctrl1 = pcc1 - np.position;
        np.ctrl2 = pcc2 - np.position;
        np.pointType = p1.pointType;

        p1.ctrl2 = pc1 - p1.position;
        p2.ctrl1 = pc2 - p2.position;

        return np;
    }

    void AddSplinePoints()
    {
        List<OTSplinePoint> points = new List<OTSplinePoint>(selectedPoints);
        while (points.Count > 0)
        {
            OTSplinePoint p1 = points[0];
            OTSplinePoint p2 = null;
            int i = splinePoints.IndexOf(p1);

            while (i >= 1 && points.Contains(splinePoints[i - 1]))
                p1 = splinePoints[--i];                

            if (i == splinePoints.Count - 1)
                p2 = splinePoints[0];
            else
                p2 = splinePoints[i + 1];


            if (spline.closed && p1 == splinePoints[0] && points.Contains(splinePoints[splinePoints.Count - 1]))
            {
                OTSplinePoint np = SubdivideSpline(splinePoints[splinePoints.Count - 1], splinePoints[0]);
                splinePoints.Add(np);
                selectedPoints.Insert(0, np);
            }

            if (points.Contains(p2))
            {
                OTSplinePoint wp = p1;
                while (points.Contains(p2))
                {
                    OTSplinePoint np = SubdivideSpline(wp, p2);
                    points.Remove(p2);
                    splinePoints.Insert(i+1, np);
                    selectedPoints.Insert(0,np);
                    i+=2;
                    wp = p2;
                    if (i < splinePoints.Count - 1)
                        p2 = splinePoints[i + 1];
                    else
                       break;
                }
            }
            else
            {
                OTSplinePoint np = SubdivideSpline(p1, p2);
                if (i == splinePoints.Count - 1)
                    splinePoints.Add( np);
                else
                    splinePoints.Insert(i+1, np);
                selectedPoints.Insert(0,np);
            }
            points.Remove(p1);
        }

        selectedPointIndexes.Clear();
        for (int p = 0; p < selectedPoints.Count; p++)
            selectedPointIndexes.Add(splinePoints.IndexOf(selectedPoints[p]));

    }

    void RemoveSplinePoint(OTSplinePoint point)
    {
        splinePoints.Remove(point);
        selectedPoints.Remove(point);
    }

    void DeleteSplinePoints()
    {
        if (splinePoints.Count > 2)
        {
            while (selectedPoints.Count>0)
            {
                if (splinePoints.Count == 2) return;
                RemoveSplinePoint(selectedPoints[0]);
                break;
            }
        }
        if (selectedPoints.Count == 0)
            selectedPoints.Add(splinePoints[0]);
        selectedPointIndexes.Clear();
        for (int p = 0; p < selectedPoints.Count; p++)
            selectedPointIndexes.Add(splinePoints.IndexOf(selectedPoints[p]));

    }

    void PointsAlign()
    {
        if (splinePoints.Count == 2)
        {
            OTSplinePoint sp1 = splinePoints[0];
            OTSplinePoint sp2 = splinePoints[1];

            sp1.pointType = OTSplinePoint.OTSplinePointType.Align;
            sp2.pointType = OTSplinePoint.OTSplinePointType.Align;


            Vector2 v = (sp2.position - sp1.position)/3;
            Matrix4x4 m = new Matrix4x4();
            m.SetTRS(Vector3.zero, Quaternion.Euler(0, 0, -90), Vector3.one);
            v = m.MultiplyPoint3x4(v);

            sp1.ctrl1 = v;
            sp1.ctrl2 = v * -1;
            sp2.ctrl1 = v * -1;
            sp2.ctrl2 = v;


        }
        else
        for (int p = 0; p < splinePoints.Count; p++)
        {
            OTSplinePoint sp = splinePoints[p];
            OTSplinePoint spp, spn;
            if (p == 0) spp = splinePoints[splinePoints.Count - 1];
            else spp = splinePoints[p - 1];
            if (p == splinePoints.Count - 1) spn = splinePoints[0];
            else spn = splinePoints[p + 1];


            sp.pointType = OTSplinePoint.OTSplinePointType.Align;


            Vector2 v = (spp.position - spn.position) / 3;
            sp.ctrl1 = v;
            sp.ctrl2 = v * -1;


        }
    }

    void PointsFree()
    {
        for (int p = 0; p < splinePoints.Count; p++)
        {
            OTSplinePoint sp = splinePoints[p];
            OTSplinePoint spp, spn;
            if (p == 0) spp = splinePoints[splinePoints.Count - 1];
            else spp = splinePoints[p - 1];
            if (p == splinePoints.Count-1) spn = splinePoints[0];
            else spn = splinePoints[p+1];


            sp.pointType = OTSplinePoint.OTSplinePointType.Free;
            sp.ctrl1 = (spp.position - sp.position) / 3;
            sp.ctrl2 = (spn.position - sp.position) / 3;            
        }
    }

    Vector2 guiPos = new Vector2(0, 0);
    Texture2D guiImg = null;

    Rect GUIRect()
    {
        if (guiImg != null)
            return new Rect(guiPos.x, guiPos.y, guiImg.width, guiImg.height);
        else
            return new Rect(0, 0, 1, 1);
    }

    Texture2D GUIImage(string image)
    {
        if (editorImages.ContainsKey(image))
        {
            guiImg = editorImages[image];
            GUI.DrawTexture(GUIRect(), guiImg);
            return guiImg;
        }
        return null;
    }

    bool GUIButton(string image)
    {
        if (editorImages.ContainsKey(image))
        {
            guiImg = editorImages[image];
            return GUI.Button(GUIRect(), guiImg);
        }
        return false;
    }

    bool GUIIconButton(string image, string icon, string iconOver, string info)
    {
        bool res = GUIButton(image);

        bool over = false;
        if (Event.current.mousePosition.x >= guiPos.x &&
            Event.current.mousePosition.x < guiPos.x + guiImg.width &&
            Event.current.mousePosition.y >= guiPos.y &&
            Event.current.mousePosition.y < guiPos.y + guiImg.height)
            over = true;

        GUIImage(image + (over ? "-on" : ""));
        if (over)
        {
            GUI.skin.box.normal.textColor = Color.white;
            GUI.skin.box.alignment = TextAnchor.MiddleCenter;
            GUI.Box(new Rect(55, 5, 150, 25), info);
        }
        Vector2 pos = guiPos;
        Texture2D img = guiImg;
        guiPos.x += 7;
        guiPos.y += 2;
        if (iconOver != "")
            GUIImage((over ? iconOver : icon));
        else
            GUIImage(icon);
        guiPos = pos;
        guiImg = img;
        return res;
    }

    bool GUIIconButton(string image, string icon, string info)
    {
        return GUIIconButton(image, icon, "", info);
    }


    void GUIY()
    {
        if (guiImg != null)
            guiPos.y += guiImg.height;
    }
    void GUIX()
    {
        if (guiImg != null)
            guiPos.x += guiImg.width;
    }

    void SetSplinePointType(OTSplinePoint.OTSplinePointType type)
    {
        for (int p = 0; p < selectedPoints.Count; p++)
            SetSplinePointType(selectedPoints[p], type);
    }

    void GUIDrawToolBar()
    {
        guiPos = Vector2.zero;
        guiPos.y = -1;
        GUIImage("vbar-top"); GUIY();
        guiPos.x = 0;
        if (GUIIconButton("vbar-btn", 
            ((spline._pointType != OTSplinePoint.OTSplinePointType.Align) ? "iconFree" : "iconAlign"),
            ((spline._pointType != OTSplinePoint.OTSplinePointType.Align) ? "iconAlign" : "iconFree"), 
            "Splinepoint type to " + ((spline._pointType == OTSplinePoint.OTSplinePointType.Align) ? "free" : "align")))
            SetSplinePointType((spline._pointType == OTSplinePoint.OTSplinePointType.Align) ? OTSplinePoint.OTSplinePointType.Free : OTSplinePoint.OTSplinePointType.Align);
        GUIY();
        if (GUIIconButton("vbar-btn", "iconAdd", "Add or subdivide points"))
            AddSplinePoints();
        GUIY();
        if (GUIIconButton("vbar-btn", "iconDel", "Remove points"))
            DeleteSplinePoints();
        GUIY();
        if (GUIIconButton("vbar-btn", "iconAuto", "All points align"))
            PointsAlign();
        GUIY();
        if (GUIIconButton("vbar-btn", "iconVector", "All points free"))
            PointsFree();

        GUIY();
        if (GUIIconButton("vbar-btn", 
            ((!spline.closed) ? "iconOpen" : "iconClose"),
            ((!spline.closed) ? "iconClose" : "iconOpen"), 
            ((spline.closed) ? "Open Spline" : "Close Spline")))
            spline.closed = !spline.closed;
        GUIY();
        guiPos.x = 0;
        GUIImage("vbar-bot"); GUIY();
    }

    Rect RectFromPoints(Vector2 p1, Vector2 p2)
    {
        return new Rect(p1.x, p1.y, p2.x - p1.x, p2.y - p1.y);
    }

    void OnGUI()
    {
        GUIDrawToolBar();
    }

    void print(string s)
    {
        Debug.Log(s);
    }

    void AddControl(int id, float distance)
    {
    }

    void OnEnable()
    {
        spline = target as OTSpline;
        spline.inEdit = true;
        Init();
    }

    void OnDisable()
    {
        spline.inEdit = false;
#if UNITY_EDITOR			
		if (!Application.isPlaying)
			UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(spline);
#endif				
		
    }

    bool Init()
    {
        if (spline.points.Length == 0) return false;

        splinePoints = new List<OTSplinePoint>(spline.points);
        selectedPointIndexes.Clear();
        selectedPoints.Clear();
        selectedPoints.Add(splinePoints[0]);
        selectedPointIndexes.Add(0);
        lastControl = GUIUtility.hotControl;
        initialized = true;
        return initialized;
    }

}
#endif