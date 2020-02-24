using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Buttons
{
    [RequireComponent(typeof(Button))]
    public class LSWButton : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private SpriteSet buttonSprite = null;
        [SerializeField] private float buttonTravel = 1f;

        [SerializeField] private float pixelsPerUnitMultiplier = 1;
        
        private Image _buttonBg;
        private Image _buttonCaps;

        private ButtonData _buttonBgData;
        private ButtonData _buttonCapsData;
        
        public readonly Color _normalColor = new Color(255/255f,255/255f,255/255f,255/255f);
        public readonly Color _hoverColor = new Color(255/255f,255/255f,255/255f,200/255f);
        public readonly Color _pressedColor = new Color(255/255f,255/255f,255/255f,100/255f);
        private bool _pointerInside;
        private bool _lockPress;
        private bool _pressed;

        private UnityEngine.Object Background => Resources.Load("UI/Button/Background");
        private UnityEngine.Object KeyCap => Resources.Load("UI/Button/KeyCap");

        private void Awake()
        {
            InitializeImages();
            RefreshSprite();    

            _buttonBgData = new ButtonData(transform.GetChild(0).GetComponent<RectTransform>(), _buttonBg);
            _buttonCapsData = new ButtonData(transform.GetChild(1).GetComponent<RectTransform>(), _buttonCaps);
            
            _buttonCapsData.Image.color = _normalColor;
        }

        private void InitializeImages()
        {
            if (!_buttonBg)
                _buttonBg = transform.GetChild(0).GetComponent<Image>();
            
            if (!_buttonCaps)
                _buttonCaps = transform.GetChild(1).GetComponent<Image>();
        }
        
        private void RefreshSprite()
        {
            _buttonBg.sprite = buttonSprite.keyShadow;
            _buttonCaps.sprite = buttonSprite.keyTop;

            _buttonBg.pixelsPerUnitMultiplier = pixelsPerUnitMultiplier;
            _buttonCaps.pixelsPerUnitMultiplier = pixelsPerUnitMultiplier;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Press();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Release();
        }    

        public void OnPointerEnter(PointerEventData eventData)
        {
            _pointerInside = true;
            ChangeHighlight(_hoverColor);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _pointerInside = false;
            ChangeHighlight(_normalColor);    
        }

        public void HoldPress()
        {
            Press();
            _lockPress = true;
        }
        
        public void ReleasePress()
        {
            _lockPress = false;
            Release();
        }
            
        private void Press()
        {
            if (_lockPress) return;    
            
            _buttonBgData.Update();
            _buttonCapsData.Update();
    
            var bgDelta = _buttonBgData.SizeDelta;
            var capsPosition = _buttonCapsData.Position;
            bgDelta.y -= buttonTravel;
            capsPosition.y -= buttonTravel;

            _buttonBgData.RectTransform.sizeDelta = bgDelta;
            _buttonCapsData.RectTransform.localPosition = capsPosition;
            _buttonCapsData.Image.color = _pressedColor;

            _pressed = true;    
        }
        
        private void Release()
        {
            if (_lockPress) return;
            
            _buttonBgData.RectTransform.sizeDelta = _buttonBgData.SizeDelta;
            _buttonCapsData.RectTransform.localPosition = _buttonCapsData.Position;
            _buttonCapsData.Image.color = _pointerInside ? _hoverColor : _normalColor;
            
            _pressed = false;
        }
        
        public void ChangeHighlight(Color color)
        {
            if (_pressed) return;
            
            _buttonCapsData.Image.color = color;
        }


        #region EDITOR FUNCTIONS

        private void Reset()
        {
            var button = GetComponent<Button>();
    
            button.transition = Selectable.Transition.None;
            button.navigation = new Navigation();

            Instantiate(Background, transform).name = Background.name;
            Instantiate(KeyCap, transform).name = KeyCap.name;    
            
            buttonSprite = Resources.Load("UI/Button/Skins/Default") as SpriteSet;
            
            OnValidate();    
        }

        private void OnValidate()
        {    
            InitializeImages();

            if (!buttonSprite) return;
                
            RefreshSprite();    
        }    

        #endregion
                
        private struct ButtonData
        {
            public readonly RectTransform RectTransform;    
            public Vector2 SizeDelta { get; private set; }
            public Vector2 Position { get; private set; }    
            public readonly Image Image;

            public ButtonData(RectTransform rt,Image image)
            {    
                RectTransform = rt;
                SizeDelta = rt.sizeDelta;
                Image = image;
                Position = rt.localPosition;    
            }    

            public void Update()
            {
                SizeDelta = RectTransform.sizeDelta;
                Position = RectTransform.localPosition;
            }
        }
    }
}        
    