using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum EffectType
{
    EmptyElement,
    Hellfire,
    Soulfire,
    Thunder,
    Silver,
    Blood,
    Holy,
    Dark,
    Inferno,
    Explosion,
    HolyFlame,
    BloodFlame,
    BlackFlame,
    Plasma,
    SpiritStorm,
    QuickSilver,
    RedLightning,
    PureSilver,
    Sacrifice,
    Unholy,
    Curse,
}
public class Effect
{
    public EffectType effectType;
    public int stackCount; 
    public float damagePerTurn;

    public Effect(EffectType effect, int stackCount, float damagePerTurn)
    {
        this.effectType = effect;
        this.stackCount += stackCount;
        this.damagePerTurn = damagePerTurn;
    }

    // Call this method to apply the effect's damage and decrement its duration
    // Apply the effect's damage and decrement its stackCount (duration)

    // Check if the effect is still active

    public void ApplyDamageEffect(Enemy enemy)
    {
        enemy.EnemyTakeDamage(damagePerTurn);
        stackCount--;
    }
    public void ApplyBloodDamageEffect(Enemy enemy)
    {
        enemy.EnemyTakeDamage(stackCount);
        stackCount--;
    }
    public bool IsActive()
    {
        return stackCount > 0;
    }
}