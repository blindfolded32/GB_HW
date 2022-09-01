using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class RegenCorutine : MonoBehaviour
{

    public Button OneTimeHealButton;
    public Button DamageButton;

    [SerializeField] private float _maxHP = 100.0f;
    [SerializeField] private Health _health;
    [SerializeField] private Text _text;

    private bool _isHealing;
   // private bool _isHealOnPause;

    private Coroutine _healingCoroutine;
    private float _time = 10.0f;

    private float _startHP = 10.0f;

    private void Awake()
    {
        _health = new Health(_maxHP);
        _health.Value = _startHP;
        _text.text = _health.Value.ToString();
        _health.OnMaxHP += StopHealing;
        DamageButton.onClick.AddListener(Damage);
        OneTimeHealButton.onClick.AddListener(OneTimeHeal);
    }

    private void Update()
    {
        if (!_isHealing) return;
        
        if (_time < 3.0f)
        {
            _time += Time.deltaTime;
        }
        else //(_time > 3.0f)
        {
            StopHealing();
        }
    }

    private void StopHealing()
    {
        _isHealing = false;
        StopCoroutine(_healingCoroutine);
    }


   private IEnumerator ReceiveHealing()
    {
        
        while (_isHealing && _time <= 3.0f)
        {
            _health.Value += 5f;
            _text.text = _health.Value.ToString();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OneTimeHeal()
    {
        if (_isHealing) return;
        _isHealing = true;
        _time = 0.0f;
        _healingCoroutine = StartCoroutine(ReceiveHealing());
    }

    private void Damage()
    {
        _health.Value -= 10.0f;
        _text.text = _health.Value.ToString();
    }
    private void OnDestroy()
    {
        DamageButton.onClick.RemoveAllListeners();
        OneTimeHealButton.onClick.RemoveAllListeners();
        _health.OnMaxHP -= StopHealing;
    }
}
