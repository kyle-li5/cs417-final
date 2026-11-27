using System;
using UnityEngine;

namespace Rowland
{
    public class PlayerStats : MonoBehaviour
    {
        public static PlayerStats Instance { get; private set; }

        [Header("Stats")]
        [SerializeField] private double health = 100;
        [SerializeField] private double mana = 0;
        [SerializeField] private int points = 0;
        [SerializeField] private double maxHealth = 100;
        [SerializeField] private double maxMana  = 100;
        [SerializeField] private double manaRegen = 0.5;
        [SerializeField] private double cooldown = 0;
        [SerializeField] private double cooldownTime = 1;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            onSpawn();
        }

        void Update()
        {
            if(mana < maxMana) mana += manaRegen * Time.deltaTime;
            if (cooldown > 0) cooldown -= Time.deltaTime;
        }
        public void onSpawn()
        {
            health = maxHealth;
            mana = maxMana;
            points = 0;
            cooldown = 0;
        }

        void onDeath()
        {

        }

        public bool isReady() {return !(cooldown > 0);}

        public void resetCooldown() {cooldown = cooldownTime;}

        public void setHealth(float _health)
        {
            health = _health;
            if (health > maxHealth) health = maxHealth;
            if (health < 0) onDeath();
        }

        public void changeHealth(float _health)
        {
            health += _health;
            if (health > maxHealth) health = maxHealth;
            if (health < 0) onDeath();
        }

        public void setMana(float _mana)
        {
            mana = _mana;
            if (mana > maxMana) mana = maxMana;
            if (mana < 0) mana = 0;
        }

        public void changeMana(float _mana)
        {
            mana += _mana;
            if (mana > maxMana) mana = maxMana;
            if (mana < 0) mana = 0;
        }

        public void setPoints(int _points) {points = _points;}

        public void changePoints(int _points) {points += _points;}

        public void setRegen(float regen) {manaRegen = regen;}

        public void changeRegen(float regen) {manaRegen += regen;}

        public void setCooldownTime(float _cooldownTime) {cooldownTime = _cooldownTime;}

        public void changeCooldownTime(float _cooldownTime) {cooldownTime += _cooldownTime;}

        public double getHealth() { return health; }

        public double getMana() { return mana; }

        public int getPoints() { return points; }
    }
}
