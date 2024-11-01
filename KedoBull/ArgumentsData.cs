
using PowerArgs;

namespace KedoBull
{
    public class ArgumentsData
    {
        [ArgRequired(PromptIfMissing = true)]
        public string Url { get; set; } = null!;

        [ArgRequired(PromptIfMissing = true)]
        public string Login { get; set; } = null!;

        public SecureStringArgument Password { get; set; } = null!;
    }
}
