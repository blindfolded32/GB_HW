using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class RegenCorutine : MonoBehaviour
{
    [SerializeField] private float _maxHP = 100.0f;
    [SerializeField] private Health _health;
    [SerializeField] private Text _text;

    private bool _isHealing;
    private bool _isHealOnPause;
    private Coroutine _healingCoroutine;

    private float _startHP = 10.0f;

    private void Awake()
    {
        _health = new Health(_maxHP);
        _health.Value = _startHP;
        _health.OnMaxHP += StopHealing;
    }

    private void Update()
    {
        if (_isHealing) return;

        if(!_isHealOnPause)
        {
            _healingCoroutine = StartCoroutine(ReceiveHealing());
        }

    }

    private void StopHealing()
    {
        _isHealing = false;
        StopCoroutine(_healingCoroutine);
        StartCoroutine(Wait());
    }

    private IEnumerator ReceiveHealing()
    {
        _isHealing = true;

        while (_isHealing)
        {
            _health.Value += 5f;
            _text.text = _health.Value.ToString();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void Damage()
    {
        _health.Value -= 10.0f;
    }

    private IEnumerator Wait()
    {
        _isHealOnPause = true;
        yield return new WaitForSeconds(3f);
        _isHealOnPause = false;
       // _health.Value = 0;
    }

    private void OnDestroy()
    {
        _health.OnMaxHP -= StopHealing;
    }
}
