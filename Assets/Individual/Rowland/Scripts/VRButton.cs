using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rowland
{

    public class VRButton : MonoBehaviour
    {
        public enum type
        {
            None,
            Health,
            Mana,
            Regen,
            Cool,
            Point,
            Spell,
        }
        public type buttonType = type.None;
        [SerializeField] private float value = 0.0f;
        [SerializeField] private string spellName = "";
        //Time that the button is set inactive after release
        [SerializeField] private float deadTime = 1.0f;
        [SerializeField] private int cost = 0;
        [SerializeField] private float cost_scale = 1.0f;
        private int presses = 0;
        [SerializeField] private int max_presses = -1; // -1 = infinite

        //Bool used to lock down button during its set dead time
        private bool _deadTimeActive = false;

        //public Unity Events we can use in the editor and tie other functions to.
        [SerializeField] private UnityEvent onPressed, onReleased;

        //Checks if the current collider entering is the Button and sets off OnPressed event.
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Button" && !_deadTimeActive && ( max_presses == -1 || presses < max_presses) && PlayerStats.Instance.getPoints() >= cost)
            {
                PlayerStats.Instance.changePoints(-1*cost);
                cost = (int)(cost * cost_scale);
                presses++;
                helper();
                onPressed?.Invoke();
                //Debug.Log("I have been pressed");
            }
        }

        //Checks if the current collider exiting is the Button and sets off OnReleased event.
        //It will also call a Coroutine to make the button inactive for however long deadTime is set to.
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Button" && !_deadTimeActive)
            {
                onReleased?.Invoke();
                //Debug.Log("I have been released");
                StartCoroutine(WaitForDeadTime());
            }
        }

        //private bool CanPress(Collider other) 
        //{
        //    return (other.tag == "Button" && !_deadTimeActive && (max_presses == -1 || presses < max_presses) && PlayerStats.Instance.getPoints() > cost)
        //}

        private void helper ()
        {
            switch (buttonType)
            {
                case type.Health:
                    PlayerStats.Instance.changeHealth(value);
                    break;
                case type.Mana:
                    PlayerStats.Instance.changeMana(value);
                    break;
                case type.Regen:
                    PlayerStats.Instance.changeRegen(value);
                    break;
                case type.Cool:
                    PlayerStats.Instance.changeCooldownTime(value);
                    break;
                case type.Point:
                    PlayerStats.Instance.changePoints((int)value);
                    break;
                case type.Spell:
                    SpellDictionary.Instance.setUnlock(spellName, true);
                    break;
            }
        }

        //Locks button activity until deadTime has passed and reactivates button activity.
        IEnumerator WaitForDeadTime()
        {
            _deadTimeActive = true;
            yield return new WaitForSeconds(deadTime);
            _deadTimeActive = false;
        }
    }
}