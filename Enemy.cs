using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Player _player;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private AudioSource _explosionAudioSource;

    [SerializeField]
    private float _enemySpeed = 4.0f;
    [SerializeField]
    private float _startingHeight = 7.0f;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _animator = GetComponent<Animator>();

        _explosionAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMovement();
    }

    void EnemyMovement()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);

        if (transform.position.y < -5.0f)
        {
            float randomX = Random.Range(-9f, 9f);

            transform.position = new Vector3(randomX, _startingHeight, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            _animator.SetTrigger("OnEnemyDestroyed");

            _enemySpeed = 0;

            Destroy(this.gameObject, 2.8f);

            if (_player != null)
            {
                _player.IncreaseScore(10);
            }

            Destroy(other.gameObject);

            _explosionAudioSource.Play();
        }

        else if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }

            _animator.SetTrigger("OnEnemyDestroyed");

            Destroy(this.gameObject, 2.8f);

            _explosionAudioSource.Play();
        }
    }
}
