using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using Hyper.Http.Formatting;
using NUnit.Framework;

namespace HyperTests
{
    [TestFixture]
    public class JsonFormatterTests : FormatterTestsBase
    {
        protected override MediaTypeFormatter GetHyperMediaTypeFormatter(IEnumerable<Type> types)
        {
            return new Hyper.Http.Formatting.JsonMediaTypeFormatter().ToHyperFormatter("json", types);
        }
    }
}