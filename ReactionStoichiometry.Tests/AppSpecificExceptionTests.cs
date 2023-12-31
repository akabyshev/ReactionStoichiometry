﻿namespace ReactionStoichiometry.Tests
{
    public sealed class AppSpecificExceptionTests
    {
        [Fact]
        public void AppSpecificException_Simple()
        {
            Assert.Throws<AppSpecificException>(testCode: static () => { AppSpecificException.ThrowIf(condition: true, message: "Must throw"); });
            Assert.NotNull(Record.Exception(testCode: static () => { AppSpecificException.ThrowIf(condition: true, message: "Getting used to xUnit"); }));
            Assert.Null(Record.Exception(testCode: static () => { AppSpecificException.ThrowIf(condition: false, message: "Must not throw"); }));
        }
    }
}
