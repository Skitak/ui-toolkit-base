using UnityEngine;
using UnityEngine.UIElements;

public class BasePage
{
    protected UIDocument uIDocument;
    public VisualElement pageRoot;

    protected bool isCloseable = true;
    protected bool isOverlay = false;

    private string name;
    private int depth;
    public string Name { get => pageRoot.name; }
    public int Depth { get => depth; }

    public BasePage(UIDocument uIDocument, VisualElement pageRoot, int depth)
    {
        this.uIDocument = uIDocument;
        this.pageRoot = pageRoot;
        this.depth = depth;

        FetchUiElements();
        LoadSettings();
        SetPageBehavior();
    }

    protected virtual void FetchUiElements() { }
    protected virtual void LoadSettings() { }
    protected virtual void SetPageBehavior() { }

    public virtual void LoadBaseSettings() { }

    public virtual void ApplyChanges() { }
    public virtual void OpenPage()
    {
        DisplayContent();
        DisplayLayout();
    }

    public virtual void ClosePage()
    {
        CloseOverlays();
        HideContent();
        HideLayout();
    }

    public void DisplayContent()
    {
        pageRoot.Q<VisualElement>(MenuManager.instance.contentName).style.display = DisplayStyle.Flex;
    }

    public void DisplayLayout()
    {
        pageRoot.style.display = DisplayStyle.Flex;
    }
    public void HideContent()
    {
        pageRoot.Q<VisualElement>(MenuManager.instance.contentName).style.display = DisplayStyle.None;
    }
    public void HideLayout()
    {
        pageRoot.style.display = DisplayStyle.None;
    }
    private void CloseOverlays()
    {
        // TODO
    }



    public BasePage GetParentPage()
    {
        if (depth != 0)
            return MenuManager.instance.GetPageFromName(pageRoot.parent.parent.name);
        else
            return this;
    }

    /// <summary>
    ///     Returns the common parents of this and otherPage
    ///     If one is parent of the other, return the parent
    /// </summary>
    /// <param name="otherPage"></param>
    /// <returns>BasePage common parent</returns>
    public BasePage GetCommonParent(BasePage otherPage)
    {

        // Get parents from same depth
        int depthDifference = Depth - otherPage.Depth;

        // Used for navigation in hierarchy
        BasePage thisPageCursor = this;
        BasePage otherPageCursor = otherPage;

        // Setting cursors to same depth in hierarchy
        if (depthDifference < 0)
            while (depthDifference++ != 0)
                otherPageCursor = otherPageCursor.GetParentPage();
        else if (depthDifference > 0)
            while (depthDifference-- != 0)
                thisPageCursor = thisPageCursor.GetParentPage();

        // Going up the hierarchy while parent is not found
        while (thisPageCursor != otherPageCursor)
        {
            otherPageCursor = otherPageCursor.GetParentPage();
            thisPageCursor = thisPageCursor.GetParentPage();
        }

        return thisPageCursor;
    }
}
