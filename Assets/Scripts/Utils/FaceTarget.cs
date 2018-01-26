using UnityEngine;

public class FaceTarget : MonoBehaviour {
	[SerializeField] private Transform m_Target;

	public Transform Target { set { m_Target = value; } }

	private void LateUpdate() {
		if (m_Target) { transform.rotation = Quaternion.LookRotation(transform.position - m_Target.position); }
	}
}