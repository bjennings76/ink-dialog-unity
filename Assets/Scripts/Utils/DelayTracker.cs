using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unspeakable.Utils {
	public class DelayTracker : Singleton<DelayTracker> {
		public static TimeDelayEvent DelayAction(float delay, Action onComplete) {
			TimeDelayEvent timeEvent = new TimeDelayEvent(onComplete, delay);
			Instance.DelayEvents.Add(timeEvent);
			return timeEvent;
		}

		public static FrameDelayEvent DealyFrame(Action onComplete) { return DelayFrames(1, onComplete); }

		public static FrameDelayEvent DelayFrames(int frameCount, Action onComplete) {
			if (!IsValid) {
				Debug.LogWarning("DelayTracker doesn't work in editor mode. Running action immediately.");
				onComplete();
				return null;
			}

			FrameDelayEvent frameEvent = new FrameDelayEvent(onComplete, frameCount);
			Instance.DelayEvents.Add(frameEvent);
			return frameEvent;
		}

		private List<BaseDelayEvent> m_DelayEvents;

		private bool HasEvents {
			get { return m_DelayEvents != null; }
		}

		private List<BaseDelayEvent> DelayEvents {
			get { return m_DelayEvents ?? (m_DelayEvents = new List<BaseDelayEvent>()); }
		}

		protected DelayTracker() { }

		private void Start() {
			gameObject.hideFlags = HideFlags.HideAndDontSave;
		}

		private void Update() {
			if (HasEvents) {
				DelayEvents.RemoveAll(e => e.Check());
			}
		}

		#region Helper Classes

		public abstract class BaseDelayEvent {
			protected Action m_Callback;
			public abstract bool Check();

			protected void Go() {
				if (m_Callback == null) {
					return;
				}

				m_Callback();
				m_Callback = null;
			}

			public void Cancel() {
				m_Callback = null;
			}
		}

		public class TimeDelayEvent : BaseDelayEvent {
			private readonly float m_GoTime;

			public TimeDelayEvent(Action callback, float delay) {
				m_Callback = callback;
				m_GoTime = Time.time + delay;
			}

			public override bool Check() {
				if (Time.time < m_GoTime) {
					return false;
				}

				Go();
				return true;
			}
		}

		public class FrameDelayEvent : BaseDelayEvent {
			private readonly int m_GoFrame;
		
			public FrameDelayEvent(Action callback, int frameCount = 1) {
				m_Callback = callback;
				m_GoFrame = Time.frameCount + frameCount;
			}

			public override bool Check() {
				if (Time.frameCount < m_GoFrame) {
					return false;
				}

				Go();
				return true;
			}
		}

		#endregion

	}
}