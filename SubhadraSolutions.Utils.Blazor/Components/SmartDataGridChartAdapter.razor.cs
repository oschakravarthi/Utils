using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Blazor.Components
{
    public partial class SmartDataGridChartAdapter<T> : AbstractSmartComponent
    {
        [Parameter]
        public IQueryable<T> Queryable { get; set; }

        [Parameter]
        public List<T> Data { get; set; }

        [Parameter]
        public RenderFragment<CellContext<T>> ChildRowContent { get; set; }

        [Parameter]
        public RenderFragment<CellContext<T>> TemplateColumnContent { get; set; }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            var hasChanged = false;
            foreach (var parameter in parameters)
            {
                if (parameter.Name == nameof(Queryable))
                {
                    hasChanged = !ReferenceEquals(this.Queryable, parameter.Value);
                    break;
                }
                if (parameter.Name == nameof(Data))
                {
                    hasChanged = !ReferenceEquals(this.Data, parameter.Value);
                    break;
                }
            }

            if (hasChanged)
            {
                await base.SetParametersAsync(parameters).ConfigureAwait(false);
            }
        }
    }
}