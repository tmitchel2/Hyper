using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using Hyper.Http.Formatting;
using HyperTests.Http.Formatting;
using NUnit.Framework;

namespace HyperTests
{
    [TestFixture]
    public class MessagePackFormatterTests : FormatterTestsBase
    {
        protected override MediaTypeFormatter GetHyperMediaTypeFormatter(IEnumerable<Type> types)
        {
            return new MsgPackMediaTypeFormatter().ToHyperFormatter("msgpack", types);
        }
    }
}