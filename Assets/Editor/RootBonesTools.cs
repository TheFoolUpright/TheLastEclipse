using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CopyBoneNames : EditorWindow
{
    private GameObject sourceObject;
    private List<GameObject> targetObjects = new List<GameObject>();

    [MenuItem("Tools/Copy Bone Names")]
    public static void ShowWindow()
    {
        GetWindow<CopyBoneNames>("Copy Bone Names");
    }

    private void OnGUI()
    {
        GUILayout.Label("Source GameObject", EditorStyles.boldLabel);
        sourceObject = (GameObject)EditorGUILayout.ObjectField(sourceObject, typeof(GameObject), true);

        GUILayout.Label("Target GameObjects", EditorStyles.boldLabel);

        int removeIndex = -1;
        for (int i = 0; i < targetObjects.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            targetObjects[i] = (GameObject)EditorGUILayout.ObjectField(targetObjects[i], typeof(GameObject), true);
            if (GUILayout.Button("Remove"))
            {
                removeIndex = i;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (removeIndex >= 0)
        {
            targetObjects.RemoveAt(removeIndex);
        }

        GUILayout.Space(10);

        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag & Drop Target GameObjects Here");

        HandleDragAndDrop(dropArea);

        if (GUILayout.Button("Copy Bone Names"))
        {
            foreach (GameObject o in targetObjects)
                CopyNames(sourceObject.transform, o.transform);

            targetObjects.Clear();
        }
    }

    private void HandleDragAndDrop(Rect dropArea)
    {
        Event currentEvent = Event.current;
        EventType currentEventType = currentEvent.type;

        if (!dropArea.Contains(currentEvent.mousePosition))
            return;

        switch (currentEventType)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (currentEventType == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        GameObject go = draggedObject as GameObject;
                        if (go != null)
                        {
                            targetObjects.Add(go);
                        }
                    }
                }

                currentEvent.Use();
                break;
        }
    }

    private void CopyNames(Transform source, Transform target)
    {
        if (source == null || target == null) return;

        target.name = source.name;

        int childCount = Mathf.Min(source.childCount, target.childCount);

        for (int i = 0; i < childCount; i++)
        {
            CopyNames(source.GetChild(i), target.GetChild(i));
        }
    }
}

public class UpdateSkinnedMeshWindow : EditorWindow
{
    [MenuItem("Tools/Update Skinned Mesh Bones")]
    public static void OpenWindow()
    {
        var window = GetWindow<UpdateSkinnedMeshWindow>();
        window.titleContent = new GUIContent("Skin Updater");
    }

    private GUIContent statusContent = new GUIContent("Waiting...");
    private List<SkinnedMeshRenderer> targetSkins = new List<SkinnedMeshRenderer>();
    private Transform rootBone;
    private bool includeInactive;
    private string statusText = "Waiting...";

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Target Skinned Mesh Renderers");
        for (int i = 0; i < targetSkins.Count; i++)
        {
            targetSkins[i] =
                EditorGUILayout.ObjectField(targetSkins[i], typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
        }

        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 40.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag and Drop Skinned Mesh Renderers Here");
        CheckForDragAndDrop(dropArea);

        rootBone = EditorGUILayout.ObjectField("RootBone", rootBone, typeof(Transform), true) as Transform;
        includeInactive = EditorGUILayout.Toggle("Include Inactive", includeInactive);

        bool enabled = (targetSkins.Count > 0 && rootBone != null);
        if (!enabled)
        {
            statusText = "Add target SkinnedMeshRenderers and a root bone to process.";
        }

        GUI.enabled = enabled;
        if (GUILayout.Button("Update Skinned Mesh Renderers"))
        {
            statusText = "== Processing bones... ==";
            foreach (var targetSkin in targetSkins)
            {
                if (targetSkin == null) continue;

                // Look for root bone
                string rootName = "";
                if (targetSkin.rootBone != null) rootName = targetSkin.rootBone.name;
                Transform newRoot = null;
                // Reassign new bones
                Transform[] newBones = new Transform[targetSkin.bones.Length];
                Transform[] existingBones = rootBone.GetComponentsInChildren<Transform>(includeInactive);
                int missingBones = 0;
                for (int i = 0; i < targetSkin.bones.Length; i++)
                {
                    if (targetSkin.bones[i] == null)
                    {
                        statusText += System.Environment.NewLine +
                                      "WARN: Do not delete the old bones before the skinned mesh is processed!";
                        missingBones++;
                        continue;
                    }

                    string boneName = targetSkin.bones[i].name;
                    bool found = false;
                    foreach (var newBone in existingBones)
                    {
                        if (newBone.name == rootName) newRoot = newBone;
                        if (newBone.name == boneName)
                        {
                            Debug.Log(System.Environment.NewLine + "· " + newBone.name + " found!");
                            newBones[i] = newBone;
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        Debug.LogWarning(System.Environment.NewLine + "· " + boneName + " missing!");
                        missingBones++;
                    }
                }

                targetSkin.bones = newBones;
                Debug.LogWarning(System.Environment.NewLine + "Done! Missing bones: " + missingBones);
                if (newRoot != null)
                {
                    Debug.Log(System.Environment.NewLine + "· Setting " + rootName + " as root bone.");
                    targetSkin.rootBone = newRoot;
                }
            }

            targetSkins.Clear();
        }

        // Draw status because yeh why not?
        statusContent.text = statusText;
        EditorStyles.label.wordWrap = true;
        GUILayout.Label(statusContent);
    }

    private void CheckForDragAndDrop(Rect dropArea)
    {
        Event currentEvent = Event.current;
        if (!dropArea.Contains(currentEvent.mousePosition)) return;

        switch (currentEvent.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (currentEvent.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (var obj in DragAndDrop.objectReferences)
                    {
                        SkinnedMeshRenderer skinnedMeshRenderer =
                            (obj as GameObject)?.GetComponent<SkinnedMeshRenderer>();
                        if (skinnedMeshRenderer == null)
                        {
                            GameObject selectedObj = obj as GameObject;
                            var skinnedMeshRendereres = selectedObj.GetComponentsInChildren<SkinnedMeshRenderer>();
                            foreach (var smr in skinnedMeshRendereres)
                            {
                                targetSkins.Add(smr);
                            }
                        }

                        if (skinnedMeshRenderer != null && !targetSkins.Contains(skinnedMeshRenderer))
                        {
                            targetSkins.Add(skinnedMeshRenderer);
                        }
                    }
                }

                break;
        }
    }
}