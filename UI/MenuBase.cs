using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuBase : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// Название меню при поиске. Уникальность не гарантируется, но требуется
    /// </summary>
    [SerializeField] public string title;

    /// <summary>
    /// Предыдущее меню, к которому можно вернуться по кнопке "Назад"
    /// </summary>
    [SerializeField] protected MenuBase previousMenu;

    #endregion

    #region StaticVariables

    /// <summary>
    /// Список загруженных меню, их можно открыть без создания
    /// </summary>
    protected static List<MenuBase> loadedMenus = new List<MenuBase>();

    /// <summary>
    /// Последнее открытое меню
    /// </summary>
    protected static MenuBase lastOpenedMenu;

    /// <summary>
    /// Ссылка на ScriptableObject библиотеки, необходимо назначить до первого использования
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
            SpesLogger.Warning("Попытка удалить null из загруженных меню");
            return false;
        }

        if (FindInLoadedMenus(menu.title))
        {
            RemoveFromLoaded(menu);
            return true;
        }

        SpesLogger.Detail("Меню " + menu.title + " не было найдено в загруженных и не может быть удалено");
        return false;
    }

    protected static void RemoveFromLoaded(MenuBase menu)
    {
        loadedMenus.Remove(menu);
    }

    /// <summary>
    /// Ищет меню в загруженных или создает новое
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
    /// Закрыть меню по названию. true если было закрыто
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
    /// Ссылка на библиотеку должна быть назначена до первого использования
    /// </summary>
    /// <param name="lib"></param>
    public static void SetLibrary(MenusLibrary lib)
    {
        menus = lib;
    }

    #endregion

    #region Functions

    /// <summary>
    /// Выполняет переход к окну с именем <paramref name="title"/>
    /// </summary>
    /// <param name="title"></param>
    public void GoToMenu(string title)
    {
        gameObject.SetActive(false);

        var m = OpenMenu(title);
    }

    /// <summary>
    /// Выполняет переход к окну с именем <paramref name="title"/>
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
    /// Возврат к предыдущему созданному меню без проверок
    /// </summary>
    public void ToPreviousMenu()
    {
        gameObject.SetActive(false);

        var m = GetFromLoaded(title);
        m.gameObject.SetActive(true);
    }

    /// <summary>
    /// Скрывает меню
    /// </summary>
    public void HideMenu()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Скрывает меню и удаляет из загруженных
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
    /// Позволяет передать данные для инициализации
    /// </summary>
    /// <param name="param"></param>
    public virtual void Initialize(string param = "")
    {

    }

    /// <summary>
    /// Вызывается при окончательном закрытии окна, для добавления логики при показе/скрытии окна использовать OnEnable и OnDisable
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
