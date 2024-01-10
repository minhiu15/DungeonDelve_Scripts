using System;
using System.Collections.Generic;
using UnityEngine;


public enum CharacterNameCode
{
    Arlan = 0,
    Lynx = 1
}

[Serializable]
public class CharacterCustom
{
    public PlayerController prefab;
    public CharacterNameCode nameCode;
}

/// <summary> Dữ liệu tất cả nhân vật trong game. </summary>
[CreateAssetMenu(fileName = "Character Data", menuName = "Game Configuration/Character Data")]
public class SO_CharacterData : ScriptableObject
{
    public List<CharacterCustom> CharactersData = new();

    private readonly Dictionary<CharacterNameCode, PlayerController> _playerControllers = new();

    private void OnEnable()
    {
        _playerControllers.Clear();
        foreach (var characterCustom in CharactersData)
        {
            _playerControllers.TryAdd(characterCustom.nameCode, characterCustom.prefab);
        }
    }

    public PlayerController GetPrefab(CharacterNameCode _characterNameCode) => _playerControllers[_characterNameCode];
    
}
