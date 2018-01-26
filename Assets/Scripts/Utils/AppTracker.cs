using System;
using UnityEngine;

namespace Unspeakable.Utils {
	public class AppTracker : Singleton<AppTracker> {
		public static event Action OnQuit;

		public static bool IsQuitting { get; private set; }

		protected AppTracker() { }

		private void Update() {
			if (!Application.isPlaying && !IsQuitting) { RaiseOnQuit(); }
		}

		private void RaiseOnQuit() {
			IsQuitting = true;
			if (OnQuit != null) { OnQuit(); }
		}

		private void OnDestroy() { RaiseOnQuit(); }
	}
}