using System;
using UnityEngine;

namespace Unspeakable.Utils {
	public class FadeCanvasGroup : MonoBehaviour {
		[SerializeField] private CanvasGroup m_CanvasGroup;

		private float m_FadeStartTime;
		private float m_FadeStart;
		private bool m_InTransition;
		private float m_FadeDuration;
		private AnimationCurve m_FadeCurve;
		private float m_FadeTarget;
		private Action m_Callback;

		private float FadeProgress { get { return (Time.time - m_FadeStartTime) / m_FadeDuration; } }
		private bool FadeComplete { get { return FadeProgress >= 1; } }

		private CanvasGroup CanvasGroup { get { return m_CanvasGroup ? m_CanvasGroup : (m_CanvasGroup = GetComponentInChildren<CanvasGroup>()); } }

		private float Alpha { get { return CanvasGroup.alpha; } set { CanvasGroup.alpha = value; } }

		private void Awake() { Alpha = 0; }

		private void LateUpdate() { UpdateFade(); }

		private void UpdateFade() {
			if (!m_InTransition) { return; }
			if (!FadeComplete) { Alpha = Mathf.Lerp(m_FadeStart, m_FadeTarget, m_FadeCurve.Evaluate(m_FadeTarget.Approximately(0) ? 1 - FadeProgress : FadeProgress)); }
			else { CompleteFade(); }
		}

		private void CompleteFade() {
			Alpha = m_FadeTarget;
			m_InTransition = false;
			if (m_Callback != null) {
				Action callback = m_Callback;
				m_Callback = null;
				callback();
			}
		}

		public void In(float duration, AnimationCurve curve) { To(1, duration, curve); }

		public void Out(float duration, AnimationCurve curve, Action callback) {
			m_Callback = callback ?? (() => { });

			if (duration.Approximately(0)) { Hide(); }
			else { To(0, duration, curve); }
		}

		public void Hide() {
			m_FadeTarget = 0;
			CompleteFade();
		}

		private void To(float target, float duration, AnimationCurve curve) {
			m_FadeTarget = target;
			m_FadeDuration = duration;
			m_FadeCurve = curve;
			m_FadeStartTime = Time.time;
			m_FadeStart = Alpha;
			m_InTransition = true;
		}
	}
}