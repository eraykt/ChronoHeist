using ChronoHeist.Core;
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
        public override void InitializeManager()
        {
            _previousButton.onClick.AddListener(StepBack);
            _nextButton.onClick.AddListener(StepForward);
            _slider.onValueChanged.AddListener(value => CommandManager.Instance.ScrubToTurn((int)value));
            _resumeButton.onClick.AddListener(ResumeGame);
            
            EventManager.RegisterEvent<EventManager.OnGameLose>(OnGameLose);
        }
        private void OnGameLose(EventManager.OnGameLose obj)
        {
            OpenLosePanel();
        }

        public void OpenLosePanel()
        {
            _losePanel.SetActive(true);

            _slider.maxValue = CommandManager.Instance.MaxHistoryCount - 1;
            _slider.value = CommandManager.Instance.CurrentIndex;
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
    
            // TurnManager'a durumu bildir
            // (TurnManager'a public bir metod eklemen gerekecek)
            TurnManager.Instance.ResumeFromRewind();
        }
    }
}
