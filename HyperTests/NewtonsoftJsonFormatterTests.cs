using System;
using System.Collections.Generic;
using Hyper.Http.Formatting;
using HyperTests.Http.Formatting;
using NUnit.Framework;

namespace HyperTests
{
    [TestFixture]
    public class NewtonsoftJsonFormatterTests : FormatterTestsBase
    {
        protected override HyperMediaTypeFormatter GetHyperMediaTypeFormatter(IEnumerable<Type> types)
        {
            return new HyperMediaTypeFormatter("json", new NewtonsoftJsonMediaTypeFormatter(), types);
        }
    }
}