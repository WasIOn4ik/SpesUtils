using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuBase : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// �������� ���� ��� ������. ������������ �� �������������, �� ���������
    /// </summary>
    [SerializeField] public string title;

    /// <summary>
    /// ���������� ����, � �������� ����� ��������� �� ������ "�����"
    /// </summary>
    [SerializeField] protected MenuBase previousMenu;

    #endregion

    #region StaticVariables

    /// <summary>
    /// ������ ����������� ����, �� ����� ������� ��� ��������
    /// </summary>
    protected static List<MenuBase> loadedMenus = new List<MenuBase>();

    /// <summary>
    /// ��������� �������� ����
    /// </summary>
    protected static MenuBase lastOpenedMenu;

    /// <summary>
    /// ������ �� ScriptableObject ����������, ���������� ��������� �� ������� �������������
    /// </summary>
    protected static MenusLibrary menus;

    #endregion

    #region StaticFunctions

    protected static bool FindInLoadedMenus(string title)
    {
        foreach (var m in loadedMenus)
        {
            if (m)
                if (m.title == title)
                    return true;
        }

        return false;
    }

    protected static MenuBase GetFromLoaded(string title)
    {
        //return loadedMenus.Find(x => { return x.title == title; });
        MenuBase m = null;
        for (int i = 0; i < loadedMenus.Count; i++)
        {
            m = loadedMenus[i];
            if (m == null)
            {
                loadedMenus.RemoveAt(i);
                i--;
            }
            else if (m.title == title)
            {
                return m;
            }
        }
        return null;
    }

    protected static void AddToLoadedMenus(MenuBase menu)
    {
        loadedMenus.Add(menu);
    }

    protected static bool TryRemoveFromLoaded(MenuBase menu)
    {
        if (menu == null)
        {
            SpesLogger.Warning("������� ������� null �� ����������� ����");
            return false;
        }

        if (FindInLoadedMenus(menu.title))
        {
            RemoveFromLoaded(menu);
            return true;
        }

        SpesLogger.Detail("���� " + menu.title + " �� ���� ������� � ����������� � �� ����� ���� �������");
        return false;
    }

    protected static void RemoveFromLoaded(MenuBase menu)
    {
        loadedMenus.Remove(menu);
    }

    /// <summary>
    /// ���� ���� � ����������� ��� ������� �����
    /// </summary>
    /// <param name="title"></param>
    /// <returns></returns>
    public static MenuBase OpenMenu(string title)
    {
        MenuBase m = GetFromLoaded(title);
        if (!m)
        {
            m = menus.GetPrefab(title);

            if (m != null)
                m = Instantiate(m);
        }

        m.gameObject.SetActive(true);

        m.previousMenu = lastOpenedMenu;
        lastOpenedMenu = m;

        return m;
    }

    /// <summary>
    /// ������� ���� �� ��������. true ���� ���� �������
    /// </summary>
    /// <param name="title"></param>
    /// <returns></returns>
    public static bool CloseMenu(string title)
    {
        var m = GetFromLoaded(title);
        if (!m)
            return false;

        m.CloseMenu();
        return true;
    }

    /// <summary>
    /// ������ �� ���������� ������ ���� ��������� �� ������� �������������
    /// </summary>
    /// <param name="lib"></param>
    public static void SetLibrary(MenusLibrary lib)
    {
        menus = lib;
    }

    #endregion

    #region Functions

    /// <summary>
    /// ��������� ������� � ���� � ������ <paramref name="title"/>
    /// </summary>
    /// <param name="title"></param>
    public void GoToMenu(string title)
    {
        gameObject.SetActive(false);

        var m = OpenMenu(title);
    }

    /// <summary>
    /// ��������� ������� � ���� � ������ <paramref name="title"/>
    /// </summary>
    /// <param name="title"></param>
    /// <param name="bCloseCurrent"></param>
    public void GoToMenu(string title, bool bCloseCurrent)
    {
        if (bCloseCurrent)
            gameObject.SetActive(false);

        var m = OpenMenu(title);
    }

    /// <summary>
    /// ������� � ����������� ���������� ���� ��� ��������
    /// </summary>
    public void ToPreviousMenu()
    {
        gameObject.SetActive(false);

        var m = GetFromLoaded(title);
        m.gameObject.SetActive(true);
    }

    /// <summary>
    /// �������� ����
    /// </summary>
    public void HideMenu()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// �������� ���� � ������� �� �����������
    /// </summary>
    public void CloseMenu()
    {
        OnClose();
        Destroy(gameObject);
    }

    public virtual void Reset()
    {

    }

    /// <summary>
    /// ��������� �������� ������ ��� �������������
    /// </summary>
    /// <param name="param"></param>
    public virtual void Initialize(string param = "")
    {

    }

    /// <summary>
    /// ���������� ��� ������������� �������� ����, ��� ���������� ������ ��� ������/������� ���� ������������ OnEnable � OnDisable
    /// </summary>
    protected virtual void OnClose()
    {

    }

    #endregion

    #region UnityCallbacks

    public virtual void Awake()
    {
        if (!FindInLoadedMenus(title))
            AddToLoadedMenus(this);
    }

    public void OnDestroy()
    {
        TryRemoveFromLoaded(this);
        OnClose();
    }

    #endregion
}
