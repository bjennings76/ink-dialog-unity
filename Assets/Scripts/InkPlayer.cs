using System;
using System.Linq;
using Unspeakable.Utils;
using DG.Tweening;
using Ink.Runtime;
using UnityEngine;

namespace Dialog {
	public class InkPlayer : MonoBehaviour {
		[SerializeField] private TextAsset m_InkJsonAsset;
		[SerializeField, QuickSelectAsset("t:StageAssets")] private StageAssets m_Assets;

		[Header("Settings")]

		[SerializeField] private float m_NextDelay = 1;

		[Header("UI")]

		[SerializeField] private Canvas m_Canvas;
		[SerializeField] private Line m_TitleLinePrefab;
		[SerializeField] private Line m_SubtitleLinePrefab;
		[SerializeField] private Line m_DefaultLinePrefab;
		[SerializeField] private Line m_NPCLinePrefab;
		[SerializeField] private Line m_PlayerLinePrefab;
		[SerializeField] private ChoiceLine m_PlayerChoicePrefab;

		[Header("Stage")]

		[SerializeField] private Transform m_Backdrop;
		[SerializeField] private Transform m_OnStageLeft;
		[SerializeField] private Transform m_OnStageRight;
		[SerializeField] private Transform m_OffStageLeft;
		[SerializeField] private Transform m_OffStageRight;
		[SerializeField] private float m_SlideDuration = 1;

		private Story m_Story;
		private int m_ResponseIndex;
		private string m_LastBack;
		private string m_LastRight;
		private string m_LastLeft;
		private bool m_OnFirstLine;

		private void OnEnable() { StartStory(); }

		private void StartStory() {
			m_Story = new Story(m_InkJsonAsset.text);
			m_Canvas.transform.DestroyAllChildren();
			m_Backdrop.DestroyAllChildren();
			m_OnStageLeft.DestroyAllChildren();
			m_OnStageRight.DestroyAllChildren();
			m_OffStageLeft.DestroyAllChildren();
			m_OffStageRight.DestroyAllChildren();
			Refresh();
		}

		private void Refresh() {
			float delay = 0;
			foreach (Line line in m_Canvas.GetComponentsInChildren<Line>().Where(l => l)) {
				line.FadeOut().SetDelay(delay);
				delay += 0.1f;
			}

			m_OnFirstLine = true;
			Next();
		}

		private bool GetTagValue(string tagKey, out string tagValue) {
			string tagString = m_Story.currentTags.FirstOrDefault(t => t.StartsWith(tagKey + ":", StringComparison.OrdinalIgnoreCase));
			if (tagString.IsNullOrEmpty()) {
				tagValue = null;
				return false;
			}
			tagValue = tagString.Split(':').ElementAtOrDefault(1);
			return true;
		}

		private void Next() {
			if (m_Story.canContinue) {
				string text = m_Story.Continue().Trim();
				Line nextLine = CreateContentView(text, m_ResponseIndex);
				m_ResponseIndex = -1;

				string back;
				string right;
				string left;

				// Check possible commands.
				if (GetTagValue("back", out back) && back != m_LastBack) {
					m_LastBack = back;
					m_Backdrop.DestroyAllChildren();
					if (m_Assets.Backdrops.ContainsKey(back)) { Instantiate(m_Assets.Backdrops[back], m_Backdrop, false); }
				}

				if (GetTagValue("right", out right) && right != m_LastRight) {
					m_LastRight = right;
					m_OnStageRight.DestroyAllChildren();
					if (m_Assets.Characters.ContainsKey(right)) {
						GameObject go = Instantiate(m_Assets.Characters[right], m_OnStageRight, false);
						go.transform.position = m_OffStageRight.position;
						go.transform.DOLocalMove(Vector3.zero, m_SlideDuration);
					}
				}

				if (GetTagValue("left", out left) && left != m_LastLeft) {
					m_LastLeft = left;
					m_OnStageLeft.DestroyAllChildren();
					if (m_Assets.Characters.ContainsKey(left)) {
						GameObject go = Instantiate(m_Assets.Characters[left], m_OnStageLeft, false);
						go.transform.position = m_OffStageLeft.position;
						go.transform.DOLocalMove(Vector3.zero, m_SlideDuration);
					}
				}

				if (m_OnFirstLine) {
					m_OnFirstLine = false;
					Next();
					return;
				}

				TextFade tf = nextLine.GetComponentInChildren<TextFade>();

				if (tf) { tf.OnComplete.AddListener(Next); }
				else { DelayTracker.DelayAction(m_NextDelay, Next); }

				return;
			}

			if (m_Story.currentChoices.Count > 0) {
				// Fade old choices.
				GetComponentsInChildren<Line>().Where(l => l.LineType == Line.Type.Player).ForEach(l => l.FadeOut());

				for (int i = 0; i < m_Story.currentChoices.Count; i++) {
					Choice choice = m_Story.currentChoices[i];
					ChoiceLine button = CreateChoiceView(choice.text.Trim());
					button.OnClick.AddListener(() => OnClickChoiceButton(button, choice));
				}
			} else {
				ChoiceLine choice = CreateChoiceView("End of story.\nRestart?");
				choice.OnClick.AddListener(StartStory);
			}
		}

		private void OnClickChoiceButton(ChoiceLine sender, Choice choice) {
			m_ResponseIndex = sender.transform.GetSiblingIndex();
			UnityUtils.DestroyObject(sender);
			m_Story.ChooseChoiceIndex(choice.index);
			Refresh();
		}

		private Line CreateContentView(string text, int insertIndex = -1) {
			Line linePrefab = GetLinePrefab(ref text);
			Line storyLine = Instantiate(linePrefab, m_Canvas.transform, false);
			storyLine.SetText(text, insertIndex >= 0);

			if (insertIndex >= 0 && linePrefab == m_PlayerLinePrefab) {
				storyLine.transform.SetSiblingIndex(insertIndex);
			}

			return storyLine;
		}

		private Line GetLinePrefab(ref string text) {
			if (text.Contains("<h4")) { return m_SubtitleLinePrefab; }
			if (text.Contains("<h")) { return m_TitleLinePrefab; }

			if (text.StartsWith("(") || m_Story.currentTags.Contains("desc") || m_Story.currentTags.Contains("writing")) {
				text = text.Trim('(', ')');
				return m_DefaultLinePrefab;
			}
			if (text.Contains("<i>")) { return m_PlayerLinePrefab; }
			return m_NPCLinePrefab;
		}

		private ChoiceLine CreateChoiceView(string text) {
			ChoiceLine choice = Instantiate(m_PlayerChoicePrefab, m_Canvas.transform, false);
			Line line = choice.GetComponent<Line>();
			line.SetText(text);
			return choice;
		}
	}
}