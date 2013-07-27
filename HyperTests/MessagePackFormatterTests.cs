using System;
using System.Collections.Generic;
using Hyper.Http.Formatting;
using HyperTests.Http.Formatting;
using NUnit.Framework;

namespace HyperTests
{
    [TestFixture]
    public class MessagePackFormatterTests : FormatterTestsBase
    {
        protected override HyperMediaTypeFormatter GetHyperMediaTypeFormatter(IEnumerable<Type> types)
        {
            return new HyperMediaTypeFormatter("msgpack", new MsgPackMediaTypeFormatter(), types);
        }
    }
}