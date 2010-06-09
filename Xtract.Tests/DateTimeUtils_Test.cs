using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Offr.Text;

namespace Xtract.Tests
{
    [TestFixture]
    public class DateTimeUtils_Test
    {
       
        [Test]
        public void TestParseTwitterCreatedAt()
        {
            string created_at = "Thu Jun 03 20:05:38 +0000 2010";
            DateTime utcDateTime = DateUtils.UTCDateTimeFromTwitterTimeStamp(created_at);
            Assert.AreEqual(6, utcDateTime.Month);
            Assert.AreEqual(2010, utcDateTime.Year);
            Assert.AreEqual(3,utcDateTime.Day);
        }
    }
}
