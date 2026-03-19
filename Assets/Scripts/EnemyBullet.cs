using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private Rigidbody _rbd;
    [SerializeField] private float _speed;

    void Awake()
    {
        _rbd = GetComponent<Rigidbody>();
    }

    void Start()
    {
        _rbd.AddForce(transform.forward * _speed, ForceMode.Impulse);
    }

    void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }
}
