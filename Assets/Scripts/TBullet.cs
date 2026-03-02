using UnityEngine;

public class TBullet : MonoBehaviour
{
    private Rigidbody _rbd;
    [SerializeField] private float _speed;

    void Awake()
    {
        _rbd = GetComponent<Rigidbody>();
    }

    void Update()
    {
        _rbd.AddForce(transform.forward * _speed * Time.deltaTime, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer != 6)
        {
            Destroy(gameObject);
            //gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
            //gameObject.SetActive(false);
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if(damageable != null)
            {
                damageable.TakeDamage(5);
            }
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
        //gameObject.SetActive(false);
    }
}
