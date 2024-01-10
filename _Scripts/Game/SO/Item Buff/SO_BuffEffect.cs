using UnityEngine;

public abstract class SO_BuffEffect : ScriptableObject
{
    [Tooltip("Namecode của item"), SerializeField] private 
     ItemNameCode NameCode;

    [Tooltip("Giá trị Buff cộng vào"), SerializeField]
    private float value;

    public float Value => value;
    public virtual void Apply(PlayerController _player) { }
}
