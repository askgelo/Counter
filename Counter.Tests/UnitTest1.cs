using System;
using Counter.Models;
using System.Collections.Generic;
using Xunit;

namespace Counter.Tests
{
    public class TestCounter
    {
        [Theory]
        [InlineData(
            new string[3] { "20.01.2017", "23.02.2017", "22.03.2017" },
            new double[3] { 10, 15, 23 },
            new double[2] { 5, 8})]
        public void TestGetAvgByTime(string[] dates, double[] values, double[] expectedResult)
        {
            var counter = new Models.Counter() { Values = new List<CounterValue>() };
            for (int i = 0; i < dates.Length; i++)
            {
                counter.Values.Add(new CounterValue()
                {
                    Date = DateTimeOffset.Parse(dates[i]),
                    Value = values[i]
                });
            }

            //var actualResult = counter.GetAvgByTime();

            //Assert.Equal(expectedResult.Length, actualResult.Count);
            //for (int i = 0; i < actualResult.Count; i++)
            //{
            //    Assert.Equal(expectedResult[i], actualResult[i]);
            //}
        }
    }
}
