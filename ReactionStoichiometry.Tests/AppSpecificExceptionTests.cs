namespace ReactionStoichiometry.Tests
{
    public sealed class AppSpecificExceptionTests
    {
        [Fact]
        public void AppSpecificException_Simple()
        {
            Assert.Throws<AppSpecificException>(static () => { AppSpecificException.ThrowIf(condition: true, message: "Must throw"); });
            Assert.NotNull(Record.Exception(static () => { AppSpecificException.ThrowIf(condition: true, message: "Getting used to xUnit"); }));
            Assert.Null(Record.Exception(static () => { AppSpecificException.ThrowIf(condition: false, message: "Must not throw"); }));
        }
    }
}
