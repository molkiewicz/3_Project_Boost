﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    enum State
    {
        Alive,
        Dying,
        Transcending
    }

    private State state = State.Alive;
    [SerializeField] private float rcsThrust = 100f;
    [SerializeField] private float mainThrust = 600f;
    [SerializeField] private AudioClip mainEngineAudioClip;
    [SerializeField] private AudioClip deathAudioClip;
    [SerializeField] private AudioClip winAudioClip;
    [SerializeField] private ParticleSystem mainEngineParticleSystem;
    [SerializeField] private ParticleSystem deathParticleSystem;
    [SerializeField] private ParticleSystem winParticleSystem;
    [SerializeField] private float timeForNoControlState = 1f;
    [SerializeField] private GameObject shield;
    private int currentShieldLevel;
    private Rigidbody _rigidBody;
    private AudioSource _audioSource;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }

        shield.SetActive(currentShieldLevel > 0);
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            _audioSource.Stop();
            mainEngineParticleSystem.Stop();
        }
    }

    private void ApplyThrust()
    {
        _rigidBody.AddRelativeForce(mainThrust * Time.deltaTime * Vector3.up);
        if (!_audioSource.isPlaying)
        {
            _audioSource.PlayOneShot(mainEngineAudioClip);
        }
        mainEngineParticleSystem.Play();
    }

    private void RespondToRotateInput()
    {

        _rigidBody.freezeRotation = true;
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        _rigidBody.freezeRotation = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) return;
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                FinishLevel();
                break;
            case "Secret":
                GainShield();
                break;
            default:
                if (currentShieldLevel <= 0)
                {
                    RespondToRocketDamage();
                }
                else
                {
                    LooseShield();
                }
                break;
        }
    }

    private void LooseShield()
    {
        currentShieldLevel--;
    }

    private void GainShield()
    {
        if (currentShieldLevel <= 3)
        {
            currentShieldLevel++;
        }
    }
    private void RespondToRocketDamage()
    {
        state = State.Dying;
        _audioSource.Stop();
        _audioSource.PlayOneShot(deathAudioClip, 0.2f);
        mainEngineParticleSystem.Stop();
        deathParticleSystem.Play();
        Invoke(nameof(ResetLevel), timeForNoControlState);
        Debug.Log("Hit");
    }

    private void FinishLevel()
    {
        state = State.Transcending;
        _audioSource.Stop();
        _audioSource.PlayOneShot(winAudioClip, 0.3f);
        mainEngineParticleSystem.Stop();
        winParticleSystem.Play();
        Invoke(nameof(LoadNextScene), timeForNoControlState);
    }

    private void ResetLevel()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

