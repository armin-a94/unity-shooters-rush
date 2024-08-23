namespace DevelopersHub.ShootersRush
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ShootersRushInput : MonoBehaviour
    {

        private static ShootersRushInput _singleton = null; public static ShootersRushInput singleton { get { if (_singleton == null) { _singleton = FindFirstObjectByType<ShootersRushInput>(); if (_singleton == null) { _singleton = new GameObject("ShootersRushInput").AddComponent<ShootersRushInput>(); } _singleton.Initialize(); } return _singleton; } }
        private bool _initialized = false;
        
        private float _horizontal = 0; public float horizontal { get { return _horizontal; } set { _horizontal = value; } }

        private ShootersRushButton _moveRightButton = null;
        private ShootersRushButton _moveLeftButton = null;
        
        public void RegisterButton(ShootersRushButton button)
        {
            if (button != null)
            {
                switch (button.type)
                {
                    case ShootersRushButton.Type.MoveRight: _moveRightButton = button; break;
                    case ShootersRushButton.Type.MoveLeft: _moveLeftButton = button; break;
                }
            }
        }
        
        public void UnregisterButton(ShootersRushButton button)
        {
            if (button != null)
            {
                switch (button.type)
                {
                    case ShootersRushButton.Type.MoveRight:
                        if (_moveRightButton == button)
                        {
                            _moveRightButton = null;
                        }
                        break;
                    case ShootersRushButton.Type.MoveLeft:
                        if (_moveLeftButton == button)
                        {
                            _moveLeftButton = null;
                        }
                        break;
                }
            }
        }
        
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_initialized)
            {
                if (_singleton != this)
                {
                    Destroy(gameObject);
                }
                return;
            }
            _initialized = true;
            DontDestroyOnLoad(gameObject);
        }
        
        public static void Remove()
        {
            if (_singleton != null)
            {
                Destroy(_singleton.gameObject);
            }
            _singleton = null;
        }

        private void Update()
        {
            if (_moveRightButton != null && _moveRightButton.isPressed)
            {
                _horizontal = 1;
            }
            else if (_moveLeftButton != null && _moveLeftButton.isPressed)
            {
                _horizontal = -1;
            }
            else
            {
                _horizontal = 0;
            }
        }
        
    }
}