using Microsoft.AspNetCore.Components;
using MudBlazor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SubhadraSolutions.Utils.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Blazor.Components
{
    public partial class TaskDialog<T>
    {
        private static JsonSerializer jsonSerializer;

        static TaskDialog()
        {
            jsonSerializer = JsonSerializer.Create(JsonSettings.RestSerializerSettingsForDebug);
        }

        public TaskDialog()
        {
            this.Outlined = true;
        }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        [Parameter]
        public Func<ValueTask<T>> TaskFunc { get; set; }

        [Parameter]
        public IDictionary<string, object> Payload { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string Icon { get; set; } = MudBlazor.Icons.Material.Filled.MonitorHeart;

        private bool isErrored = false;

        private JObject jObject = [];

        protected override async Task ResetAsync()
        {
            if (this.Payload != null)
            {
                foreach (var kvp in this.Payload)
                {
                    this.jObject[kvp.Key] = JToken.FromObject(this.Payload, jsonSerializer);
                }
            }
            try
            {
                var task = this.TaskFunc();
                var result = await task.ConfigureAwait(false);
                this.jObject["Response"] = JToken.FromObject(result, jsonSerializer);
            }
            catch (Exception ex)
            {
                isErrored = true;
                this.jObject["Exception"] = JToken.FromObject(ex, jsonSerializer);
            }
        }

        private MarkupString GetContent()
        {
            return (MarkupString)JsonSerializationHelper.Serialize(this.jObject, jsonSerializer);
        }

        protected string GetMinMaxIcon()
        {
            return this.MudDialog.Options.FullScreen.Value
                ? MudBlazor.Icons.Material.Filled.FullscreenExit
                : MudBlazor.Icons.Material.Filled.Fullscreen;
        }

        private void ToggleMinMax()
        {
            var from = this.MudDialog.Options;
            var dialogOptions = new DialogOptions
            {
                BackdropClick = from.BackdropClick,
                BackgroundClass = from.BackgroundClass,
                CloseButton = from.CloseButton,
                CloseOnEscapeKey = from.CloseOnEscapeKey,
                FullWidth = from.FullWidth,
                MaxWidth = from.MaxWidth,
                NoHeader = from.NoHeader,
                Position = from.Position,
                FullScreen = !this.MudDialog.Options.FullScreen.Value
            };
            
            this.MudDialog.SetOptionsAsync(dialogOptions);
        }

        private void Submit() => MudDialog.Close(DialogResult.Ok(true));
    }
}