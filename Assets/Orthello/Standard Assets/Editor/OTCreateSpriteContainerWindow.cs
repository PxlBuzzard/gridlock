#if UNITY_EDITOR 

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;

public class OTCreateSpriteContainerWindow : EditorWindow
{
    string importPath = "";
    string importParentPath = "";
    string importAssetPath = "";
    string _importAssetPath = "";
    string status = "";
    List<string> imageFileNames = new List<string>();
    List<string> imageNames = new List<string>();
    List<Texture2D> images = new List<Texture2D>();
    List<Vector2> imageNewSizes = new List<Vector2>();
    List<int> imageTexture = new List<int>();
    List<Vector2> imagePosition = new List<Vector2>();

    //int importIdx = 0;
    string containerName = "MySpriteSheet";
    XmlNode skNode;
    bool fit = true, _fit = true;
    bool exact = true, _exact = true;
    bool square = false, _square = false;
    bool deleteImages = false, _deleteImages = false;
	bool valid = false, noPath = false;
    bool fromDisk = false, _fromDisk = false;
    float scaleIt = 1, _scaleIt = 1;
	int padding = 0, _padding = 0;
    int sizeMaxIdx = 2, _sizeMaxIdx = 2;
    bool addToScene = true, _addToScene = true;

    bool createConfig;
    XmlDocument xml = new XmlDocument();

    bool doCalculate = false;

	float scale
    {
        get
        {
            if (scaleIt > 0)
                return scaleIt;
            else
                return 1;
        }

    }
	
    int sizeMax
    {
        get
        {
            switch (sizeMaxIdx)
            {
                case 0: return 256;
                case 1: return 512;
                case 2: return 1024; 
                case 3: return 2048; 
                case 4: return 4096; 
            }
            return 1024;
        }
    }

    
    Vector2 frames;
    Vector2 frameSize;
    Vector2 texSize;
    int texCount;

    int xp = 10, yp = 10;
    int dy = 20;

    //int importMode = 0;

    // Add a menu item
    [UnityEditor.MenuItem("Assets/Orthello/Create Sprite Container")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        OTCreateSpriteContainerWindow window = (OTCreateSpriteContainerWindow)EditorWindow.GetWindow(typeof(OTCreateSpriteContainerWindow));
        window.title = "Orthello";
        window.InitWindow();
    }

    void Reset()
    {
        imageFileNames.Clear();
        imageNames.Clear();
        images.Clear();
        imageNewSizes.Clear();
        imageTexture.Clear();
        imagePosition.Clear();
        //importMode = 0;
    }

