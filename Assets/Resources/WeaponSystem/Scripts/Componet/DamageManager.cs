using UnityEngine;
using System.Collections;

public class DamageManager : MonoBehaviour
{
    public AudioClip[] HitSound;
    public GameObject Effect;
	public GameObject smokeTrail;
	public GameObject deathEffect;
    public int HP = 100;

    private void Start()
    {

    }

    public void ApplyDamage(int damage,GameObject killer)
    {
		if(HP<0)
		return;
	
        if (HitSound.Length > 0)
        {
            AudioSource.PlayClipAtPoint(HitSound[Random.Range(0, HitSound.Length)], transform.position);
        }
        HP -= damage;
		
		if (smokeTrail != null) {
			smokeTrail.GetComponent<ParticleEmitter>().emit = true;
		}
        if (HP <= 0)
        {
			
			if(this.gameObject.GetComponent<FlightOnDead>()){
				this.gameObject.GetComponent<FlightOnDead>().OnDead(killer);
			}
            Dead();
        }
    }

    private void Dead()
    {
        if (Effect){

            GameObject obj = (GameObject)GameObject.Instantiate(deathEffect, transform.position, transform.rotation);
			if(this.GetComponent<Rigidbody>()){
				if(obj.GetComponent<Rigidbody>()){
					obj.GetComponent<Rigidbody>().velocity = this.GetComponent<Rigidbody>().velocity;
				}
			}
		}
		
		
        Destroy(this.gameObject);
    }

}
