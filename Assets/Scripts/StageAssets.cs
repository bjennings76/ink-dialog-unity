using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dialog {
	[CreateAssetMenu]
	public class StageAssets : ScriptableObject {
		[SerializeField] private GameObject[] m_Backdrops;
		[SerializeField] private GameObject[] m_Characters;

		private Dictionary<string, GameObject> m_BackdropLookup;
		private Dictionary<string, GameObject> m_CharacterLookup;

		public Dictionary<string, GameObject> Backdrops {
			get {
				if (m_BackdropLookup == null) CheckInit();
				return m_BackdropLookup;
			}
		}

		public Dictionary<string, GameObject> Characters {
			get {
				if (m_CharacterLookup == null) CheckInit();
				return m_CharacterLookup;
			}
		}

		private void CheckInit() {
			m_BackdropLookup = m_Backdrops.ToDictionary(go => go.name);
			m_CharacterLookup = m_Characters.ToDictionary(go => go.name);
		}
	}
}