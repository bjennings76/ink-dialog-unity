using JetBrains.Annotations;
using UnityEngine.Events;

namespace Dialog {
	public class ChoiceLine : Line {
		public UnityEvent OnClick;

		[UsedImplicitly]
		public void OnPress() { OnClick.Invoke(); }
	}
}