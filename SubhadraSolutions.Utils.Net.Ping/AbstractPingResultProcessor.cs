namespace SubhadraSolutions.Utils.Net.Ping
{
    public abstract class AbstractPingResultProcessor
    {
        public virtual string RemovePadding(string output, string commandPrompt)
        {
            string text = output;
            if (!string.IsNullOrEmpty(output))
            {
                int num = output.LastIndexOf(Environment.NewLine);
                if (num > -1)
                {
                    text = output.Substring(0, num);
                }
                text = output;
            }
            if (!string.IsNullOrEmpty(text))
            {
                return text;
            }
            return " ";
        }
    }

}
