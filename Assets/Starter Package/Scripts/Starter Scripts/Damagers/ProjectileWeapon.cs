using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Damager
{
	public GameObject Projectile;
	public Transform ProjectileSpawn;
	public bool WeaponOnLeft = false;
	public bool ShootTowardsMouse = false;
	public float Force = 100f;
	public float Duration = 10f;

	private Vector2 mousePosition;
	private float angle;

	private void FixedUpdate()
	{
		if (ShootTowardsMouse)
			mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

	public override void WeaponStart(Transform wielderPosition, Vector2 lastLookDirection)
	{
		base.WeaponStart(wielderPosition, lastLookDirection);

		GameObject bullet = Instantiate(Projectile, ProjectileSpawn.position, Quaternion.identity);
		bullet.GetComponent<Projectile>().SetValues(Duration, alignmnent, damageValue, this.TopDown);
		Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>(); ;

		if (!TopDown)
		{
			Vector2 direction = wielderPosition.right;
			if (WeaponOnLeft)
				direction *= -1;
			rb.AddForce(direction * Force * wielderPosition.localScale.x, ForceMode2D.Impulse);
		}
		else
		{
			if (ShootTowardsMouse)
				rb.AddForce((new Vector3(mousePosition.x, mousePosition.y, 0f) - transform.position) * Force, ForceMode2D.Impulse);
			else
				rb.AddForce(lastLookDirection * Force, ForceMode2D.Impulse);
		}
	}
}
