using System;
using GitCommands.Git.Tag;
using NUnit.Framework;

namespace GitCommandsTests.Git.Tag
{
    [TestFixture]
    public class GitCreateTagCmdTests
    {
        private const string TagName = "bla";
        private const string Revision = "0123456789";
        private const string TagMessage = "foo";
        private const string KeyId = "A9876F";
        private const string  TagMessageFile = "c:/.git/TAGMESSAGE";


        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void Validate_should_throw_if_tag_name_invalid(string tagName)
        {
            var args = new GitCreateTagArgs(tagName, Revision);
            var cmd = new GitCreateTagCmd(args, TagMessageFile);

            Assert.Throws<ArgumentException>(() => cmd.Validate());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void Validate_should_throw_if_tag_revision_invalid(string revision)
        {
            var args = new GitCreateTagArgs(TagName, revision);
            var cmd = new GitCreateTagCmd(args, TagMessageFile);

            Assert.Throws<ArgumentException>(() => cmd.Validate());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void Validate_should_throw_for_SignWithSpecificKey_if_tag_keyId_invalid(string signKeyId)
        {
            var args = new GitCreateTagArgs(TagName, Revision, TagOperation.SignWithSpecificKey, signKeyId: signKeyId);
            var cmd = new GitCreateTagCmd(args, TagMessageFile);

            Assert.Throws<ArgumentException>(() => cmd.Validate());
        }

        [Test]
        public void ToLine_should_throw_if_operation_not_supported()
        {
            var args = new GitCreateTagArgs(TagName, Revision, (TagOperation)10);
            var cmd = new GitCreateTagCmd(args, TagMessageFile);

            Assert.Throws<NotSupportedException>(() => cmd.ToLine());
        }


        [TestCase(true, "tag -f -s -F \"c:/.git/TAGMESSAGE\" \"bla\" -- \"0123456789\"")]
        [TestCase(false, "tag -s -F \"c:/.git/TAGMESSAGE\" \"bla\" -- \"0123456789\"")]
        public void ToLine_should_render_force_flag(bool force, string expected)
        {
            var args = new GitCreateTagArgs(TagName, Revision, TagOperation.SignWithDefaultKey, TagMessage, KeyId, force);
            var cmd = new GitCreateTagCmd(args, TagMessageFile);

            var cmdLine = cmd.ToLine();

            Assert.AreEqual(expected, cmdLine);
        }

        [TestCase(TagOperation.Lightweight, "tag -f \"bla\" -- \"0123456789\"")]
        [TestCase(TagOperation.Annotate, "tag -f -a -F \"c:/.git/TAGMESSAGE\" \"bla\" -- \"0123456789\"")]
        [TestCase(TagOperation.SignWithDefaultKey, "tag -f -s -F \"c:/.git/TAGMESSAGE\" \"bla\" -- \"0123456789\"")]
        [TestCase(TagOperation.SignWithSpecificKey, "tag -f -u A9876F -F \"c:/.git/TAGMESSAGE\" \"bla\" -- \"0123456789\"")]
        public void ToLine_should_render_different_operations(TagOperation operation, string expected)
        {
            var args = new GitCreateTagArgs(TagName, Revision, operation, signKeyId: KeyId, force: true);
            var cmd = new GitCreateTagCmd(args, TagMessageFile);

            var actualCmdLine = cmd.ToLine();

            Assert.AreEqual(expected, actualCmdLine);
        }

    }
}