    void SetPath()
    {
        string path;
        createConfig = false;
        noPath = false;
        if (Selection.activeObject != null && AssetDatabase.Contains(Selection.activeObject))
        {
            if (Selection.activeObject.GetType() != typeof(Object))
                path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject));
            else
                path = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (importAssetPath != path)
            {
                importAssetPath = path;
                importPath = Path.GetFullPath(importAssetPath);

                DirectoryInfo di = new DirectoryInfo(importPath);
                containerName = di.Name;
                importParentPath = di.Parent.FullName;
            }
            valid = LoadConfigXML();
            createConfig = (!valid);
            Repaint();
        }
        else
        {
            if (importAssetPath == "")
            {
                importAssetPath = "Error - no path for import selected.";
                valid = false;
                noPath = true;
            }
        }
    }

    XmlNode cfgNode;
    bool LoadConfigXML()
    {
        bool res = false;
        try
        {
            xml.Load(importPath + "/-orthello-config.xml");
            cfgNode = xml.SelectSingleNode("/orthello/config[@type='container']");
            if (cfgNode != null)
            {
                try
                {
                    sizeMaxIdx = System.Convert.ToInt32(GetConfigNode("sizeMaxIdx"));
                    exact = (GetConfigNode("exact") == "True");
                    square = (GetConfigNode("square") == "True");
                    fit = (GetConfigNode("fit") == "True");
                    deleteImages = (GetConfigNode("deleteImages") == "True");
                    scaleIt = System.Convert.ToSingle(GetConfigNode("scale"));
                    padding = System.Convert.ToInt16(GetConfigNode("padding"));
                    addToScene = (GetConfigNode("addToScene") == "True");

                    _fit = fit;
                    _square = square;
                    _exact = exact;
                    _scaleIt = scaleIt;
					_padding = padding;
                    _sizeMaxIdx = sizeMaxIdx;
                    _deleteImages = deleteImages;
                    _addToScene = addToScene; 

                }
                catch (System.Exception)
                {
                }
            }            
            res = true;
        }
        catch(System.Exception)
        {
        }
        return res;
    }

    string GetConfigNode(string nodeName)
    {
        XmlNode cNode = cfgNode.SelectSingleNode(nodeName);
        if (cNode != null && cNode.FirstChild != null)
            return cNode.FirstChild.Value;
        return "";
    }

    void SetConfigNode(string nodeName, string nodeValue)
    {
        XmlNode cNode = cfgNode.SelectSingleNode(nodeName);
        if (cNode==null)
            cNode = cfgNode.AppendChild(xml.CreateElement(nodeName));

        if (cNode != null)
        {
            if (cNode.FirstChild == null)
                cNode.AppendChild(xml.CreateTextNode(""));
            cNode.FirstChild.Value = nodeValue;
        }
    }

    void SaveConfigXML()
    {
        cfgNode = xml.SelectSingleNode("/orthello/config[@type='container']");
        if (cfgNode != null)
        {
            SetConfigNode("containerType", "sheet");
            SetConfigNode("sizeMax", "" + sizeMax);
            SetConfigNode("sizeMaxIdx", "" + sizeMaxIdx);
            SetConfigNode("exact", "" + exact);
            SetConfigNode("square", "" + square);
            SetConfigNode("fit", "" + fit);
            SetConfigNode("deleteImages", "" + deleteImages);
            SetConfigNode("scale", "" + scale);
            SetConfigNode("padding", "" + padding);
            SetConfigNode("addToScene", "" + addToScene);

            xml.Save(importPath + "/-orthello-config.xml");
            AssetDatabase.ImportAsset(importAssetPath + "/-orthello-config.xml");
        }
    }

    void CreateConfigXML()
    {
        xml.LoadXml(
            "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"+
            "<orthello>" +
                "<config type=\"container\" >" +
                    "<sizeMax /><exact /><square /><fit /><deleteImages />" +
                "</config>" +
            "</orthello>");
        SaveConfigXML();
        InitWindow();
    }

    void InitWindow()
    {
        SetPath();
        if (!valid) return;
        if (_importAssetPath == "" || _importAssetPath != importAssetPath)
        {
            _importAssetPath = importAssetPath;
            Reset();
            string[] fileEntries;
            try
            {
                fileEntries = Directory.GetFiles(importPath);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                return;
            }
            foreach (string fileName in fileEntries)
            {
                string fname = Path.GetFileName(fileName);
                string ext = Path.GetExtension(fileName);
                if (ext == ".png" || ext == ".gif" || ext == ".jpg" || ext == ".jpeg")
                {
                    TextureImporter importer = TextureImporter.GetAtPath(importAssetPath + "/" + fname) as TextureImporter;
                    if (importer.isReadable == false || importer.textureFormat != TextureImporterFormat.ARGB32 || importer.mipmapEnabled ||
                        importer.maxTextureSize < 4096 || importer.npotScale != TextureImporterNPOTScale.None)
                    {
                        importer.isReadable = true;
                        importer.textureFormat = TextureImporterFormat.ARGB32;
                        importer.mipmapEnabled = false;
                        importer.maxTextureSize = 4096;
                        importer.npotScale = TextureImporterNPOTScale.None;
                        AssetDatabase.ImportAsset(importAssetPath + "/" + fname, ImportAssetOptions.ForceUpdate);
                    }
                    images.Add(AssetDatabase.LoadAssetAtPath(importAssetPath + "/" + fname, typeof(Texture2D)) as Texture2D);
                    imageFileNames.Add(fname);
                    imageNames.Add(fname);
                    imageNewSizes.Add(Vector2.zero);
                    imageTexture.Add(1);
                    imagePosition.Add(Vector2.zero);
                }
            }
        }
        if (images.Count > 0)
        {
            status = images.Count + " textures";
        }
        else
            status = "no images found on current import path!";

        CalculateTextures();
        Repaint();
    }

    void NL()
    {
        yp += dy;
    }

    string GetParentPath(string path)
    {
        string res = "";
        string[] sa = path.Split('/');
        for (int s=0; s<sa.Length-1; s++)
        {
            res += sa[s];
            if (s < sa.Length - 2) res += "/";
        }
        return res;
    }

    void CheckSettings()
    {
        bool saveXML = false;
        if (_fit != fit || _square != square || _exact != exact || _scaleIt != scaleIt || _sizeMaxIdx != sizeMaxIdx || _padding != padding)        
        {
            doCalculate = true;
            _fit = fit;
            _square = square;
            _exact = exact;
            _scaleIt = scaleIt;
            _sizeMaxIdx = sizeMaxIdx;
			_padding = padding;
            saveXML = true;
        }
        if (_deleteImages != deleteImages || _addToScene!=addToScene)
        {
            _deleteImages = deleteImages;
            _addToScene = addToScene;
            saveXML = true;
        }
        if (saveXML)
            SaveConfigXML();
    }

    void Update()
    {
        if (_fromDisk != fromDisk)
        {
            InitWindow();
            _fromDisk = fromDisk;
        }
        
        CheckSettings();
        if (doCalculate && images.Count>0)
            CalculateTextures();
    }


    //-------------------------------------------------------------
    // Quick Rect methods 
    //-------------------------------------------------------------

    Rect R()
    {
        return new Rect(xp, yp, 1024, dy+2);
    }

    Rect R(int w)
    {
        return new Rect(xp, yp, w, dy + 2);
    }

    Rect R(int x, int w)
    {
        return new Rect(x, yp, w, dy + 2);
    }

    Rect RN()
    {
        Rect r = new Rect(xp, yp, 1024, dy + 2);
        NL();
        return r;
    }

    Rect RN(int w)
    {
        Rect r = new Rect(xp, yp, w, dy + 2);
        NL();
        return r;
    }

    Rect RN(int x, int w)
    {
        Rect r = new Rect(x, yp, w, dy + 2);
        NL();
        return r;
    }

    int ci = 1;

    void _Caption(int x1, string label1)
    {
        int oldSize = EditorStyles.label.fontSize;
        Color oldColor = GUI.contentColor;
        FontStyle oldStyle = EditorStyles.label.fontStyle;

        EditorStyles.label.fontStyle = FontStyle.Bold;
        
        EditorStyles.label.fontSize = 13;
        EditorStyles.label.fixedWidth = 600;
        GUI.contentColor = Color.Lerp(GUI.contentColor, new Color(1, .8f, .6f), 0.6f);
        EditorGUI.PrefixLabel(R(x1, 600), ci++, new GUIContent(label1));

        EditorStyles.label.fontSize = oldSize;
        EditorStyles.label.fontStyle = oldStyle;
        GUI.contentColor = oldColor;


    }
    void Caption(int x1, string label1)
    {
        _Caption(x1, label1);
        NL();
    }
    void _Caption(string label1)
    {
        _Caption(10, label1);
    }
    void Caption(string label1)
    {
        _Caption(10, label1);
        NL();
    }
    void _Label(int x1, int w1, string label1, int x2, int w2, string label2)
    {
        EditorStyles.label.fixedWidth = 600;
        EditorStyles.label.fontStyle = FontStyle.Normal;
        EditorStyles.label.fontSize = 11;
        EditorGUI.PrefixLabel(R(x1, w1), ci++, new GUIContent(label1));
        EditorGUI.PrefixLabel(R(x2, w2), ci++, new GUIContent(": " + label2));
    }
    void Label(int x1, int w1, string label1, int x2, int w2, string label2)
    {
        _Label(x1, w1, label1, x2, w2, label2);
        NL();
    }
    void _Label(string label1, string label2)
    {
        _Label(10, 145, label1, 150, 600, label2);
    }
    void Label(string label1, string label2)
    {
        _Label(label1,label2);
        NL();
    }

    string _TextInput(int x,int w, string value)
    {
        Color oldColor = GUI.contentColor;
        EditorStyles.textField.clipping = TextClipping.Clip;
        EditorStyles.textField.fixedWidth = w;
        GUI.contentColor = Color.Lerp(GUI.contentColor, new Color(1, .8f, .6f), 0.3f);
        value = EditorGUI.TextArea(R(x, w), value);
        GUI.contentColor = oldColor;
        return value;
    }

    string _TextField(string label1, int w, string value)
    {
        _Label(label1, "");
        Color oldColor = GUI.contentColor;
        EditorStyles.textField.clipping = TextClipping.Clip;
        EditorStyles.textField.fixedWidth = w;
        GUI.contentColor = Color.Lerp(GUI.contentColor, new Color(1, .8f, .6f), 0.3f);
        value = EditorGUI.TextArea(R(160, 390), value);
        GUI.contentColor = new Color(1, 1, 1);
        GUI.contentColor = oldColor;
        return value;
    }
    string TextField(string label1, int w, string value)
    {
        value = _TextField(label1 , w, value);
        NL();
        return value;
    }

    void GetFrameSize()
    {
        frameSize.x = Mathf.Round((images[0].width * scale));
        frameSize.y = Mathf.Round((images[0].height * scale));

        if (fit)
        {
			frameSize.x += (padding*2);
			frameSize.y += (padding*2);
            // images must fit on texture with max size
            // so calculate new frameSize
            if (frameSize.y > frameSize.x)
            {
                float ratio = frameSize.y / frameSize.x;
                if (frameSize.y > sizeMax)
                {
                    frameSize.y = sizeMax;
                    frameSize.x = Mathf.Round(frameSize.y / ratio);
                }
                int cframes = 0;
                int r = (int)Mathf.Floor(sizeMax / frameSize.y);
                while (cframes < images.Count)
                {
                    cframes = 0;
                    int cx = 0;
                    while (cx + frameSize.x < sizeMax)
                    {
                        cframes++;
                        cx += (int)frameSize.x;
                    }
                    cframes *= r;

                    if (cframes < images.Count)
                    {
                        r++;
                        frameSize.y = Mathf.Round(sizeMax / r);
                        frameSize.x = Mathf.Round(frameSize.y / ratio);
                    }
                }
            }
            else
            {
                float ratio = frameSize.x / frameSize.y;
                if (frameSize.x > sizeMax)
                {
                    frameSize.x = sizeMax;
                    frameSize.y = Mathf.Round(frameSize.x / ratio);
                }
                int cframes = 0;
                int c = (int)Mathf.Floor(sizeMax / frameSize.x);
                while (cframes < images.Count)
                {
                    cframes = 0;
                    int cy = 0;
                    while (cy + frameSize.y < sizeMax)
                    {
                        cframes++;
                        cy += (int)frameSize.y;
                    }
                    cframes *= c;

                    if (cframes < images.Count)
                    {
                        c++;
                        frameSize.x = Mathf.Round(sizeMax / c);
                        frameSize.y = Mathf.Round(frameSize.x / ratio);
                    }
                }
            }
        }
		else
		{
			frameSize.x += (padding*2);
			frameSize.y += (padding*2);
		}
				
        for (int i = 0; i < images.Count; i++)
            imageNewSizes[i] = frameSize;
    }

    void CalculateSpriteSheetMatrix(int max)
    {
        int fr = 0;
        int wx = 0, px = 1;
        int wy = 0, py = 1;
        texCount = 1;
        texSize = Vector2.zero;
        while (fr < images.Count)
        {
            imageTexture[fr] = texCount;
            imagePosition[fr] = new Vector2(px, py);
            fr++;
            if (fr < images.Count)
            {
                wx += (int)frameSize.x;
                px++;
                if (wx > texSize.x) texSize.x = wx;
                if (wx + frameSize.x > max)
                {
                    wy += (int)frameSize.y;
                    py++;
                    if (wy > texSize.y) texSize.y = wy;
                    wx = 0;
                    px = 1;
                    if (wy + frameSize.y > max)
                    {
                        if (fr < images.Count)
                            texCount += 1;
                        wy = 0;
                        py = 1;
                    }
                    else
                        if (wy + frameSize.y > texSize.y) texSize.y = wy + frameSize.y;
                }
                else
                    if (wx + frameSize.x > texSize.x) texSize.x = wx + frameSize.x;
            }
        }

        if (texSize.y == 0)
        {
            wy += (int)frameSize.y;
            if (wy > texSize.y) texSize.y = wy;
        }

    }

    void CalculateSpriteSheets()
    {
        GetFrameSize();


        CalculateSpriteSheetMatrix(sizeMax);
        if (texCount==1)
        {
            int size = sizeMax;
            while (true)
            {
                size = size / 2;
                CalculateSpriteSheetMatrix(size);
                if (texCount > 1)
                {
                    size *= 2;
                    CalculateSpriteSheetMatrix(size);
                    break;
                }
                if (size <= 16)
                    break;
            }
        }


        if (texSize.x > sizeMax)
        {
            frameSize *= (sizeMax / texSize.x);
            frameSize.x = Mathf.Round(frameSize.x);
            frameSize.y = Mathf.Round(frameSize.y);
            for (int i = 0; i < images.Count; i++)
                imageNewSizes[i] = frameSize;

            texSize.y *= (sizeMax / texSize.x);
            texSize.y = Mathf.Round(texSize.y);
            texSize.x = sizeMax;
        }

        if (texSize.y > sizeMax)
        {
            frameSize *= (sizeMax / texSize.y);
            frameSize.x = Mathf.Round(frameSize.x);
            frameSize.y = Mathf.Round(frameSize.y);
            for (int i = 0; i < images.Count; i++)
                imageNewSizes[i] = frameSize;

            texSize.x *= (sizeMax / texSize.y);
            texSize.x = Mathf.Round(texSize.x);
            texSize.y = sizeMax;
        }

        if (!exact)
        {
            Vector2 pow2Size = new Vector2(16, 16);
            while (pow2Size.x < texSize.x) pow2Size.x *= 2;
            while (pow2Size.y < texSize.y) pow2Size.y *= 2;
            texSize = pow2Size;            
        }

        if (!exact && square && (texSize.x != texSize.y))
        {
            if (texSize.x > texSize.y)
                texSize.y = texSize.x;
            else
                texSize.x = texSize.y;
        }

    }

    void CalculateTextures()
    {
        CalculateSpriteSheets();
        doCalculate = false;
    }


    int imageIdx = 0;
    void AddImage(Texture2D texture, Texture2D image)
    {
        Vector2 pos = imagePosition[imageIdx];
        pos.x = ((pos.x - 1) * frameSize.x);
        pos.y = texture.height-(pos.y * frameSize.y);
        texture.SetPixels((int)pos.x, (int)pos.y, (int)image.width, (int)image.height, image.GetPixels());
    }

    Texture2D SaveTexture(Texture2D tex, string saveName)
    {
        // Encode texture into PNG
        byte[] bytes = tex.EncodeToPNG();

        AssetDatabase.DeleteAsset(GetParentPath(importAssetPath) + "/" + saveName + ".png");
        // Write to a file in the project folder
        using (FileStream
            fileStream = new FileStream(importParentPath + "/" + saveName + ".png", FileMode.Create))
        {
            BinaryWriter bw = new BinaryWriter(fileStream);
            bw.Write(bytes);
            bw.Close();
            fileStream.Close();
        }
        AssetDatabase.ImportAsset(GetParentPath(importAssetPath) + "/" + saveName + ".png");
        Object o = AssetDatabase.LoadAssetAtPath(GetParentPath(importAssetPath) + "/" + saveName + ".png", typeof(Texture2D));
        return (o as Texture2D);
    }

    private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
		for (int c=0; c<rpixels.Length; c++)
			rpixels[c] = Color.clear;
		int tw = (targetWidth-(padding*2));
		int th = (targetHeight-(padding *2));
        float incX = ((float)1 / source.width) * ((float)source.width / tw);
        float incY = ((float)1 / source.height) * ((float)source.height / th);
        for (int py = 0; py < targetHeight; py++)
	        for (int px = 0; px < targetWidth; px++)
	        {	
				if (py>=padding && py < targetHeight-padding  && px>=padding && px <= targetWidth-padding)
				{
		            rpixels[(py * targetWidth)+px] = source.GetPixelBilinear(incX * ((float)(px-padding) % tw),
		                              incY * (float)(py-padding));
				}
	        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }

    Vector2 framesXY = Vector2.zero;
    List<OTContainer> createdSheets = new List<OTContainer>();
    void CreateSpriteSheetTexture(int t)
    {
        // Create a texture the size of the screen, RGB24 format
        Texture2D tex = new Texture2D((int)texSize.x, (int)texSize.y, TextureFormat.ARGB32, false);
		// Fill Texture with empty transparent color
		Color[] emptyPixels = new Color[(int)(texSize.x * texSize.y)];
		for (int c=0; c<(int)(texSize.x * texSize.y); c++)
			emptyPixels[c] = Color.clear;
		tex.SetPixels(emptyPixels);
		tex.Apply();
        // place images on texture
        while (imageTexture[imageIdx] == t)
        {
            // Create a texture the size of the screen, RGB24 format
            Texture2D image = ScaleTexture(images[imageIdx], (int)frameSize.x, (int)frameSize.y);
            AddImage(tex, image);
            if (t == 1)
            {
                framesXY = imagePosition[imageIdx];
            }
            imageIdx++;
            if (imageIdx == images.Count) break;
        }
        tex.Apply();


        framesXY.x = Mathf.Floor(texSize.x / frameSize.x);
        if (!exact)
            framesXY.y = Mathf.Floor(texSize.y / frameSize.y);

        string newName = containerName+""+((texCount>1)?"-"+t.ToString():"");
        Texture2D newTexture = SaveTexture(tex, "sheet-"+newName);
        DestroyImmediate(tex);
        if (newTexture != null && addToScene)
        {
			OT.print("Container-" + newName);
            OTSpriteSheet sheet = OT.ContainerByName("Container-" + newName) as OTSpriteSheet;
            if (sheet == null)
                sheet = OT.CreateObject(OTObjectType.SpriteSheet).GetComponent<OTSpriteSheet>();
            if (sheet != null)
            {
                sheet.name = "Container-" + newName;
                sheet.texture = newTexture;
                sheet.framesXY = framesXY;
                sheet.sheetSize = Vector2.zero;
                sheet.frameSize = Vector2.zero;
                if (!exact)
                {
                    if (framesXY.x * frameSize.x != texSize.x || framesXY.y * frameSize.y != texSize.y)
                    {
                        sheet.sheetSize = texSize;
                        sheet.frameSize = frameSize;
                    }
                }
				if (!createdSheets.Contains(sheet))
				{
					OT.print("adding "+sheet.name);
					try
					{
                		createdSheets.Add(sheet);
					}
					catch (System.Exception err)
					{						
						OT.print("error "+err.Message);
					}
				}	
				else
					OT.print("already exists "+sheet.name);
					
            }
        }
        else
            Debug.LogWarning("Orthello - CreateSpriteContainer - returned newTexture null!");
		
		
    }

    void CreateSpriteSheets()
    {
        imageIdx = 0;

        createdSheets.Clear();
        for (int t = 1; t <= texCount; t++)
            CreateSpriteSheetTexture(t);
		
        if (addToScene)
        {
            OTAnimation animation = OT.AnimationByName("Animation-" + containerName);
            if (animation == null)
                animation = OT.CreateObject(OTObjectType.Animation).GetComponent<OTAnimation>();
            animation.name = "Animation-" + containerName;
            List<OTAnimationFrameset> fs = new List<OTAnimationFrameset>();
            int frCount = (int)(framesXY.x * framesXY.y);
            int frIdx = 0;
            for (int t = 1; t <= texCount; t++)
            {
                OTSpriteSheet sheet = createdSheets[t - 1] as OTSpriteSheet;
                if (sheet != null)
                {
                    OTAnimationFrameset fr = new OTAnimationFrameset();
                    fr.container = sheet;
                    fr.startFrame = 0;
                    if (frIdx + frCount <= images.Count)
                        fr.endFrame = frCount - 1;
                    else
                        fr.endFrame = (images.Count - frIdx) - 1;

                    fs.Add(fr);
                    frIdx += frCount;
                }
            }
            animation.framesets = fs.ToArray();
            

            OTAnimatingSprite sprite = OT.ObjectByName(containerName) as OTAnimatingSprite;
            if (sprite == null)
                sprite = OT.CreateObject(OTObjectType.AnimatingSprite).GetComponent<OTAnimatingSprite>();
            if (sprite != null)
            {
                sprite.name = containerName;
                sprite.animation = animation;
                sprite.animationFrameset = "";
            }
        }
    }

    void CreateContainers()
    {
        CreateSpriteSheets();
    }
	
	
    //-------------------------------------------------------------
    Vector2 scrollPos = Vector2.zero;
    void OnGUI()
    {
        yp = 10; dy = 16;
        Caption("Create Sprite Container"); NL();

        if (Application.isPlaying)
        {
            GUI.Label(RN(10, 600), "This tool can only be used when the application is not playing!");
            return;
        }

        Label("Import Path",importAssetPath);
        if (!noPath)
        {
            Label("Images", status);
            containerName = TextField("Container Name", 200, containerName);
            NL();
        }
        if (!valid && createConfig)
        {
            GUI.Label(RN(10, 600), "To use this import path for image import, press the button below.");
            NL();
            GUI.Label(RN(10, 600), "All images in the current path will be re-imported into Unity using texture importer");
            GUI.Label(RN(10, 600), "that is set to read/write, ARGB32 format, compression and mipmaps off, max texture 4096.");
            NL();
            if (GUI.Button(R(150), "Use import path"))
                CreateConfigXML();
        }
        else
            if (valid && images.Count > 0)
            {
                Caption("Images"); NL();
                _Label("Scale", "");
                EditorStyles.textField.fixedWidth = 100;				
			
				GUIStyle s = new GUIStyle(GUI.skin.textField);
				s.fixedWidth  = 35;
			
				EditorGUI.LabelField(R(xp + 100,35), "Padding");
				GUI.skin.customStyles[0].fixedWidth = 35;
                scaleIt = EditorGUI.FloatField(R(xp + 50, 35), scaleIt,s);
				padding = EditorGUI.IntField(RN(xp + 160, 35), padding,s);
                if (scaleIt < 0) scaleIt = 0.01f;
                if (scaleIt > 1) scaleIt = 1;
                NL();
                GUI.skin.box.normal.textColor = Color.white;
                GUI.skin.box.alignment = TextAnchor.MiddleLeft;
                scrollPos = GUI.BeginScrollView(new Rect(xp, yp, 550, 11 * dy), scrollPos, new Rect(xp, yp, 190, (imageNames.Count + 1) * dy));
                int oldY = yp;
                GUI.Box(R(10, 30), "Nr."); GUI.Box(R(45, 150), "Name"); GUI.Box(R(200, 100), "Size"); GUI.Box(R(305, 100), "New Size");
                GUI.Box(R(410, 25), "Tx"); GUI.Box(R(440, 75), "Position x/y");
                NL();
                yp += 3;
                for (int m = 1; m <= imageNames.Count; m++)
                {
                    GUI.Label(R(12, 30), m.ToString());
                    imageNames[m - 1] = _TextInput(45, 150, imageNames[m - 1]);
                    int w = (int)(images[m - 1].width * scale);
                    int h = (int)(images[m - 1].height * scale);
                    GUI.Label(R(202, 100), "" + w + " x " + h + " px");
                    GUI.Label(R(307, 100), "" + imageNewSizes[m - 1].x + " x " + imageNewSizes[m - 1].y + " px");
                    GUI.Label(R(412, 25), "" + imageTexture[m - 1]);
                    GUI.Label(R(442, 74), "" + imagePosition[m - 1].x + "/" + imagePosition[m - 1].y);
                    NL();
                }
                GUI.EndScrollView();
                yp = oldY + ((imageNames.Count > 10) ? 12 : imageNames.Count + 2) * dy;
                Caption("Container"); NL();
                GUI.Label(R(12, 200), "Max. size");
                sizeMaxIdx = EditorGUI.Popup(R(110, 100), sizeMaxIdx, new string[] { "256", "512", "1024", "2048", "4096" });
                NL();
			
#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5												
				EditorGUI.indentLevel = 6;
#else
				EditorGUI.indentLevel = 4;
#endif
				xp=-42;
				int xp2 = 10;
			
                exact = EditorGUI.Toggle(R(200), "Exact", exact);
                if (exact)
                    GUI.Label(RN(xp2 + 120, 600), "Texture size related to size of images.");
                else
                    GUI.Label(RN(xp2 + 120, 600), "Texture size always power of 2.");

                if (!exact)
                {
                    square = EditorGUI.Toggle(R(200), "Make square", square);
                    if (square)
                        GUI.Label(RN(xp2 + 120, 600), "Texture(s) will be square (power of 2) sized.");
                    else
                        GUI.Label(RN(xp2 + 120, 600), "Texture(s) will be rectangular (power of 2) sized.");
                }

                fit = EditorGUI.Toggle(R(200), "Fit", fit);
                if (fit)
                    GUI.Label(RN(xp2 + 120, 600), "Images will be resized to fit on one texture.");
                else
                    GUI.Label(RN(xp2 + 120, 600), "Images can span across multiple textures.");

                deleteImages = EditorGUI.Toggle(R(200), "Delete images", deleteImages);
                if (deleteImages)
                    GUI.Label(RN(xp2 + 120, 600), "Images will be removed.");
                else
                    GUI.Label(RN(xp2 + 120, 600), "Keep images.");
			
                addToScene = EditorGUI.Toggle(R(200), "Add to scene", addToScene);
                if (addToScene)
                    GUI.Label(RN(xp2 + 120, 600), "Spritesheet(s), animation object and animating sprite will be created.");
                else
                    GUI.Label(RN(xp2 + 120, 600), "No objects will be created.");
			
			
			
                NL();
				xp = 10;
                GUI.Label(R(170, 600), "" + texCount + " texture" + ((texCount == 1) ? "" : "s") + " " + texSize.x + " x " + texSize.y + " px");

                string cmdName = "Sprite Sheet";
                if (texCount > 1) cmdName += "s";

                if (GUI.Button(R(150), "Create " + cmdName))
                    CreateContainers();
            }
        NL();
    }

    void OnSelectionChange()
    {
        InitWindow();
    }

}
#endif