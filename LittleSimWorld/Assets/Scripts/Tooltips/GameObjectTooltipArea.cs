using System.Collections;
using UnityEngine;
using Zenject;

namespace LSW.Tooltip
{
    [RequireComponent(typeof(ZenjectDynamicObjectInjection))]
    [AddComponentMenu("Tooltip/Game Object Tooltip Area")]
    public class GameObjectTooltipArea : SimpleBigTooltipArea
    {
        private Collider2D _collider;
      
        public void Start()
        {
            _collider = GetComponent<Collider2D>();
            StartCoroutine(RefreshCollider());
            
        }

        private IEnumerator RefreshCollider()
        {
            _collider.enabled = false;
            yield return null;
            _collider.enabled = true;
        }

    }
}