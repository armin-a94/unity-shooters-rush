namespace DevelopersHub.ShootersRush
{
    using System.Collections.Generic;
    using UnityEngine;
    using System.Collections;
    
    public class ShootersRushCharacterGroup : MonoBehaviour
    {
        
        private Transform _pivot = null; public Transform pivot { get { return _pivot; } }
        private List<ShootersRushCharacter> _characters = new List<ShootersRushCharacter>();
        private float _maxMoveDistance = 0;
        private float _groupWidth = 0;
        private Vector2 _moveInput = Vector2.zero; public Vector2 moveInput { get { return _moveInput; } }
        private Vector2 _animatorInput = Vector2.zero; public Vector2 animatorInput { get { return _animatorInput; } }
        private bool _initialized = false;
        public int charactersCount { get { return _characters.Count; } }
        public List<ShootersRushCharacter> characters { get { return _characters; } }
        private AudioSource _fireAudioSource = null;
        private bool _openFire = false;
        
        private void Awake()
        {
            Initialize();
            StartCoroutine(OpenFire());
        }

        private void Initialize()
        {
            if (_initialized)
            {
                return;
            }
            _initialized = true;
            _pivot = new GameObject("Container").transform;
            _pivot.SetParent(transform);
            _pivot.localPosition = new Vector3(0, 0, -0.1f);
            _pivot.localEulerAngles = Vector3.zero;
            _fireAudioSource = gameObject.AddComponent<AudioSource>();
            _fireAudioSource.playOnAwake = false;
            _fireAudioSource.loop = false;
        }

        private IEnumerator OpenFire()
        {
            yield return new WaitForSeconds(1f);
            _openFire = true;
        }
        
        private void Update()
        {
            if (_characters.Count > 0)
            {
                if (ShootersRushInput.singleton.horizontal > 0 && _pivot.localPosition.x < _maxMoveDistance)
                {
                    _moveInput.x = 1;
                }
                else if (ShootersRushInput.singleton.horizontal < 0 && _pivot.localPosition.x > -_maxMoveDistance)
                {
                    _moveInput.x = -1;
                }
                else
                {
                    _moveInput.x = 0;
                }
                _pivot.Translate(new Vector3(_moveInput.x, 0, 0) * 2f * Time.deltaTime, Space.Self);
                if (_openFire && ShootersRushManager.Singleton.openFire && ShootersRushManager.Singleton.gameStatus == 0)
                {
                    bool playSound = false;
                    for (int i = 0; i < _characters.Count; i++)
                    {
                        if (_characters[i].Fire())
                        {
                            playSound = true;
                        }
                    }
                    if (playSound && _characters[0].weapon.fireSound != null)
                    {
                        _fireAudioSource.PlayOneShot(_characters[0].weapon.fireSound);
                    }
                }
            }
            else
            {
                _moveInput.x = 0;
            }
            _animatorInput = Vector2.Lerp(_animatorInput, _moveInput, 10f * Time.deltaTime);
        }

        public void ChangeWeapon(string id)
        {
            ShootersRushWeapon prefab = ShootersRushLibrary.GetWeaponPrefab(id);
            ChangeWeapon(prefab);
        }
        
        public void ChangeWeapon(ShootersRushWeapon prefab)
        {
            if (_characters.Count > 0 && prefab != null)
            {
                for (int i = 0; i < _characters.Count; i++)
                {
                    if (_characters[i] != null)
                    {
                        _characters[i].EquipWeapon(prefab);
                        if (!ShootersRushManager.Singleton.randomizeFireTime)
                        {
                            _characters[i].weapon.ResetFireTimer();
                        }
                    }
                }
            }
        }
        
        public void RemoveAll()
        {
            foreach (var character in _characters)
            {
               Destroy(character.gameObject);
            }
            _characters.Clear();
        }
        
        public void RemoveCharacter(ShootersRushCharacter character)
        {
            if (character != null && _characters.Remove(character))
            {
                character.transform.SetParent(null);
                SetCharactersPosition();
            }
        }
        
        public void AddCharacter(ShootersRushCharacter character)
        {
            Initialize();
            character.transform.SetParent(_pivot);
            _characters.Add(character);
            SetCharactersPosition();
            character.SetGroup(this);
            character.transform.localPosition = character.targetLocalPosition;
            character.transform.localEulerAngles = Vector3.zero;
            if (_characters.Count > 0 && !ShootersRushManager.Singleton.randomizeFireTime)
            {
                for (int i = 0; i < _characters.Count; i++)
                {
                    if (_characters[i] != null && _characters[i].weapon != null)
                    {
                        _characters[i].weapon.ResetFireTimer();
                    }
                }
            }
        }
        
        public ShootersRushCharacter _GetTarget(ShootersRushEnemy enemy)
        {
            if (enemy != null)
            {
                foreach (var character in _characters)
                {
                    if (character.row == 1)
                    {
                        return character;
                    }
                }
            }
            return null;
        }
        
        private void SetCharactersPosition()
        {
            if (_characters.Count > 0)
            {
                int totalCharacters = _characters.Count;
                int maxColumns = ShootersRushManager.Singleton.characterGroupMaxColumns;
                int columns = Mathf.Min(totalCharacters, maxColumns);
                int rows = Mathf.CeilToInt((float)totalCharacters / columns);
                int i = 0;
                int remained = totalCharacters;
                _groupWidth = (columns - 1) * ShootersRushManager.Singleton.characterGroupColumnsSpace;
                if (totalCharacters % columns != 0 && totalCharacters % columns < columns - 1)
                {
                    columns = Mathf.CeilToInt((float)totalCharacters / rows);
                    rows = Mathf.CeilToInt((float)totalCharacters / columns);
                }
                for (int r = 0; r < rows; r++)
                {
                    int rowColumns = remained >= columns ? columns : remained;
                    float basePositionX = ShootersRushManager.Singleton.characterGroupColumnsSpace * 0.5f * (rowColumns - 1);
                    for (int c = 0; c < rowColumns; c++)
                    {
                        Vector3 position = new Vector3(basePositionX - (c * ShootersRushManager.Singleton.characterGroupColumnsSpace), 0, -r * ShootersRushManager.Singleton.characterGroupRowsSpace);
                        _characters[i].SetLocalPosition(position, c + 1, r + 1);
                        remained--;
                        i++;
                    }
                }
                _maxMoveDistance = ShootersRushManager.Singleton.roadWidth * 0.5f - _groupWidth * 0.5f;
            }
        }
        
    }
}