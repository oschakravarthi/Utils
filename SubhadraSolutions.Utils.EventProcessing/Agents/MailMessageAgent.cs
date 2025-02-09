using System.Net.Mail;

namespace SubhadraSolutions.Utils.EventProcessing.Agents;

public class MailMessageAgent(IEventAggregator eventAggregator) : AbstractAgent(eventAggregator)
{
    public MailMessage MessageTemplate { get; set; }

    protected override void OnEvent(object sender, IEvent e)
    {
        var mailBody = (string)e.PayloadObject;
        var mailMessage = new MailMessage();
        DynamicCopyHelper<MailMessage, MailMessage>.CopyTo(MessageTemplate, mailMessage);
        mailMessage.Body = mailBody;

        var newEvent = EventProcessingHelper.BuildGenericEvent(Topic, mailMessage, e.Id);
        eventAggregator.PublishEvent(newEvent);
    }
}