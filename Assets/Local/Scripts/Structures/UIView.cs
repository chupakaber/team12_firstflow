using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace Scripts
{
    public class UIView: MonoBehaviour
    {
        public TMP_Text GoldCountText;
        public TMP_Text HonorCountText;

        public void SetGold(int count)
        {
            GoldCountText.text = count.ToString();
        }

        public void SetHonor(int count) 
        {
            HonorCountText.text = count.ToString();
        }
    }
}
