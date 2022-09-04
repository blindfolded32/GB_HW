using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Health
    {
        public Action OnMaxHP;

        [SerializeField] private float _value;

        private float _maxValue;

        public Health (float maxValue)
        {
            _maxValue = maxValue;
        }

        public float Value 
        { 
            get => _value;
            set 
            { 
                if(value >= _maxValue)
                {
                    _value = _maxValue;
                    OnMaxHP?.Invoke();
                }
                else
                {
                    _value = value <= 0 ? 0 : value;
                }
            } 
        }
    }
}