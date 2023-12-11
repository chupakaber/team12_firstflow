using TMPro;
using UnityEngine;

namespace Scripts
{
    public class LocalizationLabelView: MonoBehaviour
    {
        public TMP_Text Label;
        public string[] Values;

        public void SwitchLanguage(int langID)
        {
            if (Values.Length < 1)
            {
                return;
            }

            if (langID < 0 || langID >= Values.Length)
            {
                langID = 0;
            }

            Label.text = Values[langID];
        }
    }
}