using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using KuroNovel.DataNode;
using KuroNovel.Utils;
using System.Collections.Generic;
using System;

namespace KuroNovel.Manager
{
    public class VNUIManager : MonoSingleton<VNUIManager>
    {
        [Header("VN Panel Properties")]
        [SerializeField] private CanvasGroup vn_GroupPanel;
        [SerializeField] private GameObject vn_DialoguePanel;
        [SerializeField] private GameObject vn_SpeakerPanel;

        [Header("Dialogue Propeties")]
        [SerializeField] private TextMeshProUGUI vn_DialogueText;
        [SerializeField] private TextMeshProUGUI vn_SpeakerText;

        [Header("Choices Properties")]
        [SerializeField] private GameObject vn_ChoiceButtonTemplate; // Button template for choices
        [SerializeField] private VerticalLayoutGroup vn_ChoicesLayoutGroup; // Layout group for organizing buttons

        [Header("Background Properties")]
        [SerializeField] private Image vn_Background;

        [Header("Sprite Properties")]
        [SerializeField] private GameObject vn_SpriteRef;
        [SerializeField] private HorizontalLayoutGroup vn_SpriteLayoutGroup;

        private List<GameObject> m_ActiveButtons = new List<GameObject>();

        public override void Awake()
        {
            base.Awake();
        }

        #region Dialogue Handler
        public void ShowDialogue(DialogueNode dialogueNode, Action onComplete)
        {
            vn_DialoguePanel.SetActive(true);
            vn_SpeakerText.text = dialogueNode.Speaker;

            vn_DialogueText.text = "";
            TextEnded = false;

            StartCoroutine(WaitForInput(dialogueNode.DialogueText, onComplete));
        }

        public bool TextEnded = false;

        private IEnumerator WaitForInput(string text, Action onComplete)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if (vn_DialogueText.text != text && !TextEnded)
                {
                    vn_DialogueText.text += text[i];
                    yield return new WaitForSeconds(0.1f);
                }
                else
                {
                    TextEnded = true;
                    vn_DialogueText.text = text;
                    break;
                }
            }

            while (!Input.GetKeyDown(KeyCode.Space))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.1f); //DUNNO

            // Hide the dialogue panel and invoke the completion callback
            vn_DialoguePanel.SetActive(false);
            onComplete?.Invoke();
        }
        #endregion
        #region Choices Handler

        /// <summary>
        /// Displays a list of choices to the player.
        /// </summary>
        /// <param name="choices">List of choices to display.</param>
        /// <param name="onChoiceSelected">Callback for when a choice is selected.</param>
        public void ShowChoices(ChoicesNode choicesNode, Action<int> onChoiceSelected)
        {
            ClearChoices();

            for (int i = 0; i < choicesNode.Choices.Count; i++)
            {
                var choiceText = choicesNode.Choices[i].Text;
                var choiceIndex = i;

                var buttonObject = Instantiate(vn_ChoiceButtonTemplate, vn_ChoicesLayoutGroup.transform);
                buttonObject.SetActive(true);

                var button = buttonObject.GetComponent<Button>();
                var buttonText = buttonObject.GetComponentInChildren<TextMeshProUGUI>();

                if (buttonText != null)
                    buttonText.text = choiceText;

                button.onClick.AddListener(() =>
                {
                    onChoiceSelected?.Invoke(choiceIndex);
                    ShowChoices(false);
                });

                m_ActiveButtons.Add(buttonObject);
            }

            ShowChoices(true);
        }

        private void ClearChoices()
        {
            foreach (var button in m_ActiveButtons)
            {
                Destroy(button);
            }

            m_ActiveButtons.Clear();
        }

        private void ShowChoices(bool set)
        {
            vn_ChoicesLayoutGroup.gameObject.SetActive(set);
        }
        #endregion
        #region BG Handler
        public void ShowBackground(BackgroundNode backgroundNode, Action onComplete)
        {
            // Use Resources.Load with the correct path
            /*string resourcePath = backgroundNode.Background.Replace(Application.dataPath, "").TrimStart('/');
            Sprite sprite = Resources.Load<Sprite>(resourcePath);*/

            if (backgroundNode.Background != null)
            {
                vn_Background.sprite = backgroundNode.Background;
                onComplete?.Invoke();
            }
            else
            {
                Debug.LogError($"Background sprite not found");
                onComplete?.Invoke();
            }

        }
        #endregion
        #region Spritr Handler
        public void ShowSprite(SpriteNode spriteNode, Action onComplete)
        {
            if (spriteNode.CharacterSprite != null)
            {
                var spObject = Instantiate(vn_SpriteRef, vn_SpriteLayoutGroup.transform);
                spObject.gameObject.SetActive(true);

                var sp = spObject.GetComponent<Image>();

                sp.sprite = spriteNode.CharacterSprite;

                onComplete?.Invoke();
            }
            else
            {
                Debug.LogError($"Background sprite not found");
                onComplete?.Invoke();
            }
        }
        #endregion
    }
}