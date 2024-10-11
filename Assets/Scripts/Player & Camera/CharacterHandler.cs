using System.Collections.Generic;
using UnityEngine;

public class CharacterHandler : MonoBehaviour
{
    public CharacterScriptableObject characterData;

    // Current character stats
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentRecovery;
    [HideInInspector]
    public float currentMight;
    [HideInInspector]
    public float currentProjectileSpeed;
    [HideInInspector]
    public float currentMagnet;

    // Character's weapons
    public List<GameObject> heldWeapons;

    // Experience variables
    public int exp = 0;
    public int level = 1;
    public int expCap;

    // SubClass LevelRange will determine how much will the exp required to level up increase in certain level ranges
    // Ex: Level 1 -> 4: each time you level up the exp required is increased by 2
    //     Level 5 -> 10: each time you level up the exp required is increased by 4
    // Use System.Serializable to expose the class's variables in the inspector
    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int expCapIncrease;
    }

    // This determines how many seconds the character has before taking damage again
    [Header("I-Frames")]
    public float invincibilityDuration;
    private float invincibilityTimer;
    private bool isInvincible;

    // A list of LevelRange to set level ranges, exp increase in each level range
    [Header("Level Ranges")]
    public List<LevelRange> levelRanges;

    void Awake()
    {
        // Load character
        //characterData = CharacterSelector.LoadData();
        //CharacterSelector.instance.DestroySingleton();

        // Assign current stats to the starting stats
        currentHealth = characterData.MaxHealth;
        currentRecovery = characterData.Recovery;
        currentMoveSpeed = characterData.MoveSpeed;
        currentMight = characterData.Might;
        currentProjectileSpeed = characterData.ProjectileSpeed;
        currentMagnet = characterData.Magnet;

        // Set the starting weapon
        AcquireWeapon(characterData.StartingWeapon);

    }
    // Start is called before the first frame update
    void Start()
    {
        // Initialize expCap as the first expCapIncrease so the character can level up immediately
        expCap = levelRanges[0].expCapIncrease;
    }

    void Update()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        // If I-Frames reaches 0, then the character is no longer invincible
        else if (isInvincible)
        {
            isInvincible = false;
        }

        // Recover health over time
        Recover();
    }

    // Increase exp on pick up exp gems/defeated a boss
    public void IncreaseExp(int amount)
    {
        exp += amount;
        LevelUpChecker();
    }

    void LevelUpChecker()
    {
        if (exp > expCap)
        {
            level += 1;
            exp -= expCap;

            foreach (LevelRange range in levelRanges)
            {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    expCap += range.expCapIncrease;
                    break;
                }
            }
        }
    }

    public void TakeDamage(float dmg)
    {
        // If the character still has I-Frames then take no damage
        if (isInvincible)
        {
            return;
        }

        currentHealth -= dmg;

        // If character takes damage, set the I-Frames timer
        invincibilityTimer = invincibilityDuration;
        isInvincible = true;

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        Debug.Log("Player is Dead");
    }
    
    // Character health regeneration
    void Recover()
    {
        if (currentHealth < characterData.MaxHealth)
        {
            currentHealth += currentRecovery * Time.deltaTime;

            // To make sure currentHealth never exceed maxHealth
            if (currentHealth > characterData.MaxHealth)
            {
                currentHealth = characterData.MaxHealth;
            }
        }
    }

    public void AcquireWeapon(GameObject wp)
    {
        // Instantiate the weapon and then set the weapon to be the child of the character
        GameObject spawnedWeapon = Instantiate(wp, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(this.transform);
        // Put the weapon into the list of weapons that the character have
        heldWeapons.Add(spawnedWeapon);
    }
}
