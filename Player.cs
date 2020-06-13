using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private UIManager _uiManager;

    [SerializeField]
    private AudioSource _laserAudioSource;
    [SerializeField]
    private AudioClip _laserAudioClip;

    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;

    [SerializeField]
    private GameObject _shieldVizualizer;
    [SerializeField]
    private GameObject _rightFire;
    [SerializeField]
    private GameObject _leftFire;

    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private float _fireRate = 0.15f;
    [SerializeField]
    private float _nextFire = 0.0f;
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private float _speedBoostSpeed = 8.5f;

    [SerializeField]
    private SpawnManager _spawnManager;

    [SerializeField]
    private bool _tripleShotEnabled = false;
    [SerializeField]
    private bool _speedBoostEnabled = false;
    [SerializeField]
    private bool _shieldEnabled = false;

    [SerializeField]
    private int _score;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        _laserAudioSource = GetComponent<AudioSource>();
        _laserAudioSource.clip = _laserAudioClip;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        CreateLaser();
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (_speedBoostEnabled == true)
        {
            transform.Translate(Vector3.right * _speedBoostSpeed * horizontalInput * Time.deltaTime);
            transform.Translate(Vector3.up * _speedBoostSpeed * verticalInput * Time.deltaTime);
        }

        else
        {
            transform.Translate(Vector3.right * _speed * horizontalInput * Time.deltaTime);
            transform.Translate(Vector3.up * _speed * verticalInput * Time.deltaTime);
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -10, 10), Mathf.Clamp(transform.position.y, -3.5f, 0), 0);
    }
    
    void CreateLaser()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (Time.time - _nextFire >= _fireRate))
        {
            _nextFire = Time.time;

            if (_tripleShotEnabled == false)
            {
                Instantiate(_laserPrefab, transform.position + new Vector3(0, 0.8f, 0), Quaternion.identity);
            }
            
            else
            {
                Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
            }

            _laserAudioSource.Play();
        }
    }

    public void Damage()
    {
        if (_shieldEnabled == true)
        {
            _shieldEnabled = false;
            _shieldVizualizer.SetActive(false);
            return;
        }

        _lives -= 1;

        _uiManager.UpdateLives(_lives);

        if (_lives == 2)
        {
            _rightFire.SetActive(true);
        }

        else if (_lives == 1)
        {
            _leftFire.SetActive(true);
        }

        else if (_lives == 0)
        {
            _leftFire.SetActive(false);
            _rightFire.SetActive(false);

            _spawnManager.onPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void ActivateTripleShot()
    {
        _tripleShotEnabled = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _tripleShotEnabled = false;
    }

    public void ActivateSpeedBoost()
    {
        _speedBoostEnabled = true;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _speedBoostEnabled = false;
    }

    public void ActivateShield()
    {
        _shieldEnabled = true;
        _shieldVizualizer.SetActive(true);
    }

    public void IncreaseScore(int points)
    {
        _score += 10;
        _uiManager.UpdateScore(_score);
    }
}
