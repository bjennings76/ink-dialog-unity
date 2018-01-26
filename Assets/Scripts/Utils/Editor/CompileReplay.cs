using UnityEditor;
using UnityEngine;

namespace Unspeakable.Utils {
	[InitializeOnLoad]
	public class CompileReplay : ScriptableObject {
		private const string kStopOnCompile = "CompileReplay_StopPlay";
		private const string kAutoPlayPref = "CompileReplay_AutoPlay";

		private static CompileReplay s_Instance;
		private static bool s_StopOnCompile = true;
		private static bool s_AutoPlay = true;

		static CompileReplay() {
			EditorApplication.delayCall += CreateSingleton;
		}

		private static void CreateSingleton() {
			if (s_Instance == null) {
				s_Instance = FindObjectOfType<CompileReplay>();
				if (s_Instance == null) { s_Instance = CreateInstance<CompileReplay>(); }
			}
		}

		public void OnEnable() {
			s_StopOnCompile = EditorPrefs.GetBool(kStopOnCompile, true);
			s_AutoPlay = EditorPrefs.GetBool(kAutoPlayPref, true);
			hideFlags = HideFlags.HideAndDontSave;
			s_Instance = this;
		}

		public CompileReplay() {
			EditorApplication.update -= Update;
			EditorApplication.update += Update;
		}

		[PreferenceItem("Compile Replay")]
		private static void OnPreferenceGUI() {
			GUIContent stopCompileContent = new GUIContent("Stop Playing on Recompile", "If the editor starts recompiling while in 'Play' mode, then automatically stop play mode.");
			bool stopCompile = EditorGUILayout.Toggle(stopCompileContent, s_StopOnCompile);
			if (s_StopOnCompile != stopCompile) {
				s_StopOnCompile = stopCompile;
				EditorPrefs.SetBool(kStopOnCompile, s_StopOnCompile);
			}

			using (new EditorGUI.DisabledScope(!s_StopOnCompile)) {
				GUIContent autoPlayContent = new GUIContent("Restore Play After Recompile", "If the editor starts recompiling while in 'Play' mode, then automatically start 'Play' mode again when the recompile completes.");
				bool autoPlay = EditorGUILayout.Toggle(autoPlayContent, s_AutoPlay);
				if (s_AutoPlay != autoPlay) {
					s_AutoPlay = autoPlay;
					EditorPrefs.SetBool(kAutoPlayPref, s_AutoPlay);
				}
			}
		}

		private void Update() {
			if (s_StopOnCompile && CheckInstance() && EditorApplication.isPlaying && EditorApplication.isCompiling) {
				Debug.Log("[Replay] Exiting play mode due to script recompilation.");
				EditorApplication.isPlaying = false;
				if (s_AutoPlay) { EditorApplication.delayCall += () => EditorApplication.isPlaying = true; }
			}
		}

		private bool CheckInstance() {
			if (s_Instance != this) {
				EditorApplication.update -= Update;
				if (Application.isPlaying) { Destroy(this); }
				else { DestroyImmediate(this); }
				return false;
			}
			return true;
		}
	}
}