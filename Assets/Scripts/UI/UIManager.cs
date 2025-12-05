using ChronoHeist.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChronoHeist.Ui
{
    public class UIManager : Manager<UIManager>
    {
        [Header("Lose Panel References")]
        [SerializeField]
        private GameObject _losePanel;
        [SerializeField]
        private Button _previousButton;
        [SerializeField]
        private Button _nextButton;
        [SerializeField]
        private Slider _slider;
        [SerializeField]
        private Button _resumeButton;

        [Header("Win Panel References")]
        [SerializeField]
        private GameObject _winPanel;
        [SerializeField]
        private TextMeshProUGUI _goldText;
        [SerializeField]
        private TextMeshProUGUI _stepText;


        public override void InitializeManager()
        {
            _previousButton.onClick.AddListener(StepBack);
            _nextButton.onClick.AddListener(StepForward);
            _slider.onValueChanged.AddListener(value => CommandManager.Instance.ScrubToTurn((int)value));
            _resumeButton.onClick.AddListener(ResumeGame);

            EventManager.RegisterEvent<EventManager.OnGameEnded>(OnGameEnded);
        }

        private void OnGameEnded(EventManager.OnGameEnded obj)
        {
            if (obj.win)
            {
                OpenWinPanel();
            }
            else
            {
                OpenLosePanel();
            }
        }

        private void OpenLosePanel()
        {
            _losePanel.SetActive(true);

            _slider.maxValue = CommandManager.Instance.MaxHistoryCount - 1;
            _slider.value = CommandManager.Instance.CurrentIndex;
        }

        private void OpenWinPanel()
        {
            _winPanel.SetActive(true);

            _goldText.SetText($"Gold: {GameManager.Instance.CollectedGold}/{GameManager.Instance.MaxGoldCount}");
            _stepText.SetText($"Steps: {GameManager.Instance.StepCount + 1}");
        }

        private void StepBack()
        {
            CommandManager.Instance.StepBack();
            _slider.value = CommandManager.Instance.CurrentIndex;
        }

        private void StepForward()
        {
            CommandManager.Instance.StepForward();
            _slider.value = CommandManager.Instance.CurrentIndex;
        }

        private void ResumeGame()
        {
            _losePanel.SetActive(false);
            TurnManager.Instance.ResumeFromRewind();
        }
    }
}