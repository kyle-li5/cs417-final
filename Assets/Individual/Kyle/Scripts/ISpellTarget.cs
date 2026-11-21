using UnityEngine;

public enum ElementType { Fire, Ice, Lightning, Earth }

public interface ISpellTarget
{
    void TakeSpellDamage(float amount, ElementType element, Vector3 hitPoint);
}