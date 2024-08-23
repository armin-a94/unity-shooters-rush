namespace DevelopersHub.ShootersRush
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    
    public class ShootersRushButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
     
        [SerializeField] private Type _type = Type.MoveRight; public Type type { get { return _type; } }
        private bool _pressed = false; public bool isPressed { get { return _pressed; } }

        public enum Type
        {
            MoveRight = 0, MoveLeft = 1
        }
        
        private void OnEnable()
        {
            _pressed = false;
            ShootersRushInput.singleton.RegisterButton(this);
        }

        private void OnDisable()
        {
            ShootersRushInput.singleton.UnregisterButton(this);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _pressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _pressed = false;
        }
        
    }
}