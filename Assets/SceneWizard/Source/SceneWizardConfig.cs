using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneConfigSetup
{
    public string name;
    public string path;
    public string parentFolder;
}

public class SceneWizardConfig : ScriptableObject
{
    public string folderPath;
    public bool allowSubfolders;
    public List<SceneConfigSetup> scenes;
}
