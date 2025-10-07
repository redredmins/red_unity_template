using UnityEngine;
using System;
using System.Collections.Generic;

namespace RedMinS
{
    public enum Language : int
    {
        None = 0,
        Korean = 1,
        //English,
        //Japanese,
        //ChineseTW,
        MAX
    }

    [Serializable]
    public class LocalizationTable
    {
        public Language language;
        public TextAsset quest;
        public TextAsset uiString;
    }

    public class DesignDataContainer : MonoBehaviour
    {
        [Header("- items")]
        [SerializeField] TextAsset itemTableText;
        

        // 국가별 언어
        [Header("Localization")]
        [SerializeField] LocalizationTable krTable;
        //[SerializeField] TablesByLanguage enTable;

        public Language curLang { private set; get; }

        // 테이블
        // public ObjectTable<ProductInfo> product { private set; get; }
        // public ObjectTable<ItemInfo> item { private set; get; }

        public StringTable uiString { private set; get; }

        //[ShowOnly] public bool isLoaded = false;


        void Awake()
        {
            curLang = (Language)PlayerPrefs.GetInt("LANGUAGE", 0);
            if (curLang == Language.None)
            {
                InitLanguage();
            }
        }

        // 모든 테이블 불러오기
        public void LoadAllTable()
        {
            //userLevel = new LevelTable(userLevelTableText);
            
            uiString = new StringTable(GetLocalizationTable(curLang).uiString);
            
            //isLoaded = true;
        }

        // 언어 바꾸기
        public void ChangeLanguage(Language lang)
        {
            if (curLang != lang)
            {
                curLang = lang;
                PlayerPrefs.SetInt("LANGUAGE", (int)curLang);

                //string sLang = LanguageLabel(curLang);
                uiString.MakeStringTable(GetLocalizationTable(curLang).uiString);
            }
        }

        // 언어
        void InitLanguage()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Korean:
                    curLang = Language.Korean;
                    break;
                /*
                case SystemLanguage.English:
                    curLang = Language.English;
                    break;
                case SystemLanguage.Japanese:
                    curLang = Language.Japanese;
                    break;
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseTraditional:
                    curLang = Language.ChineseTW;
                    break;
                */

                default:
                    curLang = Language.Korean;
                    break;
            }
        }

        LocalizationTable GetLocalizationTable(Language lang)
        {
            switch (lang)
            {
                case Language.Korean:
                    return krTable;
                //case Language.English:
                //    return enTable;

                default:
                    return krTable;//enTable;
            }
        }

        String LanguageLabel(Language lang)
        {
            switch (lang)
            {
                case Language.Korean:
                    return "ko";//LANGUAGE_KR;
                //case Language.English:
                //    return LANGUAGE_EN;
                //case Language.Japanese:
                //    return LANGUAGE_JP;
                //case Language.ChineseTW:
                //    return LANGUAGE_TW;

                default:
                    return ""; //LANGUAGE_KR;
            }
        }

        // 리소스 폴더에서 테이블 텍스트 가져오기
        TextAsset LoadTableText(string tableType)
        {
            string path = string.Format("Text/{0}table", tableType);
            TextAsset tableText = Resources.Load<TextAsset>(path);

            return tableText;
        }

        TextAsset LoadTableText(string tableType, string language)
        {
            string path = string.Format("Text/{0}{1}table", language, tableType);
            TextAsset tableText = Resources.Load<TextAsset>(path);
            //Debug.Log(tableText.text);

            return tableText;
        }
    }
}