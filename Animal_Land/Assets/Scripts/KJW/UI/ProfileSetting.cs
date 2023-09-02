using Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileSetting : MonoBehaviour
{

    public Action OnChanageProfileAction = null; // 프로필 변경을 위한 델리게이트

    [SerializeField] TextMeshProUGUI nickNameText;
    [SerializeField] Image characterImage;
    [SerializeField] List<Sprite> characterSpriteList;
    [SerializeField] List<Image> Parts;

    private IDictionary<string, Sprite> _characterSprites = new Dictionary<string, Sprite>();
    private CharacterType _characterType = CharacterType.None;

    void Start()
    {
        InitProfile();
        OnChanageProfileAction += SetProfile;
        SetProfile();
    }

    void InitProfile()
    {
        nickNameText.text = DatabaseManager.Instance.USER_NAME; // 데이터 베이스의 닉네임 설정
        _characterSprites = characterSpriteList.ToDictionary(sprite => GetCharacterTypeFromSprite(sprite)); // 캐릭터 스프라이트 List to Dictionary
    }

    string GetCharacterTypeFromSprite(Sprite sprite)
    {
        string spriteName = sprite.name;

        if (spriteName.Contains("bird"))
        {
            return CharacterType.Bird.ToString();
        }
        else if (spriteName.Contains("dog"))
        {
            return CharacterType.Dog.ToString();
        }
        else if (spriteName.Contains("cat"))
        {
            return CharacterType.Cat.ToString();
        }
        else if (spriteName.Contains("frog"))
        {
            return CharacterType.Frog.ToString();
        }

        return CharacterType.None.ToString();
    }

    void SetProfileCharacterCustom()
    {
        CharacterCustom characterCustom = new CharacterCustom(DataManager.Instance.CharacterCustomData[DataManager.Instance.PlayerData.Character]); // 대표 캐릭터 커스텀 복사 생성자

        foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
        {
            Sprite tempSprite = Resources.Load<Sprite>($"Sprites/Items/{itemType}/{characterCustom.ItemDict[itemType.ToString()]}");
            if (tempSprite != null)
            {
                Parts[(int)itemType].sprite = tempSprite;
                Parts[(int)itemType].color = Color.white;
            }
            else
            {
                Parts[(int)itemType].sprite = null;
                Parts[(int)itemType].color = Color.clear;
            }
        }
    }

    void SetProfile()
    {
        characterImage.sprite = _characterSprites[DataManager.Instance.PlayerData.Character]; // 대표 캐릭터로 설정
        SetProfileCharacterCustom();
    }
}
