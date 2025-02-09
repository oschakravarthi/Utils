namespace SubhadraSolutions.Utils.ServiceModel.Configs;

public class EndpointConfigEx : RelativeUriConfig, IEndpointConfig
{
    private string _serverUri;

    public string ServerUri
    {
        get
        {
            return this._serverUri;
        }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                value = null;
            }
            else
            {
                value = value.TrimEnd('/');
            }
            this._serverUri = value;
        }
    }

    public string Uri
    {
        get
        {
            return $"{ServerUri}/{EndpointRelativeUri}";
        }
    }
}