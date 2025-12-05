using ChronoHeist.Core;
using UnityEngine;

namespace ChronoHeist.Command
{
    public class CollectCommand : ICommand
    {
        private readonly GameObject _item;


        public CollectCommand(GameObject item)
        {
            _item = item;
        }
        
        public void Execute(bool replay)
        {
            _item.SetActive(false);
            GameManager.Instance.CollectedGold++;
        }
        
        public void Undo()
        {
            _item.SetActive(true);
            GameManager.Instance.CollectedGold--;
        }
    }
}
