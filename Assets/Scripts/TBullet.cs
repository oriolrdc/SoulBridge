using UnityEngine;

public class TBullet : MonoBehaviour
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

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer != 6)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if(damageable != null)
            {
                damageable.TakeDamage(5);
            }
        }
    }

    void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }
}
