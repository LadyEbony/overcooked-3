using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class CabientLevelSetup : MonoBehaviour {

  #if UNITY_EDITOR
  [ContextMenu("Setup")]
  public void Setup(){
    var id = 1;
    var cabs = GetComponentsInChildren<Cabient>();
    foreach(var c in cabs){
      // this looks really stupid but it works
      var obj = new SerializedObject(c);
      obj.FindProperty("id").intValue = id++;
      obj.ApplyModifiedProperties();
    }
    var scene = EditorSceneManager.GetActiveScene();
    EditorSceneManager.MarkSceneDirty(scene);
  }
  #endif

}
