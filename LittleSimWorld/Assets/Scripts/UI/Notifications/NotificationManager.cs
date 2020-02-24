using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI.Notifications
{
    public class NotificationManager : MonoBehaviour
    {
        [SerializeField] private GameObject notificationCard = null;
        [SerializeField] private int maxNotification = 2;
        [SerializeField] private float timeToHide = 10f;

        [SerializeField] private RectTransform dockPosition = null;
        [SerializeField] private float timeToDock = 5f;
        [SerializeField] private int maxDockedCard = 2;
        [SerializeField] private float showDelay = 0.5f;
        
        private Notify _notifications;
        private Queue<NotificationCard> _activeCards;
        private List<NotificationCard> _dockedCards;
        private List<NotificationCard> _notificationCardPool;
        private Queue<(string, Sprite)> _pendingNotifications;
        
        private Vector2 _spawnPosition;
        private bool _animatingNewCard;

        private void Awake()
        {
            _notifications = new Notify(this);
            _notificationCardPool = new List<NotificationCard>();
            _activeCards = new Queue<NotificationCard>();
            _dockedCards = new List<NotificationCard>();
            _pendingNotifications= new Queue<(string, Sprite)>();
        }


        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F7))
                Notify.Show("Test notification!");
#endif

            UpdateNotifications();
        }

        private void UpdateNotifications()
        {
            if (_pendingNotifications.Count == 0 || _animatingNewCard) return;
                AddNotification(_pendingNotifications.Dequeue());
            
        }

        public void CreateNotification(string message, Sprite icon)
        {
            _pendingNotifications.Enqueue((message, icon));
        }
        
        private void AddNotification((string,Sprite) cardData)
        {
            if (_activeCards.Count > 0)
            {
                if (_activeCards.Count > maxNotification)
                    DockCard(_activeCards.Dequeue());

                MoveActiveCardsUp();
            }

            var (message, icon) = cardData;
            CreateNewNotification(message, icon);

            StartCoroutine(DelayNextNotification(showDelay));
        }

        private IEnumerator DelayNextNotification(float duration)
        {
            _animatingNewCard = true;
            yield return new WaitForSeconds(duration);
            _animatingNewCard = false;
        }

        private void DockCard(NotificationCard card)
        {
            CancelInvoke(nameof(CleanUpDock));
            
            _dockedCards.Add(card);
            
            if (_dockedCards.Count > maxDockedCard)
                RemoveDockedItem();
            
            card.Dock(GetDockPosition());
            
            Invoke(nameof(CleanUpDock), timeToHide);
        }

        private void RemoveDockedItem()
        {
            _dockedCards[0].Hide();
            _dockedCards.RemoveAt(0);

            if (_dockedCards.Count == 0) return;

            foreach (var card in _dockedCards)
                card.MoveUpOnDock();
        }

        private void CleanUpDock()
        {
            if (_dockedCards.Count > 0)
                RemoveDockedItem();
            
            if (_dockedCards.Count > 0)
                Invoke(nameof(CleanUpDock), timeToHide);
        }

        private void CreateNewNotification(string message, Sprite icon)
        {
            var newCard = (_notificationCardPool.Count(card => card.Free) == 0) ?
                CreateNewCard() : _notificationCardPool.Find(card => card.Free);
            
            _activeCards.Enqueue(newCard);
            newCard.transform.position = _spawnPosition;
            newCard.Show(message, icon);
            
            CancelInvoke(nameof(DockRecentCard));
            Invoke(nameof(DockRecentCard), timeToDock);
        }

        private NotificationCard CreateNewCard()
        {
            var newCard = Instantiate(notificationCard, transform).GetComponentInChildren<NotificationCard>();
            _spawnPosition = newCard.transform.position;
            _notificationCardPool.Add(newCard);
            newCard.OnForceHide.AddListener(CardForceHide);

            return newCard;
        }

        private void CardForceHide(NotificationCard hidingCard)
        {
            var cardIndex = _dockedCards.IndexOf(hidingCard);
            var ctr = 0;
            
            foreach (var card in _dockedCards)
            {
                if (ctr > cardIndex)
                    card.MoveUpOnDock();

                ctr++;
            }
            
            _dockedCards.RemoveAt(cardIndex);
            
            CancelInvoke(nameof(CleanUpDock));
            Invoke(nameof(CleanUpDock), timeToHide);
        }

        private void DockRecentCard()
        {
            if (_activeCards.Count > 0)
                DockCard(_activeCards.Dequeue());
            
            if (_activeCards.Count > 0)
                Invoke(nameof(DockRecentCard), timeToHide);
        }

        private void MoveActiveCardsUp()
        {
            foreach (var card in _activeCards)
                card.MoveUp();
        }
        
        private Vector2 GetDockPosition()
        {
            var cardHeight = (_dockedCards.Count > 1) ?
                _dockedCards[0].GetCardHeight() : 0;

            var corners = new Vector3[4];
            dockPosition.GetWorldCorners(corners);

            var nextPosition = corners[corners.Length - 1];
            nextPosition.y -= cardHeight * (_dockedCards.Count - 1);

            return nextPosition;
        }
    }

    public class Notify
    {
        private Notify _instance;
        private static NotificationManager _manager = null;
        
        public Notify(NotificationManager manager)
        {
            _manager = manager;
        }

        public static bool Ready => (_manager != null);

        public static void Show(string message, Sprite icon = null)
        {
            if (!Ready) return;

            _manager.CreateNotification(message, icon);
        }
    }
}