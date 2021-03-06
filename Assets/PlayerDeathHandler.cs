﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
    [SerializeField] GameObject _entirePlayerGameObject;
    [SerializeField] GameObject _playerModel;
    [SerializeField] ParticleSystem _spiritParticles;

    [SerializeField] float _particleAnimationDuration = 3;
    [SerializeField] private Ease _particleEaseType = Ease.InOutSine;

    bool animationStarted = false;

    [SerializeField] private bool moveBeforeScalingOnDeath = false;

    public void TransitionPlayerBackToCheckpoint()
    {
        if (animationStarted == false)
        {
            animationStarted = true;
            StartCoroutine(PlaySpiritAnimation());
        }
    }

    private IEnumerator PlaySpiritAnimation()
    {

        Vector3 playerSpawnPos = PlayerCheckpoint.LastRegisteredCheckpointPosition;

        TweenParams tweenParams = new TweenParams()
            .SetEase(_particleEaseType);

        _spiritParticles.gameObject.SetActive(true);
        _spiritParticles.Play();

        _entirePlayerGameObject.transform.DOMove(playerSpawnPos, _particleAnimationDuration).SetAs(tweenParams).OnComplete(() => StartCoroutine(CleanupParticles()));


        if (moveBeforeScalingOnDeath)
        {
            yield return new WaitForSeconds(0.6f);
        }

        while (_playerModel.transform.localScale.x > 0)
        {
            _playerModel.transform.localScale -= Vector3.one * (Time.deltaTime * 6);
            yield return new WaitForEndOfFrame();
        }

           _playerModel.gameObject.SetActive(false);

        yield return null;

// can fade player out if we use a transparent supported material

        //Material mat = _playerModel.material;

        /*
        while (mat.color.a > 0)
        {
            Color newColor = mat.color;
            newColor.a -= Time.deltaTime;
            mat.color = newColor;
            _playerModel.material = mat;
            yield return new WaitForEndOfFrame();
        }
        */
    }

    private IEnumerator CleanupParticles()
    {
        animationStarted = false;

        _playerModel.transform.localScale = Vector3.zero;
        _playerModel.gameObject.SetActive(true);

        while (_playerModel.transform.localScale.x < 1)
        {
            _playerModel.transform.localScale += Vector3.one * (Time.deltaTime * 6);
            yield return new WaitForEndOfFrame();
        }

        _playerModel.transform.localScale = Vector3.one;

        //Material mat = _playerModel.material;

        /*
        while (mat.color.a < 1)
        {
            Color newColor = mat.color;
            newColor.a += Time.deltaTime;
            mat.color = newColor;
            _playerModel.material = mat;
            yield return new WaitForEndOfFrame();
        }
        */

        _spiritParticles.Stop();
        yield return new WaitForSeconds(2);
        _spiritParticles.gameObject.SetActive(false);
    }
}
