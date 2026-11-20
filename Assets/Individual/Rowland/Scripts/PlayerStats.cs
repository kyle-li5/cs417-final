using System;
using UnityEngine;

namespace Rowland
{
    public class PlayerStats : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private double health = 100;
        [SerializeField] private double mana = 0;
        [SerializeField] private int points = 0;
        [SerializeField] private double maxHealth = 100;
        [SerializeField] private double maxMana  = 100;
        [SerializeField] private double manaRegen = 0.5;
        [SerializeField] private double cooldown = 0;
        [SerializeField] private double cooldownTime = 1;

        private void Start()
        {
            onSpawn();
        }

        void Update()
        {
            mana += manaRegen * Time.deltaTime;
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

        public bool isReady()
        {
            return !(cooldown > 0);
        }

        public void resetCooldown()
        {
            cooldown = cooldownTime;
        }

        public void setHealth(double _health)
        {
            health = _health;
            if (health > maxHealth) health = maxHealth;
            if (health < 0) onDeath();
        }

        public void changeHealth(double _health)
        {
            health += _health;
            if (health > maxHealth) health = maxHealth;
            if (health < 0) onDeath();
        }

        public void setMana(double _mana)
        {
            mana = _mana;
            if (mana > maxMana) mana = maxMana;
            if (mana < 0) mana = 0;
        }

        public void changeMana(double _mana)
        {
            mana += _mana;
            if (mana > maxMana) mana = maxMana;
            if (mana < 0) mana = 0;
        }

        public void setPoints(int _points)
        {
            points = _points;
            if (points < 0) mana = 0;
        }

        public void changePoints(int _points)
        {
            points += _points;
            if (points < 0) mana = 0;
        }

        public void setRegen(double regen) {manaRegen = regen;}

        public void changeRegen(double regen) {manaRegen += regen;}

        public void setCooldownTime(double _cooldownTime) {cooldownTime = _cooldownTime;}

        public void changeCooldownTime(double _cooldownTime) {cooldownTime += _cooldownTime;}

        public double getHealth() { return health; }

        public double getMana() { return mana; }

        public int getPoints() { return points; }
    }
}
