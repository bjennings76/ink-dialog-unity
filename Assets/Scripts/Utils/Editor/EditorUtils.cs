using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unspeakable.Utils {
	public static class EditorUtils {

		[MenuItem("File/Save Project Shortcut %&#s")]
		public static void SaveProject() {
			AssetDatabase.SaveAssets();
			Debug.Log("Project saved.");
		}

		[MenuItem("GameObject/Group Selected %g", true)]
		public static bool CanGroupSelected() {
			return Selection.gameObjects.Length > 0;
		}

		[MenuItem("GameObject/Group Selected %g")]
		public static void GroupSelected() {
			if (!Selection.activeTransform) { return; }
			GameObject go = new GameObject(Selection.activeTransform.name + " Group");
			Undo.RegisterCreatedObjectUndo(go, "Group Selected");
			go.transform.SetParent(Selection.activeTransform.parent, false);
			go.transform.SetSiblingIndex(Selection.activeTransform.GetSiblingIndex());
			go.transform.position = Selection.transforms.Length == 1
																? Selection.transforms[0].position
																: UnityUtils.GetCenter(Selection.transforms);
			foreach (Transform transform in Selection.transforms) {
				Undo.SetTransformParent(transform, go.transform, "Group Selected");
			}
			Selection.activeGameObject = go;
		}

		[MenuItem("GameObject/Ungroup Selected %#g", true)]
		public static bool CanUngroupSelected() {
			return Selection.transforms.Any();
		}

		[MenuItem("GameObject/Ungroup Selected %#g")]
		public static void UngroupSelected() {
			if (!Selection.transforms.Any()) { return; }

			List<Object> deletables = new List<Object>();
			List<Object> selectables = new List<Object>();

			Selection.gameObjects.ForEach(go => {
				if (!go) { return; }
				if (go.transform.childCount == 0) {
					selectables.Add(go);
					return;
				}
				Transform t = go.transform;
				int index = t.GetSiblingIndex();
				Transform parent = t.parent;
				t.GetChildren().ForEach(c => {
					if (!c) { return; }
					Undo.SetTransformParent(c, parent, "Ungroup Selected");
					c.SetSiblingIndex(index);
					index++;
					selectables.Add(c.gameObject);
				});
				deletables.Add(go);
			});

			deletables.Distinct().ForEach(Undo.DestroyObjectImmediate);
			Selection.objects = selectables.ToArray();
		}

		public static bool IsInstance(GameObject go) {
			GameObject prefab = GetPrefab(go);
			return prefab && prefab != go;
		}

		public static GameObject GetPrefab(Object instance) {
			GameObject go = UnityUtils.GetGameObject(instance);
			if (!go) { return null; }
			GameObject root = PrefabUtility.FindRootGameObjectWithSameParentPrefab(go);
			if (!root) { return null; }
			return PrefabUtility.GetPrefabParent(root) as GameObject;
		}

	}
}