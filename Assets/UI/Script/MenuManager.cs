using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    public string goToClass = "goto";
    public string contentName = "Content";
    public string pageContainerName = "PageContainer";
    public string returnName = "Return";
    public string exitName = "Exit";
    Dictionary<string, BasePage> pages = new Dictionary<string, BasePage>();
    UIDocument uIDocument;
    VisualElement root;
    BasePage currentPage;
    public static MenuManager instance;

    void Start() {
        SetStaticInstance();
        SetUIDocument(gameObject.GetComponent<UIDocument>());
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
            Return();
        if (Input.GetKeyDown(KeyCode.Delete))
            Return();
    }

    void SetStaticInstance() {
        if (instance == null) instance = this;
        else Debug.LogWarning("Two instances of MenuManager");
    }

    // A utiliser en conaissance de cause. 
    // Cette fonction change toute l'UI, une seul uIDocument est géré à la fois
    public void SetUIDocument(UIDocument uIDocument) {
        this.uIDocument = uIDocument;
        root = uIDocument.rootVisualElement;

        BaseUiBehavior();

        InitializeHierarchy(root.Q<VisualElement>("PageRoot"));

        NavigateToPage("PageRoot");

    }

    // /!\ Fonction récusive
    // Cette fonction initialise les pages de la hierarachie 
    void InitializeHierarchy (VisualElement pageRoot, int hierarchyDepth = 0) {

        BasePage pageObj = CreatePageBehavior(pageRoot, hierarchyDepth++);
        pages.Add(pageObj.Name, pageObj);

        var childPagesContainer = pageRoot.Q<VisualElement>(pageContainerName);

        if (childPagesContainer != null)
            foreach (VisualElement page in childPagesContainer.Children()) {
                InitializeHierarchy(page, hierarchyDepth);
                page.style.display = DisplayStyle.None;
            }
    }

    BasePage CreatePageBehavior(VisualElement page, int depth) {
        Type pageType = Type.GetType(page.name);
        
        if (pageType == null)
            pageType = typeof(BasePage); 

        BasePage pageObject = (BasePage) Activator.CreateInstance(pageType, uIDocument, page, depth);
        return pageObject;
    }

    /// <summary>
    /// Applies general behavior to elements in UI document.
    /// </summary>
    private void BaseUiBehavior()
    {
        // Button return
        var returnButtons = root.Query<Button>(name: MenuManager.instance.returnName).ToList();
        foreach (Button button in returnButtons) {
            button.clickable.clicked += Return;
        }


        // Button goto base behavior
        var gotoButtons = root.Query<Button>(className: MenuManager.instance.goToClass).ToList();
        foreach (Button button in gotoButtons) {
            button.clickable.clicked += () => {
                
                string pageName = button.name.Substring(4);
                NavigateToPage(pageName);

            };
        }

        // Content folder is hidden
        var contentContainers = root.Query<VisualElement>(name: MenuManager.instance.contentName).ToList();
        foreach (VisualElement container in contentContainers) {
            // Debug.Log("container found");
            container.style.display = DisplayStyle.None;
        }
    }

    public void NavigateToPage(string newPageName) {
        NavigateToPage(pages[newPageName]);
    }

    public void NavigateToPage(BasePage newPage) {
        if (currentPage == null) { currentPage = newPage; }
 
        // Cursors used for navigation in hierarchy
        BasePage currentPageCursor = currentPage;
        BasePage newPageCursor = newPage;

        BasePage commonParent = currentPage.GetCommonParent(newPage);

        currentPage.ClosePage();

        // Close all pages from currentPage to common parent
        while (currentPageCursor != commonParent) {
            currentPageCursor = currentPageCursor.GetParentPage();
            currentPageCursor.ClosePage();
        }

        // Display layout only from newPage to common parent
        while (newPageCursor != commonParent) {
            newPageCursor = newPageCursor.GetParentPage();
            newPageCursor.DisplayLayout();
        }
        
        newPage.OpenPage();

        currentPage = newPage;
    }

    void Return() {
        NavigateToPage(currentPage.GetParentPage());
    }

    public BasePage GetPageFromName(string pageName) {
        return pages[pageName];
    }
}