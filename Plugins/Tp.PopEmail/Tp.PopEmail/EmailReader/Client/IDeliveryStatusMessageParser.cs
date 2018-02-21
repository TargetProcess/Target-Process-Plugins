using MailBee.Mime;

namespace Tp.PopEmailIntegration.EmailReader.Client
{
    public interface IDeliveryStatusMessageParser
    {
        bool IsDsnMessage(MailMessage message);
    }
}
