using DG.Tweening;
using MessagePipe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace GuessWho
{
    [RequireComponent(typeof(Image), typeof(CanvasGroup))]
    public class CardButton : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private const float ANIMATION_DURATION = 0.2f;

        [SerializeField] private Image _character;
        [SerializeField] private Image _mark;


        private bool _isFlipped;
        private bool _isAnimating;

        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Image _image;

        private IPublisher<CardFlippedEvent> _publisher;

        [Inject]
        private void Construct(IPublisher<CardFlippedEvent> publisher)
        {
            _publisher = publisher;
        }

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _image = GetComponent<Image>();
        }

        private void FlipCard()
        {
            if (_isAnimating)
                return;

            _isAnimating = true;
            _rectTransform.DOKill();

            Sequence sequence = DOTween.Sequence();
            sequence.Append
            (
                _rectTransform
                    .DOLocalRotate(new Vector3(0f, 90f, 0f), ANIMATION_DURATION * 0.5f)
                    .SetEase(Ease.InOutSine)
            );
            sequence.AppendCallback(() =>
            {
                _isFlipped = !_isFlipped;
                _mark.gameObject.SetActive(_isFlipped);
                _image.enabled = !_isFlipped;
                _character.gameObject.SetActive(!_isFlipped);
            });
            sequence.Append
            (
                _rectTransform
                    .DOLocalRotate(Vector3.zero, ANIMATION_DURATION * 0.5f)
                    .SetEase(Ease.InOutSine)
            );
            sequence.OnComplete(() =>
            {
                _publisher.Publish(new CardFlippedEvent { IsFlipped = _isFlipped });
                _isAnimating = false;
            });
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _rectTransform.DOScale(1.1f, ANIMATION_DURATION).From(1f).SetEase(Ease.OutBack);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _rectTransform.DOScale(1f, ANIMATION_DURATION).From(1.1f).SetEase(Ease.OutBack);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _canvasGroup.DOFade(0.5f, ANIMATION_DURATION).From(1f).SetEase(Ease.OutBack);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerCurrentRaycast.gameObject == gameObject ||
                eventData.pointerCurrentRaycast.gameObject == _mark.gameObject)
            {
                FlipCard();
            }

            _canvasGroup.DOFade(1f, ANIMATION_DURATION).From(0.5f).SetEase(Ease.OutBack);
        }
    }
}
