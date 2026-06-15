using NUnit.Framework;
using CleverenceSoftTasks;
using System;

namespace CleverenceSoftTestLib
{
    [TestFixture]
    public class StringCompressorTests
    {
        [TestCase("aaabbcccdde", "a3b2c3d2e")]
        [TestCase("abc", "abc")]
        [TestCase("aabbaa", "a2b2a2")]
        [TestCase("", "")]
        [TestCase("a", "a")]
        public void Compress_ShouldReturnCorrectString(string input, string expected)
        {
            Assert.That(StringCompressor.Compress(input), Is.EqualTo(expected));
        }

        [TestCase("a3b2c3d2e", "aaabbcccdde")]
        [TestCase("abc", "abc")]
        [TestCase("a2b2a2", "aabbaa")]
        [TestCase("", "")]
        [TestCase("a", "a")]
        public void Decompress_ShouldReturnOriginalString(string compressed, string expected)
        {
            Assert.That(StringCompressor.Decompress(compressed), Is.EqualTo(expected));
        }
    }
}
