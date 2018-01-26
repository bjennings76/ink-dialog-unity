using Unspeakable.Utils;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Dialog {
	public class Line : MonoBehaviour {
		public enum Type {
			None,
			Default,
			Player,
			NPC,
			Title,
			Subtitle,
			Choice
		}

		[SerializeField] private Type m_Type = Type.Default;
		[SerializeField] private float m_FadeDuration = 1;
		[SerializeField] private float m_SlideDuration = 0.5f;
		[SerializeField] private int m_HiddenBottomPadding = -10;

		private Canvas m_Canvas;
		private RectTransform m_CanvasTransform;
		private CanvasRenderer[] m_Renderers;
		private Text m_Text;
		private LayoutGroup m_LayoutGroup;
		private int m_TargetBottom;
		private float m_Alpha;
		private bool m_Skip;

		private float Alpha {
			get { return m_Alpha; }
			set {
				m_Alpha = Mathf.Clamp01(value);
				Renderers.ForEach(r => {
					if (r) { r.SetAlpha(m_Alpha); }
				});
			}
		}

		private Canvas Canvas { get { return !this ? null : m_Canvas ? m_Canvas : (m_Canvas = GetComponentInParent<Canvas>()); } }
		private RectTransform CanvasTransform {
			get { return !this ? null : m_CanvasTransform ? m_CanvasTransform : (m_CanvasTransform = Canvas.GetComponent<RectTransform>()); }
		}
		private CanvasRenderer[] Renderers { get { return !this ? null : m_Renderers != null ? m_Renderers : (m_Renderers = GetComponentsInChildren<CanvasRenderer>()); } }
		private Text Text { get { return !this ? null : m_Text ? m_Text : (m_Text = GetComponentInChildren<Text>()); } }
		private LayoutGroup LayoutGroup { get { return !this ? null : m_LayoutGroup ? m_LayoutGroup : (m_LayoutGroup = GetComponent<LayoutGroup>()); } }

		public Type LineType { get { return m_Type; } }

		private void Start() {
			if (m_Skip) { return; }

			m_TargetBottom = LayoutGroup.padding.bottom;
			LayoutGroup.padding.bottom = m_HiddenBottomPadding;
			Alpha = 0;
			DOTween.Sequence()
					.Append(DOTween.To(() => LayoutGroup.padding.bottom, v => LayoutGroup.padding.bottom = v, m_TargetBottom, m_SlideDuration))
					.Insert(0, DOTween.To(() => Alpha, v => Alpha = v, 1, m_FadeDuration))
					.OnUpdate(() => LayoutRebuilder.MarkLayoutForRebuild(CanvasTransform))
					.OnComplete(() => m_Skip = true);
		}

		public void SetText(string text, bool skipTransition = false) {
			Text.text = CleanText(text);
			m_Skip = skipTransition;
		}

		public Tween FadeOut() {
			m_Skip = true;
			Alpha = 1;
			Sequence sequence = DOTween.Sequence();
			sequence.Append(DOTween.To(() => Alpha, v => Alpha = v, 0, 0.5f))
					.Insert(0, DOTween.To(() => LayoutGroup.padding.bottom, v => LayoutGroup.padding.bottom = v, m_HiddenBottomPadding, 0.5f))
					.OnUpdate(() => LayoutRebuilder.MarkLayoutForRebuild(CanvasTransform))
					.OnComplete(() => UnityUtils.DestroyObject(this));
			return sequence;
		}

		private static string CleanText(string text) { return text.ReplaceRegex(@"\<[^>]+\>", ""); }
	}
}