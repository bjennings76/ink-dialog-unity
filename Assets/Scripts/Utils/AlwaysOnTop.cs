using UnityEngine;
using UnityEngine.Rendering;

namespace Unspeakable.Utils {
	public class AlwaysOnTop : MonoBehaviour {
		public bool IncludeChildren = true;

		private void OnEnable() { DelayTracker.DelayFrames(5, UpdateRenderingMode); }

		private void OnDisable() { UpdateRenderingMode(); }

		private void UpdateRenderingMode() {
			CanvasRenderer[] renderers = IncludeChildren ? GetComponentsInChildren<CanvasRenderer>() : GetComponents<CanvasRenderer>();
			renderers.ForEach(SetZTestMode);
		}

		private void SetZTestMode(CanvasRenderer r) {
			if (r.materialCount == 0) { return; }
			Material mat = new Material(r.GetMaterial(0));
			mat.SetInt("unity_GUIZTestMode", (int) (enabled ? CompareFunction.Always : CompareFunction.LessEqual));
			r.SetMaterial(mat, 0);
		}
	}
}