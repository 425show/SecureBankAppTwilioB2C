public interface IVerification
{
    Task<VerificationResult> StartVerificationAsync(string phoneNumber, string channel);

    Task<VerificationResult> CheckVerificationAsync(string phoneNumber, string code);
}

public class Verification : IVerification
{

    private readonly IConfiguration _config;

    public Verification(IConfiguration configuration)
    {
        _config = configuration;
        TwilioClient.Init(_config["Twilio:AccountSid"], _config["Twilio:AuthToken"]);
    }

    public async Task<VerificationResult> StartVerificationAsync(string phoneNumber, string channel)
    {
        try
        {
            var verificationResource = await VerificationResource.CreateAsync(
                to: phoneNumber,
                channel: channel,
                pathServiceSid: _config["Twilio:VerificationSid"]
            );
            return new VerificationResult(verificationResource.Sid);
        }
        catch (TwilioException e)
        {
            return new VerificationResult(new List<string>{e.Message});
        }
    }

    public async Task<VerificationResult> CheckVerificationAsync(string phoneNumber, string code)
    {
        try
        {
            var verificationCheckResource = await VerificationCheckResource.CreateAsync(
                to: phoneNumber,
                code: code,
                pathServiceSid: _config["Twilio:VerificationSid"]
            );
            return verificationCheckResource.Status.Equals("approved") ?
                new VerificationResult(verificationCheckResource.Sid) :
                new VerificationResult(new List<string>{"Wrong code. Try again."});
        }
        catch (TwilioException e)
        {
            return new VerificationResult(new List<string>{e.Message});
        }
    }
}
