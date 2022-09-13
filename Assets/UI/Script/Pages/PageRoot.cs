using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PageRoot : BasePage {


    public PageRoot(UIDocument uIDocument, VisualElement pageRoot, int depth) : base(uIDocument, pageRoot, depth) {
        isCloseable = false;
    }


}