using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Timers;

namespace SubhadraSolutions.Utils.Blazor.Components
{
    public partial class DigitalClock : MudComponentBase
    {
        private static readonly Timer _timer;

        static DigitalClock()
        {
            _timer = new Timer(TimeSpan.FromSeconds(1));
            _timer.Enabled = true;
        }

        private DateTime timeNow;

        public DigitalClock()
        {
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.timeNow = DateTime.UtcNow.Add(this.TimeSpan);
            InvokeAsync(StateHasChanged);
        }

        [Parameter]
        public TimeSpan TimeSpan { get; set; }

        [Parameter]
        public RenderFragment<DateTime> ChildContent { get; set; }

        protected override void OnInitialized()
        {
        }
    }
}