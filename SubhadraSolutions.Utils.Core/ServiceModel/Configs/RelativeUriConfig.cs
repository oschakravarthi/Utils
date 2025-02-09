namespace SubhadraSolutions.Utils.ServiceModel.Configs
{
    public class RelativeUriConfig : EndpointConfigBase
    {
        private string _endpointRelativeUri;

        public string EndpointRelativeUri
        {
            get
            {
                return this._endpointRelativeUri;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = null;
                }
                else
                {
                    value = value.TrimStart('/');
                }
                this._endpointRelativeUri = value;
            }
        }
    }
}