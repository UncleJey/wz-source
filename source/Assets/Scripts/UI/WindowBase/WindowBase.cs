using System;
using UnityEngine;

public abstract class WindowBase : GUIElement<WindowBase>
{
    [Flags]
    public enum WindowSettings
    {
        /// <summary>
        /// Стандартное поведение окон
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Окно можно закрыть только через вызов функции Close. CloseAll его не закрывает
        /// </summary>
        ManualCloseOnly = 1,
        /// <summary>
        /// При открытии окна не будут закрываться другие открытые окна
        /// </summary>
        NoCloseallOnOpen = 2,
        /// <summary>
        /// Не блокировать/ затемнять фон за окном
        /// </summary>
        HideDarkBack = 4,
        /// <summary>
        /// Использовать прозрачную подложку
        /// </summary>
        UseTransparentBack = 8,
        /// <summary>
        /// Объединяет все имеющиеся флаги: ручное закрытие, не закрывает другие окна, нет подложки
        /// </summary>
        Popup = ManualCloseOnly | NoCloseallOnOpen | HideDarkBack,
        /// <summary>
        /// Означает, что окно второстепенное. Оно всегда игнорируется при проверке открытых окон
        /// </summary>
        Secondary = 16,
        /// <summary>
        /// Нужно прятать главный интерфейс
        /// </summary>
        HideMainUI = 32,
        /// Принудительно закрывает все открытые окна, даже если у них флаг ManualCloseOnly. 
        /// Костыль для окна лвлапа пока не разберутся со всей системой очередности окон. 
        /// </summary>
        ForcedCloseAllWindow = 64,
        /// <summary>
        /// Использует подложку с затемнением по краям
        /// </summary>
        UseVignetteBack = 128,
    }

    protected Animator animator;

    protected const string HIDE_TRIGGER = "Hide";
    protected const string SHOW_TRIGGER = "Show";
	protected const float AnimationTime = 0.01f;

    [SerializeField, EnumFlags]
    private WindowSettings settings;
    public WindowSettings Settings
    {
        get{ return settings;}
        set { settings = value; }
    }

    public static event Func<WindowBase, bool> CanOpenGlobal;
    public event Func<bool> CanOpen;

    [NonSerialized, HideInInspector]
    public bool isClosing;
	[NonSerialized, HideInInspector]
	public bool isLoading; //false - когда все загружено
    public bool IsOpened
    {
        get
        {
            return Opened.Contains(this);
        }
    }

    protected sealed override void Initialize()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Вызывается один раз при первом запуске игры. Пустая, так что base можно не вызывать
    /// </summary>
    protected virtual void Awake() {}       

#region Close

	/// <summary>
	/// Скрыть подложку и окно с анимацией 
	/// </summary>
	public void Close( bool _forced = false) {
	    if (!IsOpened)
	    {
            if (gameObject.activeSelf) gameObject.SetActive(false);
	        return;
	    }

	    if (!_forced && !TryClose())
	        return;

        isClosing = true;
	    if (animator != null && animator.isInitialized)
	        animator.CrossFade(HIDE_TRIGGER, 0);
	    else
	        AnimatorFinish();
	    MarkClosed();

	    //if (!Settings.IsHideDarkBack())
	    //    WindowBackground.Show(false, this);

        foreach (var wnd in Opened)
        {
            if (wnd != this && wnd.Settings.IsVignetteBack())
                break; //goto next;
        }
        //GUIVignette.Hide();

//    next:

        //if (Settings.IsHideMainUI() && UIEventManager.needShowMainUI != null) UIEventManager.needShowMainUI(true);

        OnClose();
//#pragma warning disable 618
	}

	/// <summary>
	/// Скрыть только окно
	/// </summary>
	public void CloseAll()
	{
	    var set = settings;
        settings |= WindowSettings.HideDarkBack;
	    Close();
        settings = set;
	}

    /// <summary>
    /// Вызывается в самом начале функции Close. Если вернуть false, то окно не закроется
    /// </summary>
    /// <returns></returns>
    protected virtual bool TryClose()
    {
        return true;
    }

    /// <summary>
    /// Вызывается в конце функции Close, когда анимация закрытия уже запущена
    /// </summary>
    public virtual void OnClose() { }


	/// <summary>
	/// Вызывается аниматором по окончании анимации закрытия
	/// </summary>
	public void AnimatorFinish()
	{
	    if (!isClosing)
	        return;
		gameObject.SetActive(false);
		isClosing = false;
	}

    /// <summary>
    /// Вызывается когда окно полностью скрывается (обычно после окончания анимации)
    /// </summary>
    protected virtual void OnDisable() {}

#endregion Close

#region Open

    /// <summary>
    /// Открывает окно
    /// </summary>
    /// <returns>Истина если окно было открыто</returns>
	public bool Open()
	{
        if (IsOpened || !TryOpen())
	        return false;

	    bool doIt = CanOpen.Execute(Extensions.FuncMode.All, true) && CanOpenGlobal.Execute(this, Extensions.FuncMode.All, true);
	    if (!doIt) return false;

//            if (!Settings.IsNoCloseallOnOpen())
//                GUIManager.Instance.CloseAllWindows(Settings.ForcedCloseAllWindows());

        isClosing = false;
	    gameObject.SetActive(true);

        if (animator != null && animator.isInitialized)
            animator.CrossFade(SHOW_TRIGGER, 0);
/*
        if (!Settings.IsHideDarkBack())
        {
            if (Settings.IsVignetteBack())
                GUIVignette.Show();

            if (Settings.IsTransparentBack())
                WindowBackground.ShowTransparent(this);
            else
            if (!Settings.IsVignetteBack())
                WindowBackground.Show(true, this);
        }

        if (Settings.IsHideMainUI() && UIEventManager.needShowMainUI != null) UIEventManager.needShowMainUI(false);
*/
        OnOpen();

	    MarkOpened();
	    return true;
	}

    /// <summary>
    /// Вызывается в самом начале функции Open. Если вернуть false, то окно не отроектся
    /// </summary>
    /// <returns></returns>
    protected virtual bool TryOpen()
    {
        return true;
    }

   /// <summary>
   /// Вызывается после успешного открытия окна, прямо перед событиями открытия окна
   /// </summary>
    public virtual void OnOpen() { }

#endregion Open  
  
}

public static class _WindowssHelper
{
    public static bool IsManualCloseOnly(this WindowBase.WindowSettings _settings)
    {
        return (_settings & WindowBase.WindowSettings.ManualCloseOnly) > 0;
    }

    public static bool IsNoCloseallOnOpen( this WindowBase.WindowSettings _settings )
    {
        return (_settings & WindowBase.WindowSettings.NoCloseallOnOpen) > 0;
    }

    public static bool IsHideDarkBack( this WindowBase.WindowSettings _settings )
    {
        return ( _settings & WindowBase.WindowSettings.HideDarkBack ) > 0;
    }

    public static bool IsTransparentBack(this WindowBase.WindowSettings _settings)
    {
        return (_settings & WindowBase.WindowSettings.UseTransparentBack) > 0;
    }

    public static bool IsHideMainUI(this WindowBase.WindowSettings _settings)
    {
        return (_settings & WindowBase.WindowSettings.HideMainUI) > 0;
    }

    public static bool ForcedCloseAllWindows(this WindowBase.WindowSettings _settings)
    {
        return (_settings & WindowBase.WindowSettings.ForcedCloseAllWindow) > 0;
    }

    public static bool IsVignetteBack(this WindowBase.WindowSettings _settings)
    {
        return (_settings & WindowBase.WindowSettings.UseVignetteBack) > 0;
    }
}
