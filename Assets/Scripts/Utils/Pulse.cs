using UnityEngine;
using UnityEngine.UI;

namespace Unspeakable.Utils {
	[RequireComponent(typeof(Graphic))]
	public class Pulse : MonoBehaviour {
		[SerializeField] private AnimationCurve m_Curve = AnimationCurve.EaseInOut(0, 1, 1, 0);
		[SerializeField] private Graphic m_Graphic;
		[SerializeField] private Color m_ColorA = new Color(1, 1, 1, 1);
		[SerializeField] private Color m_ColorB = new Color(1, 1, 1, 0);

		private void Start() {
			m_Graphic = m_Graphic ? m_Graphic : (m_Graphic = GetComponent<Graphic>());
			if (m_Graphic) { m_ColorA = m_Graphic.color; }
			if (m_Curve.postWrapMode != WrapMode.Loop && m_Curve.postWrapMode != WrapMode.PingPong) { m_Curve.postWrapMode = WrapMode.PingPong; }
		}

		private void Update() {
			if (m_Graphic) { m_Graphic.color = Color.LerpUnclamped(m_ColorB, m_ColorA, m_Curve.Evaluate(Time.time)); }
		}
	}
}