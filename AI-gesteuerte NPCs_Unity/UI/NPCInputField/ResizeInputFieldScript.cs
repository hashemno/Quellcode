using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NPCInputField
{
    // Variables to store the width and height of the input field
    public class ResizeInputFieldScript : MonoBehaviour
    {
        private int sizeWidth;
        private int sizeHeight;
        // Start is called before the first frame update
        void Start()
        {
            sizeWidth = Screen.width / 4;
            sizeHeight = sizeWidth / 6;
            this.transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeWidth);
            this.transform.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeHeight);
        }

    }

}