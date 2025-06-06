using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Blazor.Components;

public partial class SmartCarousel<TData> : MudBaseBindableItemsControl<MudCarouselItem, TData>, IAsyncDisposable
{
    private bool _autoCycle = true;

    private Color _currentColor = Color.Inherit;

    private TimeSpan _cycleTimeout = TimeSpan.FromSeconds(5);

    private Timer _timer;

    private int userSelectedIndex = -1;

    /// <summary>
    ///     Sets the position of the arrows. By default, the position is the Center position
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Carousel.Appearance)]
    public Position ArrowsPosition { get; set; } = Position.Center;

    /// <summary>
    ///     Gets or Sets automatic cycle on item collection.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Carousel.Behavior)]
    public bool AutoCycle
    {
        get => _autoCycle;
        set
        {
            _autoCycle = value;

            if (_autoCycle)
            {
                InvokeAsync(async () => await ResetTimerAsync().ConfigureAwait(false));
            }
            else
            {
                InvokeAsync(async () => await StopTimerAsync().ConfigureAwait(false));
            }
        }
    }

    /// <summary>
    ///     Gets or Sets the Auto Cycle time
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Carousel.Behavior)]
    public TimeSpan AutoCycleTime
    {
        get => _cycleTimeout;
        set
        {
            _cycleTimeout = value;

            if (_autoCycle)
            {
                InvokeAsync(async () => await ResetTimerAsync().ConfigureAwait(false));
            }
            else
            {
                InvokeAsync(async () => await StopTimerAsync().ConfigureAwait(false));
            }
        }
    }

    /// <summary>
    ///     Gets or Sets custom class(es) for Bullets buttons
    /// </summary>
    [Category(CategoryTypes.Carousel.Appearance)]
    [Parameter]
    public string BulletsClass { get; set; }

    /// <summary>
    ///     Gets or Sets the Bullets color.
    ///     If not set, the color is determined based on the <see cref="MudCarouselItem.Color" /> property of the active child.
    /// </summary>
    [Category(CategoryTypes.Carousel.Appearance)]
    [Parameter]
    public Color? BulletsColor { get; set; }

    /// <summary>
    ///     Sets the position of the bullets. By default, the position is the Bottom position
    /// </summary>
    [Category(CategoryTypes.Carousel.Appearance)]
    [Parameter]
    public Position BulletsPosition { get; set; } = Position.Bottom;

    /// <summary>
    ///     Gets or Sets the Template for Bullets
    /// </summary>
    [Category(CategoryTypes.Carousel.Appearance)]
    [Parameter]
    public RenderFragment<BulletTemplateArg> BulletTemplate { get; set; }

    /// <summary>
    ///     Custom selected bullet icon.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Carousel.Appearance)]
    public string CheckedIcon { get; set; } = MudBlazor.Icons.Material.Filled.RadioButtonChecked;

    /// <summary>
    ///     Gets or Sets custom class(es) for Delimiters buttons.
    ///     Deprecated, use BulletsClass instead.
    /// </summary>
    [Category(CategoryTypes.Carousel.Appearance)]
    [Obsolete($"Use {nameof(BulletsClass)} instead", false)]
    [ExcludeFromCodeCoverage]
    [Parameter]
    public string DelimitersClass
    {
        get => BulletsClass;
        set => BulletsClass = value;
    }

    /// <summary>
    ///     Gets or Sets the Delimiters color.
    ///     If not set, the color is determined based on the <see cref="MudCarouselItem.Color" /> property of the active child.
    ///     Deprecated, use BulletsColor instead.
    /// </summary>
    [Obsolete($"Use {nameof(BulletsColor)} instead", false)]
    [Category(CategoryTypes.Carousel.Appearance)]
    [ExcludeFromCodeCoverage]
    [Parameter]
    public Color? DelimitersColor
    {
        get => BulletsColor;
        set => BulletsColor = value;
    }

    /// <summary>
    ///     Gets or Sets the Template for Delimiters.
    ///     Deprecated, use BulletsTemplate instead.
    /// </summary>
    [Category(CategoryTypes.Carousel.Appearance)]
    [Obsolete($"Use {nameof(BulletTemplate)} instead", false)]
    [ExcludeFromCodeCoverage]
    [Parameter]
    public RenderFragment<BulletTemplateArg> DelimiterTemplate
    {
        get => BulletTemplate;
        set => BulletTemplate = value;
    }

    /// <summary>
    ///     Gets or Sets if swipe gestures are allowed for touch devices.
    /// </summary>
    [Category(CategoryTypes.Carousel.Behavior)]
    [Parameter]
    public bool EnableSwipeGesture { get; set; } = true;

    [Parameter] public List<string> ItemLabels { get; set; }

    /// <summary>
    ///     Gets or Sets custom class(es) for 'Next' and 'Previous' arrows
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Carousel.Appearance)]
    public string NavigationButtonsClass { get; set; }

    /// <summary>
    ///     Gets or Sets the Template for the Left Arrow
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Carousel.Appearance)]
    public RenderFragment NextButtonTemplate { get; set; }

    /// <summary>
    ///     Custom next navigation icon.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Carousel.Appearance)]
    public string NextIcon { get; set; } = MudBlazor.Icons.Material.Filled.NavigateNext;

    /// <summary>
    ///     Gets or Sets the Template for the Right Arrow
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Carousel.Appearance)]
    public RenderFragment PreviousButtonTemplate { get; set; }

    /// <summary>
    ///     Custom previous navigation icon.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Carousel.Appearance)]
    public string PreviousIcon { get; set; } = MudBlazor.Icons.Material.Filled.NavigateBefore;

    [CascadingParameter(Name = "RightToLeft")]
    public bool RightToLeft { get; set; }

    /// <summary>
    ///     Gets or Sets if 'Next' and 'Previous' arrows must be visible
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Carousel.Behavior)]
    public bool ShowArrows { get; set; } = true;

    /// <summary>
    ///     Gets or Sets if bar with Bullets must be visible
    /// </summary>
    [Category(CategoryTypes.Carousel.Behavior)]
    [Parameter]
    public bool ShowBullets { get; set; } = true;

    /// <summary>
    ///     Gets or Sets if bottom bar with Delimiters must be visible.
    ///     Deprecated, use ShowBullets instead.
    /// </summary>
    [Category(CategoryTypes.Carousel.Behavior)]
    [Obsolete($"Use {nameof(ShowBullets)} instead", false)]
    [ExcludeFromCodeCoverage]
    [Parameter]
    public bool ShowDelimiters
    {
        get => ShowBullets;
        set => ShowBullets = value;
    }

    /// <summary>
    ///     Custom unselected bullet icon.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Carousel.Appearance)]
    public string UncheckedIcon { get; set; } = MudBlazor.Icons.Material.Filled.RadioButtonUnchecked;

    protected string BulletsButtonsClassName =>
        new CssBuilder()
            .AddClass(BulletsClass)
            .Build();

    protected string Classname =>
        new CssBuilder("mud-carousel")
            .AddClass($"mud-carousel-{(BulletsColor ?? _currentColor).ToDescriptionString()}")
            .AddClass(Class)
            .Build();

    protected string NavigationButtonsClassName =>
        new CssBuilder()
            .AddClass($"align-self-{ConvertPosition(ArrowsPosition).ToDescriptionString()}",
                !(NavigationButtonsClass ?? "").Contains("align-self-"))
            .AddClass("mud-carousel-elements-rtl", RightToLeft)
            .AddClass(NavigationButtonsClass)
            .Build();

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true).ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    //When an item is added, it automatically checks the color
    public override void AddItem(MudCarouselItem item)
    {
        Items.Add(item);
        if (Items.Count - 1 == SelectedIndex)
        {
            _currentColor = item.Color;
            StateHasChanged();
        }
    }

    public void MoveToEx(int index)
    {
        if (index == userSelectedIndex)
        {
            NextEx();
        }
        else
        {
            userSelectedIndex = index;
            AutoCycle = false;
        }

        MoveTo(index);
    }

    //
    // Summary:
    //     Move to Next Item
    public void NextEx()
    {
        userSelectedIndex = -1;
        AutoCycle = true;
        Next();
    }

    public void PreviousEx()
    {
        userSelectedIndex = -1;
        AutoCycle = true;
        Previous();
    }

    protected virtual async Task DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            await StopTimerAsync().ConfigureAwait(false);

            var timer = _timer;
            if (timer != null)
            {
                _timer = null;
                await timer.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender).ConfigureAwait(false);

        if (firstRender)
        {
            _timer = new Timer(TimerElapsed, null, AutoCycle ? AutoCycleTime : Timeout.InfiniteTimeSpan, AutoCycleTime);
        }
    }

    /// <summary>
    ///     Called when selected Index changed on base class
    /// </summary>
    protected override void SelectionChanged()
    {
        InvokeAsync(async () => await ResetTimerAsync().ConfigureAwait(false));

        _currentColor = SelectedContainer?.Color ?? Color.Inherit;
    }

    private static Position ConvertPosition(Position position)
    {
        return position switch
        {
            Position.Top => Position.Start,
            Position.Start => Position.Start,
            Position.Bottom => Position.End,
            Position.End => Position.End,
            _ => position
        };
    }

    /// <summary>
    ///     Provides Selection changes by horizontal swipe gesture
    /// </summary>
    private void OnSwipe(SwipeDirection direction)
    {
        if (!EnableSwipeGesture)
        {
            return;
        }

        switch (direction)
        {
            case SwipeDirection.LeftToRight:
                Previous();
                break;

            case SwipeDirection.RightToLeft:
                Next();
                break;
        }
    }

    /// <summary>
    ///     Stops and restart the AutoCycle timer
    /// </summary>
    private async Task ResetTimerAsync()
    {
        await StopTimerAsync().ConfigureAwait(false);
        await StartTimerAsync().ConfigureAwait(false);
    }

    /// <summary>
    ///     Immediately starts the AutoCycle timer
    /// </summary>
    private Task StartTimerAsync()
    {
        if (AutoCycle)
        {
            _timer?.Change(AutoCycleTime, TimeSpan.Zero);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Immediately stops the AutoCycle timer
    /// </summary>
    private Task StopTimerAsync()
    {
        _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        return Task.CompletedTask;
    }

    private void TimerElapsed(object stateInfo)
    {
        InvokeAsync(async () => await TimerTickAsync().ConfigureAwait(false));
    }

    /// <summary>
    ///     Changes the SelectedIndex to a next one (or restart on 0)
    /// </summary>
    private Task TimerTickAsync()
    {
        return InvokeAsync(Next);
    }

    public class BulletTemplateArg(int index, bool isSelectedItem, bool isUserSelectedItem, Action clickActon)
    {
        public Action ClickActon { get; } = clickActon;
        public int Index { get; } = index;
        public bool IsSelectedItem { get; } = isSelectedItem;
        public bool IsUserSelectedItem { get; } = isUserSelectedItem;
    }
}