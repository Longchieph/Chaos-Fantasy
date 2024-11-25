using UnityEngine;

// Base weapon script
public class Weapon : Item
{
    public WeaponData weaponData;
    private float cooldown;
    private float currentCooldownDuration;

    protected PlayerMovement pm;

    public virtual void Initialize(WeaponData data)
    {
        base.Initialize(data);

        currentCooldownDuration = data.cooldownDuration;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();
        Initialize(weaponData);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0f)
        {
            Attack();
        }
    }

    protected virtual void Attack()
    {
        cooldown = currentCooldownDuration;
    }
}