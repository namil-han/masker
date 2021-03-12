using Xunit;

namespace Tester
{
    using System;
    using Nih.Masker;

    public class TestEmail
    {
        [Fact]
        public void email_is_null_or_empty_throw()
        {
            Assert.Throws<ArgumentException>(() => string.Empty.Email());
        }

        [Fact]
        public void valid_email_with_default_pattern()
        {
            var result = "abcdefg@host.com".Email();
            Assert.Equal("abc****@host.com", result);
        }

        [Fact]
        public void valid_email_with_pattern()
        {
            var result = "abcdefg@host.com".Email("#*");
            Assert.Equal("a******@host.com", result);
        }
    }
}