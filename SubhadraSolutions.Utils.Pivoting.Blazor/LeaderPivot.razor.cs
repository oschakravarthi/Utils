﻿namespace SubhadraSolutions.Utils.Pivoting.Blazor
{
    public partial class LeaderPivot<T> : BaseComponent
    {
        [Parameter] public IList<Dimension<T>> Dimensions { get; set; }
        [Parameter] public IEnumerable<Measure<T>> Measures { get; set; }
        [Parameter] public bool DisplayGrandTotals { get; set; }
        [Parameter] public bool DisplayGrandTotalOption { get; set; }
        [Parameter] public bool DisplayDimensionButtons { get; set; }
        [Parameter] public bool DisplayMeasureSelectors { get; set; }
        [Parameter] public bool DisplayReloadDataButton { get; set; }
        [Parameter] public bool DisplayHiddenDimSelector { get; set; }
        [Parameter] public IQueryable<T> DataSource { get; set; }
        [Parameter] public bool IsBusy { get; set; }
        [Parameter] public EventCallback<bool> IsBusyChanged { get; set; }

        public List<Dimension<T>> RowDimensions 
        {
            get => Dimensions.Where(x => x.IsEnabled && x.IsRow).OrderBy(x => x.Sequence).ToList(); 
            set => _ = value; 
        }

        public List<Dimension<T>> ColumnDimensions 
        { 
            get => Dimensions.Where(x => x.IsEnabled && !x.IsRow).OrderBy(x => x.Sequence).ToList(); 
            set => _ = value; 
        }

        public List<Dimension<T>> HiddenDimensions
        {
            get => Dimensions.Where(x => !x.IsEnabled).OrderBy(x => x.IsRow).ThenBy(x => x.Sequence).ToList();
            set => _ = value;
        }

        public int MaxDimensionsPerAxis => Dimensions.Count == 2 ? 2 : Dimensions.Count - 1;

        public Matrix Matrix { get; set; }
        
        private MatrixBuilder<T> matrixBuilder;
        private int BusyCount;

        public LeaderPivot()
        {
            NodeBuilder<T> nodeBuilder = new NodeBuilder<T>();
            Validator<T> validator = new Validator<T>();
            matrixBuilder = new MatrixBuilder<T>(nodeBuilder, validator);
        }

        public async Task ReloadData()
        {
            await ToggleIsBusy(true);
            await RenderTable();
            await ToggleIsBusy(false);
        }

        protected override async Task OnInitializedAsync()
        {
            if (PivotStyle == null)
                PivotStyle = new LeaderPivotStyle();

            await ReloadData();
            await base.OnInitializedAsync();
        }

        private async Task RenderTable()
        {
            await ToggleIsBusy(true);
            Matrix = await matrixBuilder.BuildMatrix(this.DataSource, Dimensions, Measures, DisplayGrandTotals);
            await ToggleIsBusy(false);
        }

        private async Task ToggleNodeExpansion(string nodeID)
        {
            await ToggleIsBusy(true);
            Matrix = matrixBuilder.ToggleNodeExpansion(nodeID);
            await ToggleIsBusy(false);
        }

        private bool IsMeasureCheckBoxDisabled(Measure<T> measure) => ! measure.CanDisable;

        private void DimensionsChanged(Tuple<List<Dimension<T>>, Dimension<T>, DropZone> args)
        {
            List<Dimension<T>> axisDimensions = null;
            Dimension<T> dim = args.Item2;
            DropZone dropZone = args.Item3;
            List<Dimension<T>> enabledDimensions = Dimensions.Where(x => x.IsEnabled).ToList();

            //if (dropZone == DropZone.Hidden)
            //    axisDimensions = dim.IsRow ?  RowDimensions : ColumnDimensions;
            //else
            //    axisDimensions = dropZone == DropZone.Rows ? RowDimensions : ColumnDimensions;

            if (dropZone == DropZone.Hidden)
            {
                if (dim.IsRow)
                    axisDimensions = RowDimensions;
                else
                    axisDimensions = ColumnDimensions;
            }
            else
            {
                axisDimensions = args.Item1;
            }

            if (dropZone != DropZone.Hidden) 
            {
                bool isRows = dropZone == DropZone.Rows;
                List<Dimension<T>> crossAxisDimensions = null;
                //crossAxisDimensions = isRows ? ColumnDimensions : RowDimensions;

                if (isRows)
                    crossAxisDimensions = ColumnDimensions;
                else
                    crossAxisDimensions = RowDimensions;

                if (crossAxisDimensions.Count > 1 || enabledDimensions.Count == 2)
                {
                    dim.IsRow = isRows;
                    int sequence = 0;
                    axisDimensions.ForEach(x => x.Sequence = sequence++);

                    // if we have exactly two enabled dimensions, find the other dimension and make
                    // sure it's axis is different than the dimension that was dragged.

                    if (enabledDimensions.Count == 2 && enabledDimensions.All(x => x.IsRow == isRows))
                    {
                        Dimension<T> other = enabledDimensions.First(x => x.ID != dim.ID);
                        other.IsRow = !isRows;
                    }
                }
                else
                    return;
            }
            else
            {
                // User dragged a dimension to hidden dropzone
                // Only hide the dim if there is another one that is visible on the same axis

                if (axisDimensions.Count > 1)
                    dim.IsEnabled = false;
                else
                    return; // don't render
            }
            RenderTable();
        }

        private async Task GrandTotalsCheckedChanged(bool arg)
        {
            DisplayGrandTotals = arg;
            RenderTable();
        }

        private async Task ToggleIsBusy(bool busy)
        {
            bool wasBusy = BusyCount > 0;
            BusyCount = Math.Max(0, BusyCount + (busy ? 1 : -1));
            bool isBusy = BusyCount > 0;

            if (wasBusy != isBusy)
            {
                IsBusy = isBusy;
                await IsBusyChanged.InvokeAsync(isBusy);
            }
        }
    }
}
