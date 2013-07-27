using System;
using System.Collections.Generic;
using System.Net.Http.Formatting;
using Hyper.Http.Formatting;
using HyperTests.Http.Formatting;
using NUnit.Framework;

namespace HyperTests
{
    [TestFixture]
    public class BsonJsonFormatterTests : FormatterTestsBase
    {
        protected override MediaTypeFormatter GetHyperMediaTypeFormatter(IEnumerable<Type> types)
        {
            return new NewtonsoftBsonMediaTypeFormatter().ToHyperFormatter("bson", types);
        }
    }
}