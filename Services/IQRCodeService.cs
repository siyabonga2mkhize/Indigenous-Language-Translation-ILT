namespace PhraseBookk.Services
{
    public interface IQRCodeService
    {
        string GenerateQRCodeUrl(string data, int size = 200);
        string GenerateQRCodeBase64(string data, int size = 200);
    }
}
